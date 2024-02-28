using ImGuiNET;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Nez;
using Nez.ImGuiTools;
using Num = System.Numerics;

namespace Tools
{
  public class SpriteSheetEditingManager : Component {

  }
  /// <summary>
  /// Operates on a single spritesheet 
  /// </summary>
	public class SpriteSheetEditor : Component
	{
		public static string AtlasExportFolder = "../../Content/Atlases";

		string _imageFilename;
		string _sourceAtlasFile;

		private Num.Vector2 _textureSize;
		private float _textureAspectRatio;
		private IntPtr _texturePtr;

		private float _imageZoom = 1;
		private Num.Vector2 _imagePosition;

		private SpriteSheetData _spriteSheetData;
		public int TileWidth = 16, TileHeight = 16;
  
    private Object _selSprite;

    bool isAutoRegioning = false;
    bool isSpriteOptions = false;
    bool isDrag = false;
    Vector2 startDrag = new Vector2();
    Vector2 startPos = new Vector2();

    ImFontPtr _font;
		public SpriteSheetEditor()
		{
      _imageFilename = "Assets/Raw/Unprocessed/export/test_canvas.png";
      LoadTexture();
		}
    public override void OnAddedToEntity()
    {
      Core.GetGlobalManager<ImGuiManager>().RegisterDrawCommand(Show);
      _font = ImGui.GetIO().Fonts.AddFontFromFileTTF("Assets/Raw/Unprocessed/Roboto-Regular.ttf", 13.0f);
    }
            
		public void Show()
		{
      bool isOpen = true;
			if (ImGui.Begin("Sprite Atlas Editor", ref isOpen, ImGuiWindowFlags.MenuBar))
			{ 
				DrawMenuBar(); 
				DrawSprites();
        if (_spriteSheetData.TileWidth > 0) DrawGridLines(_spriteSheetData.TileWidth, _spriteSheetData.TileHeight);
        ImGui.End();
			}
      DrawPopups();      
      if (isAutoRegioning) DrawAutoRegionPopup();  
      if (_spriteSheetData != null) DrawPropertiesPane(_spriteSheetData, _spriteSheetData.Name);
      if (_selSprite is ComplexSpriteData sel) DrawPropertiesPane(sel, sel.Name);
      else if (_selSprite is TiledSpriteData tile) DrawPropertiesPane(tile, tile.Name);
      
      DrawSpritesPane();
      
      DrawAnimationPane();

		}
    void DrawPopups()
    {
      if (ImGui.GetIO().MouseReleased[1] && _selSprite is TiledSpriteData) 
      {
        ImGui.OpenPopup("sprite-editing");
        isSpriteOptions = true;
      }
      if (isSpriteOptions && _selSprite is TiledSpriteData tile)
      {
        ImGui.BeginPopup("sprite-editing");
        if (ImGui.MenuItem("Create ComplexSprite")) _spriteSheetData.AddSprite(tile);
        ImGui.EndPopup();
      }


    }
    void DrawSprites()
    {
      var frame = ImGui.GetStyle().FramePadding.X + ImGui.GetStyle().FrameBorderSize;
      ImGui.BeginChild("Spritesheet view", new Num.Vector2(ImGui.GetContentRegionAvail().X - 0, 0), false, 
          ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse);
      
      if (ImUtils.IsMouseAt(ImUtils.GetWindowRect()) && !isAutoRegioning && !isSpriteOptions) ImGui.SetWindowFocus();
      if (ImGui.IsWindowFocused())
      {
        var (windowMin, windowMax) = ImUtils.GetWindowArea();
        ImGui.GetForegroundDrawList().AddRect(windowMin, windowMax, ImUtils.GetColor(Color.CadetBlue));
        if (ImGui.GetIO().MouseWheel != 0) 
        {
          var minZoom = 0.2f;
          var maxZoom = 10f;

          var oldSize = _imageZoom * _textureSize;
          var zoomSpeed = _imageZoom * 0.2f;
          _imageZoom += Math.Min(maxZoom - _imageZoom, ImGui.GetIO().MouseWheel * zoomSpeed);
          _imageZoom = Mathf.Clamp(_imageZoom, minZoom, maxZoom);

          // zoom in, move up/left, zoom out the opposite
          var deltaSize = oldSize - (_imageZoom * _textureSize);
          _imagePosition += deltaSize * 0.5f;
        }
        if (ImGui.GetIO().MouseClicked[0] || ImGui.GetIO().MouseClicked[1])
        {
          if (_selSprite == null) 
          {
            foreach (var (tileId, tileData) in _spriteSheetData.Tiles)
            {
              RectangleF editorTileRegion = new RectangleF(
                  (float)tileData.Region.X, (float)tileData.Region.Y, 
                  (float)tileData.Region.Width, (float)tileData.Region.Height);
              if (ImUtils.HasMouseClickAt(editorTileRegion, _imageZoom, _imagePosition)) _selSprite = tileData;
            }
          }
          else _selSprite = null;
        }
        var dragSpeed = 0.9f;
        if (ImGui.GetIO().MouseDown[2] && !isDrag)
        {
          isDrag = true;
          startDrag = ImGui.GetIO().MousePos;
          startPos = _imagePosition;
          Console.WriteLine($"Start {startDrag}");
        }
        else if (ImGui.GetIO().MouseReleased[2]) {
          isDrag = false; 
          Console.WriteLine($"End {ImGui.GetIO().MousePos}");

        }
        if (isDrag) 
        {
          ImGui.SetMouseCursor(ImGuiMouseCursor.Hand);
          _imagePosition.X = startPos.X - (startDrag.X - ImGui.GetIO().MousePos.X) * dragSpeed;
          _imagePosition.Y = startPos.Y - (startDrag.Y - ImGui.GetIO().MousePos.Y) * dragSpeed;
        } 
        else ImGui.SetMouseCursor(ImGuiMouseCursor.Arrow);
      }
      if (_selSprite is TiledSpriteData tiledSprite) 
        ImUtils.DrawRect(ImGui.GetForegroundDrawList(), tiledSprite.Region, Color.WhiteSmoke, _imagePosition, _imageZoom);
      // else if (_selSprite is ComplexSpriteData complexSprite) ImUtils.DrawRect(ImGui.GetForegroundDrawList(), complexSprite., Color.WhiteSmoke, _imageZoom);


      ImGui.GetIO().ConfigWindowsResizeFromEdges = true;
      if (ImGui.IsWindowFocused() && ImGui.IsMouseDown(0) && ImGui.GetIO().KeyAlt)
      {
        _imagePosition += ImGui.GetMouseDragDelta(0);
        ImGui.ResetMouseDragDelta(0);
      }


      // clamp in such a way that we keep some part of the image visible
      var min = -(_textureSize * _imageZoom) * 0.8f;
      var max = ImGui.GetContentRegionAvail() * 0.8f;
      _imagePosition = Num.Vector2.Clamp(_imagePosition, min, max);
      ImGui.SetCursorPos(_imagePosition);

      var cursorPosImageTopLeft = ImGui.GetCursorScreenPos();
      ImGui.Image(_texturePtr, _textureSize * _imageZoom);
      ImGui.EndChild();
      

    
		}
		void DrawMenuBar()
		{
			var newAtlas = false;
			var openFile = false;
			if (ImGui.BeginMenuBar())
			{
				if (ImGui.BeginMenu("File"))
				{
					if (ImGui.MenuItem("New Atlas from Folder")) newAtlas = true;
					if (ImGui.MenuItem("Load Atlas or PNG")) openFile = true;
					ImGui.EndMenu();
				}
        if (ImGui.Button("Rectangle"))
        {
        }
        if (ImGui.Button("Circle")) 
        {

        }
        if (ImGui.Button("Point"))
        {

        }
        if (ImGui.Button("Polygon"))
        {

        }
        
				ImGui.EndMenuBar();
			}
			if (newAtlas) ImGui.OpenPopup("new-atlas");
			if (openFile) ImGui.OpenPopup("open-file");
			OpenFilePopup();

		}

		void OpenFilePopup()
		{
			var isOpen = true;
			if (ImGui.BeginPopupModal("open-file", ref isOpen, ImGuiWindowFlags.NoTitleBar))
			{
				var picker = FilePicker.GetFilePicker(this, Path.Combine(Environment.CurrentDirectory, "Content"), ".png|.atlas");
				picker.DontAllowTraverselBeyondRootFolder = true;
				if (picker.Draw())
				{
					var file = picker.SelectedFile;
					_imageFilename = file;
					LoadTexture();
					FilePicker.RemoveFilePicker(this);
				}
				ImGui.EndPopup();
			}
		}
		void DrawPropertiesPane(ComplexSpriteData sprite, string name)
		{
      ImGui.Begin("Complex Properties Pane");
      ImGui.SeparatorText("Sprite Properties");
      ImGui.LabelText("Name", name);
      ImGui.SeparatorText("Custom Properties");
      foreach (var (customName, customProp) in sprite.Properties) 
      {
        ImGui.LabelText(customName, "-");
      }
      ImGui.End();
		}
		void DrawPropertiesPane(TiledSpriteData sprite, string name)
		{
      // ImGui.PushFont(_font);
      ImGui.Begin("Tile Properties Pane");
      ImGui.SeparatorText("Sprite Properties");
      ImGui.LabelText("Name", name);
      ImGui.LabelText("Region", RectangleString(sprite.Region));
      ImGui.SeparatorText("Custom Properties");
      foreach (var (customName, customProp) in sprite.Properties) 
      {
        ImGui.LabelText(customName, "-");
      }
      ImGui.End();
		}
    void DrawPropertiesPane(SpriteSheetData spriteSheet, string name)
    {
      ImGui.Begin("Spritesheet Pane");
      
      ImGui.LabelText("Name", name);
      
      ImGui.LabelText("Tile width", $"{spriteSheet.TileWidth}");
      ImGui.LabelText("Tile height", $"{spriteSheet.TileHeight}");
      
      ImGui.End();
    }
    void DrawAnimationPane()
    {
    }
    void DrawSpritesPane() 
    {
      if (_spriteSheetData == null) return;
      ImGui.Begin("Sprite List");
      ImGui.SeparatorText($"Sheet ({_spriteSheetData.Tiles.Count})");
      ImGui.SeparatorText($"Sprites ({_spriteSheetData.Sprites.Count})");
      foreach (var (name, sprite) in _spriteSheetData.Sprites)
      {
        ImGui.Text($"{name}");
        if (ImGui.IsItemClicked())
        {
          
        }
      }
      ImGui.End();
    }
    void DrawAutoRegionPopup()
    {
      var isOpen = true;
      ImGui.OpenPopup("spritesheet-slicer");
      ImGui.SetNextWindowFocus();
      if (ImGui.BeginPopupModal("spritesheet-slicer", ref isOpen))
      {
        ImGui.InputInt("Tile Width", ref TileWidth);
        ImGui.InputInt("Tile Height", ref TileHeight);
        if (ImGui.Button("Generate")) 
        {
          _spriteSheetData.Slice(TileWidth, TileHeight);
        }
        ImGui.SameLine();
        if (ImGui.Button("Done")) 
        {
          ImGui.CloseCurrentPopup();
          isAutoRegioning = false;
        }
        ImGui.EndPopup();
      }
    }
    void LoadTexture()
    {
      if (_texturePtr != IntPtr.Zero) Core.GetGlobalManager<ImGuiManager>().UnbindTexture(_texturePtr);

      var _atlasTexture = Texture2D.FromStream(Core.GraphicsDevice, File.OpenRead(_imageFilename));
      _textureSize = new Num.Vector2(_atlasTexture.Width, _atlasTexture.Height);
      _textureAspectRatio = _textureSize.X / _textureSize.Y;
      _texturePtr = Core.GetGlobalManager<ImGuiManager>().BindTexture(_atlasTexture);
      
      _spriteSheetData = new SpriteSheetData(ref _atlasTexture);
 
      isAutoRegioning = true;
    }
    
    void DrawGridLines(int tw, int th)
    {
      float w = tw * _imageZoom;
      float h = th * _imageZoom;
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
    string RectangleString(Rectangle rect) => $"{rect.X}, {rect.Y}, {rect.Width}, {rect.Height}";
		void CenterImage()
		{
			var size = _textureSize * _imageZoom;
			_imagePosition = (ImGui.GetContentRegionAvail() - size) * 0.5f;
		}
		Num.Vector2 CalcFitToScreen()
		{
			var availSize = ImGui.GetContentRegionMax();
			var autoScale = _textureSize / availSize;
			if (autoScale.X > autoScale.Y)
				return new Num.Vector2(availSize.X, availSize.X / _textureAspectRatio);
			return new Num.Vector2(availSize.Y * _textureAspectRatio, availSize.Y);
		}

		Num.Vector2 CalcFillScreen()
		{
			var availSize = ImGui.GetContentRegionAvail();
			var autoScale = _textureSize / availSize;
			if (autoScale.X < autoScale.Y)
				return new Num.Vector2(availSize.X, availSize.X / _textureAspectRatio);
			return new Num.Vector2(availSize.Y * _textureAspectRatio, availSize.Y);
		}

		Num.Vector2 CalcBestFitRegion(Num.Vector2 availSize, Num.Vector2 textureSize)
		{
			var aspectRatio = textureSize.X / textureSize.Y;
			var autoScale = _textureSize / availSize;
			if (autoScale.X < autoScale.Y)
				return new Num.Vector2(availSize.X, availSize.X / aspectRatio);
			return new Num.Vector2(availSize.Y * aspectRatio, availSize.Y);
		}



		~SpriteSheetEditor()
		{
			if (_texturePtr != IntPtr.Zero)
				Core.GetGlobalManager<ImGuiManager>().UnbindTexture(_texturePtr);
		}

	}
}

