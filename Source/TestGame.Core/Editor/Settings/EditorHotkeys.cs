using Microsoft.Xna.Framework.Input;

namespace Raven
{
  public class EditorHotkeys
  {
    public KeyCombination Undo = new KeyCombination(Keys.RightControl, Keys.Z);
    public KeyCombination Redo = new KeyCombination(Keys.RightControl, Keys.Y);
  } 
}
