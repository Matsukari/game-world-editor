
namespace Raven
{
  public static class ListExt
  {
    public static T GetAtOrNull<T>(this List<T> list, int index) where T : class
    {
      try 
      {
        return list[index];
      }
      catch (Exception)
      {
        return null;
      }
    }
    public static List<T> CloneItems<T>(this List<T> list) where T: class
    {
      List<T> newList = new List<T>();
      foreach (var item in list)
      {
        if (item is ICloneable cloner) newList.Add(cloner.Clone() as T);
      }
      return newList;
    }
    public static bool[] FalseRange(this bool[] list, int except=-1)
    {
      for (int i = 0; i < list.Count(); i++) 
      {
        if (i != except) list[i] = false;
      }
      return list;
    }
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
