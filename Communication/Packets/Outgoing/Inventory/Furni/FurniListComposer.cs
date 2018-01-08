using System.Collections.Generic;

using Plus.HabboHotel.Items;
using Plus.HabboHotel.Catalog.Utilities;

namespace Plus.Communication.Packets.Outgoing.Inventory.Furni
{
    class FurniListComposer : ServerPacket
    {
        public FurniListComposer(ICollection<Item> Items, int pages, int page)
            : base(ServerPacketHeader.FurniListMessageComposer)
        {
            WriteInteger(pages);//Pages
            WriteInteger(page);//Page?

            WriteInteger(Items.Count);
            foreach (Item Item in Items)
            {
                WriteItem(Item);
            }
        }

        private void WriteItem(Item Item)
        {
            WriteInteger(Item.Id);
            WriteString(Item.GetBaseItem().Type.ToString().ToUpper());
            WriteInteger(Item.Id);
            WriteInteger(Item.GetBaseItem().SpriteId);

            if (Item.LimitedNo > 0)
            {
                WriteInteger(1);
                WriteInteger(256);
                WriteString(Item.ExtraData);
                WriteInteger(Item.LimitedNo);
                WriteInteger(Item.LimitedTot);
            }
            else
                ItemBehaviourUtility.GenerateExtradata(Item, this);

            WriteBoolean(Item.GetBaseItem().AllowEcotronRecycle);
            WriteBoolean(Item.GetBaseItem().AllowTrade);
            WriteBoolean(Item.LimitedNo == 0 ? Item.GetBaseItem().AllowInventoryStack : false);
            WriteBoolean(ItemUtility.IsRare(Item));
            WriteInteger(-1);//Seconds to expiration.
            WriteBoolean(true);
            WriteInteger(-1);//Item RoomId

            if (!Item.IsWallItem)
            {
                WriteString(string.Empty);
                WriteInteger(0);
            }
        }
    }
}