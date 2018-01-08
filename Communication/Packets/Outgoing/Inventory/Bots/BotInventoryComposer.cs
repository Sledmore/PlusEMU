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
            WriteInteger(Bots.Count);
            foreach (Bot Bot in Bots.ToList())
            {
                WriteInteger(Bot.Id);
               WriteString(Bot.Name);
               WriteString(Bot.Motto);
               WriteString(Bot.Gender);
               WriteString(Bot.Figure);
            }
        }
    }
}