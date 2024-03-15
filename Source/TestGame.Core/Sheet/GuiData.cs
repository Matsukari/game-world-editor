using ImGuiNET;
using Microsoft.Xna.Framework.Graphics;
using Nez;
using Nez.ImGuiTools;
using Microsoft.Xna.Framework;


namespace Raven.Sheet
{
  public class GuiData 
  {
    public Object Selection = null; 
    public Vector2 Position = new Vector2();
    public float Zoom = 1;

    public Shape ShapeSelection = null;
    public IPropertied ShapeContext = null;

    internal PrimitiveBatch primitiveBatch;

    public GuiData() 
    {
      primitiveBatch = new PrimitiveBatch();
    }
    public Texture2D LoadTexture(string filename)
    {
      var texture = Texture2D.FromStream(Core.GraphicsDevice, File.OpenRead(filename));
      texture.Name = filename;
      return texture;
    } 
    public void LoadTextureFromFilePopup()
    {
      var isOpen = true;
      if (ImGui.BeginPopupModal(Names.OpenFile, ref isOpen, ImGuiWindowFlags.NoTitleBar))
      {
        var picker = FilePicker.GetFilePicker(this, Path.Combine(Environment.CurrentDirectory, "Content"), ".png|.atlas");
        picker.DontAllowTraverselBeyondRootFolder = true;
        if (picker.Draw())
        {
          LoadTexture(picker.SelectedFile);
          FilePicker.RemoveFilePicker(this);
        }
        ImGui.EndPopup();
      }
    }
  }
}
