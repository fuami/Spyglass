using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.Client.NoObf;

namespace spyglass.src
{
    class SpyglassMod : ModSystem
    {
        public static bool zoomed = false;
        private static ClientManipulation cm;
        internal static double gameRuntime = 0;

        public override void Start(ICoreAPI api)
        {
            base.Start(api);
            api.RegisterItemClass("spyglass:ItemSpyglass", typeof(ItemSpyglass));
        }

        public override void StartClientSide(ICoreClientAPI api)
        {
            cm = new ClientManipulation(api);
            api.Event.RegisterGameTickListener(OnGameTick, 4); // 250 max fps - This is a simple light weight add too, so shouldn't make a diffrence.
        }

        private void OnGameTick(float dt)
        {
            gameRuntime += (double)dt;
        }

        public override void Dispose()
        {
            if ( cm != null )
            {
                cm.Dispose();
                cm = null;
            }
        }
    }
}
