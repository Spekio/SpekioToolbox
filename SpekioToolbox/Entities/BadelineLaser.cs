using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monocle;

namespace Celeste.Mod.SpekioToolbox
{
	[Pooled]
	[Tracked(false)]
	public class BadelineLaser : Entity
	{
		public ParticleType P_Dissipate = new ParticleType(FinalBossBeam.P_Dissipate);
		public const float ActiveTime = 0.12f;
		private const float AngleStartOffset = 100f;
		private const float RotationSpeed = 200f;
		private const float CollideCheckSep = 2f;
		private const float BeamLength = 2000f;
		private const float BeamStartDist = 12f;
		private const int BeamsDrawn = 15;
		private const float SideDarknessAlpha = 0.35f;
		private LaserTurret source;
		private Player player;
		private Sprite beamSprite;
		private Sprite beamStartSprite;
		private String color;
		private float chargeTimer;
		private float followTimer;
		private float activeTimer;
		private float angle;
		private float beamAlpha;
		private float sideFadeAlpha;
		private VertexPositionColor[] fade;
		public BadelineLaser()
		{
			this.fade = new VertexPositionColor[24];
			//base.vector();
			base.Depth = -1000000;
		}

		public BadelineLaser Init(LaserTurret source, Player target, String laserColor, float chargeTimer = 1.4f, float followTimer = 0.9f)
		{
			this.source = source;
			this.chargeTimer = chargeTimer;
			this.followTimer = followTimer;
			this.activeTimer = 0.12f;
			this.color = laserColor;
			if (beamSprite != null)
			{
				base.Remove(beamSprite);
			}

			if (beamStartSprite != null)
			{
				base.Remove(beamStartSprite);
			}

			base.Add(this.beamSprite = SpekioToolboxModule.SpriteBank.Create("badeline_beam_" + color));
			this.beamSprite.OnLastFrame = delegate (string anim)
			{
				if (anim == "shoot")
				{
					this.Destroy();
				}
			};
			base.Add(this.beamStartSprite = SpekioToolboxModule.SpriteBank.Create("badeline_beam_start_" + color));
			this.beamSprite.Visible = false;
			SetParticleColor();
			this.beamSprite.Play("charge", false, false);
			this.sideFadeAlpha = 0f;
			this.beamAlpha = 0f;
			int num;
			if (target.Y <= this.source.Y)//(target.Y <= this.entity.Y + 16f)
			{
				num = 1;
			}
			else
			{
				num = -1;
			}
			if (target.X >= this.source.X)
			{
				num *= -1;
			}
			this.angle = Calc.Angle(BeamOrigin, target.Center);
			Vector2 vector = Calc.ClosestPointOnLine(BeamOrigin, BeamOrigin + Calc.AngleToVector(this.angle, 2000f), target.Center);
			vector += (target.Center - BeamOrigin).Perpendicular().SafeNormalize(100f) * (float)num;
			this.angle = Calc.Angle(BeamOrigin, vector);
			return this;
		}

		public override void Added(Scene scene)
		{
			base.Added(scene);
			/*if (this.entity.Moving)
			{
				base.RemoveSelf();
			}*/
		}

		public override void Update()
		{
			if (source.activated)
			{
				base.Update();
				this.player = base.Scene.Tracker.GetEntity<Player>();
				this.beamAlpha = Calc.Approach(this.beamAlpha, 1f, 2f * Engine.DeltaTime);
				if (this.chargeTimer > 0f)
				{
					this.sideFadeAlpha = Calc.Approach(this.sideFadeAlpha, 1f, Engine.DeltaTime);
					if (this.player != null && !this.player.Dead)
					{
						this.followTimer -= Engine.DeltaTime;
						this.chargeTimer -= Engine.DeltaTime;
						if (this.followTimer > 0f && this.player.Center != BeamOrigin)
						{
							Vector2 vector = Calc.ClosestPointOnLine(BeamOrigin, BeamOrigin + Calc.AngleToVector(this.angle, 2000f), this.player.Center);
							Vector2 center = this.player.Center;
							vector = Calc.Approach(vector, center, 200f * Engine.DeltaTime);
							this.angle = Calc.Angle(BeamOrigin, vector);
						}
						else if (this.beamSprite.CurrentAnimationID == "charge")
						{
							this.beamSprite.Play("lock", false, false);
						}
						if (this.chargeTimer <= 0f)
						{
							base.SceneAs<Level>().DirectionalShake(Calc.AngleToVector(this.angle, 1f), 0.15f);
							Input.Rumble(RumbleStrength.Medium, RumbleLength.Medium);
							this.DissipateParticles();
							return;
						}
					}
				}
				else if (this.activeTimer > 0f)
				{
					this.sideFadeAlpha = Calc.Approach(this.sideFadeAlpha, 0f, Engine.DeltaTime * 8f);
					if (this.beamSprite.CurrentAnimationID != "shoot")
					{
						this.beamSprite.Play("shoot", false, false);
						this.beamStartSprite.Play("shoot", true, false);
					}
					this.activeTimer -= Engine.DeltaTime;
					if (this.activeTimer > 0f)
					{
						this.PlayerCollideCheck();
					}
				}
            }
            else
            {
				Destroy();
			}
		}

		private void DissipateParticles()
		{
			Level level = base.SceneAs<Level>();
			Vector2 vector = level.Camera.Position + new Vector2(160f, 90f);
			Vector2 vector2 = BeamOrigin + Calc.AngleToVector(this.angle, 12f);
			Vector2 vector3 = BeamOrigin + Calc.AngleToVector(this.angle, 2000f);
			Vector2 vector4 = (vector3 - vector2).Perpendicular().SafeNormalize();
			Vector2 value = (vector3 - vector2).SafeNormalize();
			Vector2 min = -vector4 * 1f;
			Vector2 max = vector4 * 1f;
			float direction = vector4.Angle();
			float direction2 = (-vector4).Angle();
			float num = Vector2.Distance(vector, vector2) - 12f;
			vector = Calc.ClosestPointOnLine(vector2, vector3, vector);
			for (int i = 0; i < 200; i += 12)
			{
				for (int j = -1; j <= 1; j += 2)
				{
					//Logger.Log("SPEKIOTOOLBOX.BadelineLaser", "1: Emit=(P_Dissipate, "+ (vector + value * (float)i + vector4 * 2f * (float)j + Calc.Random.Range(min, max)) + ","+ direction + ")");
					level.ParticlesFG.Emit(P_Dissipate, vector + value * (float)i + vector4 * 2f * (float)j + Calc.Random.Range(min, max), direction);
					//Logger.Log("SPEKIOTOOLBOX.BadelineLaser", "2: Emit=(P_Dissipate, "+ (vector + value * (float)i - vector4 * 2f * (float)j + Calc.Random.Range(min, max)) + "," + direction2 + ")");
					level.ParticlesFG.Emit(P_Dissipate, vector + value * (float)i - vector4 * 2f * (float)j + Calc.Random.Range(min, max), direction2);
					if (i != 0 && (float)i < num)
					{
						//Logger.Log("SPEKIOTOOLBOX.BadelineLaser", "3: Emit=(P_Dissipate, " + (vector - value * (float)i + vector4 * 2f * (float)j + Calc.Random.Range(min, max)) + "," + direction + ")");
						level.ParticlesFG.Emit(P_Dissipate, vector - value * (float)i + vector4 * 2f * (float)j + Calc.Random.Range(min, max), direction);
						//Logger.Log("SPEKIOTOOLBOX.BadelineLaser", "4: Emit=(P_Dissipate, " + (vector - value * (float)i - vector4 * 2f * (float)j + Calc.Random.Range(min, max)) + "," + direction2 + ")");
						level.ParticlesFG.Emit(P_Dissipate, vector - value * (float)i - vector4 * 2f * (float)j + Calc.Random.Range(min, max), direction2);
					}
				}
			}
		}

		private void PlayerCollideCheck()
		{
			Vector2 vector = BeamOrigin + Calc.AngleToVector(this.angle, 12f);
			Vector2 vector2 = BeamOrigin + Calc.AngleToVector(this.angle, 2000f);
			Vector2 value = (vector2 - vector).Perpendicular().SafeNormalize(2f);
			Player player = base.Scene.CollideFirst<Player>(vector + value, vector2 + value);
			if (player == null)
			{
				player = base.Scene.CollideFirst<Player>(vector - value, vector2 - value);
			}
			if (player == null)
			{
				player = base.Scene.CollideFirst<Player>(vector, vector2);
			}
			if (player != null)
			{
				player.Die((player.Center - this.source.Center).SafeNormalize(), false, true);
			}
		}

		public override void Render()
		{
			Vector2 vector = BeamOrigin;
			Vector2 vector2 = Calc.AngleToVector(this.angle, this.beamSprite.Width);
			this.beamSprite.Rotation = this.angle;
			this.beamSprite.Color = Color.White * this.beamAlpha;
			this.beamStartSprite.Rotation = this.angle;
			this.beamStartSprite.Color = Color.White * this.beamAlpha;
			if (this.beamSprite.CurrentAnimationID == "shoot")
			{
				vector += Calc.AngleToVector(this.angle, 0f);
			}
			for (int i = 0; i < 15; i++)
			{
				this.beamSprite.RenderPosition = vector;
				this.beamSprite.Render();
				vector += vector2;
			}
			if (this.beamSprite.CurrentAnimationID == "shoot")
			{
				this.beamStartSprite.RenderPosition = BeamOrigin - Calc.AngleToVector(this.angle, 8f);
				this.beamStartSprite.Render();
			}
			GameplayRenderer.End();
			Vector2 vector3 = vector2.SafeNormalize();
			Vector2 vector4 = vector3.Perpendicular();
			Color color = Color.Black * this.sideFadeAlpha * 0.35f;
			Color transparent = Color.Transparent;
			vector3 *= 4000f;
			vector4 *= 120f;
			int num = 0;
			this.Quad(ref num, vector, -vector3 + vector4 * 2f, vector3 + vector4 * 2f, vector3 + vector4, -vector3 + vector4, color, color);
			this.Quad(ref num, vector, -vector3 + vector4, vector3 + vector4, vector3, -vector3, color, transparent);
			this.Quad(ref num, vector, -vector3, vector3, vector3 - vector4, -vector3 - vector4, transparent, color);
			this.Quad(ref num, vector, -vector3 - vector4, vector3 - vector4, vector3 - vector4 * 2f, -vector3 - vector4 * 2f, color, color);
			GFX.DrawVertices<VertexPositionColor>((base.Scene as Level).Camera.Matrix, this.fade, this.fade.Length, null, null);
			GameplayRenderer.Begin();
		}

		private void Quad(ref int v, Vector2 offset, Vector2 a, Vector2 b, Vector2 c, Vector2 d, Color ab, Color cd)
		{
			this.fade[v].Position.X = offset.X + a.X;
			this.fade[v].Position.Y = offset.Y + a.Y;
			VertexPositionColor[] array = this.fade;
			int num = v;
			v = num + 1;
			array[num].Color = ab;
			this.fade[v].Position.X = offset.X + b.X;
			this.fade[v].Position.Y = offset.Y + b.Y;
			VertexPositionColor[] array2 = this.fade;
			num = v;
			v = num + 1;
			array2[num].Color = ab;
			this.fade[v].Position.X = offset.X + c.X;
			this.fade[v].Position.Y = offset.Y + c.Y;
			VertexPositionColor[] array3 = this.fade;
			num = v;
			v = num + 1;
			array3[num].Color = cd;
			this.fade[v].Position.X = offset.X + a.X;
			this.fade[v].Position.Y = offset.Y + a.Y;
			VertexPositionColor[] array4 = this.fade;
			num = v;
			v = num + 1;
			array4[num].Color = ab;
			this.fade[v].Position.X = offset.X + c.X;
			this.fade[v].Position.Y = offset.Y + c.Y;
			VertexPositionColor[] array5 = this.fade;
			num = v;
			v = num + 1;
			array5[num].Color = cd;
			this.fade[v].Position.X = offset.X + d.X;
			this.fade[v].Position.Y = offset.Y + d.Y;
			VertexPositionColor[] array6 = this.fade;
			num = v;
			v = num + 1;
			array6[num].Color = cd;
		}

		public void Destroy()
		{
			base.RemoveSelf();
		}

		public Vector2 BeamOrigin
		{
			get
			{
				return source.Center + new Vector2(0f, 0f);
			}
		}

		private void SetParticleColor()
		{
			if (color == "blue")
			{
				P_Dissipate.Color = Calc.HexToColor("4fa0ff");
			}
			else if (color == "green")
			{
				P_Dissipate.Color = Calc.HexToColor("3ebe00");
			}
			else if (color == "gray")
			{
				P_Dissipate.Color = Calc.HexToColor("a7a7a7");
			}
			else if (color == "purple")
			{
				P_Dissipate.Color = Calc.HexToColor("9f4fff");
			}
			else if (color == "red")
			{
				P_Dissipate.Color = Calc.HexToColor("e60022");
			}
		}
	}
}
