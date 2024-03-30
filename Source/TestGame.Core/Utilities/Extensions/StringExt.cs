

namespace Raven 
{
  public static class StringExt
  {
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
