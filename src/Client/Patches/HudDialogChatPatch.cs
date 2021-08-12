using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace spyglass.src.Client.Patches
{
    class HudDialogChatPatch : GuiDialogHideMultiPatch
    {
        public override Type GetTargetType()
        {
            return typeof(Vintagestory.Client.NoObf.HudDialogChat);
        }
    }
}
