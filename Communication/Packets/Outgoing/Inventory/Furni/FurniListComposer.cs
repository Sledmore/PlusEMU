using System.Collections.Generic;

using Plus.HabboHotel.Items;
using Plus.HabboHotel.Catalog.Utilities;

namespace Plus.Communication.Packets.Outgoing.Inventory.Furni
{
    class FurniListComposer : MessageComposer
    {
        public ICollection<Item> Items { get; }
        public int Pages { get; }
        public int Page { get; }

        public FurniListComposer(ICollection<Item> Items, int pages, int page)
            : base(ServerPacketHeader.FurniListMessageComposer)
        {
            this.Items = Items;
            this.Pages = pages;
            this.Page = page;
        }

        public override void Compose(ServerPacket packet)
        {
            packet.WriteInteger(Pages);//Pages
            packet.WriteInteger(Page);//Page?

            packet.WriteInteger(Items.Count);
            foreach (Item Item in Items)
            {
                WriteItem(Item, packet);
            }
        }

        private void WriteItem(Item Item, ServerPacket packet)
        {
            packet.WriteInteger(Item.Id);
            packet.WriteString(Item.GetBaseItem().Type.ToString().ToUpper());
            packet.WriteInteger(Item.Id);
            packet.WriteInteger(Item.GetBaseItem().SpriteId);

            if (Item.LimitedNo > 0)
            {
                packet.WriteInteger(1);
                packet.WriteInteger(256);
                packet.WriteString(Item.ExtraData);
                packet.WriteInteger(Item.LimitedNo);
                packet.WriteInteger(Item.LimitedTot);
            }
            else
                ItemBehaviourUtility.GenerateExtradata(Item, packet);

            packet.WriteBoolean(Item.GetBaseItem().AllowEcotronRecycle);
            packet.WriteBoolean(Item.GetBaseItem().AllowTrade);
            packet.WriteBoolean(Item.LimitedNo == 0 ? Item.GetBaseItem().AllowInventoryStack : false);
            packet.WriteBoolean(ItemUtility.IsRare(Item));
            packet.WriteInteger(-1);//Seconds to expiration.
            packet.WriteBoolean(true);
            packet.WriteInteger(-1);//Item RoomId

            if (!Item.IsWallItem)
            {
                packet.WriteString(string.Empty);
                packet.WriteInteger(0);
            }
        }
    }
}