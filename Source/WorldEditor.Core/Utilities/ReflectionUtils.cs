using System.Reflection;


namespace Raven
{
  public static class ReflectionUtils
  {
    public static bool IsSubclassOfOrEqual(Type type, Type from) => type.IsSubclassOf(from) || type == from;

    public static IEnumerable<T> FindFields<T>(object objectTree) where T: class
    {
      var targetType = typeof(T);
      if (objectTree.GetType().IsSubclassOf(targetType) || targetType == objectTree.GetType())
      {
        var fields = objectTree.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        foreach (var field in fields)
        {
          if ((field.FieldType).IsSubclassOf(targetType) || field.FieldType == targetType)
          {
            Console.WriteLine(objectTree.GetType().Name + field.FieldType.Name);
            Nez.Insist.IsNotNull(field.GetValue(objectTree));

            yield return field.GetValue(objectTree) as T;
            object instance = field.GetValue(objectTree);
            foreach (var subField in FindFields<T>(instance))
            {
              yield return (subField);
            }
          }
        }
      }
    }
  }
}
