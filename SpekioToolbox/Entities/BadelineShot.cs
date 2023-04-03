using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.SpekioToolbox
{
	[Pooled]
	[Tracked(true)]
	public class BadelineShot : Entity
	{
        public ParticleType P_Trail = new ParticleType(FinalBossShot.P_Trail);
		private float moveSpeedMultiplier = 100f; 
		private const float AppearTime = 0.1f;
		private Level level;
		private Vector2 speed;
		private float particleDir;
		private Vector2 anchor;
		private Vector2 perp;
		private Vector2 targetPt;
		public bool dead;
		private float cantKillTimer;
		private float appearTimer;
		private SineWave sine;
		private float sineMult;
		private Sprite sprite;
		internal Shooter parent;
		internal bool collideParent;
		private bool hasCollision;
		private String style;
		private Vector2 offset;
		public String trajectory;
		private Vector2 particleOffset = default;
		private Vector2 origParticleOffset = default;
		public bool disableShotParticles;
		public float curveStrength = 0;
		public int targetNode;
		public float timeToLive = -1;
		public bool hasTimeToLive;
		public const float cantKillAlpha = 0.2f;
		//private static long idcount = 0;
		//private  long id;

		public BadelineShot() : base(Vector2.Zero)
		{
			//idcount++;
			//id = idcount;
			base.Collider = new Hitbox(4f, 4f, -2f, -2f);
			base.Add(new PlayerCollider(new Action<Player>(OnPlayer), null, null));
			base.Depth = -1000000;
			base.Add(sine = new SineWave(1.4f, 0f));
			hasCollision = false;
		}

		public BadelineShot Init(Shooter entity, int targetNode)
		{
			//Logger.Log("SPEKIOTOOLBOX.BadelineShot", "Init Start");
			parent = entity;
			cantKillTimer = parent.cantKillTimer;
			this.targetNode= targetNode;
			trajectory = parent.trajectory;
			if (this.trajectory == null) { this.trajectory = "wavy"; }
			offset = parent.offset;
			style = parent.shotStyle;
			if (sprite!=null)
            {
				base.Remove(sprite);
			}
			SetStyle();
			Add(sprite);
			if (cantKillTimer>0)
			{
				sprite.Color = Color.White * cantKillAlpha;
			}
			timeToLive = parent.timeToLive;
			hasTimeToLive = (timeToLive >= 0);
			hasCollision = parent.shotCollision;
			Depth = parent.shotDepth;
			collideParent = true;
			anchor = Position = parent.Center+offset;
			disableShotParticles = parent.disableShotParticles;
			curveStrength = parent.curveStrength;
			moveSpeedMultiplier = parent.shotSpeed;
			if (this.targetNode == -1)
			{
				targetPt = parent.targets[0];
			}
            else {
				targetPt = parent.targets[this.targetNode];
			}
			targetPt = GetAngleOffsetTarget(parent.Center, targetPt, parent.angleOffset);
			SetSpriteRotation();
			dead = false;
			appearTimer = 0.1f;
			sine.Reset();
			sineMult = 0f;
			sprite.OnFrameChange = OnFrameChange;
			sprite.Play("charge", true, false);
			InitSpeed();
			return this;
			
		}

		private void SetStyle()
		{
			switch (style)
			{
				case "blue":
				case "badeline blue":
					sprite = SpekioToolboxModule.SpriteBank.Create("badeline_projectile_blue");
					P_Trail.Color = Calc.HexToColor("cedfff");
					P_Trail.Color2 = Calc.HexToColor("4fa0ff");
					base.Collider = new Hitbox(4f, 4f, -2f, -2f);
					origParticleOffset = particleOffset = default;
					break;
				case "green":
				case "badeline green":
					sprite = SpekioToolboxModule.SpriteBank.Create("badeline_projectile_green");
					P_Trail.Color = Calc.HexToColor("d3ffc7");
					P_Trail.Color2 = Calc.HexToColor("3ebe00");
					base.Collider = new Hitbox(4f, 4f, -2f, -2f);
					origParticleOffset = particleOffset = default;
					break;
				case "yellow":
				case "badeline yellow":
					sprite = SpekioToolboxModule.SpriteBank.Create("badeline_projectile_yellow");
					P_Trail.Color = Calc.HexToColor("e6e2b9");
					P_Trail.Color2 = Calc.HexToColor("e6c747");
					base.Collider = new Hitbox(4f, 4f, -2f, -2f);
					origParticleOffset = particleOffset = default;
					break;
				case "gray":
				case "badeline gray":
					sprite = SpekioToolboxModule.SpriteBank.Create("badeline_projectile_gray");
						P_Trail.Color = Calc.HexToColor("e7e7e7");
						P_Trail.Color2 = Calc.HexToColor("a7a7a7");
					base.Collider = new Hitbox(4f, 4f, -2f, -2f);
					origParticleOffset = particleOffset = default;
					break;
				case "purple":
				case "badeline purple":
					sprite = SpekioToolboxModule.SpriteBank.Create("badeline_projectile_purple");
					P_Trail.Color = Calc.HexToColor("eaceff");
					P_Trail.Color2 = Calc.HexToColor("9f4fff");
					base.Collider = new Hitbox(4f, 4f, -2f, -2f);
					origParticleOffset = particleOffset = default;
					break;
				case "red":
				case "badeline red":
					sprite = SpekioToolboxModule.SpriteBank.Create("badeline_projectile_red");
					P_Trail.Color = Calc.HexToColor("ffced5");
					P_Trail.Color2 = Calc.HexToColor("ff4f7d");
					base.Collider = new Hitbox(4f, 4f, -2f, -2f);
					origParticleOffset = particleOffset = default;
					break;

				case "simple blue":
					sprite = SpekioToolboxModule.SpriteBank.Create("simple_projectile_blue");
					P_Trail.Color = Calc.HexToColor("cedfff");
					P_Trail.Color2 = Calc.HexToColor("4fa0ff");
					base.Collider = new Hitbox(4f, 4f, -2f, -2f);
					origParticleOffset = particleOffset = default;
					break;
				case "simple green":
					sprite = SpekioToolboxModule.SpriteBank.Create("simple_projectile_green");
					P_Trail.Color = Calc.HexToColor("d3ffc7");
					P_Trail.Color2 = Calc.HexToColor("3ebe00");
					base.Collider = new Hitbox(4f, 4f, -2f, -2f);
					origParticleOffset = particleOffset = default;
					break;
				case "simple yellow":
					sprite = SpekioToolboxModule.SpriteBank.Create("simple_projectile_yellow");
					P_Trail.Color = Calc.HexToColor("e6e2b9");
					P_Trail.Color2 = Calc.HexToColor("e6c747");
					base.Collider = new Hitbox(4f, 4f, -2f, -2f);
					origParticleOffset = particleOffset = default;
					break;
				case "simple gray":
					sprite = SpekioToolboxModule.SpriteBank.Create("simple_projectile_gray");
					P_Trail.Color = Calc.HexToColor("e7e7e7");
					P_Trail.Color2 = Calc.HexToColor("a7a7a7");
					base.Collider = new Hitbox(4f, 4f, -2f, -2f);
					origParticleOffset = particleOffset = default;
					break;
				case "simple purple":
					sprite = SpekioToolboxModule.SpriteBank.Create("simple_projectile_purple");
					P_Trail.Color = Calc.HexToColor("eaceff");
					P_Trail.Color2 = Calc.HexToColor("9f4fff");
					base.Collider = new Hitbox(4f, 4f, -2f, -2f);
					origParticleOffset = particleOffset = default;
					break;
				case "simple red":
					sprite = SpekioToolboxModule.SpriteBank.Create("simple_projectile_red");
					P_Trail.Color = Calc.HexToColor("ffced5");
					P_Trail.Color2 = Calc.HexToColor("ff4f7d");
					base.Collider = new Hitbox(4f, 4f, -2f, -2f);
					origParticleOffset = particleOffset = default;
					break;
				case "ring orange":
					sprite = SpekioToolboxModule.SpriteBank.Create("ring_projectile_orange");
					P_Trail.Color = Calc.HexToColor("ff7d00");
					P_Trail.Color2 = Calc.HexToColor("ffcd00");
					base.Collider = new ColliderList(new Collider[]
					{
						new Hitbox(8f, 8f, -4f, -4f)
					});
					origParticleOffset = particleOffset = new Vector2(6, 0);
					break;
				case "ring pink":
					sprite = SpekioToolboxModule.SpriteBank.Create("ring_projectile_pink");
					P_Trail.Color = Calc.HexToColor("ff00c6");
					P_Trail.Color2 = Calc.HexToColor("de00ff");
					base.Collider = new ColliderList(new Collider[]
					{
						new Hitbox(8f, 8f, -4f, -4f)
					});
					origParticleOffset = particleOffset = new Vector2(6, 0);
					break;
				case "ring green":
					sprite = SpekioToolboxModule.SpriteBank.Create("ring_projectile_green");
					P_Trail.Color = Calc.HexToColor("00ff00");
					P_Trail.Color2 = Calc.HexToColor("00FF94");
					base.Collider = new ColliderList(new Collider[]
					{
						new Hitbox(8f, 8f, -4f, -4f)
					});
					origParticleOffset = particleOffset = new Vector2(6, 0);
					break;
				case "ring blue":
					sprite = SpekioToolboxModule.SpriteBank.Create("ring_projectile_blue");
					P_Trail.Color = Calc.HexToColor("cedfff");
					P_Trail.Color2 = Calc.HexToColor("4fa0ff");
					base.Collider = new ColliderList(new Collider[]
					{
						new Hitbox(8f, 8f, -4f, -4f)
					});
					particleOffset = new Vector2(6, 0);
					break;
				case "ring gray":
					sprite = SpekioToolboxModule.SpriteBank.Create("ring_projectile_gray");
					P_Trail.Color = Calc.HexToColor("e7e7e7");
					P_Trail.Color2 = Calc.HexToColor("a7a7a7");
					base.Collider = new ColliderList(new Collider[]
					{
						new Hitbox(8f, 8f, -4f, -4f)
					});
					origParticleOffset = particleOffset = new Vector2(6, 0);
					break;
				case "ring red":
					sprite = SpekioToolboxModule.SpriteBank.Create("ring_projectile_red");
					P_Trail.Color = Calc.HexToColor("ffced5");
					P_Trail.Color2 = Calc.HexToColor("ff4f7d");
					base.Collider = new ColliderList(new Collider[]
					{
						new Hitbox(8f, 8f, -4f, -4f)
					});
					origParticleOffset = particleOffset = new Vector2(6, 0);
					break;

				case "fireball blue":
					sprite = SpekioToolboxModule.SpriteBank.Create("fireball_projectile_blue");
					P_Trail.Color = Calc.HexToColor("cedfff");
					P_Trail.Color2 = Calc.HexToColor("4fa0ff");
					base.Collider = new Hitbox(4f, 4f, -2f, -2f);
					origParticleOffset = particleOffset = new Vector2(4, 0);
					break;
				case "fireball red":
					sprite = SpekioToolboxModule.SpriteBank.Create("fireball_projectile_red");
					P_Trail.Color = Calc.HexToColor("ffced5");
					P_Trail.Color2 = Calc.HexToColor("ff4f7d");
					base.Collider = new Hitbox(4f, 4f, -2f, -2f);
					origParticleOffset = particleOffset = new Vector2(4, 0);
					break;
				case "fireball green":
					sprite = SpekioToolboxModule.SpriteBank.Create("fireball_projectile_green");
					P_Trail.Color = Calc.HexToColor("00ff00");
					P_Trail.Color2 = Calc.HexToColor("00FF94");
					base.Collider = new Hitbox(4f, 4f, -2f, -2f);
					origParticleOffset = particleOffset = new Vector2(4, 0);
					break;
				case "fireball yellow":
					sprite = SpekioToolboxModule.SpriteBank.Create("fireball_projectile_yellow");
					P_Trail.Color = Calc.HexToColor("e6e2b9");
					P_Trail.Color2 = Calc.HexToColor("e6c747");
					base.Collider = new Hitbox(4f, 4f, -2f, -2f);
					origParticleOffset = particleOffset = new Vector2(4, 0);
					break;
				case "fireball gray":
					sprite = SpekioToolboxModule.SpriteBank.Create("fireball_projectile_gray");
					P_Trail.Color = Calc.HexToColor("e7e7e7");
					P_Trail.Color2 = Calc.HexToColor("a7a7a7");
					base.Collider = new Hitbox(4f, 4f, -2f, -2f);
					origParticleOffset = particleOffset = new Vector2(4, 0);
					break;
				case "fireball purple":
					sprite = SpekioToolboxModule.SpriteBank.Create("fireball_projectile_purple");
					P_Trail.Color = Calc.HexToColor("eaceff");
					P_Trail.Color2 = Calc.HexToColor("9f4fff");
					base.Collider = new Hitbox(4f, 4f, -2f, -2f);
					origParticleOffset = particleOffset = new Vector2(4, 0);
					break;
			}
		}


		private void SetSpriteRotation()
		{
			float angle =speed.Angle();
			if (angle <= (45 * (float)Math.PI / 180f) && angle >= (-45 * (float)Math.PI / 180f))//right quadrent
			{
				sprite.FlipX = false;
				sprite.Rotation = 0;
			}
			else if (angle < (-45 * (float)Math.PI / 180f) && angle >= (-135 * (float)Math.PI / 180f))//up quadrent
			{
				sprite.FlipX = true;
				sprite.Rotation = (float)Math.PI / 2;
			}
			else if (angle <= (135 * (float)Math.PI / 180f) && angle > (45 * (float)Math.PI / 180f))//down quadrent
			{
				sprite.FlipX = false;
				sprite.Rotation = (float)Math.PI / 2;
			}
			else//left quadrent
			{
				sprite.FlipX = true; 
				sprite.Rotation = 0;
			}
		}


		public void OnFrameChange(String s)
		{  
			//Logger.Log("SPEKIOTOOLBOX.module", "OnFrameChange: Current Animation=" + sprite.CurrentAnimationID + " Frame="+ sprite.CurrentAnimationFrame+" Total Frames="+ sprite.CurrentAnimationTotalFrames);
			if(sprite.CurrentAnimationID== "destroy" && sprite.CurrentAnimationFrame == sprite.CurrentAnimationTotalFrames-1) {
				Destroy();
				//Logger.Log("SPEKIOTOOLBOX.module", "OnFrameChange: Current Animation Destory()");
			}
		}

		private void InitSpeed()
		{
			speed = ((targetPt - base.Center).SafeNormalize(100f)) * moveSpeedMultiplier;
			/*if (angleOffset != 0f)
			{
				speed = speed.Rotate(angleOffset);
			}*/
			perp = speed.Perpendicular().SafeNormalize();
			particleDir = (-speed).Angle();
		}

		public override void Added(Scene scene)
		{
			base.Added(scene);
			level = base.SceneAs<Level>();
		}

		public override void Removed(Scene scene)
		{
			base.Removed(scene);
			level = null;
		}

		public override void Update()
		{

			base.Update();
			if (collideParent)
			{
				//Logger.Log("SPEKIOTOOLBOX.BadelineShot", "((int)parent.Left) - parent.prevFrame.X: " + ((int)parent.Left) + "-" + parent.prevFrame.X);
				if (Collide.CheckRect(this, parent.prevFrame))
				{
					anchor.X += ((int)parent.Left) - parent.prevFrame.X;
					anchor.Y += ((int)parent.Top) - parent.prevFrame.Y;
				}
				else
				{
					Depth = -1000000;
					collideParent = false;
				}
			}
			if (appearTimer > 0f)
			{
				Position = anchor;
				appearTimer -= Engine.DeltaTime;
				return;
			}
			if (cantKillTimer > 0f)
			{
				cantKillTimer -= Engine.DeltaTime;
			}
			if (timeToLive > 0f)
            {
				timeToLive -= Engine.DeltaTime;

			}

			if (cantKillTimer > 0f)
			{
				sprite.Color = Color.White * cantKillAlpha;
			}
			else
			{
				sprite.Color = Color.White;
			}

			if (!dead)
			{
				if (trajectory == "wavy")
				{
					anchor += speed * Engine.DeltaTime;
					Position = anchor + perp * sineMult * sine.Value * 3f;
					sineMult = Calc.Approach(sineMult, 1f, 2f * Engine.DeltaTime);
				}
				else if (trajectory == "curved")
				{
					speed = speed.Rotate(curveStrength * (float)Math.PI / 180f / 60f);
					anchor += speed * Engine.DeltaTime;
					Position = anchor;
				}
				else if (trajectory == "homing")
				{
					//Logger.Log("SPEKIOTOOLBOX.BadelineShot", "["+id+"] "+ "targetNode=" + targetNode);
					Vector2 t;
					if (targetNode == -1) { 
						Player player = Scene.Tracker.GetEntity<Player>();
						if (player != null)
						{
							if (curveStrength >= 0) { 
								t = player.Position;
								float angle = (float)Math.Atan2(t.Y - Position.Y, t.X - Position.X);
								speed = speed.RotateTowards(angle, curveStrength * (float)Math.PI / 180f / 60f);
                            }
                            else
                            {
								t = player.Position;
								float angle = (float)Math.Atan2(t.Y - Position.Y, t.X - Position.X);
								angle = angle + (float)Math.PI;
								angle = (angle + 2 * (float)Math.PI) % (2 * (float)Math.PI);
								speed = speed.RotateTowards(angle, -curveStrength * (float)Math.PI / 180f / 60f);
							}
						}
                    }
                    else
                    {
						if (curveStrength >= 0)
						{
							t = parent.targets[targetNode];
							float angle = (float)Math.Atan2(t.Y - Position.Y, t.X - Position.X);
							speed = speed.RotateTowards(angle, curveStrength * (float)Math.PI / 180f / 60f);
                        }
                        else
                        {
							t = parent.targets[targetNode];
							float angle = (float)Math.Atan2(t.Y - Position.Y, t.X - Position.X);
							angle = angle + (float)Math.PI;
							angle = (angle + 2 * (float)Math.PI) % (2 * (float)Math.PI);
							speed = speed.RotateTowards(angle, curveStrength * (float)Math.PI / 180f / 60f);
						}
					}
					anchor += speed * Engine.DeltaTime;
					Position = anchor;
				}
				else
				{
					anchor += speed * Engine.DeltaTime;
					Position = anchor;
				}
				if (!level.IsInBounds(this.Center, 240))
				{
					Destroy();
				}
				if (base.Scene.OnInterval(0.04f))
				{
					//Logger.Log("SPEKIOTOOLBOX.BadelineShot", "base.Center=" + base.Center+ " particleOffset=" + particleOffset);
					//Logger.Log("SPEKIOTOOLBOX.BadelineShot", "base.Center+particleOffset=" + base.Center + "particleOffset:" + particleOffset);
					if (!disableShotParticles&&cantKillTimer<=0) { 
						level.ParticlesFG.Emit(P_Trail, 1, base.Center + particleOffset, Vector2.One * 2f, particleDir);
					}
				}
				SetSpriteRotation();

				particleDir = (-speed).Angle();
				particleOffset = origParticleOffset.Rotate((-speed).Angle());
			}

            if (hasTimeToLive && timeToLive <= 0)
            {
				dead = true;
				sprite.Play("destroy", false, false);
			}
			
			if (hasCollision)
			{
				List<Entity> solids = Scene.Tracker.GetEntities<Solid>();
				for (int i = 0; i < solids.Count; i++)
				{
					if (Collide.Check(this, solids[i]))
					{
						if (!solids[i].Equals(parent))
						{
							dead = true;
							sprite.Play("destroy", false, false);
						}
					}
				}
			}

			List<Entity> projectileBlockFields = Scene.Tracker.GetEntities<ProjectileBlockField>();
			for (int i = 0; i < projectileBlockFields.Count; i++)
			{
				ProjectileBlockField pbf = (ProjectileBlockField)projectileBlockFields[i];
				if (String.IsNullOrEmpty(pbf.activeFlag) || level.Session.GetFlag(pbf.activeFlag))
				{
					if (!pbf.directionalBlocking) { 
						if (Collide.Check(this, pbf))
						{
							if (pbf.instantRemoval)
							{
								Destroy();
							}
							else
							{
								dead = true;
								sprite.Play("destroy", false, false);
							}
						}
                    }
                    else
                    {
						if (Collide.Check(this, pbf))
						{
							float a = (speed.Angle() * 180 / (float)Math.PI+450) % 360;
							float startAngle = pbf.blockAngleStart;
							float endAngle = pbf.blockAngleEnd;
							//Logger.Log("SPEKIOTOOLBOX.BadelineShot", "shotAngle=" + a+ " startAngle=" + startAngle + " endAngle=" + endAngle);
							bool isWithinRange = (startAngle <= endAngle)
											? (a >= startAngle && a <= endAngle)
											: (a >= startAngle || a <= endAngle);

							//Logger.Log("SPEKIOTOOLBOX.BadelineShot", "shotAngle=" + a.ToDeg() + " startAngle=" + pbf.blockAngleStart + " endAngle=" + pbf.blockAngleEnd);
							if (isWithinRange) {
								if (pbf.instantRemoval)
								{
									Destroy();
								}
								else
								{
									dead = true;
									sprite.Play("destroy", false, false);
								}
						}
						}
					}
				}
			}

			
		}
		
		public override void Render()
		{
			Color color = sprite.Color;
			Vector2 position = sprite.Position;
            if (cantKillTimer>0) {
				sprite.Color = Color.Black*(cantKillAlpha/4f);

            }
            else
            {
				sprite.Color = Color.Black;
			}
			sprite.Position = position + new Vector2(-1f, 0f);
			sprite.Render();
			sprite.Position = position + new Vector2(1f, 0f);
			sprite.Render();
			sprite.Position = position + new Vector2(0f, -1f);
			sprite.Render();
			sprite.Position = position + new Vector2(0f, 1f);
			sprite.Render();
			sprite.Color = color;
			sprite.Position = position;
			base.Render();
		}

		public void Destroy()
		{
			dead = true;
			base.RemoveSelf();
		}

		private void OnPlayer(Player player)
		{
			if (!dead && cantKillTimer<=0)
			{
				player.Die((player.Center - Position).SafeNormalize(), false, true);
			}
		}
		private Vector2 GetAngleOffsetTarget(Vector2 originV, Vector2 targetV, float offsetDegrees)
		{
			Vector2 newTarget = new Vector2(targetV.X - originV.X, targetV.Y - originV.Y);
			newTarget = newTarget.Rotate(offsetDegrees * (float)Math.PI / 180f);
			newTarget = newTarget + originV;
			return newTarget;
		}

		public enum ShotPatterns
		{
			Single,
			Double,
			Triple
		}
	}
	/*FinalBossShot.P_Trail = new ParticleType
			{
				Size = 1f,
				Color = Calc.HexToColor("ffced5"), #projectile00.png inner
				Color2 = Calc.HexToColor("ff4f7d"), #projectile00.png outer
				ColorMode = ParticleType.ColorModes.Blink,
				FadeMode = ParticleType.FadeModes.Late,
				SpeedMin = 10f,
				SpeedMax = 30f,
				DirectionRange = 0.6981317f,
				LifeMin = 0.3f,
				LifeMax = 0.6f
			};*/
}
