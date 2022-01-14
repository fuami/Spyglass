using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Util;

namespace spyglass.src
{
    class ItemSpyglass : Item
    {
        public override string GetHeldTpUseAnimation(ItemSlot activeHotbarSlot, Entity byEntity)
        {
            return "spyglassing";
        }

        public override WorldInteraction[] GetHeldInteractionHelp( ItemSlot inSlot )
        {
            WorldInteraction worldInteraction = new WorldInteraction();
            worldInteraction.ActionLangCode = "spyglass:item-spyglass-use";
            worldInteraction.MouseButton = EnumMouseButton.Right;

            return new WorldInteraction[] { worldInteraction }.Append(base.GetHeldInteractionHelp(inSlot));
        }

        public override void OnHeldInteractStart(ItemSlot slot, EntityAgent byEntity, BlockSelection blockSel, EntitySelection entitySel, bool firstEvent, ref EnumHandHandling handling)
        {
            if (!byEntity.Controls.Sneak)
            {
                handling = EnumHandHandling.Handled;
                updateState(true, byEntity);
            }
            else
                base.OnHeldInteractStart(slot, byEntity, blockSel, entitySel, firstEvent,ref handling);
        }

        public override bool OnHeldInteractStep(float secondsUsed, ItemSlot slot, EntityAgent byEntity, BlockSelection blockSel, EntitySelection entitySel)
        {
            if (!byEntity.Controls.Sneak)
            {
                updateState(true, byEntity);
                return true;
            }
            else
               return base.OnHeldInteractStep(secondsUsed, slot, byEntity, blockSel, entitySel);
        }

        public override void OnHeldInteractStop(float secondsUsed, ItemSlot slot, EntityAgent byEntity, BlockSelection blockSel, EntitySelection entitySel)
        {
            updateState(false, byEntity);
        }

        public override bool OnHeldInteractCancel(float secondsUsed, ItemSlot slot, EntityAgent byEntity, BlockSelection blockSel, EntitySelection entitySel, EnumItemUseCancelReason cancelReason)
        {
            updateState(false, byEntity);
            return true;
        }

        private void updateState(bool newState, EntityAgent byEntity)
        {
            if (byEntity.World.Side == EnumAppSide.Client)
            {
                if (ClientManipulation.IsLocalPlayer(byEntity))
                {
                    SpyglassMod.zoomed = newState;
                }
            }
        }
    }

}