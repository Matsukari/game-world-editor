using Nez;
using Microsoft.Xna.Framework;
using Num = System.Numerics;

namespace Raven
{
  public static class ListExt
  {
    public static List<bool> FalseRange(this List<bool> list, int count)
    {
      list.Clear();
      for (int i = 0; i < count; i++) list.Add(false);
      return list;
    }
    public static List<bool> EqualFalseRange(this List<bool> list, int count)
    {
      if (list.Count() != count) list = FalseRange(list, count);
      return list;
    }
    public static List<string> ConvertToNameList<T>(this List<T> list)
    {
      List<string> result = new List<string>();
      if (typeof(T) == typeof(IPropertied) || typeof(T).IsSubclassOf(typeof(IPropertied))) 
      {
        foreach (var item in list)
        {
          result.Add(((IPropertied)item).Name);
        }
      }
      return result;
    }
    
  }
}
