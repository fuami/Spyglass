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
using spyglass.src.Client.Patches;

namespace spyglass.src
{
    class ClientPatcher
    {
        private Harmony harmony;

        public void Start(ILogger logger)
        {
            harmony = new Harmony("spyglass");

            BasePatch[] patches = new BasePatch[] {
                new OnBeforeRenderFrame3D(),
                new Set3DProjection(),
                new OnMouseMove()
            };

            foreach ( var patch in patches )
            {
                try
                {
                    patch.patch(harmony);
                }
                catch (Exception e)
                {
                    logger.Error("While preforming {0} : " + e.ToString(),patch.GetType().FullName);
                }
            }
        }

        public void Stop()
        {
            harmony.UnpatchAll();
        }
    }
}
