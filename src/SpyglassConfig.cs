using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace spyglass.src
{
    class SpyglassConfig
    {
        public int edgeSize = 32; // thickness of edge
        public float edgeOpacity = 0.5f; // transparency of edge

        public float glassBrightness = 1.0f; // less then 1.0, will dim the view when using spyglass

        public float transitionTimeBasis = 525f; // ms to do a complete zoom ( rougly ), higher slows animation, lower quickens it.
        public float minimumZoomTime = 250f; // minimum time using spyglass.

        public float mouseSensitivityAdjustment = 2.0f; // 200% - raise to make panning slower when using spyglass

        public bool enableMouseWheelAdjustment = true; // mouse wheel while using spyglass adjustment
        public bool enableZoomInThirdPerson = true; // if disabled you will just see your character use the spyglass.
    }
}
