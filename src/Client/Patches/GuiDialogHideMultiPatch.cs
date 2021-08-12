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
    abstract class GuiDialogHideMultiPatch : BasePatch
    {
        public static bool HideGui(GuiDialog target)
        {
            return ClientManipulation.hideGuis();
        }

        public static IEnumerable<CodeInstruction> AdjustGuiOpacity_Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator g)
        {
            List<CodeInstruction> output = new List<CodeInstruction>();

            Label returnEqual = g.DefineLabel();
            output.Add(new CodeInstruction(OpCodes.Ldarg_0));
            output.Add(new CodeInstruction(OpCodes.Call, typeof(GuiDialogHideMultiPatch).GetMethod("HideGui", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public)));
            output.Add(new CodeInstruction(OpCodes.Brtrue, returnEqual));

            /* we are just adding this code to the beginning of the function
             * 
             * if ( Patcher.HideGui(this) ) return;
             * 
             * After that its the same method.
             * 
             */

            CodeInstruction jumpToLabel = new CodeInstruction(OpCodes.Nop);
            jumpToLabel.labels.Add(returnEqual);

            output.AddRange(instructions);

            output.Add(jumpToLabel);
            output.Add(new CodeInstruction(OpCodes.Ret));

            return output;
        }

        public override void patch(Harmony harmony)
        {
            if (SpyglassMod.config.hideHUDWhileSpying)
            {
                // patch to face interfaces.
                var OnRenderGUI = GetTargetType().GetMethod("OnRenderGUI", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
                var adjustGuiOpacity_Transpiler = typeof(GuiDialogHideMultiPatch).GetMethod("AdjustGuiOpacity_Transpiler", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);
                harmony.Patch(OnRenderGUI, transpiler: new HarmonyMethod(adjustGuiOpacity_Transpiler));
            }
        }

        public abstract Type GetTargetType();
    }
}
