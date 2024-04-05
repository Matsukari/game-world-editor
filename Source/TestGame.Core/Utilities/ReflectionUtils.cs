using System.Reflection;


namespace Raven
{
  public static class ReflectionUtils
  {
    public static IEnumerable<(object, FieldInfo)> FindFields(object objectTree, Type targetType)
    {
      if (objectTree.GetType().IsSubclassOf(targetType) || targetType == objectTree.GetType())
      {
        var properties = objectTree.GetType().GetFields();
        foreach (var property in properties)
        {
          if ((property.FieldType).IsSubclassOf(targetType) || property.FieldType == targetType)
          {
            yield return (objectTree, property);
            object instance = property.GetValue(objectTree);
            foreach (var subproperty in FindFields(instance, targetType))
            {
              yield return (subproperty);
            }
          }
        }
      }
    }
  }
}
