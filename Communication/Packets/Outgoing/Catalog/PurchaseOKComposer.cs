using Plus.HabboHotel.Catalog;
using Plus.HabboHotel.Items;

namespace Plus.Communication.Packets.Outgoing.Catalog
{
    public class PurchaseOKComposer : MessageComposer
    {
        public CatalogItem Item { get; }
        public ItemData BaseItem { get; }

        public PurchaseOKComposer(CatalogItem Item, ItemData BaseItem)
            : base(ServerPacketHeader.PurchaseOKMessageComposer)
        {
            this.Item = Item;
            this.BaseItem = BaseItem;
        }

        public PurchaseOKComposer()
            : base(ServerPacketHeader.PurchaseOKMessageComposer)
        {
            
        }

        public override void Compose(ServerPacket packet)
        {
            if(Item != null)
            {
                packet.WriteInteger(BaseItem.Id);
                packet.WriteString(BaseItem.ItemName);
                packet.WriteBoolean(false);
                packet.WriteInteger(Item.CostCredits);
                packet.WriteInteger(Item.CostPixels);
                packet.WriteInteger(0);
                packet.WriteBoolean(true);
                packet.WriteInteger(1);
                packet.WriteString(BaseItem.Type.ToString().ToLower());
                packet.WriteInteger(BaseItem.SpriteId);
                packet.WriteString("");
                packet.WriteInteger(1);
                packet.WriteInteger(0);
                packet.WriteString("");
                packet.WriteInteger(1);
            } else
            {
                packet.WriteInteger(0);
                packet.WriteString("");
                packet.WriteBoolean(false);
                packet.WriteInteger(0);
                packet.WriteInteger(0);
                packet.WriteInteger(0);
                packet.WriteBoolean(true);
                packet.WriteInteger(1);
                packet.WriteString("s");
                packet.WriteInteger(0);
                packet.WriteString("");
                packet.WriteInteger(1);
                packet.WriteInteger(0);
                packet.WriteString("");
                packet.WriteInteger(1);
            }
        }
    }
}