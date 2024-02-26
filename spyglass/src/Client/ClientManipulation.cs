using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using spyglass.src.Client;
using Vintagestory.API.Client;
using Vintagestory.API.Common;

namespace spyglass.src
{
    class ClientManipulation
    {
        // prevent instant cycling by buffering changes when they are too fast.
        private static bool isZoomed = false;
        private static ClientTime lastChange;
        private static ICoreClientAPI capi;

        // state
        private static EnumCameraMode realCameraMode = EnumCameraMode.FirstPerson;
        private static bool resetCameraMode = false;


        // settings.
        private const float percentZoomed = 0.08f;
        private const float percentUnzoomed = 1.0f;

        // animation state
        private static UpdateableTween zoomAnimation;

        // track patches.
        private readonly ClientPatcher patcher;

        public ClientManipulation(ICoreClientAPI capi)
        {
            ClientManipulation.capi = capi;
            zoomAnimation = new UpdateableTween(0f, SpyglassMod.config.transitionTimeBasis);

            patcher = new ClientPatcher();
            patcher.Start(capi.Logger);
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

        internal static bool IsLocalPlayer(EntityAgent byEntity)
        {
            return capi.World.Player.Entity == byEntity;
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

        public static float GetWheelAdjustment()
        {
            float adjust = 40.0f / Math.Abs(SpyglassMod.config.zoomWheelSpeedMultiplier);

            if (SpyglassMod.config.invertZoomWheel)
                return -adjust;

            return adjust;
        }

        public static bool AttemptingToZoom()
        {
            // validate if we can in fact, zoom.
            IPlayerInventoryManager manager = capi?.World?.Player?.InventoryManager;

            if (manager != null)
            {
                int slotIndex = manager.ActiveHotbarSlotNumber;
                IInventory inv = manager.GetHotbarInventory();
                if (inv != null && slotIndex >= 0 && slotIndex < inv.Count())
                {
                    ItemSlot itemSlot = inv[slotIndex];

                    AssetLocation code = itemSlot?.Itemstack?.Item?.Code;
                    if (code != null && code.BeginsWith("spyglass", "spyglass-"))
                    {
                        return SpyglassMod.zoomed;
                    }
                }
            }

            return SpyglassMod.zoomed = false;
        }

        public static bool HideGuis()
        {
            return isZoomed && getPercentZoomed() > 0.01;
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
                return zoomAnimation.getValue(SpyglassMod.getZoomRatio());
            }
            else
            {
                SpyglassMod.ResetZoomRatio();
                return zoomAnimation.getValue(0f);
            }
        }


    }
}
