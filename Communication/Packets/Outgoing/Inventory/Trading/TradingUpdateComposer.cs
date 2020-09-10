using System.Linq;
using Plus.HabboHotel.Rooms.Trading;
using Plus.HabboHotel.Items;

namespace Plus.Communication.Packets.Outgoing.Inventory.Trading
{
    class TradingUpdateComposer : MessageComposer
    {
        public Trade Trade { get; }

        public TradingUpdateComposer(Trade trade)
            : base(ServerPacketHeader.TradingUpdateMessageComposer)
        {
            this.Trade = trade;
        }

        public override void Compose(ServerPacket packet)
        {
            foreach (TradeUser user in Trade.Users.ToList())
            {
                packet.WriteInteger(user.RoomUser.UserId);
                packet.WriteInteger(user.OfferedItems.Count);

                foreach (Item item in user.OfferedItems.Values)
                {
                    packet.WriteInteger(item.Id);
                    packet.WriteString(item.GetBaseItem().Type.ToString().ToLower());
                    packet.WriteInteger(item.Id);
                    packet.WriteInteger(item.Data.SpriteId);
                    packet.WriteInteger(0);//Not sure.
                    if (item.LimitedNo > 0)
                    {
                        packet.WriteBoolean(false);//Stackable
                        packet.WriteInteger(256);
                        packet.WriteString("");
                        packet.WriteInteger(item.LimitedNo);
                        packet.WriteInteger(item.LimitedTot);
                    }
                    else
                    {
                        packet.WriteBoolean(true);//Stackable
                        packet.WriteInteger(0);
                        packet.WriteString("");
                    }

                    packet.WriteInteger(0);
                    packet.WriteInteger(0);
                    packet.WriteInteger(0);

                    if (item.GetBaseItem().Type == 's')
                        packet.WriteInteger(0);
                }

                packet.WriteInteger(user.OfferedItems.Count);//Item Count
                packet.WriteInteger(user.OfferedItems.Values.Where(x => x.Data.InteractionType == InteractionType.EXCHANGE).Sum(t => t.Data.BehaviourData));
            }
        }
    }
}