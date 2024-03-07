using ImGuiNET;
using Microsoft.Xna.Framework;
using Nez;

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
        if (prop.Name != "") 
        {
          name = prop.Name;
        }
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
    void RenderImGui(Sheet.PropertiesRenderer renderer);
    static (string, Type)[] _propertyTypes = new []
    {
      ("String", typeof(string)),
      ("Integer", typeof(int)),
      ("Float", typeof(float)),
      ("Boolean", typeof(bool)),
      ("Color", typeof(Color)),
    };

    public static bool RenderProperties(IPropertied propertied)
    {
      string changedName = null;
      object changedProperty = null;
      string changedNameOfProperty = null;
      if (propertied.Properties == null) return false;
      foreach (var (property, propertyData) in propertied.Properties)
      {
        if (ImGui.TreeNode(property))
        {
          var nameHolder = property;
          changedNameOfProperty = property;
          if (ImGui.InputText("Name", ref nameHolder, 20, ImGuiInputTextFlags.EnterReturnsTrue))
          {
            changedName = nameHolder;
          }
          // Property value itself is the data
          if (propertyData.GetType().IsPrimitive)
          {
            switch (propertyData)
            {
              case int intValue: 
                if (ImGui.InputInt("Value", ref intValue)) 
                {
                  changedProperty = intValue;
                }
                break;
              case bool boolValue: 
                if (ImGui.Button("Value")) 
                {
                  boolValue = !boolValue;
                  changedProperty = boolValue;
                }
                break;
              case float floatValue: 
                if (ImGui.InputFloat("Value", ref floatValue)) 
                {
                  changedProperty = floatValue;
                }
                break;
            }
          }
          else if (propertyData is string stringValue)
          {
            if (ImGui.InputText("Value", ref stringValue, 20, ImGuiInputTextFlags.EnterReturnsTrue)) 
            {
              changedProperty = stringValue;
            }
          }
          // Property's value contains a set of data
          else 
          {
            var fields = propertyData.GetType().GetFields(); 
            if (fields.Count() > 1) ImGui.TreeNode(propertyData.GetType().Name);
            foreach (var propertyFieldInfo in fields)
            {
              var field = propertyFieldInfo.GetValue(propertyData);
              var fieldName = propertyFieldInfo.Name;
              // A duplicate, which is most likely a property describing the key of data
              if (fieldName == "Name") continue;
              switch (field)
              {
                case RectangleF rectField: 
                  var numerics = rectField.ToNumerics();
                  if (ImGui.InputFloat4(fieldName, ref numerics))
                    propertyFieldInfo.SetValue(fieldName, numerics.ToRectangleF()); 
                  return true;
              }
            }
            if (fields.Count() > 1) ImGui.TreePop();
          }
          ImGui.TreePop();
        }
      }
      if (changedName != null)
      {
        propertied.Properties.Data.ChangeKey(changedNameOfProperty, changedName);
        return true;
      }
      else if (changedProperty != null)
      {
        propertied.Properties.Data[changedNameOfProperty] = changedProperty;
        return true;
      }
      return false;
    }
    static Type _pickedPropertyType = null;
    public static bool HandleNewProperty(IPropertied propertied, Sheet.Editor editor)
    {
      if (ImGui.GetIO().MouseClicked[1] && ImGui.IsWindowFocused() && ImGui.IsWindowHovered()) ImGui.OpenPopup("prop-popup");
      if (ImGui.BeginPopupContextItem("prop-popup"))
      {
        if (ImGui.BeginMenu("New Property"))
        {
          ImGui.Indent();
          foreach (var (name, type) in _propertyTypes)
          {
            if (ImGui.MenuItem(name)) 
            {
              ImGui.EndMenu();
              ImGui.EndPopup();
              ImGui.CloseCurrentPopup();
              ImGui.OpenPopup("property-name");
              _pickedPropertyType = type;
              return false;
            }
          }
          ImGui.Unindent();
          ImGui.EndMenu();
        }
        ImGui.EndPopup();
      }
      if (_pickedPropertyType != null)
      {
        Sheet.ImGuiViews.NamePopupModal(editor, "property-name", ()=>
        {
          // String doesnt have a parameterless constructor
          if (_pickedPropertyType == typeof(string)) propertied.Properties.Data[Sheet.ImGuiViews.InputName] = "";
          else 
          {
            propertied.Properties.Data[Sheet.ImGuiViews.InputName] = 
              System.Convert.ChangeType(System.Activator.CreateInstance(_pickedPropertyType), _pickedPropertyType); 
            
          }
          _pickedPropertyType = null;
        });
        return true;
      }
      return false;
    }
  }
  public class Propertied : IPropertied
  {
    public PropertyList Properties { get; set; } = new PropertyList();
    public string Name { get; set; } = "";
    public virtual void RenderImGui(Sheet.PropertiesRenderer renderer)
    {
      var name = Name;
      ImGui.Begin(GetType().Name, ImGuiWindowFlags.NoFocusOnAppearing);
      if (ImGui.IsWindowHovered()) ImGui.SetWindowFocus();

      if (ImGui.InputText("Name", ref name, 10, ImGuiInputTextFlags.EnterReturnsTrue)) Name = name;
      if (IPropertied.RenderProperties(this)) OnChangeProperty(name);
      if (IPropertied.HandleNewProperty(this, renderer.Editor)) OnChangeProperty(name);
      ImGui.End();

      if (HasName()) OnCreateProperty(Name);
    }
    bool HasName() => Name != null && Name != string.Empty;
    protected virtual void OnCreateProperty(string name) {}
    protected virtual void OnChangeProperty(string name) {}

  }
}
