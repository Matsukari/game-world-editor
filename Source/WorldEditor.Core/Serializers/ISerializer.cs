using Nez.Persistence;

namespace Raven.Serializers
{
  interface ISerializer<T>
  {
    void Save(string file, T obj);
    T Load(string file);
  }

}
