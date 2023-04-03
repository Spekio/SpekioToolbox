using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.SpekioToolbox
{
	[CustomEntity("SpekioToolbox/LaserTurret")]
	[Tracked(false)]
	public class LaserTurret : Solid
	{
		private Coroutine attackCoroutine;
		public Vector2 position;
		public Sprite sprite;
		public Sprite chargeSprite;
        private SoundSource laserSfx;
		private Level level;
		private float cooldownTimer;
		public VertexLight light;
		public String toggleFlag;
		public bool startActivated;
		public bool activated;
		public Vector2 target;
		public float[] cooldowns;
		public int cooldownIndex;
		public float timeOffset;
		public bool silent;
		public string laserColor;
		public float chargeTimer;
		public float followTimer;
		private bool playerHasMoved;
		private bool waitForPlayer;

		public LaserTurret(EntityData data, Vector2 offset) : base(data.Position + offset, 16f, 16f, true)
		{
			//Logger.Log("SPEKIOTOOLBOX.LaserTurret", "Properties");
			List <KeyValuePair<String, object>> l = data.Values.ToList();
			for (int i = 0; i < l.Count; i++)
			{
				//Logger.Log("SPEKIOTOOLBOX.LaserTurret", l[i].Key + "=" + l[i].Value);
			}
			this.attackCoroutine = new Coroutine(false);
			base.Add(this.attackCoroutine);
			startActivated = activated = data.Bool("startActivated");
			waitForPlayer = data.Bool("waitForPlayer");
			cooldownTimer = timeOffset = data.Float("timeOffset");
			chargeTimer = data.Float("chargeTimer");
			followTimer = data.Float("followTimer");
            if (chargeTimer < followTimer) { chargeTimer = followTimer; }
			string[] cooldownStrings = data.Attr("cooldown").Split(',');
			cooldowns = Array.ConvertAll(cooldownStrings, float.Parse);
			cooldownIndex = 0;
			silent = data.Bool("silent");
			toggleFlag = data.Attr("toggleFlag");
			laserColor = data.Attr("laserColor").ToLower();
			InitializeSprite();
			Collider = new Hitbox(16f, 16f, -8f, -8f);
			SurfaceSoundIndex = 3;
			Depth = -13000;
			Add(this.laserSfx = new SoundSource());
			if (laserColor == "purple") { light = new VertexLight(Calc.HexToColor("cc00ff"), 1f, 8, 12); }
			else if (laserColor == "blue") { light = new VertexLight(Calc.HexToColor("0000ff"), 1f, 8, 12); }
			else if (laserColor == "gray") { light = new VertexLight(Calc.HexToColor("ffffff"), 1f, 8, 12); }
			else if (laserColor == "green") { light = new VertexLight(Calc.HexToColor("00ff00"), 1f, 8, 12); }
			else { light = new VertexLight(Calc.HexToColor("ff0000"), 1f, 8, 12); }
			Add(light);
		}

		private void InitializeSprite()
		{
			//Logger.Log("SPEKIOTOOLBOX.LaserTurret", "Creating laser turret sprite: " + "laser_turret_" + laserColor);
			sprite = SpekioToolboxModule.SpriteBank.Create("laser_turret_" + laserColor);
			sprite.CenterOrigin();
			Add(sprite);
			//Logger.Log("SPEKIOTOOLBOX.LaserTurret", "Creating laser turret charge sprite: " + "laser_turret_charge_" + laserColor);
			chargeSprite = SpekioToolboxModule.SpriteBank.Create("laser_turret_charge_" + laserColor);
			chargeSprite.CenterOrigin();
			chargeSprite.Y += 6;
			Add(chargeSprite);
		}

		public override void Added(Scene scene)
		{
			base.Added(scene);
			this.level = base.SceneAs<Level>();
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
				this.attackCoroutine.Active = true;
				this.sprite.Play("on", false, false);
				light.Visible = true;
			}
			else
			{
				CancelShoot();
				this.attackCoroutine.Active = false;
				this.sprite.Play("off", false, false);
				light.Visible = false;
				cooldownTimer = timeOffset;
			}
		}
		public override void Update()
		{
			base.Update();
			Player entity = base.Scene.Tracker.GetEntity<Player>();
			if (!this.playerHasMoved && entity != null && entity.Speed != Vector2.Zero)
			{
				this.playerHasMoved = true;
			}
			if (playerHasMoved||!waitForPlayer) { 
				if (!String.IsNullOrEmpty(toggleFlag))
				{
					if (activated != (startActivated ^ level.Session.GetFlag(toggleFlag)))
					{
						activated = startActivated ^ level.Session.GetFlag(toggleFlag);
						//this.playerHasMoved = true;
						if (activated)
						{
							this.attackCoroutine.Active = true;
							this.sprite.Play("on", false, false);
							light.Visible = true;
						}
						else
						{
							CancelShoot();
							this.attackCoroutine.Active = false;
							this.sprite.Play("off", false, false);
							light.Visible = false;
							cooldownTimer = timeOffset;
						}
					}
				}

				if (activated)
				{
					this.cooldownTimer -= Engine.DeltaTime;
					if (cooldownTimer <= 0f)
					{
						attackCoroutine.Replace(Shoot());
						cooldownTimer += cooldowns[cooldownIndex];
						cooldownIndex = (cooldownIndex + 1) % cooldowns.Length;
					}
				}
			}
		}

		public override void Render()
		{
			sprite.DrawSimpleOutline();
			base.Render();
			
		}

		private IEnumerator Shoot()
        {
			if (!silent)
			{
				this.laserSfx.Play("event:/char/badeline/boss_laser_charge", null, 0f);
			}
			//Logger.Log("SPEKIOTOOLBOX.LaserTurret", "Play:attackBegin");
			this.chargeSprite.Play("attackBegin", true, false);
			yield return 0.1f;
			Player entity = this.level.Tracker.GetEntity<Player>();
			if (entity != null)
			{
				this.level.Add(Engine.Pooler.Create<BadelineLaser>().Init(this, entity, laserColor, chargeTimer, followTimer));
			}
			yield return followTimer;
			//Logger.Log("SPEKIOTOOLBOX.LaserTurret", "Play:attackLock");
			this.chargeSprite.Play("attackLock", true, false);
			yield return (chargeTimer-followTimer);
			if (!silent)
			{
				this.laserSfx.Stop(true);
				Audio.Play("event:/char/badeline/boss_laser_fire", this.Position);
			}
			//Logger.Log("SPEKIOTOOLBOX.LaserTurret", "Play:idle");
			this.chargeSprite.Play("idle", false, false);
			yield break;
		}

		private void CancelShoot()
        {
			this.laserSfx.Stop(true);
			this.attackCoroutine.Cancel();
			this.chargeSprite.Play("idle", false, false);
		}
	}
}
