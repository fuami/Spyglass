using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using Vintagestory.API.Client;
using Vintagestory.Client.NoObf;

namespace spyglass.src.Client.Patches
{
    class OnBeforeRenderFrame3D : BasePatch
    {
        public static EnumCameraMode AdjustCameraMode(EnumCameraMode mode)
        {
            return ClientManipulation.HandleCameraMode(mode);
        }

        public static IEnumerable<CodeInstruction> AdjustCameraMode_Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            List<CodeInstruction> output = new List<CodeInstruction>();

            var CameraMode = typeof(Camera).GetField("CameraMode", BindingFlags.Instance | BindingFlags.NonPublic);

            output.Add(new CodeInstruction(OpCodes.Ldarg_0));
            output.Add(new CodeInstruction(OpCodes.Dup));
            output.Add(new CodeInstruction(OpCodes.Ldfld, CameraMode));
            output.Add(new CodeInstruction(OpCodes.Call, typeof(OnBeforeRenderFrame3D).GetMethod("AdjustCameraMode", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public)));
            output.Add(new CodeInstruction(OpCodes.Stfld, CameraMode));

            /* we are just adding this code to the beginning of the function
             * 
             * CameraMode = Patcher.adjustCameraMode( CameraMode );
             * 
             * After that its the same method.
             * 
             */

            output.AddRange(instructions);
            return output;
        }


        public override void patch(Harmony harmony)
        {
            // patch to switch to first person mode when using spyglass.
            var OnBeforeRenderFrame3D = typeof(PlayerCamera).GetMethod("OnBeforeRenderFrame3D", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
            var adjustCameraMode_Transpiler = typeof(OnBeforeRenderFrame3D).GetMethod("AdjustCameraMode_Transpiler", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);
            harmony.Patch(OnBeforeRenderFrame3D, transpiler: new HarmonyMethod(adjustCameraMode_Transpiler));
        }
    }
}
