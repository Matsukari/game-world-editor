
namespace Raven
{
  /// <summary>
  /// An interface that contains a list of custom properties.
  /// </summary>
  public interface IPropertied
  {
    string Name { get; set; }
    PropertyList Properties { get; }
  }
}
