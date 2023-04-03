using System;
using System.Collections.Generic;
using System.Linq;
using Celeste.Mod.Entities;
using FMOD.Studio;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.SpekioToolbox
{
	[CustomEntity("SpekioToolbox/Cannon")]
	[Tracked(false)]
	public class Cannon : Shooter
	{
		public string direction;
		public string cannonStyle;
		public VertexLight light;
		private StaticMover staticMover;
		public Sprite sprite;
		public Shaker shaker;


		public Cannon(EntityData data, Vector2 offset) : base(data,data.Position + offset, 16f, 16f, true)
		{
			//Logger.Log("SPEKIOTOOLBOX.Cannon", "Constructor");
			Depth = -9000;
			shotDepth = Depth + 5;

			targets = new Vector2[] { new Vector2()};

			shotStyle = data.Attr("shotStyle").ToLower();
			String shotColor = data.Attr("shotColor").ToLower();
			if (shotColor != null && shotColor != "") { shotStyle = shotColor; }
			//Logger.Log("SPEKIOTOOLBOX.AimingCannon", "shotStyle" + shotStyle);


			cannonStyle = data.Attr("cannonStyle").ToLower();
			String cannonColor = data.Attr("cannonColor").ToLower();
			if (cannonColor != null && cannonColor != "") { cannonStyle = cannonColor; }
			//Logger.Log("SPEKIOTOOLBOX.AimingCannon", "shotStyle" + cannonStyle);

			direction = data.Attr("direction").ToLower();
			InitializeSprite();
			SetTarget();

			if (cannonStyle == "purple") { light = new VertexLight(Calc.HexToColor("ffff00"), 1f, 8, 12); }
			else if (cannonStyle == "green") { light = new VertexLight(Calc.HexToColor("ffff00"), 1f, 8, 12); }
			else { light = new VertexLight(Calc.HexToColor("66ffff"), 1f, 8, 12); }
			Add(light);
			if (attachToSolid) { Attach(); }
			Add(shaker = new Shaker(false, null));

		}


		private void Attach()
		{
			this.staticMover = new StaticMover {
				OnMove = v => {
					if (this != null)
					{
						if (staticMover.Platform != null)
							this.LiftSpeed = staticMover.Platform.LiftSpeed;
						this.MoveHExact((int)v.X);
						this.MoveVExact((int)v.Y);
					}
				}
			};

			this.staticMover.OnAttach = delegate (Platform p)
			{
				base.Depth = p.Depth + 1;
			};

			if (direction == "up")
			{
				this.staticMover.SolidChecker = ((Solid s) => base.CollideCheck(s, this.Position + Vector2.UnitY));
				this.staticMover.JumpThruChecker = ((JumpThru jt) => base.CollideCheck(jt, this.Position + Vector2.UnitY));
				base.Add(this.staticMover);
			}
			else if (direction == "left")
			{
				this.staticMover.SolidChecker = ((Solid s) => base.CollideCheck(s, this.Position + Vector2.UnitX));
				this.staticMover.JumpThruChecker = ((JumpThru jt) => base.CollideCheck(jt, this.Position + Vector2.UnitX));
				base.Add(this.staticMover);
			}
			else if (direction == "right")
			{
				this.staticMover.SolidChecker = ((Solid s) => base.CollideCheck(s, this.Position - Vector2.UnitX));
				this.staticMover.JumpThruChecker = ((JumpThru jt) => base.CollideCheck(jt, this.Position - Vector2.UnitX));
				base.Add(this.staticMover);
			}
			else //down
			{
				this.staticMover.SolidChecker = ((Solid s) => base.CollideCheck(s, this.Position - Vector2.UnitY));
				this.staticMover.JumpThruChecker = ((JumpThru jt) => base.CollideCheck(jt, this.Position - Vector2.UnitY));
				base.Add(this.staticMover);
			}
		}
		private void InitializeSprite()
        {
			//Logger.Log("SPEKIOTOOLBOX.Cannon", "Creating cannon sprite: " + "cannon_" + direction + "_" + cannonColor);
			sprite = SpekioToolboxModule.SpriteBank.Create("cannon_" + direction + "_" + cannonStyle);
			sprite.CenterOrigin();
			Add(sprite);
		}

		private void SetTarget()
		{
			if (direction == "left")
			{
				targets[0] = new Vector2(this.X - 100f, this.Y);
			}
			else if (direction == "right")
			{
				targets[0] = new Vector2(this.X + 100f, this.Y);
			}
			else if (direction == "up")
			{
				targets[0] = new Vector2(this.X, this.Y - 100f);
			}
			else  //down
			{
				targets[0] = new Vector2(this.X, this.Y + 100f);
			}
		}

		public override void Added(Scene scene)
		{
			base.Added(scene);
			this.level = base.SceneAs<Level>();
			if (String.IsNullOrEmpty(toggleFlag))
			{
				activated = startActivated;
			}
			else {
				activated = (startActivated ^ level.Session.GetFlag(toggleFlag));
			}

			if (activated)
			{
				this.sprite.Play("on", false, false);
				light.Visible = true;
			}
			else
			{
				this.sprite.Play("off", false, false);
				light.Visible = false;

				cooldownTimer = timeOffset;
			}
		}

		public override void Update()
		{
			base.Update();

			if (!String.IsNullOrEmpty(toggleFlag))
			{
				if (activated != (startActivated ^ level.Session.GetFlag(toggleFlag)))
				{
					activated = startActivated ^ level.Session.GetFlag(toggleFlag);

					if (activated)
					{
						sprite.Play("on", false, false);
						light.Visible = true;
					}
					else
					{
						sprite.Play("off", false, false);
						light.Visible = false;
						cooldownTimer = timeOffset;
					}
				}
			}

			if (activated)
			{
				if (cooldownTimer - Engine.DeltaTime <= 0f)
				{
					shootingThisFrame = true;
					Shoot();
					cooldownTimer += cooldowns[cooldownIndex];
					cooldownIndex = (cooldownIndex + 1) % cooldowns.Length;
				}
				cooldownTimer -= Engine.DeltaTime;

			}
		}
		public override void Render()
		{
			base.Render();
			sprite.DrawSimpleOutline();
			Vector2 vector = new Vector2(Math.Sign(shaker.Value.X), Math.Sign(shaker.Value.Y));
			sprite.DrawSubrect(Vector2.Zero + vector, new Rectangle(0, 0, (int)sprite.Width, (int)sprite.Height));
		}

		private void Shoot()
		{
			//Logger.Log("SPEKIOTOOLBOX.Cannon", "Shoot()");
			shootingThisFrame = true;
			PlayShotSound();
			SetTarget();
			shaker.ShakeFor(.2f, false);
			level.Add(Engine.Pooler.Create<BadelineShot>().Init(this, 0));
		}
	}
}
