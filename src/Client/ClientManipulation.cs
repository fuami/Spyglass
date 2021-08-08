using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using spyglass.src.Client;
using Vintagestory.API.Client;

namespace spyglass.src
{
    class ClientManipulation
    {
        // prevent instant cycling by buffering changes when they are too fast.
        private static bool isZoomed = false;
        static ClientTime lastChange;

        // state
        static EnumCameraMode realCameraMode = EnumCameraMode.FirstPerson;
        static bool resetCameraMode = false;

        // settings.
        static readonly float percentZoomed = 0.08f;
        static readonly float percentUnzoomed = 1.0f;

        // animation state
        static UpdateableTween zoomAnimation;

        // track patches.
        private readonly ClientPatcher patcher;

        public ClientManipulation(Vintagestory.API.Common.ILogger logger)
        {
            zoomAnimation = new UpdateableTween(0f, SpyglassMod.config.transitionTimeBasis);

            patcher = new ClientPatcher();
            patcher.Start(logger);
            lastChange = ClientTime.Eariler();
        }

        public void Dispose()
        {
            patcher.Stop();
        }

        internal static bool EnableEffect()
        {
            if (!SpyglassMod.config.enableZoomInThirdPerson && realCameraMode != EnumCameraMode.FirstPerson)
                return false;

            return true;
        }

        public static EnumCameraMode HandleCameraMode(EnumCameraMode mode)
        {
            if (EnableEffect())
            {
                if (getPercentZoomed() > 0.001f)
                {
                    resetCameraMode = true;
                    return EnumCameraMode.FirstPerson;
                }
                else if (resetCameraMode)
                {
                    resetCameraMode = false;
                    return realCameraMode;
                }
            }

            return realCameraMode = mode;
        }

        public static double GetSensitivityMultiplier()
        {
            if (EnableEffect())
                return 1.0 + SpyglassMod.config.mouseSensitivityAdjustment * getPercentZoomed();
            return 1.0;
        }

        public static float GetZoomAdjust()
        {
            if (EnableEffect())
                return (1.0f - getPercentZoomed()) * (percentUnzoomed - percentZoomed) + percentZoomed;
            return 1.0f;
        }

        public static bool AttemptingToZoom()
        {
            return SpyglassMod.zoomed;
        }

        public static float getPercentZoomed()
        {
            if (isZoomed != AttemptingToZoom())
            {
                if (lastChange.ElapsedMilliseconds > SpyglassMod.config.minimumZoomTime)
                {
                    isZoomed = AttemptingToZoom();
                    lastChange = ClientTime.StartNew();
                }
            }

            if (isZoomed)
            {
                return zoomAnimation.getValue(SpyglassMod.zoomRatio);
            }
            else
            {
                SpyglassMod.ResetZoomRatio();
                return zoomAnimation.getValue(0f);
            }
        }


    }
}
