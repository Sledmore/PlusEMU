using System.Linq;
using System.Collections.Generic;
using Plus.HabboHotel.Users.Inventory.Bots;

namespace Plus.Communication.Packets.Outgoing.Inventory.Bots
{
    class BotInventoryComposer : ServerPacket
    {
        public BotInventoryComposer(ICollection<Bot> Bots)
            : base(ServerPacketHeader.BotInventoryMessageComposer)
        {
            base.WriteInteger(Bots.Count);
            foreach (Bot Bot in Bots.ToList())
            {
                base.WriteInteger(Bot.Id);
               base.WriteString(Bot.Name);
               base.WriteString(Bot.Motto);
               base.WriteString(Bot.Gender);
               base.WriteString(Bot.Figure);
            }
        }
    }
}