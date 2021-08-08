using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using spyglass.src.Client;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.Client.NoObf;

namespace spyglass.src
{
    class SpyglassMod : ModSystem
    {
        public static bool zoomed = false; // enabled by item.
        internal static float zoomRatio = 0.97f; // adjusted via mouse-wheel while zoomed.

        private static ClientManipulation clientLogic;
        internal static double gameRuntime = 0; // used by Util/ClientTime

        private static string configFile = "spyglass.json";
        private static SpyglassConfig loadedConfig;

        public static SpyglassConfig config
        {
            get => loadedConfig;
        }

        internal static void ResetZoomRatio()
        {
            zoomRatio = 0.97f;
        }

        public override void Start(ICoreAPI api)
        {
            base.Start(api);

            try
            {
                loadedConfig = api.LoadModConfig<SpyglassConfig>(configFile);
                if (loadedConfig == null) throw new FileNotFoundException();
            }
            catch (Exception)
            {
                loadedConfig = new SpyglassConfig();
                api.StoreModConfig<SpyglassConfig>(loadedConfig, configFile);
            }

            api.RegisterItemClass("spyglass:ItemSpyglass", typeof(ItemSpyglass));
        }

        public override void StartClientSide(ICoreClientAPI api)
        {
            clientLogic = new ClientManipulation(api.Logger);
            api.Gui.RegisterDialog(new[]{ new ZoomWheel(api) });
            api.Event.RegisterGameTickListener(OnGameTick, 4); // 250 max fps - This is a simple light weight add too, so shouldn't make a diffrence.
        }

        private void OnGameTick(float dt)
        {
            gameRuntime += (double)dt;
        }

        public override void Dispose()
        {
            if ( clientLogic != null )
            {
                clientLogic.Dispose();
                clientLogic = null;
            }
        }

    }
}
