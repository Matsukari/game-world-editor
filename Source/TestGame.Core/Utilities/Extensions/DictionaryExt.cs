

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
  }
}
