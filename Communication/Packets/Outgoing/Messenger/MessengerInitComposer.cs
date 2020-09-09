using System;

namespace Plus.Communication.Packets.Outgoing.Messenger
{
    class MessengerInitComposer : MessageComposer
    {
        public MessengerInitComposer()
            : base(ServerPacketHeader.MessengerInitMessageComposer)
        {
            
        }

        public override void Compose(ServerPacket packet)
        {
            packet.WriteInteger(Convert.ToInt32(PlusEnvironment.GetSettingsManager().TryGetValue("messenger.buddy_limit")));//Friends max.
            packet.WriteInteger(300);
            packet.WriteInteger(800);
            packet.WriteInteger(0); // category count
        }
    }
}
