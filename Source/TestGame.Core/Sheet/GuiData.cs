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
  }
  public class WorldGuiData : GuiData
  {
    public object LayerSelected;
    public object LevelSelected;
  }
}
