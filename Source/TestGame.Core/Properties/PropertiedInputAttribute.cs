
namespace Raven 
{
  [System.AttributeUsage(System.AttributeTargets.Property)]
  public class PropertiedInputAttribute : System.Attribute
  {
    public string Name;
    public PropertiedInputAttribute(string name)
    {
      Name = name;
    }
  }
}
