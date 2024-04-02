

namespace Raven 
{
  public static class StringExt
  {
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
        string newName = $"{Path.GetFileNameWithoutExtension(newPath)}({count}){Path.GetExtension(newPath)}";
        newPath = Path.Combine(Path.GetDirectoryName(newPath), newName);
        count++;
      }
      return newPath;
    }
  }
}
