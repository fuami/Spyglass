using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using Vintagestory.API.Client;
using Vintagestory.API.MathTools;

namespace spyglass.src.Client.Patches
{
    class GuiDialogHudPatch : BasePatch
    {
        public static bool HideGui(GuiDialog target)
        {
            if (target is ZoomWheel)
                return false;
            return ClientManipulation.HideGuis();
        }

        public static IEnumerable<CodeInstruction> AdjustGuiOpacity_Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator g)
        {
            List<CodeInstruction> output = new List<CodeInstruction>();

            Label returnEqual = g.DefineLabel();
            output.Add(new CodeInstruction(OpCodes.Ldarg_0));
            output.Add(new CodeInstruction(OpCodes.Call, typeof(GuiDialogHudPatch).GetMethod("HideGui", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public)));
            output.Add(new CodeInstruction(OpCodes.Brtrue, returnEqual));

            /* we are just adding this code to the beginning of the function
             * 
             * if ( Patcher.HideGui(this) ) return false;
             * 
             * After that its the same method.
             * 
             */

            CodeInstruction jumpToLabel = new CodeInstruction(OpCodes.Nop);
            jumpToLabel.labels.Add(returnEqual);

            output.AddRange(instructions);

            output.Add(jumpToLabel);
            output.Add(new CodeInstruction(OpCodes.Ldc_I4_0));
            output.Add(new CodeInstruction(OpCodes.Ret));

            return output;
        }

        public override void patch(Harmony harmony)
        {
            if (SpyglassMod.config.hideHUDWhileSpying)
            {
                // patch to face interfaces.
                var ShouldReceiveRenderEvents = typeof(Vintagestory.API.Client.GuiDialog).GetMethod("ShouldReceiveRenderEvents", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
                var adjustGuiOpacity_Transpiler = typeof(GuiDialogHudPatch).GetMethod("AdjustGuiOpacity_Transpiler", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);
                harmony.Patch(ShouldReceiveRenderEvents, transpiler: new HarmonyMethod(adjustGuiOpacity_Transpiler));
            }
        }

    }
}
