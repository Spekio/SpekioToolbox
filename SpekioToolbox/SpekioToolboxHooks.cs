using System.Reflection;
using System;
using System.Collections;
using Celeste;
using Celeste.Mod;
using IL.Celeste;
using MonoMod.Cil;
using MonoMod.Utils;
using On.Celeste;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.SpekioToolbox
{
    class SpekioToolboxHooks
    {
        public static void Load()
        {
            //Logger.Log("SPEKIOTOOLBOX.Hooks", "Load() Start");
            On.Celeste.DashBlock.Break_Vector2_Vector2_bool_bool += LinkedDashBlock.Break;
            //Logger.Log("SPEKIOTOOLBOX.Hooks", "Load() Finish");
        }

        public static void Unload()
        {
            //Logger.Log("SPEKIOTOOLBOX.Hooks", "Unload() Start");
            On.Celeste.DashBlock.Break_Vector2_Vector2_bool_bool -= LinkedDashBlock.Break;
            //sLogger.Log("SPEKIOTOOLBOX.Hooks", "Unload() Finish");
        }

    }
}
