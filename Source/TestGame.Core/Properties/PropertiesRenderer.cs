using ImGuiNET;
using System.Reflection;
using Microsoft.Xna.Framework;
using Nez;
using Icon = IconFonts.FontAwesome5;

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
    public static void RenderBestMatch(string name, ref object propertyData)
    {
      // Property value itself is the data
      if (propertyData.GetType().IsPrimitive)
      {
        switch (propertyData)
        {
          case int intValue: 
            if (ImGui.InputInt(name, ref intValue)) 
            {
              propertyData = intValue;
            }
            break;
          case bool boolValue: 
            if (ImGui.Button(name)) 
            {
              boolValue = !boolValue;
              propertyData = boolValue;
            }
            break;
          case float floatValue: 
            if (ImGui.InputFloat(name, ref floatValue)) 
            {
              propertyData = floatValue;
            }
            break;
        }
      }
      else if (propertyData is Vector2 vector)
      {
        var vecNum = vector.ToNumerics();
        if (ImGui.InputFloat2(name, ref vecNum)) propertyData = vecNum;
      }
      else if (propertyData is string stringValue)
      {
        if (ImGui.InputText(name, ref stringValue, 20, ImGuiInputTextFlags.EnterReturnsTrue)) 
        {
          propertyData = stringValue;
        }
      }
    }
    public static bool Render(ImGuiWinManager manager, IPropertied propertied, bool tree=false)
    {
      string changedName = null;
      object changedProperty = null;
      string changedNameOfProperty = null;
      bool anyOtherChanges = false;
      if (propertied.Properties == null) return false;

      bool header = false;
      if (tree) header = ImGui.TreeNode("Properties");
      else
        header = ImGui.CollapsingHeader("Properties", ImGuiTreeNodeFlags.DefaultOpen);

      if (ImGui.IsItemClicked(ImGuiMouseButton.Right) && ImGui.IsWindowFocused() && ImGui.IsWindowHovered()) ImGui.OpenPopup("prop-popup");

      if (header)
      {

        ImGui.BeginChild("Properties", new System.Numerics.Vector2(ImGui.GetContentRegionAvail().X, 200));
 
        if (propertied.Properties.Data.Count() == 0) ImGuiUtils.TextMiddle("No properties yet.");

        foreach (var (property, propertyData) in propertied.Properties)
        {
          var node = ImGui.TreeNode(property);
          if (ImGui.IsItemClicked(ImGuiMouseButton.Right)) 
          {
            ImGui.OpenPopup("prop-options");
            _propOnOptions = new Tuple<string, object>(property, propertyData);
          }

          if (node)
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
            else if (propertyData is string stringValue && ImGui.InputText("Value", ref stringValue, 20, ImGuiInputTextFlags.EnterReturnsTrue))
            {
              changedProperty = stringValue;
            }
            // Property's value contains a set of data
            else 
            {
              // Console.WriteLine($"Type of : {property} {propertyData.GetType().Name}");
              var subProperties = 
                propertyData.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy); 
              var subPropertiesCount = subProperties.Where(prop => prop.IsDefined(typeof(PropertiedInputAttribute), false)).Count();
              anyOtherChanges = RenderHardTypes(subProperties, propertyData);
            }
            ImGui.TreePop();
          }
        }
        ImGui.EndChild();
        if (tree) ImGui.TreePop();
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
        RenderHardTypes(subPropertyInfo, propertyData);
      }
      return false;
    }
    static bool RenderHardTypes(PropertyInfo subPropertyInfo, object propertyData)
    {
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
      return false;
    }


    static Type _pickedPropertyType = null;
    public static bool HandleNewProperty(IPropertied propertied, ImGuiWinManager manager, Action<string> onCreate=null)
    {
      if (ImGui.BeginPopupContextItem("prop-popup"))
      {
        if ((_cutProperty != null || _copiedProperty != null) && ImGui.MenuItem(Icon.Paste + "  Paste Property"))
        {
          Tuple<string, object> gotProperty;
          if (_cutProperty != null)
          {
            gotProperty = _cutProperty;
            _cutProperty = null;
          }
          else 
          {
            gotProperty = new Tuple<string, object>(_copiedProperty.Item1, _copiedProperty.Item2.AttemptCopy());
          }


          propertied.Properties.Add(gotProperty.Item2);
        }
        if (ImGui.BeginMenu(IconFonts.FontAwesome5.Plus + "  New Property"))
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
              _once = true;
              return false;
            }
          }
          ImGui.EndMenu();
        }


        ImGui.EndPopup();
      }
      if (_propOnOptions != null && ImGui.BeginPopupContextItem("prop-options"))
      {
        if (ImGui.MenuItem(Icon.Trash + "  Delete"))
        {
          propertied.Properties.Remove(_propOnOptions.Item1);
        }
        if (ImGui.MenuItem(Icon.Copy + "  Copy"))
        {
          _copiedProperty = _propOnOptions;
          _cutProperty = null;
        }
        if (ImGui.MenuItem(Icon.Cut + "  Cut"))
        {
          _cutProperty = _propOnOptions;
          propertied.Properties.Remove(_cutProperty.Item1);
          _copiedProperty = null;
        }
        if (ImGui.MenuItem(Icon.Clone + "  Duplicate"))
        {
          if (_propOnOptions.Item2.GetType().IsValueType)
            propertied.Properties.Add(_propOnOptions.Item2);
          else if (_propOnOptions.Item2 is ICloneable cloner)
            propertied.Properties.Add(cloner.Clone());
        }

        ImGui.EndPopup();
      }
      if (_once && _propCreated)
      {
        _once = false;
        _propCreated = false;
        return true;
      }
      if (_pickedPropertyType != null)
      {
        manager.NameModal.Open((name)=>{NameProperty(propertied, name); if (onCreate != null) onCreate.Invoke("");});
      }
      return _propCreated;
    }
    static bool _once = true;
    static bool _propCreated = false;
    static Tuple<string, object> _propOnOptions;
    static Tuple<string, object> _cutProperty;
    static Tuple<string, object> _copiedProperty;

    static void NameProperty(IPropertied propertied, string name)
    {
      // String doesnt have a parameterless constructor
      if (_pickedPropertyType == typeof(string)) propertied.Properties.Data[name] = "";
      else 
      {
        propertied.Properties.Data[name] = System.Convert.ChangeType(System.Activator.CreateInstance(_pickedPropertyType), _pickedPropertyType); 
      }
      _propCreated = true;
      _pickedPropertyType = null;
    }
  }
}
