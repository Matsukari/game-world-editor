
namespace Raven
{
  public interface IPropertied
  {
    string Name { get; set; }
    PropertyList Properties { get; }
  }
}
