using System.Linq;
using Plus.HabboHotel.Rooms.Trading;
using Plus.HabboHotel.Items;

namespace Plus.Communication.Packets.Outgoing.Inventory.Trading
{
    class TradingUpdateComposer : ServerPacket
    {
        public TradingUpdateComposer(Trade trade)
            : base(ServerPacketHeader.TradingUpdateMessageComposer)
        {
            foreach (TradeUser user in trade.Users.ToList())
            {
                base.WriteInteger(user.RoomUser.UserId);
                base.WriteInteger(user.OfferedItems.Count);

                foreach (Item item in user.OfferedItems.Values)
                {
                    base.WriteInteger(item.Id);
                    base.WriteString(item.GetBaseItem().Type.ToString().ToLower());
                    base.WriteInteger(item.Id);
                    base.WriteInteger(item.Data.SpriteId);
                    base.WriteInteger(0);//Not sure.
                    if (item.LimitedNo > 0)
                    {
                        base.WriteBoolean(false);//Stackable
                        base.WriteInteger(256);
                        base.WriteString("");
                        base.WriteInteger(item.LimitedNo);
                        base.WriteInteger(item.LimitedTot);
                    }
                    else
                    {
                        base.WriteBoolean(true);//Stackable
                        base.WriteInteger(0);
                        base.WriteString("");
                    }

                    base.WriteInteger(0);
                    base.WriteInteger(0);
                    base.WriteInteger(0);

                    if (item.GetBaseItem().Type == 's')
                        base.WriteInteger(0);
                }

                base.WriteInteger(user.OfferedItems.Count);//Item Count
                base.WriteInteger(user.OfferedItems.Values.Where(x => x.Data.InteractionType == InteractionType.EXCHANGE).Sum(t => t.Data.BehaviourData));
            }
        }
    }
}