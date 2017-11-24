using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Plus.HabboHotel.Items;

namespace Plus.Communication.Packets.Outgoing.Rooms.Engine
{
    class ItemUpdateComposer : ServerPacket
    {
        public ItemUpdateComposer(Item Item, int UserId)
            : base(ServerPacketHeader.ItemUpdateMessageComposer)
        {
            WriteWallItem(Item, UserId);
        }

        private void WriteWallItem(Item Item, int UserId)
        {
           base.WriteString(Item.Id.ToString());
            base.WriteInteger(Item.GetBaseItem().SpriteId);
           base.WriteString(Item.wallCoord);
            switch (Item.GetBaseItem().InteractionType)
            {
                case InteractionType.POSTIT:
                   base.WriteString(Item.ExtraData.Split(' ')[0]);
                    break;

                default:
                   base.WriteString(Item.ExtraData);
                    break;
            }
            base.WriteInteger(-1);
            base.WriteInteger((Item.GetBaseItem().Modes > 1) ? 1 : 0);
            base.WriteInteger(UserId);
        }
    }
}