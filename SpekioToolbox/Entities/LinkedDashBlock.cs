using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;
using MonoMod;
using System.Reflection;

namespace Celeste.Mod.SpekioToolbox
{
    [CustomEntity("SpekioToolbox/LinkedDashBlock")]
    [TrackedAs(typeof(DashBlock))]
    public class LinkedDashBlock : DashBlock
    {
        public bool permanent;
        public string linkId = "flag";
        public char tileType;

        public LinkedDashBlock(EntityData data, Vector2 offset, EntityID id) : base(data, offset, id)
        {

            this.linkId = data.Attr("linkId");
            this.permanent = data.Bool("permanent");
            this.tileType = data.Char("tiletype");
        }

        internal static void Break(On.Celeste.DashBlock.orig_Break_Vector2_Vector2_bool_bool orig, DashBlock self, Vector2 from, Vector2 direction, bool playSound, bool playDebrisSound)
        {
            //Logger.Log("SPEKIOTOOLBOX.LinkedDashBlock", "internal static void Break() started x="+self.X);
            if (self is LinkedDashBlock)
            {
            //    Logger.Log("SPEKIOTOOLBOX.LinkedDashBlock", "Calling (self as LinkedDashBlock).OnBreak(from, direction) x=" + self.X);
                    (self as LinkedDashBlock).OnBreak(from, direction);
            }
            //Logger.Log("SPEKIOTOOLBOX.LinkedDashBlock", "Break Hook Call to Orig X=" + self.X);
            orig(self, from, direction, playSound, playDebrisSound);
        }

        public void LinkedBreak(Vector2 from, Vector2 direction, bool playSound, bool playDebrisSound)
        {
            //Logger.Log("SPEKIOTOOLBOX.LinkedDashBlock", "LinkedBreak()) started X="+ this.X);
            if (playSound)
            {
                if (this.tileType == '1')
                {
                    Audio.Play("event:/game/general/wall_break_dirt", this.Position);
                }
                else if (this.tileType == '3')
                {
                    Audio.Play("event:/game/general/wall_break_ice", this.Position);
                }
                else if (this.tileType == '9')
                {
                    Audio.Play("event:/game/general/wall_break_wood", this.Position);
                }
                else
                {
                    Audio.Play("event:/game/general/wall_break_stone", this.Position);
                }
            }
            int num = 0;
            while ((float)num < base.Width / 8f)
            {
                int num2 = 0;
                while ((float)num2 < base.Height / 8f)
                {
                    base.Scene.Add(Engine.Pooler.Create<Debris>().Init(this.Position + new Vector2((float)(4 + num * 8), (float)(4 + num2 * 8)), this.tileType, playDebrisSound).BlastFrom(from));
                    num2++;
                }
                num++;
            }
            this.Collidable = false;
            if (this.permanent)
            {
            //    Logger.Log("SPEKIOTOOLBOX.LinkedDashBlock", "Removed Permanantly X=" + this.X);
                this.RemoveAndFlagAsGone();
                return;
            }
            //Logger.Log("SPEKIOTOOLBOX.LinkedDashBlock", "Removed Temporarily X=" + this.X);
            base.RemoveSelf();
        }


        public void OnBreak(Vector2 from, Vector2 direction)
        {
            //Logger.Log("SPEKIOTOOLBOX.LinkedDashBlock", "OnBreak(Vector2 from, Vector2 direction)) started X=" + this.X);
            Level level = (Scene as Level);
            if (this.permanent) { 
                level.Session.SetFlag("SpekioLinkedDashBlock_" + this.linkId, true);
            }
            //Logger.Log("SPEKIOTOOLBOX.LinkedDashBlock", "level.Session.SetFlag(SpekioLinkedDashBlock_"+ this.linkId+", true) done");
            List<Entity> dashblocks = Scene.Tracker.GetEntities<DashBlock>();
            for (int i=0; i<dashblocks.Count; i++)
            {
            //    Logger.Log("SPEKIOTOOLBOX.LinkedDashBlock", "dashblocks[" + i + "] isLinkedDashBlock="+ (dashblocks[i] is LinkedDashBlock) + " X=" + dashblocks[i].X);
                if (dashblocks[i] is LinkedDashBlock)
                { 
                    LinkedDashBlock ldb = (dashblocks[i] as LinkedDashBlock);
                    Logger.Log("SPEKIOTOOLBOX.LinkedDashBlock", "dashblocks[" + i + "] Object.ReferenceEquals(ldb"+ldb.X+", this"+this.X+")="+Object.ReferenceEquals(ldb, this) + " X=" + dashblocks[i].X);
                    if (ldb.linkId == this.linkId && !Object.ReferenceEquals(ldb, this))
                    {
                        ldb.LinkedBreak(from, direction, true, true);
                    }
                    
                }

            }

        }

        public override void Added(Scene scene)
        {

         //   Logger.Log("SPEKIOTOOLBOX.LinkedDashBlock", "Added(Scene scene) started");
            base.Added(scene);
            if (this != null && Scene != null)
            {
        //        Logger.Log("SPEKIOTOOLBOX.LinkedDashBlock", "SpekioLinkedDashBlock_" + this.linkId + ((Scene as Level).Session.GetFlag("SpekioLinkedDashBlock_" + this.linkId)));
                if ((Scene as Level).Session.GetFlag("SpekioLinkedDashBlock_" + this.linkId))
                {
        //            Logger.Log("SPEKIOTOOLBOX.LinkedDashBlock", "Removing block on room entry.");
                    this.Collidable = false;
                    if (permanent)
                    {
                        this.RemoveAndFlagAsGone();
                        return;
                    }
                    base.RemoveSelf();
                }
          }
        }
    }
}
