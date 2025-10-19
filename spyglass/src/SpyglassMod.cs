using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using spyglass.src.Client;
using spyglass.src.Network;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Server;
using Vintagestory.Client.NoObf;
using Vintagestory.Server;

namespace spyglass.src
{
    class SpyglassMod : ModSystem
    {
        internal const float MIN_ZOOM = 0.8f;
        internal const float MAX_ZOOM = 1.0f;

        public static bool zoomed = false; // enabled by item.
        private static float zoomRatio = 0.0f; // adjusted via mouse-wheel while zoomed.

        private static ClientManipulation clientLogic;
        internal static double gameRuntime = 0; // used by Util/ClientTime

        private static string configFile = "spyglass.json";
        private static SpyglassConfig loadedConfig;

        internal static float getZoomRatio()
        {
            return zoomRatio;
        }

        internal static void setZoomRatio(float value)
        {
            zoomRatio = value;
            if (zoomRatio < SpyglassMod.MIN_ZOOM) zoomRatio = MIN_ZOOM;
            if (zoomRatio > SpyglassMod.MAX_ZOOM) zoomRatio = MAX_ZOOM;
        }

        public static SpyglassConfig config
        {
            get => loadedConfig;
        }

        internal static void setRatioToDefault()
        {
            setZoomRatio(MIN_ZOOM + (MAX_ZOOM - MIN_ZOOM) * config.defaultZoomPosition);
        }

        internal static void ResetZoomRatio()
        {
            if (!config.preserveZoomBetweenUses)
                setRatioToDefault();
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
            clientLogic = new ClientManipulation(api);
            api.Network.RegisterChannel("spyglassChannel")
                .RegisterMessageType(typeof(ServerConfigSyncPacket))
                .SetMessageHandler<ServerConfigSyncPacket>(serverConfig =>
                {
                    loadedConfig.vignetteStyle = serverConfig.vignetteStyle.ToString();
                    loadedConfig.edgeOpacity = serverConfig.edgeOpacity;
                    loadedConfig.edgeSize = serverConfig.edgeSize;
                });
            api.Gui.RegisterDialog(new[]{ new ZoomWheel(api) });
            api.Event.RegisterGameTickListener(OnGameTick, 4); // 250 max fps - This is a simple light weight add too, so shouldn't make a diffrence.
            setRatioToDefault();
        }

        public override void StartServerSide(ICoreServerAPI api)
        {
            var serverNetworkChannel = api.Network.RegisterChannel("spyglassChannel")
                .RegisterMessageType(typeof(ServerConfigSyncPacket));
            api.Event.PlayerJoin += byPlayer =>
            {
                if (loadedConfig.overrideClientConfig)
                {
                    serverNetworkChannel.SendPacket(new ServerConfigSyncPacket()
                    {
                        edgeOpacity = loadedConfig.edgeOpacity,
                        edgeSize = loadedConfig.edgeSize,
                        vignetteStyle = loadedConfig.GetVinetteStyle()
                    }, byPlayer);
                }
            };
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
