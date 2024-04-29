
using Nez.Persistence;

namespace Raven.Serializers
{
  public class DeepDictionary
  {
    public Dictionary<string, object> Data { get; private set; } = new Dictionary<string, object>();

  }
  class JsonCache
  {
    static internal Dictionary<string, object> Data { get; private set; } = new Dictionary<string, object>();
  }
  public abstract class JsonSerializer<T> : ISerializer<T>
  {
    /// <summary>
    /// Store here the datas found in OnFoundCustomData() until Realize() ends.
    /// </summary> 
    public bool PrettyPrint = true;

    static JsonTypeConverter[] _converters = new JsonTypeConverter[]
    {
          new AnimatedSpriteJsonConverter(),
          new AnimationJsonConverter(),
          new ColorJsonConverter(),
          new RectangleJsonConverter(),
          new SheetJsonConverter(),
          new SourcedSpriteJsonConverter(),
          new SpriteJsonConverter(),
          new SpriteSceneAnimationFrameJsonConverter(),
          new SpriteSceneJsonConverter(),
          new TileJsonConverter(),
          new EditorContentDataJsonConverter(),
          new WorldJsonConverter(),
          new TileInstanceJsonConverter(),
          new SpriteSceneInstanceJsonConverter(),
          new TileLayerJsonConverter(),
    };
    /// <summary>
    /// Serializes object with expected TypeConverters
    /// </summary> 
    public void Save(string file, T obj)
    {
      var settings = new JsonSettings();
      settings.PreserveReferencesHandling = true;
      settings.TypeNameHandling = TypeNameHandling.Auto;
      settings.TypeConverters = _converters;
      settings.PrettyPrint = PrettyPrint;
      File.WriteAllText(file, Json.ToJson(obj, settings));
    }

    protected virtual T BuildFrom(T obj, Dictionary<string, object> cache) => obj; 
    /// <summary>
    /// Some properties or fields in an object may not be inscluded on the object interntionally. 
    /// For textures or references, you just need to save their filenames for loading process; this is where you load them. 
    /// This attempts to create the full object based on the deserialized properties
    /// </summary>
    protected abstract T Realize(T obj);

    /// <summary>
    /// Creates an actual object with Realize()
    /// </summary>
    public T Load(string file)
    {
      var obj = Json.FromJson<T>(File.ReadAllText(file), new JsonSettings{TypeConverters = _converters});
      obj = BuildFrom(obj, JsonCache.Data);
      // JsonCache.Data.Clear();
      return Realize(obj);
    }
  }
}
