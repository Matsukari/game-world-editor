using ImGuiNET;

namespace Raven
{
  public class PropertyList
  {
    public Dictionary<string, object> Data = new Dictionary<string, object>();
    int _counter = 0;
    public void Add<T>(T obj)
    {
      string name = $"{obj.GetType().Name}.{_counter++}";
      if (obj is IPropertied prop) 
      {
        if (prop.Name != "") name = prop.Name;
      }
      Data.TryAdd(name, obj);
    }
    public void Remove(string name) => Data.Remove(name);
    public Dictionary<string, object>.Enumerator GetEnumerator() { return Data.GetEnumerator(); }
  }
  public interface IPropertied
  {
    PropertyList Properties { get; }
    string Name { get; set; }
    void RenderImGui();
    public static bool RenderProperties(IPropertied propertied)
    {
      return false;
    }
    public static bool HandleNewProperty(IPropertied propertied)
    {
      if (ImGui.GetIO().MouseClicked[1] && ImGui.IsWindowFocused() && ImGui.IsWindowHovered()) ImGui.OpenPopup("prop-popup");
      if (ImGui.BeginPopupContextItem("prop-popup"))
      {
        if (ImGui.BeginMenu("New Property"))
        {
          ImGui.Separator();
          ImGui.Indent();
          if (ImGui.MenuItem("String")) Console.WriteLine("String");
          if (ImGui.MenuItem("Boolean")) Console.WriteLine("String");
          if (ImGui.MenuItem("Integer")) Console.WriteLine("String");
          if (ImGui.MenuItem("Float")) Console.WriteLine("String");
          if (ImGui.MenuItem("Vector2")) Console.WriteLine("String");
          ImGui.Unindent();
          ImGui.EndMenu();
        }
        ImGui.EndPopup();
      }
      return false;
    }
  //   string _modalNaming = "";
  //   public static void HandleEditSprite(IPropertied propertied)
  //   {
  //     if (ImGui.GetIO().MouseClicked[1] && ImGui.IsWindowFocused() && ImGui.IsWindowHovered()) ImGui.OpenPopup("sprite-popup");
  //     if (propertied is Raven.Sp.Sprite && ImGui.BeginPopupContextItem("sprite-popup"))
  //     {
  //       if (ImGui.BeginMenu("Convert to"))
  //       {
  //         if (ImGui.MenuItem("Spritex")) 
  //         {
  //           ImGui.EndMenu();
  //           ImGui.EndPopup();
  //           ImGui.CloseCurrentPopup();
  //           ImGui.OpenPopup("name-modal");
  //           return;
  //         }
  //         ImGui.EndMenu();
  //       }
  //       ImGui.EndPopup();
  //     }
  //     if (ImGui.BeginPopupModal("name-modal"))
  //     {
  //       Editor.Set(Editor.EditingState.Modal);
  //       if (ImGui.InputText("Name", ref _modalNaming, 10, ImGuiInputTextFlags.EnterReturnsTrue))
  //       {
  //         var spritex = Editor.SpriteSheet.CreateSpritex(_modalNaming, Gui.Selection as Sprites.Sprite);
  //         Editor.Set(Editor.EditingState.Default);
  //         Editor.GetSubEntity<SpritexView>().Edit(spritex);
  //         _modalNaming = "";
  //         ImGui.EndPopup();
  //         ImGui.CloseCurrentPopup();
  //       }
  //       ImGui.EndPopup();
  //     }
  //   }
  //
  }
  public class Propertied : IPropertied
  {
    public PropertyList Properties { get; set; } = new PropertyList();
    public string Name { get; set; } = "";
    public virtual void RenderImGui()
    {
      var name = Name;
      ImGui.Begin(GetType().Name, ImGuiWindowFlags.NoFocusOnAppearing);
      if (ImGui.IsWindowHovered()) ImGui.SetWindowFocus();
      if (ImGui.InputText("Name", ref name, 10, ImGuiInputTextFlags.EnterReturnsTrue)) Name = name;
      IPropertied.HandleNewProperty(this);
      if (IPropertied.RenderProperties(this)) OnChangeProperty(name);
      ImGui.End();
      if (Name != null && Name != string.Empty) OnCreateProperty(Name);
    }
    protected virtual void OnCreateProperty(string name) {}
    protected virtual void OnChangeProperty(string name) {}

  }
  public enum CustomPropertyType 
  {
    STRING,
    FILE,
    COLOR,
    INT,
    FLOAT,
    BOOL,
    VECTOR2
  };

}
