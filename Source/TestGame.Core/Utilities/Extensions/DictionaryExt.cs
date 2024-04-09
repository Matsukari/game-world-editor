

namespace Raven 
{
  public static class DictionaryExt
  {
    public static bool ChangeKey<TKey, TValue>(this IDictionary<TKey, TValue> dict, 
        TKey oldKey, TKey newKey)
    {
      TValue value;
      if (!dict.Remove(oldKey, out value))
        return false;

      dict[newKey] = value;  
      return true;
    }
    public static void AddWithUniqueName<T>(this IDictionary<string, T> dict, string key, T value)
    {
      T res;
      if (dict.TryGetValue(key, out res))
      {
        key = key.EnsureNoRepeat();
        AddWithUniqueName(dict, key, value);
      }
      else 
        dict.Add(key, value);
    }
  }
}
