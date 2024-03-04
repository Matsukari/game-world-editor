using Nez.Sprites;
using Nez.ImGuiTools;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using Nez;
using Nez.Textures;
using Nez.Sprites;
using Nez.Tiled;
using Nez.ImGuiTools;
using Nez.Tweens;

namespace TestGame
{
  class Compa : IComparer<IRenderable>
  {
    public int Compare(IRenderable self, IRenderable other)
    {
      var res = other.RenderLayer.CompareTo(self.RenderLayer);
      if (res == 0)
      {
        res = other.Bounds.Bottom > self.Bounds.Bottom ? -1 : other.Bounds.Bottom < self.Bounds.Bottom ? 1 : 0;
        // if (self.RenderLayer == 5) {
        //   Console.WriteLine($"same ${self.Bounds.Bottom}");
        // }
        return res;
      }
      return res;
    }
  }
  public class Player : Component, IUpdatable {
    private VirtualIntegerAxis hor;
    private VirtualIntegerAxis ver;
    private VirtualButton button;
    private Mover mover;
    private SubpixelVector2 subpixel = new SubpixelVector2();
    public int customPower = 10;
    [Inspectable]
    private string asdfaksjdlkajsldkj  = "asd";

    public override void OnAddedToEntity()
    {
      var renderer = Entity.AddComponent(new SpriteRenderer(Entity.Scene.Content.LoadTexture(Assets.Characters.Ghost_king)));
      renderer.RenderLayer = 5;
      var collider = Entity.AddComponent(new BoxCollider());
      Flags.SetFlagExclusive(ref collider.PhysicsLayer, 1);
      Flags.SetFlagExclusive(ref collider.CollidesWithLayers, 0);
      mover = Entity.AddComponent(new Mover());
      Input();
    }

    void IUpdatable.Update() 
    {
      Entity.Scene.RenderableComponents.SetRenderLayerNeedsComponentSort(5);
      var dir = new Vector2(hor.Value, ver.Value);
      if (button.IsReleased) {
        Entity.Scene.AddEntity(new Projectile());
        Console.WriteLine("Pressed button");
      }

      var motion = dir * Time.DeltaTime * 300;
      mover.CalculateMovement(ref motion, out var res);
      subpixel.Update(ref motion);
      mover.ApplyMovement(motion);
      
      
    }
    public void Input() 
    {
      button = new VirtualButton();
      button.Nodes.Add(new VirtualButton.KeyboardKey(Keys.J));
      hor = new VirtualIntegerAxis();
      hor.Nodes.Add(new VirtualAxis.KeyboardKeys(VirtualInput.OverlapBehavior.TakeNewer, Keys.A, Keys.D));
      ver = new VirtualIntegerAxis();
      ver.Nodes.Add(new VirtualAxis.KeyboardKeys(VirtualInput.OverlapBehavior.TakeNewer, Keys.W, Keys.S));
      
    }
  }

  public class Projectile : Entity {
    public class ProjectileController : Component, IUpdatable {
      public ProjectileMover mover;
      public float vel = 300;
      public override void OnAddedToEntity() {
        mover = Entity.AddComponent(new ProjectileMover());
        var animator = Entity.GetComponent<SpriteAnimator>();
        animator.OnAnimationCompletedEvent += (a)=>{
          if (a == "explode") {
            animator.Stop();
            Console.WriteLine("Destryoed projectile");
            Entity.Destroy();
          }
        };
      }

      void IUpdatable.Update() {
        var animator = Entity.GetComponent<SpriteAnimator>();
        if (mover.Move(new Vector2(0, -vel) * Time.DeltaTime) && animator.CurrentAnimationName != "explode") {
          animator.Play("explode", SpriteAnimator.LoopMode.Once);
          vel= 0;

          Console.WriteLine("Exlode start");
        }
      }
    } 
    public override void OnAddedToScene()
    {
      var bulletTex = Scene.Content.LoadTexture(Assets.Effects.Ghost_ball_idle);
      var bulletHit = Scene.Content.LoadTexture(Assets.Effects.Ghost_ball_explode);
      AddComponent(new ProjectileController());
      var collider = AddComponent(new CircleCollider());
      var animator = AddComponent(new SpriteAnimator());
      animator.AddAnimation("move", Sprite.SpritesFromAtlas(bulletTex, 16, 16).ToArray());
      animator.AddAnimation("explode", Sprite.SpritesFromAtlas(bulletHit, 48, 48).ToArray());
      Flags.SetFlagExclusive(ref collider.PhysicsLayer, 1);
      Flags.SetFlagExclusive(ref collider.CollidesWithLayers, 0);
      animator.Play("move");

      var pos = Scene.FindEntity("player").Position;
      Position = pos;

      base.OnAddedToScene();
    }
  }


	// [BaseScene("Basic Scene", 9999, "Scene with a single Entity. The minimum to have something to show")]
	public class GameScene : Scene
	{
		public override void Initialize()
		{
			base.Initialize();

			// default to 1280x720 with no SceneResolutionPolicy
			SetDesignResolution(1280, 720, SceneResolutionPolicy.None);
      
			Screen.SetSize(1280, 720);
      Content.RootDirectory = "Assets";
      var orbTex = Content.LoadTexture(Assets.Objects.Magic_ore_mid);
      for (int i = 0; i < 10; i++ ) {
        var orb = CreateEntity($"orb ${i}", new Vector2(Nez.Random.Range(0, 100f)));
        var collider = orb.AddComponent<CircleCollider>();
        // Flags.SetFlagExclusive(ref collider.PhysicsLayer, 2);
        // Flags.SetFlagExclusive(ref collider.CollidesWithLayers, 2);
        orb.AddComponent(new SpriteRenderer(orbTex));
        orb.Transform.SetScale(5);
      }
      

      var mapEntity = CreateEntity("map_ground");
      var mapObjsEntity = CreateEntity("map_objs");
      var map = Content.LoadTiledMap(Assets.Unprocessed.Map);
      var mapRenderer = mapEntity.AddComponent(new TiledMapRenderer(map));
      var mapObjsRenderer = mapEntity.AddComponent(new TiledMapRenderer(map));
			var topLeft = new Vector2(map.TileWidth, map.TileWidth);
			var bottomRight = new Vector2(map.TileWidth * (map.Width - 1),
      map.TileWidth * (map.Height - 1));
			mapEntity.AddComponent(new CameraBounds(topLeft, bottomRight));
      mapRenderer.SetLayersToRender(new string[]{"ground", "terrain", "terrain2"});
      mapObjsRenderer.SetLayerToRender("entities");
      mapRenderer.RenderLayer = 10;
      mapObjsRenderer.RenderLayer = 5;


      Camera.SetZoom(4f); 

      var player = CreateEntity("player", new Vector2(Screen.Width / 2, Screen.Height / 2));
      player.AddComponent(new Player());
      var x = new TmxMap();
      var tree = CreateEntity("tree", new Vector2(Screen.Width / 2, Screen.Height / 2));
      for (int i = 0; i < 1000; i ++) {
        tree.AddComponent(new SpriteRenderer(Content.LoadTexture(Assets.Unprocessed.Export.Tree1 + ".png")));
        tree.AddComponent(new BoxCollider());
      }

      var rends = tree.GetComponents<SpriteRenderer>();
      foreach (var i in rends) {
        i.RenderLayer = 5;
        i.SetLocalOffset(new Vector2(Nez.Random.Range(-3000, 4000), Nez.Random.Range(-3400, 2000)));
      }

      Camera.AddComponent(new FollowCamera(player));
      

      RenderableComponentList.CompareUpdatableOrder = new Compa();
      
		}
	}
}

