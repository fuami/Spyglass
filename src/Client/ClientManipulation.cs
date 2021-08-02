using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.API.Client;

namespace spyglass.src
{
    class ClientManipulation
    {
        private static ICoreClientAPI capi;

        // prevent instant cycling by buffering changes when they are too fast.
        private static bool currentMode = false;

        // timing tracking.
        static Stopwatch lastChange;
        static Stopwatch zoomTime;
        static Stopwatch unzoomTime;

        // state
        static float currentZoom = 1.0f;
        static float lastZoom = 1.0f;
        static EnumCameraMode realCameraMode = EnumCameraMode.FirstPerson;
        static bool resetCameraMode = false;

        // settings.
        static readonly float percentZoomed = 0.1f;
        static readonly float percentUnzoomed = 1.0f;
        static readonly float timeTillZoomed = 550f;

        private readonly ClientPatcher patcher;

        public ClientManipulation(ICoreClientAPI api)
        {
            patcher = new ClientPatcher();
            patcher.Start();
            capi = api;
        }

        public void Dispose()
        {
            patcher.Stop();
        }

        public static EnumCameraMode HandleCameraMode(EnumCameraMode mode)
        {
            if (GetLinearZoom() != 0)
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
            return 1.0 + 2.0 * GetLinearZoom();
        }

        public static float GetZoomAdjust()
        {
            return (1.0f - GetLinearZoom()) * (percentUnzoomed - percentZoomed) + percentZoomed;
        }

        public static bool zoomed()
        {
            return SpyglassMod.zoomed;
        }

        public static float GetLinearZoom()
        {
            if (capi.IsGamePaused)
            {
                // just hold zoom, this isn't perfect, ( it will just snap out of the animation when unpaused )
                // but I'm not sure its worht the effort to improve it.
                return lastZoom;
            }

            if (currentMode != zoomed() && lastChange != null)
            {
                if (lastChange.ElapsedMilliseconds > 250)
                    currentMode = zoomed();
            }
            else if (lastChange == null)
            {
                lastChange = Stopwatch.StartNew();
            }

            if (currentMode)
            {
                unzoomTime = null;

                if (zoomTime == null)
                {
                    lastChange = zoomTime = Stopwatch.StartNew();
                    currentZoom = lastZoom;
                }

                return lastZoom = ZoomInterpolate(currentZoom, 1f, zoomTime.ElapsedMilliseconds);
            }
            else
            {
                zoomTime = null;

                if (unzoomTime == null)
                {
                    lastChange = unzoomTime = Stopwatch.StartNew();
                    currentZoom = lastZoom;
                }

                return lastZoom = ZoomInterpolate(currentZoom, 0f, unzoomTime.ElapsedMilliseconds);
            }
        }

        private static float ZoomInterpolate(float f, float s, long timeSinceStart)
        {
            float transitionLength = Math.Abs(f - s);

            float mu = Math.Max(0f, Math.Min(1f, timeSinceStart / (timeTillZoomed * transitionLength)));
            float mu2 = (1f - (float)Math.Cos(mu * Math.PI)) / 2f;
            return (f * (1f - mu2) + s * mu2);
        }

    }
}
