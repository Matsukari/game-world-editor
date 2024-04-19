
namespace Raven
{
  public static class ListExt
  {
    /// <summary>
    /// Gets an item or null if not present
    /// </summary> 
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
    public static List<T> Copy<T>(this List<T> list) where T: struct 
    {
      List<T> newList = new List<T>();
      foreach (var item in list) newList.Add(item);
      return newList;
    }
    /// <summary>
    /// Clones all items in the list if they type implement IClonable 
    /// </summary> 
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
    /// <summary>
    /// Loop over the items in the list. If the type given contains a string field named "Name", then recusrively loop all over again
    /// and check if the field's value for each item overlaps with each other, if they did, then simply add '-1', until no 'Name' field's 
    /// value ovelaps with another
    /// </summary> 
    public static List<T> EnsureNoRepeatNameField<T>(this List<T> list) where T: class
    {
      for (int i = list.Count-1; i >= 0; i--)
      {
        var item = list[i];
        var field = item.GetType().GetField("Name");
        if (field != null && field.GetValue(item) is string nameString)
        {
          for (int j = 0; j < list.Count; j++)
          {
            if (i != j && nameString == GetNameField(list[j])) 
            {
              field.SetValue(item, nameString.EnsureNoRepeat());
              return EnsureNoRepeatNameField(list);
            }
          }
          field.SetValue(item, nameString);
        }
      }

      return list;
    }
    public static string GetNameField<T>(this T item) where T: class
    {
      var prop = item.GetType().GetField("Name");
      if (prop.GetValue(item) is string nameString) return nameString;
      return string.Empty;
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
