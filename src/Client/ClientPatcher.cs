using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.GameContent;
using Vintagestory.API.MathTools;
using Vintagestory.API.Client;
using Vintagestory.Client.NoObf;

namespace spyglass.src
{
    class ClientPatcher
    {
        private Harmony harmony;

        public static double AdjustSensitivity( double defaultDivisor )
        {
            return ClientManipulation.GetSensitivityMultiplier() * defaultDivisor;
        }

        public static float AdjustFov(float fov)
        {
            return fov * ClientManipulation.GetZoomAdjust();
        }

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
            output.Add(new CodeInstruction(OpCodes.Call, typeof(ClientPatcher).GetMethod("AdjustCameraMode", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public)));
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

        public static IEnumerable<CodeInstruction> AdjustFov_Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            List<CodeInstruction> output = new List<CodeInstruction>();

            output.Add(new CodeInstruction(OpCodes.Ldarg_2));
            output.Add(new CodeInstruction(OpCodes.Call, typeof(ClientPatcher).GetMethod("AdjustFov", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public)));
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

        public static IEnumerable<CodeInstruction> AdjustSensitivity_Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            List<CodeInstruction> output = new List<CodeInstruction>();

            foreach (var i in instructions)
            {
                if (i.opcode == OpCodes.Ldc_R8 && (double)i.operand == 100.0 && output.Count > 4 )
                {
                    CodeInstruction get_MouseSensivity = output[output.Count - 3];
                    if (get_MouseSensivity.opcode == OpCodes.Call && (MethodInfo)get_MouseSensivity.operand == typeof(ClientSettings).GetMethod("get_MouseSensivity", BindingFlags.Public|BindingFlags.Static))
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
                        output.Add(new CodeInstruction(OpCodes.Call, typeof(ClientPatcher).GetMethod("AdjustSensitivity", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public)));
                    }
                    else
                        output.Add(i);
                }
                else
                    output.Add(i);
            }

            return output;
        }

        public void Start()
        {
            harmony = new Harmony("spyglass");

            // patch to switch to first person mode when using spyglass.
            var OnBeforeRenderFrame3D = typeof(PlayerCamera).GetMethod("OnBeforeRenderFrame3D", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
            var adjustCameraMode_Transpiler = typeof(ClientPatcher).GetMethod("AdjustCameraMode_Transpiler", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);
            harmony.Patch(OnBeforeRenderFrame3D, transpiler: new HarmonyMethod(adjustCameraMode_Transpiler));

            // patch to adjust FOV without editing settings.
            var Set3DProjection = typeof(ClientMain).GetMethod("Set3DProjection", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
            var adjustFov_Transpiler = typeof(ClientPatcher).GetMethod("AdjustFov_Transpiler", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);
            harmony.Patch(Set3DProjection, transpiler: new HarmonyMethod(adjustFov_Transpiler));

            // patch to adjust mouse sensativity without editing settings.
            var OnMouseMove = typeof(ClientMain).GetMethod("OnMouseMove", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
            var adjustSensitivity_Transpiler = typeof(ClientPatcher).GetMethod("AdjustSensitivity_Transpiler", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);
            harmony.Patch(OnMouseMove, transpiler: new HarmonyMethod(adjustSensitivity_Transpiler));
        }

        public void Stop()
        {
            harmony.UnpatchAll();
        }
    }
}
