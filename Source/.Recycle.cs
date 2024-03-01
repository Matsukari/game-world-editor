void DrawGridLines(int tw, int th)
    {
      float w = tw * ContentZoom;
      float h = th * ContentZoom;
      var (min, max) = ImUtils.GetWindowArea();
      var drawList = ImGui.GetWindowDrawList();
      int cols = (int)(_textureSize.X / tw);
      int rows = (int)(_textureSize.Y / th);
      uint color = ImGui.ColorConvertFloat4ToU32(new Num.Vector4(0.3f, 0.3f, 0.3f, 0.3f));
      for (int x = 0; x <= cols; x++) 
      { 
        float xx = x;
        drawList.AddLine(
            min + new Num.Vector2(_imagePosition.X + xx * w, _imagePosition.Y), 
            min + new Num.Vector2(_imagePosition.X + xx * w, _imagePosition.Y + rows * h), color);
        for (int y = 0; y <= rows; y++) 
        {
          float yy = y;
          drawList.AddLine(
              min + new Num.Vector2(_imagePosition.X, _imagePosition.Y + y * h), 
              min + new Num.Vector2(_imagePosition.X + cols * w, _imagePosition.Y + y * h), color);
        }
      }
    }


    void DrawPopups()
    {
      if (ImGui.GetIO().MouseReleased[1] && _selSprite is TiledSpriteData) 
      {
        ImGui.OpenPopup("sprite-editing", ImGuiPopupFlags.NoOpenOverExistingPopup);
        isSpriteOptions = true;
      }
      if (isSpriteOptions && _selSprite is TiledSpriteData tile)
      {
        ImGui.BeginPopup("sprite-editing");
        if (ImGui.MenuItem("Create ComplexSprite")) SpriteSheet.AddSprite(tile);
        ImGui.EndPopup();
      }
    }
