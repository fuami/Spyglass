using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using Vintagestory.API.Client;
using Vintagestory.API.MathTools;

namespace spyglass.src.Client.Patches
{
    class HudElementCoordinatesPatch : GuiDialogHideMultiPatch
    {
        public override Type GetTargetType()
        {
            return typeof(Vintagestory.Client.NoObf.HudElementCoordinates);
        }
    }
}
