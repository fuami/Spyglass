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
        static readonly float fullZoomTotalTime = 525f; // ms

        // animation state
        static readonly UpdateableTween zoomAnimation = new UpdateableTween(0f, fullZoomTotalTime);

        // track patches.
        private readonly ClientPatcher patcher;

        public ClientManipulation(Vintagestory.API.Common.ILogger logger)
        {
            patcher = new ClientPatcher();
            patcher.Start(logger);
            lastChange = ClientTime.Eariler();
        }

        public void Dispose()
        {
            patcher.Stop();
        }

        public static EnumCameraMode HandleCameraMode(EnumCameraMode mode)
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

            return realCameraMode = mode;
        }

        public static double GetSensitivityMultiplier()
        {
            return 1.0 + 2.0 * getPercentZoomed();
        }

        public static float GetZoomAdjust()
        {
            return (1.0f - getPercentZoomed()) * (percentUnzoomed - percentZoomed) + percentZoomed;
        }

        public static bool AttemptingToZoom()
        {
            return SpyglassMod.zoomed;
        }

        public static float getPercentZoomed()
        {
            if (isZoomed != AttemptingToZoom())
            {
                if (lastChange.ElapsedMilliseconds > 250)
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
