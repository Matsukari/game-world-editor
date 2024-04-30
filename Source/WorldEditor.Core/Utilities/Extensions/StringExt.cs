using System.Text.RegularExpressions;


namespace Raven 
{
  public static class StringExt
  {
    static public string PascalToWords(this string text)
    {
      Regex r = new Regex(@"(?<=[A-Z])(?=[A-Z][a-z])|(?<=[^A-Z])(?=[A-Z])|(?<=[A-Za-z])(?=[^A-Za-z])");
      return r.Replace(text, " ");

    }
    static public object AttemptCopy(this object obj)
    {
      if (obj.GetType().IsValueType) return obj;
      else if (obj.GetType().IsClass && obj is ICloneable cloner)  return cloner.Clone();
      throw new Exception();
    }

    static public string BestWrap(this string name, int chars = 30)
    {
      if (name.Length > chars) name = name.Substring(name.Length-chars, chars);
      if (Path.GetDirectoryName(name) != string.Empty && Path.GetFileName(name).Length > chars) 
        name = $"../{Path.GetFileName(name)}";
      else 
        name = "..." + name;
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
