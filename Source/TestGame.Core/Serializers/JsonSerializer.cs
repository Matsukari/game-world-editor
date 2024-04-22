
using Nez.Persistence;

namespace Raven.Serializers
{
  public abstract class JsonSerializer<T> : ISerializer<T>
  {
    public bool PrettyPrint = true;
    /// <summary>
    /// Serializes object with expected TypeConverters
    /// </summary> 
    public void Save(string file, T obj)
    {
      var settings = new JsonSettings();
      settings.PreserveReferencesHandling = true;
      settings.TypeNameHandling = TypeNameHandling.Auto;
      settings.TypeConverters = new JsonTypeConverter[] 
      {
        new AnimationJsonConverter(),
        new ColorJsonConverter(),
        new RectangleJsonConverter(),
        new SheetJsonConverter(),
        new SceneSpriteJsonConverter(),
        new SpriteJsonConverter(),
        new SpriteSceneAnimationFrameJsonConverter(),
        new SpriteSceneJsonConverter(),
        new TileJsonConverter(),
        new EditorContentDataJsonConverter(),
      };
      settings.PrettyPrint = PrettyPrint;
      File.WriteAllText(file, Json.ToJson(obj, settings));
    }
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
      // if (file.exi)
      return Realize(Json.FromJson<T>(File.ReadAllText(file)));
    }
  }
}
