using System;
using System.Collections.Generic;
using System.Linq;
using Celeste.Mod.Entities;
using FMOD.Studio;
using Microsoft.Xna.Framework;
using Monocle;


namespace Celeste.Mod.SpekioToolbox
{
	[CustomEntity("SpekioToolbox/ProjectileBlockField")]
	[Tracked(true)]
	public class ProjectileBlockField : Entity

	{
		public Level level;
		public String activeFlag;
		public bool instantRemoval;
		public bool directionalBlocking;
		public float blockAngleStart;
		public float blockAngleEnd;


		public ProjectileBlockField(Vector2 position, int width, int height) : base(position)
		{
			base.Collider = new Hitbox((float)width, (float)height, 0f, 0f);

		}

		public ProjectileBlockField(EntityData data, Vector2 offset) : this(data.Position + offset, data.Width, data.Height)
		{
			activeFlag = data.Attr("activeFlag");
			instantRemoval = data.Bool("instantRemoval");
			directionalBlocking = data.Bool("directionalBlocking");
			blockAngleStart = data.Float("blockAngleStart");
			blockAngleEnd = data.Float("blockAngleEnd");
		}
		public override void Added(Scene scene)
		{
			base.Added(scene);
			this.level = base.SceneAs<Level>();
		}
		/*public override void Update()
		{
			base.Update();

			if (String.IsNullOrEmpty(activeFlag)|| level.Session.GetFlag(activeFlag))
			{
				//destroy projectiles
				//Destroy();
				List<Entity> badelineShots = Scene.Tracker.GetEntities<BadelineShot>();
				for (int i = 0; i < badelineShots.Count; i++)
				{
					if (Collide.Check(this, badelineShots[i]))
					{
						BadelineShot badelineShot = (BadelineShot)badelineShots[i];
						//badelineShot.Destroy();
						//badelineShot.dead;
					}
				}

			}
		}*/
	}
}