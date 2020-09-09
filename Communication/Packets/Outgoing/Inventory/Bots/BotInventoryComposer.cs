using System.Linq;
using System.Collections.Generic;
using Plus.HabboHotel.Users.Inventory.Bots;

namespace Plus.Communication.Packets.Outgoing.Inventory.Bots
{
    class BotInventoryComposer : MessageComposer
    {
        public ICollection<Bot> Bots { get; }

        public BotInventoryComposer(ICollection<Bot> Bots)
            : base(ServerPacketHeader.BotInventoryMessageComposer)
        {
            this.Bots = Bots;
        }

        public override void Compose(ServerPacket packet)
        {
            packet.WriteInteger(Bots.Count);
            foreach (Bot Bot in Bots.ToList())
            {
                packet.WriteInteger(Bot.Id);
                packet.WriteString(Bot.Name);
                packet.WriteString(Bot.Motto);
                packet.WriteString(Bot.Gender);
                packet.WriteString(Bot.Figure);
            }
        }
    }
}