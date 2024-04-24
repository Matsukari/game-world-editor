using Microsoft.Xna.Framework.Input;

namespace Raven 
{
  public class KeyCombination
  {
    public List<List<Keys>> Keys = new List<List<Keys>>();

    bool IsShift(Keys key) => key == Microsoft.Xna.Framework.Input.Keys.LeftShift || key == Microsoft.Xna.Framework.Input.Keys.RightShift;
    bool IsControl(Keys key) => key == Microsoft.Xna.Framework.Input.Keys.LeftControl || key == Microsoft.Xna.Framework.Input.Keys.RightControl;
    bool IsAlt(Keys key) => key == Microsoft.Xna.Framework.Input.Keys.LeftAlt || key == Microsoft.Xna.Framework.Input.Keys.RightAlt;

    public bool IsPressed()
    {
      foreach (var alt in Keys) 
      {
        var result = true;
        foreach (var key in alt)
        {
          if (IsShift(key)) result = result && Nez.InputUtils.IsShiftDown();
          else if (IsControl(key)) result = result && Nez.InputUtils.IsControlDown();
          else if (IsAlt(key)) result = result && Nez.InputUtils.IsAltDown();
          else if (key == Microsoft.Xna.Framework.Input.Keys.Space) result = result && Nez.Input.IsKeyDown(key);
          else 
            result = result && Nez.Input.IsKeyPressed(key); 
        }

        if (result)
          return result;
      }
      return false;
    }
    public KeyCombination(params Keys[] keys) => Keys.Add(keys.ToList());
  }

}
