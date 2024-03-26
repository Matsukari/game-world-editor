using Microsoft.Xna.Framework;
using Nez;
using ImGuiNET;

namespace Raven.Sheet
{
  public class LayerGui : Propertied
  {
    Layer _layer;
    public LayerGui(Layer layer)
    {
      _layer = layer;
    }
    public virtual void Draw(Batcher batcher, Camera camera)
    {
    }
    protected override void OnRenderAfterName(PropertiesRenderer renderer)
    {
      var offset = _layer.Offset.ToNumerics();
      if (ImGui.InputFloat2("Offset", ref offset)) _layer.Offset = offset;
      ImGui.InputFloat("Opacity", ref _layer.Opacity);
    }
  }

}
