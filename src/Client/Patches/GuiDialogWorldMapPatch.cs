using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;

namespace spyglass.src.Client.Patches
{
    class GuiDialogWorldMapPatch : GuiDialogHideMultiPatch
    {
        public override Type GetTargetType()
        {
            return typeof(Vintagestory.GameContent.GuiDialogWorldMap);
        }
    }
}
