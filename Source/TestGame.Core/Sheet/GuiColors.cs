using Microsoft.Xna.Framework;

namespace Raven.Sheet
{
  public struct GuiColors 
  {
    public Color SelectionOutline = new Color(0.85f, 0.85f, 0.85f);
    public Color SelectionFill = new Color(0.85f, 0.85f, 0.85f, 0.2f);
    public Color SpriteGridOutline = new Color(0.3f, 0.3f, 0.3f, 0.17f);
    public Color SpriteRegionInactiveOutline = new Color(0.3f, 0.3f, 0.3f, 0.5f);
    public Color SpriteRegionActiveOutline = new Color(0.5f, 0.5f, 0.5f, 1f);
    public Color SpriteRegionActiveFill = new Color(0.5f, 0.5f, 0.5f, 0.3f);
    public Color SelectionPoint = new Color(0.9f, 0.9f, 0.9f);
    public Color ContentActiveOutline = Color.CadetBlue;
    public Color AnnotatedShapeActive = new Color(0.5f, 0.5f, 0.7f, 0.5f);
    public Color AnnotatedShapeInactive = new Color(0.5f, 0.5f, 0.7f, 0.3f);
    public Color AnnotatedName = new Color(0.5f, 0.5f, 0.7f, 1f);
    public Color Background = new Color(0.15f, 0.15f, 0.15f);
    public Color DeleteButton = Color.PaleVioletRed;
    public GuiColors() {}
  }
}
