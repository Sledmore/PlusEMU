using System;

namespace Plus.Communication.Packets.Outgoing.Messenger
{
    class MessengerInitComposer : ServerPacket
    {
        public MessengerInitComposer()
            : base(ServerPacketHeader.MessengerInitMessageComposer)
        {
            base.WriteInteger(Convert.ToInt32(PlusEnvironment.GetSettingsManager().TryGetValue("messenger.buddy_limit")));//Friends max.
            base.WriteInteger(300);
            base.WriteInteger(800);
            base.WriteInteger(0); // category count
        }
    }
}
