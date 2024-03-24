using ImGuiNET;
using Microsoft.Xna.Framework;
using Nez;
using System.Reflection;

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
    public PropertyList Copy() 
    {
      var copy = new Dictionary<string, object>();
      foreach (var prop  in Data)
      {
        if (prop.Value.GetType().IsByRef && !(prop.Value is ICloneable))
        {
          throw new Exception("Err copy with a non clonable property");
        }
        copy.TryAdd(prop.Key, prop.Value);
      }
      var list = new PropertyList();
      list.Data = copy;
      list._counter = _counter;
      return list;
    }
    public void OverrideOrAddAll(PropertyList properties) 
    {
      foreach (var prop in properties)
      {
        Data[prop.Key] = prop.Value;
      }
    }
    public void Remove(string name) => Data.Remove(name);
    public Dictionary<string, object>.Enumerator GetEnumerator() { return Data.GetEnumerator(); }
  }
  [System.AttributeUsage(System.AttributeTargets.Property)]
  public class PropertiedInputAttribute : System.Attribute
  {
    public string Name;
    public PropertiedInputAttribute(string name)
    {
      Name = name;
    }
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
      bool anyOtherChanges = false;
      if (propertied.Properties == null) return false;
      if (ImGui.CollapsingHeader("Properties", ImGuiTreeNodeFlags.DefaultOpen))
      {
        if (ImGui.IsItemClicked(ImGuiMouseButton.Right) && ImGui.IsWindowFocused() && ImGui.IsWindowHovered()) ImGui.OpenPopup("prop-popup");

        ImGui.BeginChild("Properties");
        foreach (var (property, propertyData) in propertied.Properties)
        {
          if (ImGui.TreeNode(property))
          {
            var nameHolder = property;
            changedNameOfProperty = property;
            if (ImGui.InputText("Name", ref nameHolder, 25, ImGuiInputTextFlags.EnterReturnsTrue))
            {
              changedName = nameHolder;
            }
            ImGui.LabelText("Type", propertyData.GetType().Name);

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
              // Console.WriteLine($"Type of : {property} {propertyData.GetType().Name}");
              var subProperties = 
                propertyData.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy); 
              // var subPropertiesCount = subProperties.Where(prop => prop.IsDefined(typeof(PropertiedInputAttribute), false)).Count();
              anyOtherChanges = RenderHardTypes(subProperties, propertyData);
            }
            ImGui.TreePop();
          }
        }
        ImGui.EndChild();
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
      return anyOtherChanges;
    }
    static bool RenderHardTypes(PropertyInfo[] subProperties, object propertyData)
    {
      foreach (var subPropertyInfo in subProperties)
      {
        // Attribute that must be present in the property to be queried and placed as renderable imgui component
        var attr = (PropertiedInputAttribute)subPropertyInfo.GetCustomAttribute<PropertiedInputAttribute>(false);
        if (attr == null) continue;

        var subProperty = subPropertyInfo.GetValue(propertyData);
        var subPropertyName = subPropertyInfo.Name;
        switch (subProperty)
        {
          case RectangleF rectProperty: 
            var numerics = rectProperty.ToNumerics();
            if (ImGui.InputFloat4(subPropertyName, ref numerics))
              subPropertyInfo.SetValue(propertyData, numerics.ToRectangleF()); 
            return true;
          case Vector2 vecProperty: 
            var vecNum = vecProperty.ToNumerics();
            if (ImGui.InputFloat2(subPropertyName, ref vecNum))
              subPropertyInfo.SetValue(propertyData, vecNum.ToVector2()); 
            return true;
        }
      }
      return false;
    }
    static Type _pickedPropertyType = null;
    public static bool HandleNewProperty(IPropertied propertied, Sheet.Editor editor)
    {
      if (ImGui.BeginPopupContextItem("prop-popup"))
      {
        if (ImGui.BeginMenu(IconFonts.FontAwesome5.Plus + " New Property"))
        {
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
    public virtual string Name { get; set; } = "";
    protected bool FocusFactor = true;
    public virtual void RenderImGui(Sheet.PropertiesRenderer renderer)
    {
      var name = Name;
      ImGui.Begin(GetIcon() + "   " + GetName(), ImGuiWindowFlags.NoFocusOnAppearing);
      if (ImGui.IsWindowHovered() && FocusFactor) ImGui.SetWindowFocus();

      OnRenderBeforeName();
      if (ImGui.InputText("Name", ref name, 10, ImGuiInputTextFlags.EnterReturnsTrue)) 
      {
        OnChangeName(Name, name);
        Name = name;
      }
      OnRenderAfterName(renderer);
      if (IPropertied.RenderProperties(this)) OnChangeProperty(name);
      if (IPropertied.HandleNewProperty(this, renderer.Editor)) OnChangeProperty(name);
      ImGui.End();
    }
    public bool HasName() => Name != null && Name != string.Empty;
    public virtual string GetIcon() => "";
    public virtual string GetName() => GetType().Name;
    protected virtual void OnChangeProperty(string name) {}
    protected virtual void OnRenderBeforeName() {}
    protected virtual void OnRenderAfterName(Sheet.PropertiesRenderer renderer) {}
    protected virtual void OnChangeName(string prev, string curr) {}

  }
}
