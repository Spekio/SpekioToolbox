using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Celeste.Mod.SpekioToolbox
{

	[CustomEntity("SpekioToolbox/ToggleTouchSwitch")]
	[TrackedAs(typeof(ToggleTouchSwitch))]
	public class ToggleTouchSwitch : Entity
		{

		public ParticleType FireDeactivated = new ParticleType(TouchSwitch.P_Fire);
		public Switch Switch;
		private SoundSource touchSfx;
		private MTexture border = GFX.Game["objects/touchswitch/container"];
		private Sprite icon = new Sprite(GFX.Game, "objects/touchswitch/icon");
		private Color inactiveColor = Calc.HexToColor("5fcde4");
		private Color activeColor = Color.White;
		private Color finishColor = Calc.HexToColor("f141df");
		private float ease;
		private Wiggler wiggler;
		private Vector2 pulse = Vector2.One;
		private float timer;
		private BloomPoint bloom;
        private bool hadPlayer;
		private bool hadHoldable;
		private bool hadSeeker;
		private Level level
			{
				get
				{
					return (Level)base.Scene;
				}
			}

			public ToggleTouchSwitch(Vector2 position) : base(position)
			{
				base.Depth = 2000;
				base.Add(this.Switch = new Switch(false));
				FireDeactivated.Color = FireDeactivated.Color2 = inactiveColor;
				base.Add(this.icon);
				base.Add(this.bloom = new BloomPoint(0f, 16f));
				this.bloom.Alpha = 0f;
				this.icon.Add("idle", "", 0f, new int[1]);
				this.icon.Add("spin", "", 0.1f, new Chooser<string>("spin", 1f), new int[] {0,1,2,3,4,5});
				this.icon.Play("spin", false, false);
				this.icon.Color = this.inactiveColor;
				this.icon.CenterOrigin();
				base.Collider = new Hitbox(16f, 16f, -8f, -8f);
				this.Switch.OnActivate = delegate ()
				{
					this.wiggler.Start();
					for (int i = 0; i < 32; i++)
					{
						float num = Calc.Random.NextFloat(6.2831855f);
						this.level.Particles.Emit(TouchSwitch.P_FireWhite, this.Position + Calc.AngleToVector(num, 6f), num);
					}
					this.icon.Rate = 4f;
				};
			this.Switch.OnDeactivate = delegate ()
			{
				//this.wiggler.Stop();
				for (int i = 0; i < 32; i++)
				{
					float num = Calc.Random.NextFloat(6.2831855f);
					this.level.Particles.Emit(FireDeactivated, this.Position + Calc.AngleToVector(num, 6f), num);
				}
				this.icon.Rate = 1f;
			};
			this.Switch.OnFinish = delegate ()
				{
					this.ease = 0f;
				};
				this.Switch.OnStartFinished = delegate ()
				{
					this.icon.Rate = 0.1f;
					this.icon.Play("idle", false, false);
					this.icon.Color = this.finishColor;
					this.ease = 1f;
				};
				base.Add(this.wiggler = Wiggler.Create(0.5f, 4f, delegate (float v)
				{
					this.pulse = Vector2.One * (1f + v * 0.25f);
				}, false, false));
				base.Add(new VertexLight(Color.White, 0.8f, 16, 32));
				base.Add(this.touchSfx = new SoundSource());
			}

			public ToggleTouchSwitch(EntityData data, Vector2 offset) : this(data.Position + offset)
			{
			}

			public void TurnOn()
			{
				if (!this.Switch.Activated)
				{
					this.touchSfx.Play("event:/game/general/touchswitch_any", null, 0f);
					if (this.Switch.Activate())
					{
						SoundEmitter.Play("event:/game/general/touchswitch_last_oneshot");
						base.Add(new SoundSource("event:/game/general/touchswitch_last_cutoff"));
					}
			    }
			}

		public void TurnOff()
		{
	
			if (this.Switch.Activated)
			{
				this.touchSfx.Play("event:/game/general/touchswitch_any", "Pitch", -7.5f);
				this.Switch.Deactivate();
			}
		}

		public override void Update()
			{
				this.timer += Engine.DeltaTime * 8f;
				this.ease = Calc.Approach(this.ease, (this.Switch.Finished || this.Switch.Activated) ? 1f : 0f, Engine.DeltaTime * 2f);
				this.icon.Color = Color.Lerp(this.inactiveColor, this.Switch.Finished ? this.finishColor : this.activeColor, this.ease);
				this.icon.Color *= 0.5f + ((float)Math.Sin((double)this.timer) + 1f) / 2f * (1f - this.ease) * 0.5f + 0.5f * this.ease;
				this.bloom.Alpha = this.ease;
			if (this.Switch.Finished)
			{
				if (this.icon.Rate > 0.1f)
				{
					this.icon.Rate -= 2f * Engine.DeltaTime;
					if (this.icon.Rate <= 0.1f)
					{
						this.icon.Rate = 0.1f;
						this.wiggler.Start();
						this.icon.Play("idle", false, false);
						this.level.Displacement.AddBurst(this.Position, 0.6f, 4f, 28f, 0.2f, null, null);
					}
				}
				else if (base.Scene.OnInterval(0.03f))
				{
					Vector2 position = this.Position + new Vector2(0f, 1f) + Calc.AngleToVector(Calc.Random.NextAngle(), 5f);
					this.level.ParticlesBG.Emit(TouchSwitch.P_Fire, position);
				}
			}
			else
			{
					var temp = this.Collider;

				
					this.Collider = new Hitbox(30f, 30f, -15f, -15f);
					bool hasPlayer = this.CollideCheck<Player>();
					if (hasPlayer)
					{
						if (!hadPlayer)
						{
							if (this.Switch.Activated) { TurnOff(); } else { TurnOn(); }
						}
					}
					hadPlayer = hasPlayer;

					this.Collider = new Hitbox(20f, 20f, -10f, -10f);
					bool hasHoldable = this.CollideCheckByComponent<Holdable>();
					if (hasHoldable)
					{
						if (!hadHoldable)
						{
							if (this.Switch.Activated) { TurnOff(); } else { TurnOn(); }
						}
					}
					hadHoldable = hasHoldable;

					this.Collider = new Hitbox(24f, 24f, -12f, -12f);
					bool hasSeeker = this.CollideCheck<Seeker>();
					if (hasSeeker)
					{
						if (!hadSeeker)
						{
							if (this.Switch.Activated) { TurnOff(); } else { TurnOn(); }
						}
					}
					hadSeeker = hasSeeker;

					this.Collider = temp;
				}

			base.Update();
			}

			// Token: 0x06001A7D RID: 6781 RVA: 0x000AAE94 File Offset: 0x000A9094
			public override void Render()
			{
				this.border.DrawCentered(this.Position + new Vector2(0f, -1f), Color.Black);
				this.border.DrawCentered(this.Position, this.icon.Color, this.pulse);
				base.Render();
			}
		}
	}
