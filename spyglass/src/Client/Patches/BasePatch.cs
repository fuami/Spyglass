using HarmonyLib;

namespace spyglass.src.Client.Patches
{
    internal abstract class BasePatch
    {
        public abstract void patch(Harmony harmony);

    }
}