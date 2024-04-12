using ImGuiNET;
using Icon = IconFonts.FontAwesome5;

namespace Raven.Widget
{
	public class FileManWindow
	{
		static readonly Dictionary<object, FileManWindow> _filePickers = new Dictionary<object, FileManWindow>();

		public string RootFolder;
		public string CurrentFolder;
		public string SelectedFile;
		public List<string> AllowedExtensions = new List<string>();
		public bool HideHiddenFolders = true;
		public bool OnlyAllowFolders;
		public bool DontAllowTraverselBeyondRootFolder;
    public int CurrentFormat = 0;
    public string Title;
    public static string AllFormats = "All formats";

		public static FileManWindow GetFolderPicker(string title, object o, string startingPath)
			=> GetFileManWindow(title, o, startingPath, null, true);

		public static FileManWindow GetFileManWindow(string title, object o, string startingPath, string[] exts, bool onlyAllowFolders = false)
		{
			if (File.Exists(startingPath))
			{
				startingPath = new FileInfo(startingPath).DirectoryName;
			}
			else if (string.IsNullOrEmpty(startingPath) || !Directory.Exists(startingPath))
			{
				startingPath = Environment.CurrentDirectory;
				if (string.IsNullOrEmpty(startingPath))
					startingPath = AppContext.BaseDirectory;
			}

			if (!_filePickers.TryGetValue(o, out FileManWindow fp))
			{
				fp = new FileManWindow();
        fp.Title = title;
				fp.RootFolder = startingPath;
				fp.CurrentFolder = startingPath;
				fp.OnlyAllowFolders = onlyAllowFolders;

        if (exts != null)
        {
          fp.AllowedExtensions = exts.ToList();
        }
        fp.AllowedExtensions.Add(AllFormats);
        fp.CurrentFormat = fp.AllowedExtensions.Count() -1;

				_filePickers.Add(o, fp);
			}

			return fp;
		}

		public static void RemoveFileManWindow(object o) => _filePickers.Remove(o);

		public static void RemoveFileManWindow(FileManWindow picker)
		{
			object o = null;
			foreach (var kv in _filePickers)
			{
				if (kv.Value == picker)
				{
					o = kv.Key;
					break;
				}
			}

			if (o != null)
				RemoveFileManWindow(o);
		}

    public string PickedFile = "";
		public bool Draw(ImGuiWinManager imgui)
		{
			ImGui.Text($"{Title} at " + Path.GetFileName(RootFolder) + CurrentFolder.Replace(RootFolder, ""));
			bool result = false;

      var di = new DirectoryInfo(CurrentFolder);
      if (di.Exists)
      {
        if (di.Parent != null && (!DontAllowTraverselBeyondRootFolder || CurrentFolder != RootFolder))
        {
          if (ImGui.Button(Icon.CaretUp))
          {
            CurrentFolder = di.Parent.FullName;
          }	
          ImGui.SameLine();
        }
        if (ImGui.Button(Icon.FolderPlus))
        {
          imgui.NameModal.Open(name => Directory.CreateDirectory(Path.Join(CurrentFolder, name)));
        }
        if (ImGui.BeginChild(1, new System.Numerics.Vector2(ImGui.GetContentRegionAvail().X, ImGui.GetContentRegionAvail().Y-60)))
        {
          var fileSystemEntries = GetFileSystemEntries(di.FullName);
          foreach (var fse in fileSystemEntries)
          {
            if (Directory.Exists(fse))
            {
              var name = Path.GetFileName(fse);
              ImGui.Selectable(Icon.Folder + "  " + name + "/", false, ImGuiSelectableFlags.DontClosePopups);
              if (ImGui.IsMouseDoubleClicked(ImGuiMouseButton.Left) && ImGui.IsItemClicked())
              {
                SelectedFile = string.Empty;
                CurrentFolder = fse;
              }
            }
            else
            {
              var name = Path.GetFileName(fse);
              bool isSelected = SelectedFile == fse;
              if (ImGui.Selectable(name, isSelected, ImGuiSelectableFlags.DontClosePopups))
              {
                SelectedFile = fse;
                PickedFile = fse;
              }

              if ((ImGui.IsMouseDoubleClicked(0) || ImGui.IsKeyPressed(ImGuiKey.Enter)) && SelectedFile != null && SelectedFile != string.Empty)
              {
                result = true;
                ImGui.CloseCurrentPopup();
              }
            }
          }
        }
      }
      ImGui.EndChild();

      var newName = (SelectedFile == null) ? "" : SelectedFile;
      ImGui.Text("File name: ");
      ImGui.SameLine();
      ImGui.SetNextItemWidth(ImGui.GetContentRegionAvail().X);
      if (ImGui.InputText("##2", ref newName, 1000, ImGuiInputTextFlags.EnterReturnsTrue))
      {
        result = true;
      }
      SelectedFile = newName;

      ImGui.Text("File type: ");
      ImGui.SameLine();
      ImGui.Combo("##3", ref CurrentFormat, AllowedExtensions.ToArray(), AllowedExtensions.Count());

      ImGui.SameLine();
      var size = new System.Numerics.Vector2(ImGui.GetContentRegionAvail().X * 0.5f, 20);
      if (OnlyAllowFolders)
      {
        if (ImGui.Button("Ok", size))
        {
          result = true;
          SelectedFile = CurrentFolder;
          ImGui.CloseCurrentPopup();
        }
        ImGui.SameLine();
      }
      else if (SelectedFile != null)
      {
        if (ImGui.Button("Ok", size))
        {
          result = true;
          ImGui.CloseCurrentPopup();
        }
        ImGui.SameLine();
      }
			if (ImGui.Button("Cancel", size))
			{
				result = false;
				RemoveFileManWindow(this);
				ImGui.CloseCurrentPopup();
			}

			return result;
		}

		bool TryGetFileInfo(string fileName, out FileInfo realFile)
		{
			try
			{
				realFile = new FileInfo(fileName);
				return true;
			}
			catch
			{
				realFile = null;
				return false;
			}
		}

		List<string> GetFileSystemEntries(string fullName)
		{
			var files = new List<string>();
			var dirs = new List<string>();

			foreach (var fse in Directory.GetFileSystemEntries(fullName))
			{
				if (Directory.Exists(fse) && (!HideHiddenFolders || !Path.GetFileName(fse).StartsWith(".")))
				{
					dirs.Add(fse);
				}
				else if (!OnlyAllowFolders)
				{
					if (AllowedExtensions != null)
					{
						var ext = Path.GetExtension(fse);
						if (AllowedExtensions[CurrentFormat].Contains(ext) || AllowedExtensions[CurrentFormat] == AllFormats)
							files.Add(fse);
					}
					else
					{
						files.Add(fse);
					}
				}
			}
			
			dirs.Sort();
			files.Sort();
			
			var ret = new List<string>(dirs);
			ret.AddRange(files);

			return ret;
		}
	
	} 
}
