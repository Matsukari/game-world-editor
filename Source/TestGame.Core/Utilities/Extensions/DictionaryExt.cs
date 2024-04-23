

namespace Raven 
{
  public static class DictionaryExt
  {
    public static Dictionary<string, object> Dig(this Dictionary<string, object> dict, string key) 
    {
      var dig = new Dictionary<string, object>();
      dict.TryAdd(key, dig);
      return dig;
    }
    public static Dictionary<K, V> CloneItems<K, V>(this Dictionary<K, V> list) where V: class
    {
      Dictionary<K, V> newDictionary = new Dictionary<K, V>();
      foreach (var item in list)
      {
        if (item.Value is ICloneable cloner) newDictionary.Add(item.Key, cloner.Clone() as V);
      }
      return newDictionary;
    }
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
