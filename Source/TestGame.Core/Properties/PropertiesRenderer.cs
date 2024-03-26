using ImGuiNET;
using System.Reflection;
using Microsoft.Xna.Framework;
using Nez;

namespace Raven
{
  public class PropertiesRenderer
  { 
    static (string, Type)[] _propertyTypes = new []
    {
      ("String", typeof(string)),
      ("Integer", typeof(int)),
      ("Float", typeof(float)),
      ("Boolean", typeof(bool)),
      ("Color", typeof(Color)),
    };
    public static bool Render(Editor editor, IPropertied propertied)
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
    public static bool HandleNewProperty(IPropertied propertied, Editor editor)
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
        editor.NameModal.Open((name)=>NameProperty(propertied, name));
        return true;
      }
      return false;
    }
    static void NameProperty(IPropertied propertied, string name)
    {
      // String doesnt have a parameterless constructor
      if (_pickedPropertyType == typeof(string)) propertied.Properties.Data[name] = "";
      else 
      {
        propertied.Properties.Data[name] = System.Convert.ChangeType(System.Activator.CreateInstance(_pickedPropertyType), _pickedPropertyType); 
      }
      _pickedPropertyType = null;
    }
  }
}