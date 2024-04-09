
namespace Raven 
{
  public static class StringExt
  {
    static public object AttemptCopy(this object obj)
    {
      if (obj.GetType().IsValueType) return obj;
      else if (obj.GetType().IsClass && obj is ICloneable cloner)  return cloner.Clone();
      throw new Exception();
    }

    static public string BestWrap(this string name, int chars = 30)
    {
      var file = Path.GetFileName(name);
      if (file.Length > chars) file.Substring(file.Length-chars, file.Length);
      if (Path.GetDirectoryName(name) != string.Empty) 
        name = $"../{file}";
      else 
        name = file;
      return name; 
    }

    static public string EnsureNoRepeat(this string name)
    {
      var last = name.Last();
      if (Char.IsNumber(last)) 
      {
        name = name[..^1];
        name += (last - '0') + 1;
      }
      else name += "-1";
      return name;
    }
    static public string GetUniqueFileName(this string fullpath)
    {
      string newPath = fullpath;
      int count = 1;

      while (File.Exists(newPath))
      {
        string newName = $"{Path.GetFileNameWithoutExtension(fullpath)}({count}){Path.GetExtension(fullpath)}";
        newPath = Path.Combine(Path.GetDirectoryName(newPath), newName);
        count++;
      }
      return newPath;
    }
  }
}
