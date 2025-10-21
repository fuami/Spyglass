using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace spyglass.src
{
    public enum VignetteStyle
    {
        circle, // what it says.
        square, // what it says
        box, // rather then a square it cuts off the edges keeping screen aspect ratio mostly intact.
        edge
    };

    class SpyglassConfig
    {
        public String vignetteStyle = "edge";

        public int edgeSize = 32; // thickness of edge
        public float edgeOpacity = 0.5f; // transparency of edge

        public float glassBrightness = 1.0f; // less then 1.0, will dim the view when using spyglass
        public float glassColor = 0.94f; // the brightness of the glass material, by default this is nearly white like quartz.

        public float transitionTimeBasis = 525f; // ms to do a complete zoom ( rougly ), higher slows animation, lower quickens it.
        public float minimumZoomTime = 250f; // minimum time using spyglass.
        public float defaultZoomPosition = 0.85f;

        public float zoomWheelSpeedMultiplier = 1.0f;
        public bool invertZoomWheel = false;

        public float mouseSensitivityAdjustment = 2.0f; // 200% - raise to make panning slower when using spyglass

        public bool enableMouseWheelAdjustment = true; // mouse wheel while using spyglass adjustment
        public bool enableZoomInThirdPerson = true; // if disabled you will just see your character use the spyglass.

        public bool hideHUDWhileSpying = true; // removes minimap/coordinates/statsbar/hotbar to focus only on the what your looking at.
        public bool preserveZoomBetweenUses = false; // restores previous zoom when using spyglass

        public bool overrideClientConfig = false; // whether the server overrides connected clients' vignette settings
        
        public VignetteStyle GetVinetteStyle()
        {
            try
            {
                return Enum.Parse<VignetteStyle>(SpyglassMod.config.vignetteStyle);
            }
            catch { }

            return VignetteStyle.edge;
        }
    }
}
