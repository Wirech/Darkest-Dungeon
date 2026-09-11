/* Port of Spine 2.1.25 SkeletonBinary (C#) for the vendored spine-js 2.1 runtime. */
(function (spine) {
  var TIMELINE_SCALE = 0;
  var TIMELINE_ROTATE = 1;
  var TIMELINE_TRANSLATE = 2;
  var TIMELINE_ATTACHMENT = 3;
  var TIMELINE_COLOR = 4;
  var TIMELINE_FLIPX = 5;
  var TIMELINE_FLIPY = 6;
  var CURVE_LINEAR = 0;
  var CURVE_STEPPED = 1;
  var CURVE_BEZIER = 2;

  function SkeletonBinary(attachmentLoader) {
    this.attachmentLoader = attachmentLoader;
    this.scale = 1;
  }

  SkeletonBinary.prototype.readSkeletonData = function (arrayBuffer) {
    var input = new BinaryInput(arrayBuffer);
    var scale = this.scale;
    var skeletonData = new spine.SkeletonData();
    skeletonData.hash = input.readString();
    if (!skeletonData.hash) skeletonData.hash = null;
    skeletonData.version = input.readString();
    if (!skeletonData.version) skeletonData.version = null;
    skeletonData.width = input.readFloat();
    skeletonData.height = input.readFloat();
    var nonessential = input.readBoolean();
    if (nonessential) {
      input.readString();
    }

    for (var i = 0, n = input.readInt(true); i < n; i++) {
      var name = input.readString();
      var parent = null;
      var parentIndex = input.readInt(true) - 1;
      if (parentIndex !== -1) parent = skeletonData.bones[parentIndex];
      var boneData = new spine.BoneData(name, parent);
      boneData.x = input.readFloat() * scale;
      boneData.y = input.readFloat() * scale;
      boneData.scaleX = input.readFloat();
      boneData.scaleY = input.readFloat();
      boneData.rotation = input.readFloat();
      boneData.length = input.readFloat() * scale;
      boneData.flipX = input.readBoolean();
      boneData.flipY = input.readBoolean();
      boneData.inheritScale = input.readBoolean();
      boneData.inheritRotation = input.readBoolean();
      if (nonessential) input.readInt32();
      skeletonData.bones.push(boneData);
    }

    for (i = 0, n = input.readInt(true); i < n; i++) {
      var ikConstraintData = new spine.IkConstraintData(input.readString());
      for (var ii = 0, nn = input.readInt(true); ii < nn; ii++)
        ikConstraintData.bones.push(skeletonData.bones[input.readInt(true)]);
      ikConstraintData.target = skeletonData.bones[input.readInt(true)];
      ikConstraintData.mix = input.readFloat();
      ikConstraintData.bendDirection = input.readSByte();
      skeletonData.ikConstraints.push(ikConstraintData);
    }

    for (i = 0, n = input.readInt(true); i < n; i++) {
      var slotName = input.readString();
      var boneForSlot = skeletonData.bones[input.readInt(true)];
      var slotData = new spine.SlotData(slotName, boneForSlot);
      var color = input.readInt32();
      slotData.r = ((color >>> 24) & 0xff) / 255;
      slotData.g = ((color >>> 16) & 0xff) / 255;
      slotData.b = ((color >>> 8) & 0xff) / 255;
      slotData.a = (color & 0xff) / 255;
      slotData.attachmentName = input.readString();
      slotData.additiveBlending = input.readBoolean();
      skeletonData.slots.push(slotData);
    }

    var defaultSkin = this.readSkin(input, "default", nonessential);
    if (defaultSkin) {
      skeletonData.defaultSkin = defaultSkin;
      skeletonData.skins.push(defaultSkin);
    }
    for (i = 0, n = input.readInt(true); i < n; i++)
      skeletonData.skins.push(this.readSkin(input, input.readString(), nonessential));

    for (i = 0, n = input.readInt(true); i < n; i++) {
      var eventData = new spine.EventData(input.readString());
      eventData.intValue = input.readInt(false);
      eventData.floatValue = input.readFloat();
      eventData.stringValue = input.readString();
      skeletonData.events.push(eventData);
    }

    for (i = 0, n = input.readInt(true); i < n; i++)
      this.readAnimation(input.readString(), input, skeletonData);

    return skeletonData;
  };

  SkeletonBinary.prototype.readSkin = function (input, skinName, nonessential) {
    var slotCount = input.readInt(true);
    if (slotCount === 0) return null;
    var skin = new spine.Skin(skinName);
    for (var i = 0; i < slotCount; i++) {
      var slotIndex = input.readInt(true);
      for (var ii = 0, nn = input.readInt(true); ii < nn; ii++) {
        var name = input.readString();
        skin.addAttachment(slotIndex, name, this.readAttachment(input, skin, name, nonessential));
      }
    }
    return skin;
  };

  SkeletonBinary.prototype.readAttachment = function (input, skin, attachmentName, nonessential) {
    var scale = this.scale;
    var name = input.readString();
    if (name == null) name = attachmentName;
    var type = input.readByte();
    if (type === spine.AttachmentType.region) {
      var path = input.readString();
      if (path == null) path = name;
      var region = this.attachmentLoader.newRegionAttachment(skin, name, path);
      if (!region) return null;
      region.path = path;
      region.x = input.readFloat() * scale;
      region.y = input.readFloat() * scale;
      region.scaleX = input.readFloat();
      region.scaleY = input.readFloat();
      region.rotation = input.readFloat();
      region.width = input.readFloat() * scale;
      region.height = input.readFloat() * scale;
      var regionColor = input.readInt32();
      region.r = ((regionColor >>> 24) & 0xff) / 255;
      region.g = ((regionColor >>> 16) & 0xff) / 255;
      region.b = ((regionColor >>> 8) & 0xff) / 255;
      region.a = (regionColor & 0xff) / 255;
      region.updateOffset();
      return region;
    }
    if (type === spine.AttachmentType.boundingbox) {
      var box = this.attachmentLoader.newBoundingBoxAttachment(skin, name);
      if (!box) return null;
      box.vertices = input.readFloatArray(scale);
      return box;
    }
    if (type === spine.AttachmentType.mesh) {
      path = input.readString();
      if (path == null) path = name;
      var mesh = this.attachmentLoader.newMeshAttachment(skin, name, path);
      if (!mesh) return null;
      mesh.path = path;
      mesh.regionUVs = input.readFloatArray(1);
      mesh.triangles = input.readShortArray();
      mesh.vertices = input.readFloatArray(scale);
      mesh.updateUVs();
      var meshColor = input.readInt32();
      mesh.r = ((meshColor >>> 24) & 0xff) / 255;
      mesh.g = ((meshColor >>> 16) & 0xff) / 255;
      mesh.b = ((meshColor >>> 8) & 0xff) / 255;
      mesh.a = (meshColor & 0xff) / 255;
      mesh.hullLength = input.readInt(true) * 2;
      if (nonessential) {
        mesh.edges = input.readIntArray();
        mesh.width = input.readFloat() * scale;
        mesh.height = input.readFloat() * scale;
      }
      return mesh;
    }
    if (type === spine.AttachmentType.skinnedmesh) {
      path = input.readString();
      if (path == null) path = name;
      mesh = this.attachmentLoader.newSkinnedMeshAttachment(skin, name, path);
      if (!mesh) return null;
      mesh.path = path;
      var uvs = input.readFloatArray(1);
      var triangles = input.readShortArray();
      var vertexCount = input.readInt(true);
      var weights = [];
      var bones = [];
      for (var i = 0; i < vertexCount; i++) {
        var boneCount = input.readFloat() | 0;
        bones.push(boneCount);
        for (var nn = i + boneCount * 4; i < nn; i += 4) {
          bones.push(input.readFloat() | 0);
          weights.push(input.readFloat() * scale);
          weights.push(input.readFloat() * scale);
          weights.push(input.readFloat());
        }
      }
      mesh.bones = bones;
      mesh.weights = weights;
      mesh.triangles = triangles;
      mesh.regionUVs = uvs;
      mesh.updateUVs();
      meshColor = input.readInt32();
      mesh.r = ((meshColor >>> 24) & 0xff) / 255;
      mesh.g = ((meshColor >>> 16) & 0xff) / 255;
      mesh.b = ((meshColor >>> 8) & 0xff) / 255;
      mesh.a = (meshColor & 0xff) / 255;
      mesh.hullLength = input.readInt(true) * 2;
      if (nonessential) {
        mesh.edges = input.readIntArray();
        mesh.width = input.readFloat() * scale;
        mesh.height = input.readFloat() * scale;
      }
      return mesh;
    }
    return null;
  };

  SkeletonBinary.prototype.readAnimation = function (name, input, skeletonData) {
    var timelines = [];
    var scale = this.scale;
    var duration = 0;
    var i, n, ii, nn, iii, nnn, frameIndex, frameCount, timelineType;

    for (i = 0, n = input.readInt(true); i < n; i++) {
      var slotIndex = input.readInt(true);
      for (ii = 0, nn = input.readInt(true); ii < nn; ii++) {
        timelineType = input.readByte();
        frameCount = input.readInt(true);
        if (timelineType === TIMELINE_COLOR) {
          var colorTimeline = new spine.ColorTimeline(frameCount);
          colorTimeline.slotIndex = slotIndex;
          for (frameIndex = 0; frameIndex < frameCount; frameIndex++) {
            var time = input.readFloat();
            var color = input.readInt32();
            colorTimeline.setFrame(
              frameIndex,
              time,
              ((color >>> 24) & 0xff) / 255,
              ((color >>> 16) & 0xff) / 255,
              ((color >>> 8) & 0xff) / 255,
              (color & 0xff) / 255
            );
            if (frameIndex < frameCount - 1) readCurve(input, frameIndex, colorTimeline);
          }
          timelines.push(colorTimeline);
          duration = Math.max(duration, colorTimeline.frames[frameCount * 5 - 5]);
        } else if (timelineType === TIMELINE_ATTACHMENT) {
          var attachmentTimeline = new spine.AttachmentTimeline(frameCount);
          attachmentTimeline.slotIndex = slotIndex;
          for (frameIndex = 0; frameIndex < frameCount; frameIndex++)
            attachmentTimeline.setFrame(frameIndex, input.readFloat(), input.readString());
          timelines.push(attachmentTimeline);
          duration = Math.max(duration, attachmentTimeline.frames[frameCount - 1]);
        }
      }
    }

    for (i = 0, n = input.readInt(true); i < n; i++) {
      var boneIndex = input.readInt(true);
      for (ii = 0, nn = input.readInt(true); ii < nn; ii++) {
        timelineType = input.readByte();
        frameCount = input.readInt(true);
        if (timelineType === TIMELINE_ROTATE) {
          var rotateTimeline = new spine.RotateTimeline(frameCount);
          rotateTimeline.boneIndex = boneIndex;
          for (frameIndex = 0; frameIndex < frameCount; frameIndex++) {
            rotateTimeline.setFrame(frameIndex, input.readFloat(), input.readFloat());
            if (frameIndex < frameCount - 1) readCurve(input, frameIndex, rotateTimeline);
          }
          timelines.push(rotateTimeline);
          duration = Math.max(duration, rotateTimeline.frames[frameCount * 2 - 2]);
        } else if (timelineType === TIMELINE_TRANSLATE || timelineType === TIMELINE_SCALE) {
          var translateTimeline = timelineType === TIMELINE_SCALE
            ? new spine.ScaleTimeline(frameCount)
            : new spine.TranslateTimeline(frameCount);
          var timelineScale = timelineType === TIMELINE_SCALE ? 1 : scale;
          translateTimeline.boneIndex = boneIndex;
          for (frameIndex = 0; frameIndex < frameCount; frameIndex++) {
            translateTimeline.setFrame(
              frameIndex,
              input.readFloat(),
              input.readFloat() * timelineScale,
              input.readFloat() * timelineScale
            );
            if (frameIndex < frameCount - 1) readCurve(input, frameIndex, translateTimeline);
          }
          timelines.push(translateTimeline);
          duration = Math.max(duration, translateTimeline.frames[frameCount * 3 - 3]);
        } else if (timelineType === TIMELINE_FLIPX || timelineType === TIMELINE_FLIPY) {
          var flipTimeline = timelineType === TIMELINE_FLIPX
            ? new spine.FlipXTimeline(frameCount)
            : new spine.FlipYTimeline(frameCount);
          flipTimeline.boneIndex = boneIndex;
          for (frameIndex = 0; frameIndex < frameCount; frameIndex++)
            flipTimeline.setFrame(frameIndex, input.readFloat(), input.readBoolean());
          timelines.push(flipTimeline);
          duration = Math.max(duration, flipTimeline.frames[frameCount * 2 - 2]);
        }
      }
    }

    for (i = 0, n = input.readInt(true); i < n; i++) {
      var ikConstraint = skeletonData.ikConstraints[input.readInt(true)];
      frameCount = input.readInt(true);
      var ikTimeline = new spine.IkConstraintTimeline(frameCount);
      ikTimeline.ikConstraintIndex = skeletonData.ikConstraints.indexOf(ikConstraint);
      for (frameIndex = 0; frameIndex < frameCount; frameIndex++) {
        ikTimeline.setFrame(frameIndex, input.readFloat(), input.readFloat(), input.readSByte());
        if (frameIndex < frameCount - 1) readCurve(input, frameIndex, ikTimeline);
      }
      timelines.push(ikTimeline);
      duration = Math.max(duration, ikTimeline.frames[frameCount * 3 - 3]);
    }

    for (i = 0, n = input.readInt(true); i < n; i++) {
      var skin = skeletonData.skins[input.readInt(true)];
      for (ii = 0, nn = input.readInt(true); ii < nn; ii++) {
        slotIndex = input.readInt(true);
        for (iii = 0, nnn = input.readInt(true); iii < nnn; iii++) {
          var attachment = skin.getAttachment(slotIndex, input.readString());
          frameCount = input.readInt(true);
          var ffdTimeline = new spine.FfdTimeline(frameCount);
          ffdTimeline.slotIndex = slotIndex;
          ffdTimeline.attachment = attachment;
          for (frameIndex = 0; frameIndex < frameCount; frameIndex++) {
            time = input.readFloat();
            var vertexCount = !attachment
              ? 0
              : attachment.type === spine.AttachmentType.mesh
                ? attachment.vertices.length
                : attachment.weights.length / 3 * 2;
            var end = input.readInt(true);
            var vertices;
            if (end === 0) {
              vertices = attachment.type === spine.AttachmentType.mesh
                ? attachment.vertices
                : new Array(vertexCount);
              if (attachment.type !== spine.AttachmentType.mesh) {
                for (var z = 0; z < vertexCount; z++) vertices[z] = 0;
              }
            } else {
              vertices = new Array(vertexCount);
              for (z = 0; z < vertexCount; z++) vertices[z] = 0;
              var start = input.readInt(true);
              end += start;
              for (var v = start; v < end; v++)
                vertices[v] = input.readFloat() * (scale === 1 ? 1 : scale);
              if (attachment.type === spine.AttachmentType.mesh) {
                var meshVertices = attachment.vertices;
                for (v = 0; v < vertices.length; v++) vertices[v] += meshVertices[v];
              }
            }
            ffdTimeline.setFrame(frameIndex, time, vertices);
            if (frameIndex < frameCount - 1) readCurve(input, frameIndex, ffdTimeline);
          }
          timelines.push(ffdTimeline);
          duration = Math.max(duration, ffdTimeline.frames[frameCount - 1]);
        }
      }
    }

    var drawOrderCount = input.readInt(true);
    if (drawOrderCount > 0) {
      var drawOrderTimeline = new spine.DrawOrderTimeline(drawOrderCount);
      var slotCount = skeletonData.slots.length;
      for (i = 0; i < drawOrderCount; i++) {
        var offsetCount = input.readInt(true);
        var drawOrder = new Array(slotCount);
        for (ii = slotCount - 1; ii >= 0; ii--) drawOrder[ii] = -1;
        var unchanged = new Array(slotCount - offsetCount);
        var originalIndex = 0, unchangedIndex = 0;
        for (ii = 0; ii < offsetCount; ii++) {
          slotIndex = input.readInt(true);
          while (originalIndex !== slotIndex) unchanged[unchangedIndex++] = originalIndex++;
          drawOrder[originalIndex + input.readInt(true)] = originalIndex++;
        }
        while (originalIndex < slotCount) unchanged[unchangedIndex++] = originalIndex++;
        for (ii = slotCount - 1; ii >= 0; ii--)
          if (drawOrder[ii] === -1) drawOrder[ii] = unchanged[--unchangedIndex];
        drawOrderTimeline.setFrame(i, input.readFloat(), drawOrder);
      }
      timelines.push(drawOrderTimeline);
      duration = Math.max(duration, drawOrderTimeline.frames[drawOrderCount - 1]);
    }

    var eventCount = input.readInt(true);
    if (eventCount > 0) {
      var eventTimeline = new spine.EventTimeline(eventCount);
      for (i = 0; i < eventCount; i++) {
        time = input.readFloat();
        var eventData = skeletonData.events[input.readInt(true)];
        var event = new spine.Event(eventData);
        event.intValue = input.readInt(false);
        event.floatValue = input.readFloat();
        event.stringValue = input.readBoolean() ? input.readString() : eventData.stringValue;
        eventTimeline.setFrame(i, time, event);
      }
      timelines.push(eventTimeline);
      duration = Math.max(duration, eventTimeline.frames[eventCount - 1]);
    }

    skeletonData.animations.push(new spine.Animation(name, timelines, duration));
  };

  function readCurve(input, frameIndex, timeline) {
    switch (input.readByte()) {
      case CURVE_STEPPED:
        timeline.curves.setStepped(frameIndex);
        break;
      case CURVE_BEZIER:
        timeline.curves.setCurve(frameIndex, input.readFloat(), input.readFloat(), input.readFloat(), input.readFloat());
        break;
      default:
        timeline.curves.setLinear(frameIndex);
        break;
    }
  }

  function BinaryInput(arrayBuffer) {
    this.view = new DataView(arrayBuffer);
    this.bytes = new Uint8Array(arrayBuffer);
    this.index = 0;
  }

  BinaryInput.prototype.readByte = function () {
    return this.bytes[this.index++];
  };

  BinaryInput.prototype.readSByte = function () {
    var value = this.readByte();
    return value > 127 ? value - 256 : value;
  };

  BinaryInput.prototype.readBoolean = function () {
    return this.readByte() !== 0;
  };

  BinaryInput.prototype.readFloat = function () {
    var value = this.view.getFloat32(this.index, false);
    this.index += 4;
    return value;
  };

  BinaryInput.prototype.readInt32 = function () {
    var value = this.view.getInt32(this.index, false);
    this.index += 4;
    return value >>> 0;
  };

  BinaryInput.prototype.readInt = function (optimizePositive) {
    if (optimizePositive === undefined) return this.readInt32();
    var b = this.readByte();
    var result = b & 0x7F;
    if ((b & 0x80) !== 0) {
      b = this.readByte();
      result |= (b & 0x7F) << 7;
      if ((b & 0x80) !== 0) {
        b = this.readByte();
        result |= (b & 0x7F) << 14;
        if ((b & 0x80) !== 0) {
          b = this.readByte();
          result |= (b & 0x7F) << 21;
          if ((b & 0x80) !== 0) {
            b = this.readByte();
            result |= (b & 0x7F) << 28;
          }
        }
      }
    }
    return optimizePositive ? result : ((result >>> 1) ^ -(result & 1));
  };

  BinaryInput.prototype.readString = function () {
    var charCount = this.readInt(true);
    if (charCount === 0) return null;
    if (charCount === 1) return "";
    charCount--;
    var chars = [];
    var b = 0;
    var charIndex = 0;
    while (charIndex < charCount) {
      b = this.readByte();
      if (b > 127) break;
      chars[charIndex++] = String.fromCharCode(b);
    }
    if (charIndex < charCount) this.readUtf8Slow(chars, charCount, charIndex, b);
    return chars.join("");
  };

  BinaryInput.prototype.readUtf8Slow = function (chars, charCount, charIndex, b) {
    while (true) {
      switch (b >> 4) {
        case 0: case 1: case 2: case 3: case 4: case 5: case 6: case 7:
          chars[charIndex] = String.fromCharCode(b);
          break;
        case 12: case 13:
          chars[charIndex] = String.fromCharCode((b & 0x1F) << 6 | this.readByte() & 0x3F);
          break;
        case 14:
          chars[charIndex] = String.fromCharCode((b & 0x0F) << 12 | (this.readByte() & 0x3F) << 6 | this.readByte() & 0x3F);
          break;
      }
      if (++charIndex >= charCount) break;
      b = this.readByte() & 0xFF;
    }
  };

  BinaryInput.prototype.readFloatArray = function (scale) {
    var n = this.readInt(true);
    var array = new Array(n);
    for (var i = 0; i < n; i++) array[i] = this.readFloat() * scale;
    return array;
  };

  BinaryInput.prototype.readShortArray = function () {
    var n = this.readInt(true);
    var array = new Array(n);
    for (var i = 0; i < n; i++) array[i] = (this.readByte() << 8) + this.readByte();
    return array;
  };

  BinaryInput.prototype.readIntArray = function () {
    var n = this.readInt(true);
    var array = new Array(n);
    for (var i = 0; i < n; i++) array[i] = this.readInt(true);
    return array;
  };

  if (spine.FlipXTimeline) {
    spine.FlipXTimeline.prototype.apply = function (skeleton, lastTime, time, firedEvents, alpha) {
      var frames = this.frames;
      if (time < frames[0]) {
        if (lastTime > time) this.apply(skeleton, lastTime, Number.MAX_VALUE, null, 0);
        return;
      } else if (lastTime > time) lastTime = -1;
      var frameIndex = (time >= frames[frames.length - 2] ? frames.length : spine.Animation.binarySearch(frames, time, 2)) - 2;
      if (frames[frameIndex] < lastTime) return;
      skeleton.bones[this.boneIndex].flipX = frames[frameIndex + 1] != 0;
    };
  }
  if (spine.FlipYTimeline) {
    spine.FlipYTimeline.prototype.apply = function (skeleton, lastTime, time, firedEvents, alpha) {
      var frames = this.frames;
      if (time < frames[0]) {
        if (lastTime > time) this.apply(skeleton, lastTime, Number.MAX_VALUE, null, 0);
        return;
      } else if (lastTime > time) lastTime = -1;
      var frameIndex = (time >= frames[frames.length - 2] ? frames.length : spine.Animation.binarySearch(frames, time, 2)) - 2;
      if (frames[frameIndex] < lastTime) return;
      skeleton.bones[this.boneIndex].flipY = frames[frameIndex + 1] != 0;
    };
  }

  spine.SkeletonBinary = SkeletonBinary;
})(spine);
