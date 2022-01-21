using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.Client.NoObf;

namespace spyglass.src.Client.Patches
{
    class ImmersiveFpSystemRenderEntities : BasePatch
    {

        public static bool LoadIsRender(Entity ent)
        {
            if ( ClientSettings.ImmersiveFpMode && ent is EntityPlayer && ClientManipulation.IsLocalPlayer((EntityPlayer)ent))
            {
                if (ClientManipulation.getPercentZoomed() > 0.01)
                {
                    return false;
                }
            }

            return ent.IsRendered;
        }
        
        public static IEnumerable<CodeInstruction> ImmersiveFpMode_Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator g)
        {
            List<CodeInstruction> output = new List<CodeInstruction>();
            FieldInfo IsRendered = typeof(Entity).GetField("IsRendered", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
            
            foreach (CodeInstruction ci in instructions)
            {
                if ( ci.opcode == OpCodes.Ldfld && (FieldInfo)ci.operand == IsRendered)
                {
                    output.Add(new CodeInstruction(OpCodes.Call, typeof(ImmersiveFpSystemRenderEntities).GetMethod("LoadIsRender", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public)));
                }
                else
                {
                    output.Add(ci);
                }
            }

            return output;
        }

        public override void patch(Harmony harmony)
        {
            // patch to face interfaces.
            var OnRenderOpaque3D_SystemRenderEntities = typeof(SystemRenderEntities).GetMethod("OnRenderOpaque3D", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
            var ImmersiveFpMode_Transpiler = typeof(ImmersiveFpSystemRenderEntities).GetMethod("ImmersiveFpMode_Transpiler", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);
            harmony.Patch(OnRenderOpaque3D_SystemRenderEntities, transpiler: new HarmonyMethod(ImmersiveFpMode_Transpiler));
        }

    }
}
