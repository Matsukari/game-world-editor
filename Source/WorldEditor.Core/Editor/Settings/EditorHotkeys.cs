using Microsoft.Xna.Framework.Input;

namespace Raven
{
  public class EditorHotkeys
  {
    public KeyCombination Undo = new KeyCombination(Keys.RightControl, Keys.Z);
    public KeyCombination Redo = new KeyCombination(Keys.RightControl, Keys.Y);
    public KeyCombination SaveFile = new KeyCombination(Keys.RightControl, Keys.S);
    public KeyCombination CloseFile = new KeyCombination(Keys.RightControl, Keys.S);
    public KeyCombination CloseScene = new KeyCombination(Keys.RightControl, Keys.S);
   
    public KeyCombination ConvertToScene = new KeyCombination(Keys.S);
    public KeyCombination ConvertToAnimation = new KeyCombination(Keys.A);

    public KeyCombination Rotate = new KeyCombination(Keys.S);
    public KeyCombination MoveOnly = new KeyCombination(Keys.S);
    public KeyCombination Selection = new KeyCombination(Keys.S);
    public KeyCombination HandPan = new KeyCombination(Keys.S);

    public KeyCombination AddProperty = new KeyCombination(Keys.S);
    public KeyCombination Delete = new KeyCombination(Keys.Delete);

    public KeyCombination Copy = new KeyCombination(Keys.RightControl, Keys.C);
    public KeyCombination Paste = new KeyCombination(Keys.RightControl, Keys.V);

    public KeyCombination AdvancedMode = new KeyCombination(Keys.F11);


  } 
}
