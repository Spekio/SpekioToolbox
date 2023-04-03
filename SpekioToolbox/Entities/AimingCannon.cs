using System;
using System.Collections.Generic;
using System.Linq;
using Celeste.Mod.Entities;
using FMOD.Studio;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.SpekioToolbox
{
	[CustomEntity("SpekioToolbox/AimingCannon")]
	[Tracked(false)]
	public class AimingCannon : Shooter
	{
		public Sprite sprite;
		public Sprite sprite2;
		public VertexLight light;
		public VertexLight light2;
		public string attachDirection;
		public string cannonStyle;
		private StaticMover staticMover;
		private Player player;
		private float angle;
		const float TauOverSixteen = 0.392699f;
		public bool targetPlayer;
		public int orientation =0;
		public Shaker shaker;
		public Vector2 shakeVector = default;
		public bool moveTargetWithCannon=false;


		public AimingCannon(EntityData data, Vector2 offset) : base(data, data.Position + offset, 16f, 16f, true)
		{
			/*List<KeyValuePair<String, object>> l = data.Values.ToList();
			for (int i = 0; i < l.Count; i++) {
				Logger.Log("SPEKIOTOOLBOX.AimingCannon", "Key = " + l[i].Key);
			}*/
			Depth = -13000;
			shotDepth = Depth - 5;
			attachDirection = data.Attr("attachDirection").ToLower();
			if (attachDirection != "None"){attachToSolid = true;}
				moveTargetWithCannon = data.Bool("moveTargetWithCannon");
			shotStyle = data.Attr("shotStyle").ToLower();

			cannonStyle = data.Attr("cannonStyle").ToLower();
			//Logger.Log("SPEKIOTOOLBOX.AimingCannon", "cannonStyle=" + cannonStyle);
			if (data.Nodes.Length==1) {
				targetPlayer = false;
				Vector2 v = (Vector2)data.Nodes.GetValue(0) + offset;
				targets = new Vector2[] { v };
			}
            else{
				targetPlayer = true;
				targets = new Vector2[] { new Vector2(this.X, this.Y - 100f) };
			}
			InitializeSprite();
			light = new VertexLight(Calc.HexToColor("00ff00"), 1f, 8, 12);
			light2 = new VertexLight(Calc.HexToColor("ff0000"), 1f, 8, 12);
			Add(light);
			Add(light2);
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
						if (moveTargetWithCannon)
                        {
							targets[0] += v;
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
			//Logger.Log("SPEKIOTOOLBOX.AimingCannon", "Creating AimingCannon sprite: " + "aiming_cannon_" + cannonStyle);
			sprite = SpekioToolboxModule.SpriteBank.Create("aiming_cannon_" + cannonStyle);
			sprite.CenterOrigin();
			Add(sprite);
			sprite2 = SpekioToolboxModule.SpriteBank.Create("aiming_cannon_gunbarrel_" + cannonStyle);
			Add(sprite2);
		}


		public override void Added(Scene scene)
		{
			base.Added(scene);
			level = base.SceneAs<Level>();
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
				light2.Visible = false;
			}
			else
			{
				this.sprite.Play("off", false, false);
				light.Visible = false;
				light2.Visible = true;
				cooldownTimer = timeOffset;
			}
			setOrientation();
		}
		public override void Update()
		{
			base.Update();
			if (attachToSolid) {
				if ((int)Left != prevFrame.X || (int)Top != prevFrame.Y) {
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
						sprite.Play("on", false, false);
						light.Visible = true;
						light2.Visible = false;
					}
					else
					{
						sprite.Play("off", false, false);
						light.Visible = false;
						light2.Visible = true;
						cooldownTimer = timeOffset;
					}
				}
			}
			setOrientation();

			if (activated) {
				
				if (cooldownTimer - Engine.DeltaTime <= 0f)
				{
					Shoot();
					cooldownTimer += cooldowns[cooldownIndex];
					cooldownIndex = (cooldownIndex + 1) % cooldowns.Length;
				}
				cooldownTimer -= Engine.DeltaTime;
			}
		}

		private void setOrientation()
        {
			player = Scene.Tracker.GetEntity<Player>();

			if (player != null&&targetPlayer)
			{
				Vector2 vector = Calc.ClosestPointOnLine(Center, Center + Calc.AngleToVector(this.angle, 2000f), player.Center);
				Vector2 center = player.Center;
				vector = Calc.Approach(vector, center, 200f * Engine.DeltaTime);
				angle = Calc.Angle(Center, vector);
				orientation = (int)Math.Floor((Calc.Angle(Center, vector) + TauOverSixteen / 2f) / TauOverSixteen) + 8;
				//Logger.Log("SPEKIOTOOLBOX.AimingCannon", "((Calc.Angle(Center, vector) + TauOverSixteen / 2f)" + ((Calc.Angle(Center, vector) + TauOverSixteen / 2f)));
				//Logger.Log("SPEKIOTOOLBOX.AimingCannon", "angle" + angle);
				//Logger.Log("SPEKIOTOOLBOX.AimingCannon", "orientation" + orientation);
			}
			else 
			{
				Vector2 vector = Calc.ClosestPointOnLine(Center, Center + Calc.AngleToVector(this.angle, 2000f), targets[0]);
				Vector2 center = targets[0];
				vector = Calc.Approach(vector, center, 200f * Engine.DeltaTime);
				angle = Calc.Angle(Center, vector);
				orientation = (int)Math.Floor((Calc.Angle(Center, vector) + TauOverSixteen / 2f) / TauOverSixteen) + 8;
			}

			switch (orientation)
				{
					case 0://left
					sprite2.Play("a", true, false);
					sprite2.FlipX = false;
					sprite2.FlipY = false;
					sprite2.Rotation = (float)Math.PI / -2;
					offset = new Vector2(-10, 0);
					shakeVector = new Vector2(Math.Abs(Math.Sign(shaker.Value.X)), 0);
					break;
					case 1:
						sprite2.Play("b", true, false);
						sprite2.FlipX = false;
						sprite2.FlipY = false;
						sprite2.Rotation = 0;
						offset = new Vector2(-8, -5);
					shakeVector = new Vector2(Math.Abs(Math.Sign(shaker.Value.X * .5f)), Math.Abs(Math.Sign(shaker.Value.Y * .25f)));
					break;
					case 2:
						sprite2.Play("c", true, false);
						sprite2.FlipX = false;
						sprite2.FlipY = false;
						sprite2.Rotation = 0;
						offset = new Vector2(-7, -7);
					shakeVector = new Vector2(Math.Abs(Math.Sign(shaker.Value.X * .5f)), Math.Abs(Math.Sign(shaker.Value.Y*.5f)));
					break;
					case 3:
						sprite2.Play("b", true, false);
						sprite2.FlipX = true;
						sprite2.FlipY = false;
						sprite2.Rotation = (float)Math.PI / -2;
						offset = new Vector2(-5, -8);
						shakeVector = new Vector2(Math.Abs(Math.Sign(shaker.Value.X * .25f)) , Math.Abs(Math.Sign(shaker.Value.Y * .5f)));
						break;
					case 4://up
						sprite2.Play("a", true, false);
						sprite2.FlipX = false;
						sprite2.FlipY = false;
						sprite2.Rotation = 0f;
						offset = new Vector2(0, -10);
						shakeVector = new Vector2(0,Math.Abs(Math.Sign(shaker.Value.Y)));
					break;
					case 5:
						sprite2.Play("b", true, false);
						sprite2.FlipX = true;
						sprite2.FlipY = true;
						sprite2.Rotation = (float)Math.PI / -2;
						offset = new Vector2(5, -8);
						shakeVector = new Vector2(Math.Abs(-Math.Sign(shaker.Value.X * .25f)), Math.Abs(Math.Sign(shaker.Value.Y * .5f)));
						break;
					case 6:
						sprite2.Play("c", true, false);
						sprite2.FlipX = true;
						sprite2.FlipY = false;
						sprite2.Rotation = 0;
						offset = new Vector2(7, -7);
					shakeVector = new Vector2(-Math.Abs(Math.Sign(shaker.Value.X * .5f)), Math.Abs(Math.Sign(shaker.Value.Y * .5f)));
					break;
					case 7:
						sprite2.Play("b", true, false);
						sprite2.FlipX = true;
						sprite2.FlipY = false;
						sprite2.Rotation = 0;
						offset = new Vector2(8, -5);
					shakeVector = new Vector2(-Math.Abs(Math.Sign(shaker.Value.X * .5f)), Math.Abs(Math.Sign(shaker.Value.Y * .25f)));
					break;
					case 8://right
						sprite2.Play("a", true, false);
						sprite2.FlipX = true;
						sprite2.FlipY = false;
						sprite2.Rotation = (float)Math.PI / 2;
						offset = new Vector2(10, 0);
					shakeVector = new Vector2(-Math.Abs(Math.Sign(shaker.Value.X)), 0);
					break;
					case 9:
						sprite2.Play("b", true, false);
						sprite2.FlipX = true;
						sprite2.FlipY = true;
						sprite2.Rotation = 0;
						offset = new Vector2(8, 5);
					shakeVector = new Vector2(-Math.Abs(Math.Sign(shaker.Value.X * .5f)), -Math.Abs(Math.Sign(shaker.Value.Y * .25f)));
					break;
					case 10:
						sprite2.Play("c", true, false);
						sprite2.FlipX = true;
						sprite2.FlipY = true;
						sprite2.Rotation = 0;
						offset = new Vector2(7, 7);
					shakeVector = new Vector2(-Math.Abs(Math.Sign(shaker.Value.X * .5f)),- Math.Abs(Math.Sign(shaker.Value.Y * .5f)));
					break;
					case 11:
						sprite2.Play("b", true, false);
						sprite2.FlipX = true;
						sprite2.FlipY = false;
						sprite2.Rotation = (float)Math.PI / 2;
						offset = new Vector2(5, 8);
					shakeVector = new Vector2(-Math.Abs(Math.Sign(shaker.Value.X * .25f)),-Math.Abs(Math.Sign(shaker.Value.Y * .5f)));
					break;
					case 12://down
						sprite2.Play("a", true, false);
						sprite2.FlipX = false;
						sprite2.FlipY = true;
						sprite2.Rotation = 0;
						offset = new Vector2(0, 10);
						shakeVector = new Vector2(0,-Math.Abs(Math.Sign(shaker.Value.Y)));
					break;
					case 13:
						sprite2.Play("b", true, false);
						sprite2.FlipX = true;
						sprite2.FlipY = true;
						sprite2.Rotation = (float)Math.PI / 2;
						offset = new Vector2(-5, 8);
						shakeVector = new Vector2(Math.Abs(Math.Sign(shaker.Value.X * .25f)), -Math.Abs(Math.Sign(shaker.Value.Y * .5f)));
						break;
					case 14:
						sprite2.Play("c", true, false);
						sprite2.FlipX = false;
						sprite2.FlipY = true;
						sprite2.Rotation = 0;
						offset = new Vector2(-7, 7);
					shakeVector = new Vector2(Math.Abs(Math.Sign(shaker.Value.X * .5f)), -Math.Abs(Math.Sign(shaker.Value.Y * .5f)));
					break;
					case 15:
						sprite2.Play("b", true, false);
						sprite2.FlipX = false;
						sprite2.FlipY = true;
						sprite2.Rotation = 0;
						offset = new Vector2(-8, 5);
					shakeVector = new Vector2(Math.Abs(Math.Sign(shaker.Value.X * .5f)), -Math.Abs(Math.Sign(shaker.Value.Y * .25f)));
					break;
					case 16:
						sprite2.Play("a", true, false);
						sprite2.FlipX = false;
						sprite2.FlipY = false;
						sprite2.Rotation = (float)Math.PI / -2;
						offset = new Vector2(-10, 0);
						shakeVector = new Vector2(Math.Abs(Math.Sign(shaker.Value.X)), 0);
					break;
			}
		}
		public override void Render()
		{
			sprite.DrawSimpleOutline();
			base.Render();
			//sprite2.DrawSimpleOutline();
			//Vector2 vector = new Vector2(Math.Sign(shaker.Value.X), 0);
			sprite2.DrawSubrect(Vector2.Zero + shakeVector, new Rectangle(0, 0, (int)sprite.Width, (int)sprite.Height));
		}


		private void Shoot()
		{
			shootingThisFrame = true;
			int n = 0;
			if (targetPlayer)
			{
				n = -1;
				Player player = Scene.Tracker.GetEntity<Player>();

				if (player != null)
				{
					targets[0] = player.Position;
				}
			}
			PlayShotSound();
			shaker.ShakeFor(.2f, false);
		
			level.Add(Engine.Pooler.Create<BadelineShot>().Init(this, n));
		}
	}
}
