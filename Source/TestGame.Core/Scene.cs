

namespace Raven 
{
  public abstract class Scene : Nez.Scene 
  {
    public abstract void Render(Nez.Batcher batcher, Nez.Camera camera);
  }
}
