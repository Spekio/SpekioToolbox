using System;
using System.Collections.Generic;
using System.Linq;
using Celeste.Mod.Entities;
using FMOD.Studio;
using Microsoft.Xna.Framework;
using Monocle;


namespace Celeste.Mod.SpekioToolbox
{
	[Tracked(true)]
	public class Shooter : Solid
	{
		public Vector2 position;
		public Level level;
		public SoundSource chargeSfx;
		public float cooldownTimer;
		public String toggleFlag;
		public Boolean startActivated;
		public Boolean activated;
		public Vector2[] targets;
		public float[] cooldowns;
		public int cooldownIndex;
		public float timeOffset;
		public string shotStyle = "red";
		public string trajectory = "wavy";
		public Boolean silent;
		public Boolean attachToSolid;
		public Boolean shotCollision = true;
		public float shotSpeed = 1f;
		public Rectangle prevFrame;
		public Vector2 offset = Vector2.Zero;
		public EventInstance shootSound;
		public EventInstance shootSound2;
		public static bool isPlayingThisFrameAlready;
		public bool finishedCDCountdownThisFrame;
		public bool shootingThisFrame = false;
		public bool disableShotParticles = false;
		public static Random rnd = new Random();
		public float angleOffset = 0f;
		public float curveStrength = 0f;
		public int shotDepth;
		public float timeToLive = -1f;
		public float cantKillTimer = 0f;

		public Shooter(EntityData data, Vector2 offset, float height, float width, bool safe) : base(offset, height, width, safe)
		{
			//List<KeyValuePair<String, object>> l = data.Values.ToList();
			//for (int i = 0; i < l.Count; i++) {
			//	Logger.Log("SPEKIOTOOLBOX.Cannon", "Key = " + l[i].Key);
			//}
			//Logger.Log("SPEKIOTOOLBOX.Shooter", "test");
			trajectory = data.Attr("trajectory").ToLower();
			if (trajectory == null || trajectory == "") { trajectory = "wavy"; }
			startActivated = activated = data.Bool("startActivated");
			cooldownTimer = timeOffset = data.Float("timeOffset");
			attachToSolid = data.Bool("attachToSolid");
			shotSpeed = data.Float("shotSpeed");
			string[] cooldownStrings = data.Attr("cooldown").Split(',');
			cooldowns = Array.ConvertAll(cooldownStrings, float.Parse);
			cooldownIndex = 0;
			shotCollision = data.Bool("shotCollision");
			silent = data.Bool("silent");
			toggleFlag = data.Attr("toggleFlag");
			Collider = new Hitbox(16f, 16f, -8f, -8f);
			SurfaceSoundIndex = 3;
			shotDepth = Depth = -9000;
			Add(chargeSfx = new SoundSource());
			prevFrame = new Rectangle((int)Left, (int)Top, (int)Width, (int)Height);
			isPlayingThisFrameAlready = false;
			disableShotParticles = data.Bool("disableShotParticles");
			angleOffset = data.Float("angleOffset");
			curveStrength = data.Float("curveStrength");
			cantKillTimer = data.Float("harmlessTimer");
			//Logger.Log("SPEKIOTOOLBOX.Shooter", "cantKillTimer="+ cantKillTimer);
			if (data.Has("timeToLive")) { 
				timeToLive = data.Float("timeToLive");
			}else 
			{ 
				timeToLive = -1;
			}
			targets = new Vector2[] { new Vector2(this.X, this.Y - 100f) };
		}
		public override void Render()
        {
			base.Render();
			isPlayingThisFrameAlready = false;
			shootingThisFrame = false;
		}

	
		public void PlayShotSound()
		{

			if (!silent && !isPlayingThisFrameAlready)
			{
				Player player = Scene.Tracker.GetEntity<Player>();

				if (player != null) 
				{
					Vector2 playerPos = player.Position;
					List<Entity> shooters = level.Tracker.GetEntities<Shooter>();
					Vector2 closestShooterPos = this.Position;
					Vector2 secondClosestShooterPos = closestShooterPos;
					float shortestDistance = Vector2.DistanceSquared(playerPos, this.Position);
					int countFiring = 0;
					for (int i = 0; i < shooters.Count(); i++)
					{

						Shooter s = (Shooter)shooters[i];
						if (!s.silent && s.activated)
						{
							if (s.shootingThisFrame || (s.cooldownTimer - Engine.DeltaTime) <= 0)
							{
								countFiring++;
								float distance = Vector2.DistanceSquared(playerPos, s.Position);
								if (distance < shortestDistance)
								{
									secondClosestShooterPos = closestShooterPos;
									shortestDistance = distance;
									closestShooterPos = s.Position;
								}
							}
						}
					}
					isPlayingThisFrameAlready = true;
					//shootSound?.stop(STOP_MODE.ALLOWFADEOUT);
					shootSound = Audio.Play(SFX.char_bad_boss_bullet, closestShooterPos, "end", 1f);
					shootSound?.setVolume(.75f);
					shootSound.start();
					//Logger.Log("SPEKIOTOOLBOX.Cannon", "P=" + playerPos);
					//Logger.Log("SPEKIOTOOLBOX.Cannon", "1=" + closestShooterPos);
					if (countFiring > 1)
					{
						shootSound2 = Audio.Play(SFX.char_bad_boss_bullet, secondClosestShooterPos, "end", 1f);
						shootSound2?.setVolume(.75f);
						shootSound2.start();

						//Logger.Log("SPEKIOTOOLBOX.Cannon", "2=" + secondClosestShooterPos);
					}

				}
				
			}
		}
		public override void Update()
		{
			base.Update();
			if (attachToSolid) {
				if ((int)Left != prevFrame.X || (int)Top != prevFrame.Y) {
					prevFrame = new Rectangle((int)Left, (int)Top, (int)Width, (int)Height);
				}
			}
		}
	}
}
