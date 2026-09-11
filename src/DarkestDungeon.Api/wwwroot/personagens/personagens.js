const state = {
  characters: [],
  classes: [],
  loadingList: false,
  saving: false,
  seeding: false,
  deleting: false
};

const corpoPlayers = new Map();
const conjuntoCache = new Map();
const geracaoDoCorpo = new Map();
let loopRaf = 0;
let lastFrame = 0;

const elements = {
  list: document.querySelector('#character-list'),
  section: document.querySelector('#characters-section'),
  empty: document.querySelector('#empty-state'),
  loading: document.querySelector('#loading-state'),
  error: document.querySelector('#error-state'),
  errorMessage: document.querySelector('#error-message'),
  retryList: document.querySelector('#retry-list'),
  retryError: document.querySelector('#retry-error'),
  status: document.querySelector('#status-message'),
  count: document.querySelector('#character-count'),
  dialog: document.querySelector('#character-dialog'),
  form: document.querySelector('#character-form'),
  formError: document.querySelector('#form-error'),
  classSelect: document.querySelector('#classe'),
  save: document.querySelector('#save-character'),
  seedHeroes: document.querySelector('#seed-heroes')
};

const numericFields = ['nivel', 'nivelDaArma', 'nivelDaArmadura', 'aparencia'];

async function requestJson(url, options = {}) {
  const response = await fetch(url, {
    headers: { 'Content-Type': 'application/json', ...(options.headers ?? {}) },
    ...options
  });
  const text = await response.text();
  const body = text ? JSON.parse(text) : null;
  if (!response.ok) {
    const error = new Error(body?.mensagem || 'A operação não pôde ser concluída.');
    error.status = response.status;
    error.body = body;
    throw error;
  }
  return body;
}

function setHidden(element, hidden) {
  element.hidden = hidden;
}

function setStatus(message = '') {
  elements.status.textContent = message;
}

function showListState() {
  setHidden(elements.loading, !state.loadingList);
  setHidden(elements.error, state.loadingList || state.characters !== null);
  setHidden(elements.section, state.loadingList || state.characters === null || state.characters.length === 0);
  setHidden(elements.empty, state.loadingList || state.characters === null || state.characters.length > 0);
  setHidden(elements.retryList, state.loadingList || state.characters !== null);
  elements.count.textContent = state.characters?.length ?? 0;
}

function formatNumber(value) {
  if (value === null || value === undefined || value === '') return '—';
  return value;
}

function formatAparencia(value) {
  const letras = ['A', 'B', 'C', 'D'];
  if (typeof value === 'number' && value >= 0 && value < letras.length) return letras[value];
  if (typeof value === 'string' && letras.includes(value.toUpperCase())) return value.toUpperCase();
  if (typeof value === 'string' && /^\d+$/.test(value)) return formatAparencia(Number(value));
  return 'A';
}

function mediaSlot(slot, rotuloPadrao) {
  const rotulo = escapeHtml(slot?.rotulo || rotuloPadrao);
  const ok = slot?.status === 'OK' && slot?.url;
  const corpo = ok
    ? `<img class="media-slot-image" src="${escapeHtml(slot.url)}" alt="${rotulo}" onerror="this.outerHTML='<span class=\\'media-missing\\'>sem imagem</span>'">`
    : '<span class="media-missing">sem imagem</span>';
  return `
    <figure class="media-slot">
      <figcaption>${rotulo}</figcaption>
      ${corpo}
    </figure>
  `;
}

function corpoSlot(slot, personagemId) {
  const rotulo = escapeHtml(slot?.rotulo || 'Corpo inteiro');
  const versoes = slot?.versoes?.length
    ? slot.versoes
    : [
      { id: 'emEspera', rotulo: 'Em espera', disponivel: false },
      { id: 'animado', rotulo: 'Animado', disponivel: false },
      { id: 'caminhada', rotulo: 'Caminhada', disponivel: false }
    ];
  const opcoes = versoes.map(versao => {
    const disabled = versao.disponivel ? '' : ' disabled';
    const selected = versao.id === 'emEspera' ? ' selected' : '';
    return `<option value="${escapeHtml(versao.id)}"${disabled}${selected}>${escapeHtml(versao.rotulo)}</option>`;
  }).join('');
  return `
    <figure class="media-slot media-slot-corpo">
      <figcaption>${rotulo}</figcaption>
      <div class="media-slot-stage" data-corpo-host="${escapeHtml(personagemId)}">
        <span class="media-missing">sem imagem</span>
      </div>
      <label class="corpo-versao">
        <span class="visually-hidden">Versão do corpo</span>
        <select data-corpo-versao="${escapeHtml(personagemId)}">${opcoes}</select>
      </label>
    </figure>
  `;
}

function pararPlayers() {
  for (const player of corpoPlayers.values()) {
    player.ativo = false;
  }
  corpoPlayers.clear();
  geracaoDoCorpo.clear();
  if (loopRaf) {
    cancelAnimationFrame(loopRaf);
    loopRaf = 0;
  }
  lastFrame = 0;
}

function mostrarSemImagem(host) {
  host.innerHTML = '<span class="media-missing">sem imagem</span>';
}

function primeiroNomeDeAnimacao(skeletonData) {
  return skeletonData.animations?.[0]?.name || null;
}

async function carregarImagem(url) {
  const imagem = new Image();
  imagem.decoding = 'async';
  imagem.src = url;
  await imagem.decode();
  return imagem;
}

async function carregarConjunto(conjunto) {
  const chave = `${conjunto.urlAtlas}|${conjunto.urlEsqueleto}|${conjunto.urlTextura}`;
  if (conjuntoCache.has(chave)) {
    return conjuntoCache.get(chave);
  }

  const pending = (async () => {
    const [atlasTexto, skelBuffer, textura] = await Promise.all([
      fetch(conjunto.urlAtlas).then(r => { if (!r.ok) throw new Error('atlas'); return r.text(); }),
      fetch(conjunto.urlEsqueleto).then(r => { if (!r.ok) throw new Error('skel'); return r.arrayBuffer(); }),
      carregarImagem(conjunto.urlTextura)
    ]);

    const atlas = new spine.Atlas(atlasTexto, {
      load(page) {
        page.rendererObject = textura;
        if (!page.width) page.width = textura.width;
        if (!page.height) page.height = textura.height;
      },
      unload() {}
    });
    atlas.updateUVs(atlas.pages[0]);
    const loader = new spine.AtlasAttachmentLoader(atlas);
    const binary = new spine.SkeletonBinary(loader);
    const skeletonData = binary.readSkeletonData(skelBuffer);
    return { atlas, skeletonData, textura };
  })();

  conjuntoCache.set(chave, pending);
  try {
    return await pending;
  } catch (error) {
    conjuntoCache.delete(chave);
    throw error;
  }
}

function blitTriangulo(ctx, img, x0, y0, x1, y1, x2, y2, u0, v0, u1, v1, u2, v2) {
  if (![x0, y0, x1, y1, x2, y2, u0, v0, u1, v1, u2, v2].every(Number.isFinite)) return;
  ctx.beginPath();
  ctx.moveTo(x0, y0);
  ctx.lineTo(x1, y1);
  ctx.lineTo(x2, y2);
  ctx.closePath();
  ctx.save();
  ctx.clip();
  const det = u0 * (v1 - v2) + u1 * (v2 - v0) + u2 * (v0 - v1);
  if (Math.abs(det) < 0.0001) {
    ctx.restore();
    return;
  }
  ctx.setTransform(
    (x0 * (v1 - v2) + x1 * (v2 - v0) + x2 * (v0 - v1)) / det,
    (y0 * (v1 - v2) + y1 * (v2 - v0) + y2 * (v0 - v1)) / det,
    (x0 * (u2 - u1) + x1 * (u0 - u2) + x2 * (u1 - u0)) / det,
    (y0 * (u2 - u1) + y1 * (u0 - u2) + y2 * (u1 - u0)) / det,
    (x0 * (u1 * v2 - u2 * v1) + x1 * (u2 * v0 - u0 * v2) + x2 * (u0 * v1 - u1 * v0)) / det,
    (y0 * (u1 * v2 - u2 * v1) + y1 * (u2 * v0 - u0 * v2) + y2 * (u0 * v1 - u1 * v0)) / det
  );
  ctx.drawImage(img, 0, 0);
  ctx.restore();
}

function desenharRegiao(ctx, attachment, slot, ox, oy, escala) {
  const vertices = [];
  attachment.computeVertices(0, 0, slot.bone, vertices);
  const region = attachment.rendererObject;
  const img = region?.page?.rendererObject;
  if (!img) return;
  const uvs = attachment.uvs;
  const xs = [
    ox + vertices[0] * escala,
    ox + vertices[2] * escala,
    ox + vertices[4] * escala,
    ox + vertices[6] * escala
  ];
  const ys = [
    oy + vertices[1] * escala,
    oy + vertices[3] * escala,
    oy + vertices[5] * escala,
    oy + vertices[7] * escala
  ];
  const us = [uvs[0] * img.width, uvs[2] * img.width, uvs[4] * img.width, uvs[6] * img.width];
  const vs = [uvs[1] * img.height, uvs[3] * img.height, uvs[5] * img.height, uvs[7] * img.height];
  ctx.save();
  ctx.globalAlpha = slot.a * attachment.a;
  blitTriangulo(ctx, img, xs[0], ys[0], xs[1], ys[1], xs[2], ys[2], us[0], vs[0], us[1], vs[1], us[2], vs[2]);
  blitTriangulo(ctx, img, xs[2], ys[2], xs[3], ys[3], xs[0], ys[0], us[2], vs[2], us[3], vs[3], us[0], vs[0]);
  ctx.restore();
}

function desenharMalha(ctx, attachment, slot, ox, oy, escala) {
  const region = attachment.rendererObject;
  const img = region?.page?.rendererObject;
  if (!img) return;
  const vertexCount = attachment.uvs.length / 2;
  const world = new Array(vertexCount * 2);
  attachment.computeWorldVertices(0, 0, slot, world);
  const triangles = attachment.triangles;
  ctx.save();
  ctx.globalAlpha = slot.a * attachment.a;
  for (let i = 0; i < triangles.length; i += 3) {
    const i0 = triangles[i] * 2;
    const i1 = triangles[i + 1] * 2;
    const i2 = triangles[i + 2] * 2;
    const x0 = ox + world[i0] * escala;
    const y0 = oy + world[i0 + 1] * escala;
    const x1 = ox + world[i1] * escala;
    const y1 = oy + world[i1 + 1] * escala;
    const x2 = ox + world[i2] * escala;
    const y2 = oy + world[i2 + 1] * escala;
    const u0 = attachment.uvs[i0] * img.width;
    const v0 = attachment.uvs[i0 + 1] * img.height;
    const u1 = attachment.uvs[i1] * img.width;
    const v1 = attachment.uvs[i1 + 1] * img.height;
    const u2 = attachment.uvs[i2] * img.width;
    const v2 = attachment.uvs[i2 + 1] * img.height;
    blitTriangulo(ctx, img, x0, y0, x1, y1, x2, y2, u0, v0, u1, v1, u2, v2);
  }
  ctx.restore();
}

function desenharEsqueleto(player) {
  const { canvas, skeleton, noLugar } = player;
  const ctx = canvas.getContext('2d');
  if (!ctx) return;
  skeleton.updateWorldTransform();
  const pontos = [];
  for (const slot of skeleton.drawOrder) {
    const attachment = slot.attachment;
    if (!attachment) continue;
    try {
      if (attachment.type === spine.AttachmentType.region) {
        const vertices = [];
        attachment.computeVertices(0, 0, slot.bone, vertices);
        for (let i = 0; i < 8; i += 2) pontos.push(vertices[i], vertices[i + 1]);
      } else if (attachment.type === spine.AttachmentType.mesh || attachment.type === spine.AttachmentType.skinnedmesh) {
        const world = new Array(attachment.uvs.length);
        attachment.computeWorldVertices(0, 0, slot, world);
        for (let i = 0; i < world.length; i += 2) pontos.push(world[i], world[i + 1]);
      }
    } catch {
      continue;
    }
  }
  if (pontos.length < 4) return;
  let minX = Infinity, minY = Infinity, maxX = -Infinity, maxY = -Infinity;
  for (let i = 0; i < pontos.length; i += 2) {
    const px = pontos[i];
    const py = pontos[i + 1];
    if (!Number.isFinite(px) || !Number.isFinite(py)) continue;
    minX = Math.min(minX, px);
    minY = Math.min(minY, py);
    maxX = Math.max(maxX, px);
    maxY = Math.max(maxY, py);
  }
  if (!Number.isFinite(minX) || !Number.isFinite(minY) || !Number.isFinite(maxX) || !Number.isFinite(maxY)) return;
  const padding = 8;
  const largura = Math.max(1, maxX - minX);
  const altura = Math.max(1, maxY - minY);
  const escala = Math.min((canvas.width - padding * 2) / largura, (canvas.height - padding * 2) / altura);
  const ox = (canvas.width - largura * escala) / 2 - minX * escala;
  const oy = (canvas.height - altura * escala) / 2 - minY * escala;
  ctx.clearRect(0, 0, canvas.width, canvas.height);
  ctx.imageSmoothingEnabled = true;
  ctx.imageSmoothingQuality = 'high';
  for (const slot of skeleton.drawOrder) {
    const attachment = slot.attachment;
    if (!attachment) continue;
    try {
      if (attachment.type === spine.AttachmentType.region) {
        desenharRegiao(ctx, attachment, slot, ox, oy, escala);
      } else if (attachment.type === spine.AttachmentType.mesh || attachment.type === spine.AttachmentType.skinnedmesh) {
        desenharMalha(ctx, attachment, slot, ox, oy, escala);
      }
    } catch {
      continue;
    }
  }
  void noLugar;
}

function avancarPose(player, dt) {
  if (!player.animar || !player.state) return;
  player.state.update(dt);
  player.skeleton.setToSetupPose();
  player.state.apply(player.skeleton);
  if (player.noLugar) {
    const raiz = player.skeleton.getRootBone();
    if (raiz) {
      raiz.x = raiz.data.x;
      raiz.y = raiz.data.y;
    }
  }
}

function tickCorpo(agora) {
  const dt = lastFrame ? Math.min(0.05, (agora - lastFrame) / 1000) : 0;
  lastFrame = agora;
  let animando = false;
  for (const player of corpoPlayers.values()) {
    if (!player.ativo || !player.skeleton || !player.animar) continue;
    animando = true;
    avancarPose(player, dt);
    desenharEsqueleto(player);
  }
  loopRaf = animando ? requestAnimationFrame(tickCorpo) : 0;
}

function garantirLoop() {
  if (!loopRaf) {
    lastFrame = 0;
    loopRaf = requestAnimationFrame(tickCorpo);
  }
}

function garantirCanvas(host) {
  let canvas = host.querySelector('canvas.media-slot-canvas');
  if (!canvas) {
    host.replaceChildren();
    canvas = document.createElement('canvas');
    canvas.className = 'media-slot-canvas';
    canvas.setAttribute('aria-hidden', 'true');
    host.append(canvas);
  }
  const cssW = Math.max(1, Math.floor(host.clientWidth) || 280);
  const cssH = Math.max(1, Math.floor(host.clientHeight) || 240);
  const dpr = Math.min(3, Math.max(2, window.devicePixelRatio || 1));
  const width = Math.floor(cssW * dpr);
  const height = Math.floor(cssH * dpr);
  if (canvas.width !== width) canvas.width = width;
  if (canvas.height !== height) canvas.height = height;
  return canvas;
}

async function montarPlayer(host, conjunto, { animar, noLugar }) {
  const pacote = await carregarConjunto(conjunto);
  const canvas = garantirCanvas(host);
  spine.Bone.yDown = true;
  const skeleton = new spine.Skeleton(pacote.skeletonData);
  skeleton.setToSetupPose();
  if (pacote.skeletonData.defaultSkin) skeleton.setSkin(pacote.skeletonData.defaultSkin);
  const stateData = new spine.AnimationStateData(pacote.skeletonData);
  const animationState = new spine.AnimationState(stateData);
  const nome = primeiroNomeDeAnimacao(pacote.skeletonData);
  if (nome) animationState.setAnimationByName(0, nome, true);
  animationState.timeScale = animar ? 1 : 0;
  const player = {
    ativo: true,
    canvas,
    skeleton,
    state: animationState,
    animar,
    noLugar
  };
  if (nome) {
    skeleton.setToSetupPose();
    animationState.update(0);
    animationState.apply(skeleton);
    if (noLugar) {
      const raiz = skeleton.getRootBone();
      if (raiz) {
        raiz.x = raiz.data.x;
        raiz.y = raiz.data.y;
      }
    }
  }
  return player;
}

async function aplicarVersaoDoCorpo(personagemId, versaoId) {
  const host = document.querySelector(`[data-corpo-host="${personagemId}"]`);
  const character = (state.characters ?? []).find(item => item.id === personagemId);
  if (!host || !character) return;
  const geracao = (geracaoDoCorpo.get(personagemId) || 0) + 1;
  geracaoDoCorpo.set(personagemId, geracao);
  const slot = character.midias?.corpoInteiro;
  const anterior = corpoPlayers.get(personagemId);
  if (anterior) {
    anterior.ativo = false;
    corpoPlayers.delete(personagemId);
  }
  const aindaVigente = () => geracaoDoCorpo.get(personagemId) === geracao;
  try {
    if (slot?.status !== 'OK' || !slot.conjuntoIdle) {
      if (aindaVigente()) mostrarSemImagem(host);
      return;
    }
    let conjunto = slot.conjuntoIdle;
    let animar = false;
    let noLugar = false;
    if (versaoId === 'animado') {
      animar = true;
    } else if (versaoId === 'caminhada') {
      if (!slot.conjuntoWalk) {
        if (aindaVigente()) mostrarSemImagem(host);
        return;
      }
      conjunto = slot.conjuntoWalk;
      animar = true;
      noLugar = true;
    } else if (versaoId !== 'emEspera') {
      const extra = (slot.conjuntos ?? []).find(item => item.ciclo === versaoId);
      if (!extra) {
        if (aindaVigente()) mostrarSemImagem(host);
        return;
      }
      conjunto = extra;
      animar = true;
      noLugar = versaoId === 'walk' || versaoId === 'combat';
    }
    const player = await montarPlayer(host, conjunto, { animar, noLugar });
    if (!aindaVigente()) {
      player.ativo = false;
      return;
    }
    corpoPlayers.set(personagemId, player);
    desenharEsqueleto(player);
    if (animar) garantirLoop();
  } catch {
    if (aindaVigente()) mostrarSemImagem(host);
  }
}

function ligarSeletorDeCorpo(card, personagemId) {
  const seletor = card.querySelector(`[data-corpo-versao="${personagemId}"]`);
  if (!seletor) return;
  seletor.addEventListener('change', () => {
    aplicarVersaoDoCorpo(personagemId, seletor.value);
  });
}

function skillLines(skills, categoria) {
  const filtradas = (skills ?? []).filter(skill => (skill.categoria || '') === categoria);
  if (filtradas.length === 0) {
    return '';
  }
  return filtradas.map(skill => `
    <span class="skill-line">
      ${mediaSlot(skill.midia, skill.nome || 'Habilidade')}
      <span class="skill-name">${escapeHtml(skill.nome || 'Habilidade sem nome')}</span>
      <em class="skill-level">Nv. ${formatNumber(skill.numeroDoNivel ?? 0)}</em>
    </span>
  `).join('');
}

function renderSkills(skills) {
  pararPlayers();
  elements.list.innerHTML = '';
  elements.count.textContent = skills.length;
  for (const character of skills) {
    const card = document.createElement('article');
    card.className = 'character-card';
    const resistencias = character.resistencias ?? {};
    card.innerHTML = `
      <div class="card-top">
        <div>
          <p class="eyebrow">Herói</p>
          <h3 class="card-name">${escapeHtml(character.nome)}</h3>
          <span class="card-class">${escapeHtml(character.nomeClasse || character.classe)}</span>
        </div>
        <span class="count-badge" title="Nível">${character.nivel}</span>
      </div>
      <p class="card-meta">Arma ${formatNumber(character.nivelDaArma)} · Armadura ${formatNumber(character.nivelDaArmadura)} · Aparência ${escapeHtml(formatAparencia(character.aparencia))}</p>
      <div class="card-media">
        ${mediaSlot(character.midias?.retrato, 'Retrato')}
        ${corpoSlot(character.midias?.corpoInteiro, character.id)}
        ${mediaSlot(character.midias?.arma, 'Arma')}
        ${mediaSlot(character.midias?.armadura, 'Armadura')}
      </div>
      ${renderTrinketSlots(character)}
      <div class="card-category ficha-efetiva">
        <h3>HP (ficha efetiva)</h3>
        <div class="metric-row"><span class="metric-label">Atual / máximo</span><span class="metric-value metric-efetivo">${character.hpAtual} / ${character.hpMaximo}</span></div>
        <div class="metric-row metric-base"><span class="metric-label">Base</span><span class="metric-value">${formatNumber(character.fichaBase?.hpAtual)} / ${formatNumber(character.fichaBase?.hpMaximo)}</span></div>
      </div>
      <div class="card-category">
        <h3>Stress</h3>
        <div class="metric-row"><span class="metric-label">Stress</span><span class="metric-value">${character.stress}</span></div>
      </div>
      <div class="card-category ficha-efetiva">
        <h3>Atributos (ficha efetiva)</h3>
        ${metricComBase('Esquiva', character.esquiva, character.fichaBase?.esquiva)}
        ${metricComBase('Velocidade', character.velocidade, character.fichaBase?.velocidade)}
        ${metricComBase('Crítico', character.critico, character.fichaBase?.critico)}
        ${metricComBase('Dano', `${formatNumber(character.danoBaseMinimo)}–${formatNumber(character.danoBaseMaximo)}`, `${formatNumber(character.fichaBase?.danoBaseMinimo)}–${formatNumber(character.fichaBase?.danoBaseMaximo)}`)}
        ${metricComBase('Precisão', character.precisao, character.fichaBase?.precisao)}
        ${metricComBase('Proteção', character.protecao, character.fichaBase?.protecao)}
        <div class="metric-row"><span class="metric-label">Passos à frente</span><span class="metric-value">${formatNumber(character.passosAFrente)}</span></div>
        <div class="metric-row"><span class="metric-label">Passos atrás</span><span class="metric-value">${formatNumber(character.passosAtras)}</span></div>
      </div>
      <div class="card-category ficha-efetiva">
        <h3>Resistências (ficha efetiva)</h3>
        ${metricComBase('Atordoamento', resistencias.atordoamento, character.fichaBase?.resistencias?.atordoamento)}
        ${metricComBase('Sangramento', resistencias.sangramento, character.fichaBase?.resistencias?.sangramento)}
        ${metricComBase('Envenenamento', resistencias.envenenamento, character.fichaBase?.resistencias?.envenenamento)}
        ${metricComBase('Debuff', resistencias.debuff, character.fichaBase?.resistencias?.debuff)}
        ${metricComBase('Movimento', resistencias.movimento, character.fichaBase?.resistencias?.movimento)}
        ${metricComBase('Doença', resistencias.doenca, character.fichaBase?.resistencias?.doenca)}
        ${metricComBase('Golpe mortal', resistencias.golpeMortal, character.fichaBase?.resistencias?.golpeMortal)}
        ${metricComBase('Armadilha', resistencias.armadilha, character.fichaBase?.resistencias?.armadilha)}
      </div>
      <div class="card-skills"><h3>Habilidades de combate</h3>${skillLines(character.habilidades, 'Combate')}</div>
      <div class="card-skills"><h3>Habilidades de acampamento</h3>${skillLines(character.habilidades, 'Acampamento')}</div>
      <div class="card-category card-class-extras">
        <h3>Classe</h3>
        <div class="metric-row"><span class="metric-label">Religiosa</span><span class="metric-value">${character.classeReligiosa ? 'Sim' : 'Não'}</span></div>
        <div class="metric-row"><span class="metric-label">Provisão inicial</span><span class="metric-value">${escapeHtml(character.provisaoInicial || '—')}</span></div>
        <div class="metric-row"><span class="metric-label">Bônus ao crítico</span><span class="metric-value">${escapeHtml(character.bonusAoCriticoDaClasse || '—')}</span></div>
      </div>
      <div class="card-actions"><button class="delete-button" data-delete-id="${character.id}" type="button">Excluir personagem</button></div>
    `;
    elements.list.append(card);
    ligarSeletorDeCorpo(card, character.id);
    aplicarVersaoDoCorpo(character.id, 'emEspera');
    ligarSeletoresDeTrinket(card, character);
  }
}

function metricComBase(rotulo, efetivo, baseValor) {
  return `
    <div class="metric-row">
      <span class="metric-label">${escapeHtml(rotulo)}</span>
      <span class="metric-value metric-efetivo">${formatNumber(efetivo)}</span>
    </div>
    <div class="metric-row metric-base"><span class="metric-label">Base</span><span class="metric-value">${formatNumber(baseValor)}</span></div>
  `;
}

function rotuloRaridade(valor) {
  if (valor === null || valor === undefined || valor === '') return '';
  return String(valor);
}

function renderTrinketSlots(character) {
  return `
    <div class="card-category trinket-slots">
      <h3>Acessórios</h3>
      ${renderTrinketSlot(character, 1, character.espacoTrinket1)}
      ${renderTrinketSlot(character, 2, character.espacoTrinket2)}
    </div>
  `;
}

function renderTrinketSlot(character, espaco, slot) {
  const ocupado = Boolean(slot?.acessorioId);
  const estado = ocupado ? 'ocupado' : 'vazio';
  const rotulo = ocupado
    ? `${escapeHtml(slot.nomeExibicao || 'Acessório')} ${rotuloRaridade(slot.raridade)}`.trim()
    : 'Vazio';
  return `
    <label class="trinket-slot trinket-slot-${estado}">
      <span>Espaço ${espaco} — ${rotulo}</span>
      <select data-trinket-espaco="${espaco}" data-personagem-id="${escapeHtml(character.id)}" data-classe="${escapeHtml(character.classe)}">
        <option value="">Vazio</option>
      </select>
    </label>
  `;
}

async function ligarSeletoresDeTrinket(card, character) {
  const seletores = card.querySelectorAll('[data-trinket-espaco]');
  for (const seletor of seletores) {
    const espaco = Number(seletor.dataset.trinketEspaco);
    const outro = espaco === 1 ? character.espacoTrinket2 : character.espacoTrinket1;
    const atual = espaco === 1 ? character.espacoTrinket1 : character.espacoTrinket2;
    seletor.disabled = true;
    try {
      const params = new URLSearchParams({ classe: String(character.classe) });
      if (outro?.acessorioId) params.set('excluirId', outro.acessorioId);
      const lista = await requestJson(`/acessorios?${params.toString()}`);
      seletor.innerHTML = '<option value="">Vazio</option>';
      if (!lista?.length) {
        const vazio = document.createElement('option');
        vazio.disabled = true;
        vazio.textContent = 'Nenhum trinket disponível para esta classe.';
        seletor.append(vazio);
      } else {
        for (const item of lista) {
          const option = document.createElement('option');
          option.value = item.id;
          option.textContent = `${item.nomeExibicao} (${item.raridade ?? '—'})`;
          seletor.append(option);
        }
      }
      if (atual?.acessorioId) {
        const existe = Array.from(seletor.options).some(o => o.value === atual.acessorioId);
        if (!existe) {
          const option = document.createElement('option');
          option.value = atual.acessorioId;
          option.textContent = atual.nomeExibicao || 'Acessório atual';
          seletor.append(option);
        }
        seletor.value = atual.acessorioId;
      }
    } catch {
      seletor.innerHTML = '<option value="">Não foi possível carregar trinkets</option>';
    } finally {
      seletor.disabled = false;
    }

    seletor.addEventListener('change', async () => {
      if (state.saving) return;
      const valor = seletor.value || null;
      const controles = card.querySelectorAll('[data-trinket-espaco]');
      controles.forEach(el => { el.disabled = true; });
      try {
        await requestJson(`/personagens/${character.id}/acessorios/${espaco}`, {
          method: 'PUT',
          body: JSON.stringify({ acessorioId: valor })
        });
        await loadCharacters('Acessório atualizado.');
      } catch (error) {
        setStatus(error.message);
        seletor.value = atual?.acessorioId || '';
        controles.forEach(el => { el.disabled = false; });
      }
    });
  }
}

function escapeHtml(value) {
  return String(value)
    .replaceAll('&', '&amp;')
    .replaceAll('<', '&lt;')
    .replaceAll('>', '&gt;')
    .replaceAll('"', '&quot;')
    .replaceAll("'", '&#039;');
}

async function loadCharacters(statusApos) {
  pararPlayers();
  state.loadingList = true;
  state.characters = null;
  showListState();
  if (!statusApos) setStatus('Consultando personagens...');
  try {
    state.characters = await requestJson('/personagens');
    renderSkills(state.characters);
    setStatus(statusApos || `${state.characters.length} personagem(ns) encontrado(s).`);
  } catch (error) {
    state.characters = null;
    elements.errorMessage.textContent = error.message;
    setStatus('A consulta falhou.');
  } finally {
    state.loadingList = false;
    showListState();
  }
}

async function loadClasses() {
  if (state.classes.length > 0) return;
  const classes = await requestJson('/classes');
  state.classes = classes;
  elements.classSelect.innerHTML = '<option value="">Selecione uma classe</option>';
  for (const item of classes) {
    const option = document.createElement('option');
    option.value = item.classe;
    option.textContent = item.nomeExibicao;
    elements.classSelect.append(option);
  }
}

function openDialog() {
  elements.form.reset();
  elements.formError.textContent = '';
  setHidden(elements.formError, true);
  elements.dialog.showModal();
  loadClasses().catch(error => {
    elements.formError.textContent = `Não foi possível carregar as classes: ${error.message}`;
    setHidden(elements.formError, false);
  });
}

function closeDialog() {
  if (!state.saving) elements.dialog.close();
}

function readForm() {
  const formData = new FormData(elements.form);
  const payload = Object.fromEntries(formData.entries());
  for (const field of numericFields) payload[field] = Number(payload[field]);
  payload.classe = Number(payload.classe);
  return payload;
}

function validateForm(payload) {
  const errors = {};
  if (!payload.nome?.trim()) errors.nome = 'Informe o nome do personagem.';
  if (Number.isNaN(payload.classe)) errors.classe = 'Selecione uma classe.';
  if (payload.nivel < 0 || payload.nivel > 6) errors.nivel = 'Nível deve ficar entre 0 e 6.';
  if (payload.nivelDaArma < 1 || payload.nivelDaArma > 5) errors.nivelDaArma = 'Nível da arma deve ficar entre 1 e 5.';
  if (payload.nivelDaArmadura < 1 || payload.nivelDaArmadura > 5) errors.nivelDaArmadura = 'Nível da armadura deve ficar entre 1 e 5.';
  return errors;
}

function showFormErrors(errors) {
  document.querySelectorAll('[data-error-for]').forEach(element => {
    element.textContent = errors[element.dataset.errorFor] || '';
  });
  const messages = Object.values(errors);
  if (messages.length > 0) {
    elements.formError.textContent = messages.join(' ');
    setHidden(elements.formError, false);
  }
}

async function saveCharacter(event) {
  event.preventDefault();
  if (state.saving) return;
  const payload = readForm();
  const errors = validateForm(payload);
  showFormErrors(errors);
  if (Object.keys(errors).length > 0) return;

  state.saving = true;
  elements.save.disabled = true;
  elements.save.textContent = 'Salvando...';
  try {
    await requestJson('/personagens', { method: 'POST', body: JSON.stringify(payload) });
    elements.dialog.close();
    await loadCharacters();
    setStatus('Personagem criado com sucesso.');
  } catch (error) {
    elements.formError.textContent = error.message;
    setHidden(elements.formError, false);
  } finally {
    state.saving = false;
    elements.save.disabled = false;
    elements.save.textContent = 'Salvar personagem';
  }
}

async function seedHeroes() {
  if (state.seeding || state.saving) return;
  state.seeding = true;
  if (elements.seedHeroes) elements.seedHeroes.disabled = true;
  setStatus('Cadastrando um herói de cada classe...');
  try {
    await loadClasses();
    if (!state.characters) {
      try {
        state.characters = await requestJson('/personagens');
      } catch {
        state.characters = [];
      }
    }
    const existentes = new Set((state.characters ?? []).map(item => String(item.classe)));
    let criados = 0;
    let pulados = 0;
    const falhas = [];
    for (const item of state.classes) {
      const classe = item.classe;
      if (existentes.has(String(classe))) {
        pulados += 1;
        continue;
      }
      try {
        await requestJson('/personagens', {
          method: 'POST',
          body: JSON.stringify({
            nome: item.nomeExibicao,
            classe,
            nivel: 0,
            nivelDaArma: 1,
            nivelDaArmadura: 1,
            aparencia: 0
          })
        });
        criados += 1;
        existentes.add(String(classe));
      } catch (error) {
        falhas.push(`${item.nomeExibicao}: ${error.message}`);
      }
    }
    const partes = [`${criados} herói(s) cadastrado(s).`];
    if (pulados) partes.push(`${pulados} classe(s) já tinham personagem.`);
    if (falhas.length) partes.push(`${falhas.length} falha(s): ${falhas.join(' · ')}`);
    await loadCharacters(partes.join(' '));
  } catch (error) {
    setStatus(error.message);
  } finally {
    state.seeding = false;
    if (elements.seedHeroes) elements.seedHeroes.disabled = false;
  }
}

async function deleteCharacter(id) {
  if (state.deleting || !window.confirm('Excluir este personagem? Esta ação não pode ser desfeita.')) return;
  state.deleting = true;
  try {
    await requestJson(`/personagens/${id}`, { method: 'DELETE' });
    setStatus('Personagem excluído com sucesso.');
    await loadCharacters();
  } catch (error) {
    setStatus(error.status === 404 ? 'O personagem já não existe.' : error.message);
  } finally {
    state.deleting = false;
  }
}

document.querySelector('#open-create').addEventListener('click', openDialog);
if (elements.seedHeroes) {
  elements.seedHeroes.addEventListener('click', seedHeroes);
}
document.querySelector('#open-create-empty').addEventListener('click', openDialog);
document.querySelector('#close-dialog').addEventListener('click', closeDialog);
document.querySelector('#cancel-dialog').addEventListener('click', closeDialog);
document.querySelector('#retry-list').addEventListener('click', loadCharacters);
document.querySelector('#retry-error').addEventListener('click', loadCharacters);
elements.form.addEventListener('submit', saveCharacter);
elements.list.addEventListener('click', event => {
  const button = event.target.closest('[data-delete-id]');
  if (button) deleteCharacter(button.dataset.deleteId);
});

loadCharacters();
