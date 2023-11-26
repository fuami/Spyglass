using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using Vintagestory.Client.NoObf;

namespace spyglass.src.Client.Patches
{
    class OnMouseMove : BasePatch
    {
        public static double AdjustSensitivity(double defaultDivisor)
        {
            return ClientManipulation.GetSensitivityMultiplier() * defaultDivisor;
        }

        public static IEnumerable<CodeInstruction> AdjustSensitivity_Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            List<CodeInstruction> output = new List<CodeInstruction>();

            foreach (var i in instructions)
            {
                if (i.opcode == OpCodes.Ldc_R8 && (double)i.operand == 100.0 && output.Count > 4)
                {
                    CodeInstruction get_MouseSensivity = output[output.Count - 3];
                    if (get_MouseSensivity.opcode == OpCodes.Call && (MethodInfo)get_MouseSensivity.operand == typeof(ClientSettings).GetMethod("get_MouseSensivity", BindingFlags.Public | BindingFlags.Static))
                    {
                        /*
                         * Target these two lines:
                         * this.MouseDeltaX += (double)(args.DeltaX * ClientSettings.MouseSensivity) / 100.0;
                         * this.MouseDeltaY += (double)(args.DeltaY * ClientSettings.MouseSensivity) / 100.0;
                         * 
                         * and edits them to be...
                         * 
                         * this.MouseDeltaX += (double)(args.DeltaX * ClientSettings.MouseSensivity) / Patcher.adjustSensitivity(100.0);
                         * this.MouseDeltaY += (double)(args.DeltaY * ClientSettings.MouseSensivity) / Patcher.adjustSensitivity(100.0);
                         * 
                         */
                        output.Add(i);
                        output.Add(new CodeInstruction(OpCodes.Call, typeof(OnMouseMove).GetMethod("AdjustSensitivity", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public)));
                    }
                    else
                        output.Add(i);
                }
                else
                    output.Add(i);
            }

            return output;
        }

        public override void patch(Harmony harmony)
        {
            // patch to adjust mouse sensativity without editing settings.
            var OnMouseMove = typeof(ClientMain).GetMethod("OnMouseMove", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
            var adjustSensitivity_Transpiler = typeof(OnMouseMove).GetMethod("AdjustSensitivity_Transpiler", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);
            harmony.Patch(OnMouseMove, transpiler: new HarmonyMethod(adjustSensitivity_Transpiler));
        }
    }
}
