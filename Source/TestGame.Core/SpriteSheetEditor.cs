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
    bool isDrag = false;
    Vector2 startDrag = new Vector2();
    Vector2 startPos = new Vector2();


		public SpriteSheetEditor()
		{
      _imageFilename = "Assets/Raw/Unprocessed/export/test_canvas.png";
      LoadTexture();
		}
    public override void OnAddedToEntity()
    {
      Core.GetGlobalManager<ImGuiManager>().RegisterDrawCommand(Show);
    }
            
		public void Show()
		{
      bool isOpen = true;
			if (ImGui.Begin("Sprite Atlas Editor", ref isOpen, ImGuiWindowFlags.MenuBar))
			{
        
				DrawMenuBar(); 
				DrawSprites();
        if (_spriteSheetData.TileWidth > 0) DrawGridLines(_spriteSheetData.TileWidth, _spriteSheetData.TileHeight);
        if (isAutoRegioning) DrawAutoRegionPopup(); 
        ImGui.End();
			}
      
      if (_spriteSheetData != null) DrawPropertiesPane(_spriteSheetData, _spriteSheetData.Name);
      if (_selSprite is ComplexSpriteData sel) DrawPropertiesPane(sel, sel.Name);
      else if (_selSprite is TiledSpriteData tile) DrawPropertiesPane(tile, tile.Name);
      
      DrawSpritesPane();
      
      DrawAnimationPane();

		}

    void DrawSprites()
    {
      var frame = ImGui.GetStyle().FramePadding.X + ImGui.GetStyle().FrameBorderSize;
      ImGui.BeginChild("Spritesheet view", new Num.Vector2(ImGui.GetContentRegionAvail().X - 0, 0), false, 
          ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse);
 
      if (ImGui.IsWindowFocused() && ImGui.GetIO().MouseWheel != 0)
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
      ImGui.Begin("Properties Pane");
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
      ImGui.Begin("Properties Pane");
      ImGui.SeparatorText("Sprite Properties");
      ImGui.LabelText("Name", name);
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
      }
      if (ImGui.Button("Add")) 
      {
        
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
          DoAutoRegion(TileWidth, TileHeight);
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
		void DoAutoRegion(int cellWidth, int cellHeight)
		{
      _spriteSheetData.TileWidth = cellWidth;
      _spriteSheetData.TileHeight = cellHeight;
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
    (Num.Vector2, Num.Vector2) GetWindowArea() 
    {
      Num.Vector2 vMin = ImGui.GetWindowContentRegionMin();
      Num.Vector2 vMax = ImGui.GetWindowContentRegionMax();
      vMin += ImGui.GetWindowPos();
      vMax += ImGui.GetWindowPos();
      return (vMin, vMax);
    }
    void DrawGridLines(int tw, int th)
    {
      float w = tw * _imageZoom;
      float h = th * _imageZoom;
      var (min, max) = GetWindowArea();
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

