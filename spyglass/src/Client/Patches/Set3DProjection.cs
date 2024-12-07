using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using Vintagestory.Client.NoObf;

namespace spyglass.src.Client.Patches
{
    class Set3DProjection : BasePatch
    {
        public static float AdjustFov(float fov)
        {
            return fov * ClientManipulation.GetZoomAdjust();
        }

        public static IEnumerable<CodeInstruction> AdjustFov_Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            List<CodeInstruction> output = new List<CodeInstruction>();

            output.Add(new CodeInstruction(OpCodes.Ldarg_2));
            output.Add(new CodeInstruction(OpCodes.Call, typeof(Set3DProjection).GetMethod("AdjustFov", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public)));
            output.Add(new CodeInstruction(OpCodes.Starg_S, 2));

            /* we are just adding this code to the beginning of the function
             * 
             * fov = Patcher.adjustFov( fov );
             * 
             * After that its the same method.
             * 
             */

            output.AddRange(instructions);
            return output;
        }

        public override void patch(Harmony harmony)
        {
            // patch to adjust FOV without editing settings.
            var Set3DProjection = typeof(ClientMain).GetMethod("Set3DProjection", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance, new [] { typeof(float), typeof(float) });
            var adjustFov_Transpiler = typeof(Set3DProjection).GetMethod("AdjustFov_Transpiler", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);
            harmony.Patch(Set3DProjection, transpiler: new HarmonyMethod(adjustFov_Transpiler));
        }
    }
}
