using ImGuiNET;
using Icon = IconFonts.FontAwesome5;
using Nez;

namespace Raven
{
  public class LevelInspector : Widget.PropertiedWindow
  {
    public override string Name { get => Level.Name; set => Level.Name = value;}
    public override PropertyList Properties { get => Level.Properties; set => Level.Properties = value; }
        
    public readonly Level Level;
    public bool Selected = false;

    int _currentLayer = 0;
    List<bool> _layerSelected = new List<bool>();
    public Layer CurrentLayer { get => Level.Layers.GetAtOrNull(_currentLayer); }

    public LevelInspector(Level level) => Level = level;

    public void SetCurrentLayer(int index) 
    {
      SyncLayersGui();
      _currentLayer = index;
      _layerSelected.FalseAll();
      _layerSelected[index] = true;
    }

    public override string GetIcon() => Icon.ObjectGroup;
    public override string GetName() => "Level";   

    public override void OutRender(ImGuiWinManager imgui)
    {
      DrawLayerOptionsPopup();
      DrawLayerHeaderPopup();
    }
    void DrawLayerHeaderPopup()
    {
      if (_isOpenLayerHeaderPopup)
      {
        _isOpenLayerHeaderPopup = false;
        ImGui.OpenPopup("layer-header-options-popup");
      }
      if (ImGui.BeginPopupContextItem("layer-header-options-popup"))
      {
        if (ImGui.BeginMenu(Icon.LayerGroup + "  Create Layer"))
        {
          if (ImGui.MenuItem(Icon.ThLarge + "  Tiled")) 
          {
            var layer = new TileLayer(Level, 16, 16);
            AddLayer(layer);
            SetCurrentLayer(_currentLayer + 1);
          }
          if (ImGui.MenuItem(Icon.ArrowsAlt + "  Freeform"))
          {  
            var layer = new FreeformLayer(Level);
            AddLayer(layer);
            SetCurrentLayer(_currentLayer + 1);
          }
          ImGui.EndMenu();
        }
        ImGui.EndPopup();
      }
    }
    void AddLayer(Layer layer)
    {
      Level.AddLayer(layer);
      Core.GetGlobalManager<CommandManagerHead>().Current.Record(new AddLayerCommand(Level, layer));
    }
    void DrawLayerOptionsPopup()
    {
      if (_isOpenLayerOptionPopup)
      {
        _isOpenLayerOptionPopup = false;
        ImGui.OpenPopup("layer-options-popup");
      }
      if (ImGui.BeginPopupContextItem("layer-options-popup") && _layerOnOptions != null)
      {

        if ((_cutLayer != null || _copiedLayer != null) && ImGui.MenuItem(Icon.Paste + "  Paste Layer"))
        {
          Layer gotLayer;
          if (_cutLayer != null)
          {
            gotLayer = _cutLayer;
            _cutLayer = null;
          }
          else 
            gotLayer = _copiedLayer.Copy();

          AddLayer(gotLayer); 
          gotLayer.Level.OrderAt(gotLayer, gotLayer.Level.Layers.FindIndex(item => item.Name == _layerOnOptions.Name));
        }

        ImGui.Separator();

        if (ImGui.MenuItem(Icon.LevelDownAlt + "  Send to back"))
        {
          _layerOnOptions.Level.OrderAt(_layerOnOptions, 0);  
        }
        if (ImGui.MenuItem(Icon.ChevronDown + "  Move down"))
        {
          _layerOnOptions.Level.BringDown(_layerOnOptions);  
        }
        if (ImGui.MenuItem(Icon.ChevronUp + "  Move up"))
        {
          _layerOnOptions.Level.BringUp(_layerOnOptions);  
        }
        if (ImGui.MenuItem(Icon.LevelUpAlt + "  Bring to front"))
        {
          _layerOnOptions.Level.OrderAt(_layerOnOptions, _layerOnOptions.Level.Layers.Count-1);  
        }

        ImGui.Separator();

        var lockState = (_layerOnOptions.IsLocked) ? Icon.LockOpen + "  Unlock" : Icon.Lock + "  Lock";
        if (ImGui.MenuItem(lockState))
        {
          _layerOnOptions.IsLocked = !_layerOnOptions.IsLocked;
          Core.GetGlobalManager<CommandManagerHead>().Current.Record(new ModifyClassFieldCommand(_layerOnOptions, "IsLocked", !_layerOnOptions.IsLocked));
        }
        var visib = (_layerOnOptions.IsVisible) ? Icon.EyeSlash + "  Hide" : Icon.Eye + "  Show";
        if (ImGui.MenuItem(visib))
        {
          _layerOnOptions.IsVisible = !_layerOnOptions.IsVisible;
          Core.GetGlobalManager<CommandManagerHead>().Current.Record(new ModifyClassFieldCommand(_layerOnOptions, "IsVisible", !_layerOnOptions.IsVisible));
        }

        ImGui.Separator();

        if (ImGui.MenuItem(Icon.Trash + "  Delete"))
        {
          Level.Layers.Remove(_layerOnOptions);  
          Core.GetGlobalManager<CommandManagerHead>().Current.Record(new RemoveLayerCommand(Level, _layerOnOptions));
        }
        if (ImGui.MenuItem(Icon.Copy + "  Copy"))
        {
          _copiedLayer = _layerOnOptions;
          _cutLayer = null;
        }
        if (ImGui.MenuItem(Icon.Cut + "  Cut"))
        {
          _cutLayer = _layerOnOptions;
          _cutLayer.DetachFromLevel();
          _copiedLayer = null;
          Core.GetGlobalManager<CommandManagerHead>().Current.Record(new RemoveLayerCommand(Level, _cutLayer), ()=>_cutLayer = null);
        }
        if (ImGui.MenuItem(Icon.Clone + "  Duplicate"))
        {
          var layer = _layerOnOptions.Copy();
          layer.Offset.X += 200;
          _layerOnOptions.Level.AddLayer(layer);
          Core.GetGlobalManager<CommandManagerHead>().Current.Record(new AddLayerCommand(Level, layer));
        }

        ImGui.EndPopup();
      }
    }
    internal Layer _copiedLayer;
    internal Layer _cutLayer;
    void DrawLayerOptions(Layer layer, ref Layer removeLayer)
    {
      // Options next to name
      var visibState = (!layer.IsVisible) ? Icon.EyeSlash : Icon.Eye;
      var lockState = (!layer.IsLocked) ? Icon.LockOpen: Icon.Lock;

      ImGuiUtils.SpanX((ImGui.GetContentRegionMax().X - ImGuiUtils.CalcTextSizeHorizontal(layer.Name).X - 140));
      ImGui.PushID($"level-{layer.Name}-id");
      if (ImGui.SmallButton(visibState))
      {
        layer.IsVisible = !layer.IsVisible;
        Core.GetGlobalManager<CommandManagerHead>().Current.Record(new ModifyClassFieldCommand(layer, "IsVisible", !layer.IsLocked));
      }
      ImGui.SameLine();
      if (ImGui.SmallButton(lockState))
      {
        layer.IsLocked = !layer.IsLocked;
        Core.GetGlobalManager<CommandManagerHead>().Current.Record(new ModifyClassFieldCommand(layer, "IsLocked", !layer.IsLocked));
      }
      ImGui.SameLine();
      if (ImGui.SmallButton(Icon.Times))
        removeLayer = layer;
      ImGui.PopID();

    }
    void DrawLayerContent(Layer layer)
    {
      if (!layer.IsVisible || layer.IsLocked) ImGui.BeginDisabled();
      var name = layer.Name;
      var offset = layer.Offset.ToNumerics();
      if (ImGui.InputText("Name", ref name, 20, ImGuiInputTextFlags.EnterReturnsTrue)) layer.Name = name;
      if (layer.IsVisible && !layer.IsLocked) ImGui.BeginDisabled();
      ImGui.LabelText("Type", layer.GetType().Name);
      ImGui.LabelText("Level", layer.Level.Name);
      if (layer.IsVisible && !layer.IsLocked) ImGui.EndDisabled();
      ImGui.InputFloat("Opacity", ref layer.Opacity);
      if (ImGui.InputFloat2("Offset", ref offset)) layer.Offset = offset.ToVector2();
      if (!layer.IsVisible || layer.IsLocked) ImGui.EndDisabled();
    }
    void DrawLevelContent()
    {
      ImGui.LabelText("Width", $"{Level.ContentSize.X} px");
      ImGui.LabelText("Height", $"{Level.ContentSize.Y} px");
      var levelOffset = Level.LocalOffset.ToNumerics();
      if (ImGui.InputFloat2("Position", ref levelOffset)) Level.LocalOffset = levelOffset;
    }
    void SyncLayersGui()
    {
      if (_layerSelected.Count() != Level.Layers.Count())
      {
        _layerSelected.FalseRange(Level.Layers.Count());
        try 
        {
          _layerSelected[_currentLayer] = true;
        }
        catch (Exception) 
        {
          if (_layerSelected.Count() == 0) return;
          _currentLayer = 0;
          _layerSelected[_currentLayer] = true;
        }
      }
    }
    Layer _layerOnOptions = null;
    bool _isOpenLayerHeaderPopup = false;
    bool _isOpenLayerOptionPopup = false;
    protected override void OnRenderAfterName(ImGuiWinManager imgui)
    {
      SyncLayersGui();
      DrawLevelContent();
      var header = ImGui.CollapsingHeader("Layers", ImGuiTreeNodeFlags.DefaultOpen);

      // The hader
      if (ImGui.IsItemClicked(ImGuiMouseButton.Right))
      {
        _isOpenLayerHeaderPopup = true;
      }
      if (header)
      {
        ImGui.BeginChild($"level-layers-content-child");
        // Draw layers
        Layer removeLayer = null;
        for (int i = Level.Layers.Count()-1; i >= 0; i--)
        {
          var layer = Level.Layers[i];
          var flags = ImGuiTreeNodeFlags.AllowItemOverlap 
            | ImGuiTreeNodeFlags.OpenOnArrow | ImGuiTreeNodeFlags.OpenOnDoubleClick; 

          if (_layerSelected[i]) flags |= ImGuiTreeNodeFlags.Selected;

          var layerNode = (ImGui.TreeNodeEx(layer.Name, flags));

          // Multiple select
          if (ImGui.IsItemClicked() && !ImGui.IsItemToggledOpen())
          {
            // Reset selection
            if (!ImGui.GetIO().KeyCtrl)
            {
              for (int j = 0; j < _layerSelected.Count(); j++) _layerSelected[j] = false;
            }
            _layerSelected[i] = true;
            _currentLayer = i;
          }

          // Layer options; select
          if (ImGui.IsItemClicked(ImGuiMouseButton.Right)) 
          {
            _layerOnOptions = layer;
            _isOpenLayerOptionPopup = true;
          }
          DrawLayerOptions(layer, ref removeLayer);
         
          // Layer content
          if (layerNode)
          { 
            ImGui.PushID($"layer-{layer.Name}-content");
            DrawLayerContent(layer);
            ImGui.PopID();
            ImGui.TreePop();
          }
        }
        if (removeLayer != null)
        {
          Level.Layers.Remove(removeLayer);
          Core.GetGlobalManager<CommandManagerHead>().Current.Record(new RemoveLayerCommand(Level, removeLayer));
        }
        ImGui.EndChild();
      }
    }
  }
}
