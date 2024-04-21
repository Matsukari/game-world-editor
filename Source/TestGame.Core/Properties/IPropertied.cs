
namespace Raven
{
  /// <summary>
  /// An interface that contains a list of custom properties.
  /// </summary>
  public interface IPropertied
  {
    public string Name { get; set; }
    public PropertyList Properties { get; }
  }
}
