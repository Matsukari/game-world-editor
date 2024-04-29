
namespace Raven 
{
    /// <summary>
    /// Sorts the Scenes based on their Y corrdinates; bottom are in the front, upper is sent to back
    /// </summary>
    class SceneYComparer : IComparer<SpriteSceneInstance>
    {
      public int Compare(SpriteSceneInstance self, SpriteSceneInstance other)
      {
        var selfBot = self.Props.Transform.Position.Y;
        var otherBot = other.Props.Transform.Position.Y;

        var res = otherBot > selfBot ? -1 : otherBot < selfBot ? 1 : 0;
        return res;
      }
    }
}
