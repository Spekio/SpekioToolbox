using System;
using System.Collections.Generic;
using System.Linq;
using Celeste.Mod.Entities;
using FMOD.Studio;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.SpekioToolbox
{
	[CustomEntity("SpekioToolbox/BulletGenerator")]
	[Tracked(false)]
	public class BulletGenerator : Shooter
	{
		public Sprite sprite;
		public string attachDirection;
		private StaticMover staticMover;
		private Player player;
		public bool targetPlayer;
		public Vector2 shakeVector = default;
		public bool moveTargetWithCannon = false;
		public bool showSprite = true;
		public string shotSequence = "simultaneous";
		public int targetIndex = 0;
		public bool reverseOrder = false;
		public int multishot = 1;
		public string cannonStyle;



		public BulletGenerator(EntityData data, Vector2 offset) : base(data, data.Position + offset, 16f, 16f, true)
		{
			this.Collidable = false;
			attachDirection = data.Attr("attachDirection").ToLower();
			if (attachDirection != "None") { attachToSolid = true; }
			moveTargetWithCannon = data.Bool("moveTargetWithCannon");
			shotStyle = data.Attr("shotStyle").ToLower();
			//Logger.Log("SPEKIOTOOLBOX.BulletGenerator", "shotStyle=" + shotStyle);

			if (data.Nodes.Length >= 1)
			{
				targetPlayer = false;
				targets = (Vector2[])data.Nodes.Clone();
				//(Vector2)data.Nodes.GetValue(0)+ offset;
				for (int i = 0; i < targets.Length; i++)
				{
					targets[i] += offset;
					//Logger.Log("SPEKIOTOOLBOX.BulletGenerator", "Targets["+i+"=" + targets[i]);
				}
			}
			else
			{
				targetPlayer = true;
				targets = new Vector2[] { new Vector2(this.X, this.Y - 100f) };
			}
			if (data.Has("depth"))
			{
				Depth = data.Int("depth");
			}
			else
			{
				Depth = -9000;
			}
			shotDepth = Depth - 1;
			showSprite = activated = data.Bool("showSprite");
			cannonStyle = data.Attr("cannonStyle").ToLower();
			InitializeSprite();
			if (attachToSolid) { Attach(); }
			shotSequence = data.Attr("shotSequence").ToLower();

		}

		private void Attach()
		{
			this.staticMover = new StaticMover
			{
				OnMove = v =>
				{
					if (this != null)
					{
						if (staticMover.Platform != null)
							this.LiftSpeed = staticMover.Platform.LiftSpeed;
						this.MoveHExact((int)v.X);
						this.MoveVExact((int)v.Y);
						if (moveTargetWithCannon)
						{
							for (int i = 0; i < targets.Length; i++)
							{
								targets[i] += v;
							}
						}
					}
				}
			};

			this.staticMover.OnAttach = delegate (Platform p)
			{
				base.Depth = p.Depth + 1;
			};

			if (attachDirection == "down")
			{
				this.staticMover.SolidChecker = ((Solid s) => base.CollideCheck(s, this.Position + Vector2.UnitY));
				this.staticMover.JumpThruChecker = ((JumpThru jt) => base.CollideCheck(jt, this.Position + Vector2.UnitY));
				base.Add(this.staticMover);
			}
			else if (attachDirection == "right")
			{
				this.staticMover.SolidChecker = ((Solid s) => base.CollideCheck(s, this.Position + Vector2.UnitX));
				this.staticMover.JumpThruChecker = ((JumpThru jt) => base.CollideCheck(jt, this.Position + Vector2.UnitX));
				base.Add(this.staticMover);
			}
			else if (attachDirection == "left")
			{
				this.staticMover.SolidChecker = ((Solid s) => base.CollideCheck(s, this.Position - Vector2.UnitX));
				this.staticMover.JumpThruChecker = ((JumpThru jt) => base.CollideCheck(jt, this.Position - Vector2.UnitX));
				base.Add(this.staticMover);
			}
			else //up
			{
				this.staticMover.SolidChecker = ((Solid s) => base.CollideCheck(s, this.Position - Vector2.UnitY));
				this.staticMover.JumpThruChecker = ((JumpThru jt) => base.CollideCheck(jt, this.Position - Vector2.UnitY));
				base.Add(this.staticMover);
			}
		}
		private void InitializeSprite()
		{
			//Logger.Log("SPEKIOTOOLBOX.BulletGenerator", "Creating AimingCannon sprite: " + "aiming_cannon_" + cannonStyle);
			if (showSprite)
			{
				sprite = SpekioToolboxModule.SpriteBank.Create("bullet_generator_" + cannonStyle);
				sprite.CenterOrigin();
				Add(sprite);
			}
		}

		public override void Added(Scene scene)
		{
			base.Added(scene);
			level = base.SceneAs<Level>();
			if (String.IsNullOrEmpty(toggleFlag))
			{
				activated = startActivated;
			}
			else
			{
				activated = (startActivated ^ level.Session.GetFlag(toggleFlag));
			}

			if (activated)
			{
				if (showSprite) { this.sprite.Play("on", false, false); }

			}
			else
			{
				if (showSprite) { this.sprite.Play("off", false, false); }
				cooldownTimer = timeOffset;
			}
		}
		public override void Update()
		{
			base.Update();
			if (attachToSolid)
			{
				if ((int)Left != prevFrame.X || (int)Top != prevFrame.Y)
				{
					prevFrame = new Rectangle((int)Left, (int)Top, (int)Width, (int)Height);
				}
			}
			if (!String.IsNullOrEmpty(toggleFlag))
			{
				if (activated != (startActivated ^ level.Session.GetFlag(toggleFlag)))
				{
					activated = startActivated ^ level.Session.GetFlag(toggleFlag);

					if (activated)
					{
						if (showSprite)
						{
							sprite.Play("on", false, false);
						}
					}
					else
					{
						if (showSprite) { sprite.Play("off", false, false); }
						cooldownTimer = timeOffset;
					}
				}
			}

			if (activated)
			{

				multishot = 0;
				while (cooldownTimer - Engine.DeltaTime <= 0f)
				{
					cooldownTimer += cooldowns[cooldownIndex];
					cooldownIndex = (cooldownIndex + 1) % cooldowns.Length;
					multishot++;

				}
				if (multishot > 0) { Shoot(); }
				cooldownTimer -= Engine.DeltaTime;
			}
		}

		public override void Render()
		{

			if (showSprite)
			{
				sprite.DrawSimpleOutline();
			}
			base.Render();

		}


		private void Shoot()
		{
			//Logger.Log("SPEKIOTOOLBOX.BulletGenerator", "Shoot()");
			shootingThisFrame = true;
			PlayShotSound();
			if (targetPlayer)
			{
				Player player = Scene.Tracker.GetEntity<Player>();

				if (player != null)
				{
					targets[0] = player.Position;
				}
			
				level.Add(Engine.Pooler.Create<BadelineShot>().Init(this, -1));

			}
			else if (shotSequence == "simultaneous")
			{
				for (int i = 0; i < targets.Length; i++)
				{
					//Logger.Log("SPEKIOTOOLBOX.BulletGenerator", "targets[" + i + "]");
					level.Add(Engine.Pooler.Create<BadelineShot>().Init(this, i));
				}
			}
			else if (shotSequence == "sequential")
			{
				//Logger.Log("SPEKIOTOOLBOX.BulletGenerator", "multishot=" + multishot);
				for (int i = 0; i < multishot; i++)
				{
					//Logger.Log("SPEKIOTOOLBOX.BulletGenerator", "targets[" + targetIndex + "]");
					//Vector2 t = GetAngleOffsetTarget(this.Center, targets[targetIndex], angleOffset);
					level.Add(Engine.Pooler.Create<BadelineShot>().Init(this, targetIndex));
					targetIndex = (targetIndex + 1) % targets.Length;
				}
			}
			else if (shotSequence == "random")
			{
				for (int i = 0; i < multishot; i++)
				{
					int j = rnd.Next(0, targets.Length);
					//Vector2 t = GetAngleOffsetTarget(this.Center, targets[j], angleOffset);
					level.Add(Engine.Pooler.Create<BadelineShot>().Init(this, j));
				}
			}
			else if (shotSequence == "up and down")
			{
				for (int i = 0; i < multishot; i++)
				{
					//Vector2 t = GetAngleOffsetTarget(this.Center, targets[targetIndex], angleOffset);
					//Logger.Log("SPEKIOTOOLBOX.BulletGenerator", "targets[" + targetIndex + "]");
					level.Add(Engine.Pooler.Create<BadelineShot>().Init(this, targetIndex));

					if (targets.Length > 1)
					{
						if (targetIndex == 0) { reverseOrder = false; }
						else if (targetIndex == targets.Length - 1) { reverseOrder = true; }

						if (reverseOrder) { targetIndex--; }
						else { targetIndex++; }
					}
				}
			}

		}
	}
}
