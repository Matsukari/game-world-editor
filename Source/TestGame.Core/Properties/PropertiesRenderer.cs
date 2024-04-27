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
    static object RenderPrimitiveType(object propertyData, string label = "Value")
    {
      switch (propertyData)
      {
        case int intValue: 
          if (ImGui.InputInt(label, ref intValue)) 
          {
            return intValue;
          }
          break;
        case bool boolValue: 
          if (ImGui.Button(label)) 
          {
            boolValue = !boolValue;
            return boolValue;
          }
          break;
        case float floatValue: 
          if (ImGui.InputFloat(label, ref floatValue)) 
          {
            return floatValue;
          }
          break;
      }
      return null;
    }
    public static object RenderBestMatch(object propertyData, ref bool anyOtherChanges)
    {
      // Property value itself is the data
      if (propertyData.GetType().IsPrimitive)
      {
        return RenderPrimitiveType(propertyData);
      }
      else if (propertyData is string stringValue)
      {
        if (ImGui.InputText("Value", ref stringValue, 20, ImGuiInputTextFlags.EnterReturnsTrue)) 
          return stringValue;
      }
      else 
      {
        // Console.WriteLine($"Type of : {property} {propertyData.GetType().Name}");
        var subProperties = 
          propertyData.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy); 
        Console.WriteLine("Properties " + subProperties.Count().ToString());
        var subPropertiesCount = subProperties.Where(prop => prop.IsDefined(typeof(PropertiedInputAttribute), false)).Count();
        Console.WriteLine("Got " + subPropertiesCount.ToString());
        anyOtherChanges = RenderHardTypes(subProperties, propertyData);
      }
      return null;
    }
    public static int MinimumPropChildHeight = 250;
    static bool _isOpenPropPopup = false;
    static bool _isOpenPropOptions = false;

    public static void Update(ImGuiWinManager manager)
    {
      if (_isOpenPropPopup)
      {
        _isOpenPropPopup = false;
        ImGui.OpenPopup("prop-popup");
      }
      if (_isOpenPropOptions)
      {
        _isOpenPropOptions = false;
        ImGui.OpenPopup("prop-options");
      }

      if (ImGui.BeginPopup("prop-popup"))
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

          _lastPropertied.Properties.Add(gotProperty.Item2, gotProperty.Item1);
        }
        if (ImGui.BeginMenu(IconFonts.FontAwesome5.Plus + "  New Property"))
        {
          foreach (var (name, type) in _propertyTypes)
          {
            if (ImGui.MenuItem(name)) 
            {
              _pickedPropertyType = type;
            }
          }
          ImGui.EndMenu();
        }


        ImGui.EndPopup();
      }
      if (_propOnOptions != null && ImGui.BeginPopup("prop-options"))
      {
        if (ImGui.MenuItem(Icon.Trash + "  Delete"))
        {
          _lastPropertied.Properties.Remove(_propOnOptions.Item1);
        }
        if (ImGui.MenuItem(Icon.Copy + "  Copy"))
        {
          _copiedProperty = _propOnOptions;
          _cutProperty = null;
        }
        if (ImGui.MenuItem(Icon.Cut + "  Cut"))
        {
          _cutProperty = _propOnOptions;
          _lastPropertied.Properties.Remove(_cutProperty.Item1);
          _copiedProperty = null;
        }
        if (ImGui.MenuItem(Icon.Clone + "  Duplicate"))
        {
          if (_propOnOptions.Item2.GetType().IsValueType)
            _lastPropertied.Properties.Add(_propOnOptions.Item2, _propOnOptions.Item1);
          else if (_propOnOptions.Item2 is ICloneable cloner)
            _lastPropertied.Properties.Add(cloner.Clone(), _propOnOptions.Item1);
        }

        ImGui.EndPopup();
      }

      if (_pickedPropertyType != null)
      {
        var lastPick = _pickedPropertyType;
        manager.NameModal.Open((name)=>{NameProperty(_lastPropertied, lastPick, name); if (_lastCreator != null) _lastCreator.Invoke("");});
        _pickedPropertyType = null;
      }

    }

    public static bool Render(ImGuiWinManager manager, IPropertied propertied, Action<string> creator=null, bool tree=false)
    {
      object changedProperty = null;
      string changedNameOfProperty = null;
      bool anyOtherChanges = false;

      if (propertied.Properties == null) return false;

      bool header = false;
      if (tree) header = ImGui.TreeNode("Properties");
      else
        header = ImGui.CollapsingHeader("Properties", ImGuiTreeNodeFlags.DefaultOpen);

      if (ImGui.IsItemClicked(ImGuiMouseButton.Right) && ImGui.IsWindowFocused() && ImGui.IsWindowHovered()) 
      {
        _lastPropertied = propertied;
        _lastCreator = creator;
        _isOpenPropPopup = true;
      }

      if (header)
      {
        var width = ImGui.GetContentRegionAvail().X;
        var height = Math.Max(ImGui.GetContentRegionAvail().Y, MinimumPropChildHeight);

        if (!tree)
          ImGui.BeginChild("Properties", new System.Numerics.Vector2(width, height));
 
        if (propertied.Properties.Data.Count() == 0 && !tree) ImGuiUtils.TextMiddle("No properties yet.");

        foreach (var pObject in propertied.Properties)
        {
          var property = pObject.Key;
          var propertyData = pObject.Value;
          if (property == null) continue;
          var node = ImGui.TreeNode(property);
          if (ImGui.IsItemClicked(ImGuiMouseButton.Right)) 
          {
            _isOpenPropOptions = true;
            _lastCreator = creator;
            _lastPropertied = propertied;
            _propOnOptions = new Tuple<string, object>(property, propertyData);
          }
          if (node)
          {
            var nameHolder = property;
            changedNameOfProperty = property;
            if (
                ImGui.InputText("Name", ref nameHolder, 25, ImGuiInputTextFlags.EnterReturnsTrue)) pObject.Key = nameHolder;
                ImGui.LabelText("Type", propertyData.GetType().Name);

            changedProperty = RenderBestMatch(propertyData, ref anyOtherChanges);
            ImGui.TreePop();
          }
        }
        if (!tree)
          ImGui.EndChild();
        if (tree) ImGui.TreePop();
      }
      if (changedProperty != null)
      {
        propertied.Properties.AddOrSet(changedProperty, changedNameOfProperty);
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
      Console.WriteLine(": " + subPropertyName);
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
      if (subProperty.GetType().IsPrimitive)
      {
        var data = RenderPrimitiveType(subProperty, subPropertyName);
        if (data != null)
        {
          subPropertyInfo.SetValue(propertyData, data);
          return true;
        }
      }
      return false;
    }


    static Type _pickedPropertyType;
    static IPropertied _lastPropertied;
    static Action<string> _lastCreator;

    static bool _once = true;
    static bool _propCreated = false;
    static Tuple<string, object> _propOnOptions;
    static Tuple<string, object> _cutProperty;
    static Tuple<string, object> _copiedProperty;

    static void NameProperty(IPropertied propertied, Type pickedType, string name)
    {
      // String doesnt have a parameterless constructor
      if (pickedType == typeof(string)) propertied.Properties.AddOrSet("", name);
      else 
      {
        propertied.Properties.AddOrSet(System.Convert.ChangeType(System.Activator.CreateInstance(pickedType), pickedType), name); 
      }
      _propCreated = true;
    }
  }
}
