using System;
using Microsoft.Xna.Framework;
using Celeste.Mod.SpekioToolbox;
using Monocle;

namespace Celeste.Mod.SpekioToolbox
{
    class SpekioToolboxModule : EverestModule
    {

        public static SpekioToolboxModule Instance;
        public static SpriteBank SpriteBank { get; private set; }

        public SpekioToolboxModule()
        {
            Instance = this;
        }

        public override Type SettingsType => null;
        public override Type SessionType => typeof(SpekioToolboxSession);
        public static SpekioToolboxSession Session => (SpekioToolboxSession)Instance._Session;

        public override void Load()
        {
            //Logger.Log("SPEKIOTOOLBOX.module", "Module Load()");
            SpekioToolboxHooks.Load();
        }

        public override void Unload()
        {
            // Logger.Log("SPEKIOTOOLBOX.module", "Module Unload()");
            SpekioToolboxHooks.Unload();
        }
        public override void LoadContent(bool firstLoad)
        {
            base.LoadContent(firstLoad);
            SpriteBank = new SpriteBank(GFX.Game, "Graphics/SpekioToolbox/SpekioToolboxSprites.xml");
        }
    }
}
