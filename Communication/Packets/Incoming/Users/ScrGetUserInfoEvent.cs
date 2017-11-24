using System;
using Plus.Communication.Packets.Incoming;
using Plus.HabboHotel.GameClients;
using Plus.Communication.Packets.Outgoing.Users;

namespace Plus.Communication.Packets.Incoming.Users
{
    class ScrGetUserInfoEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            Session.SendPacket(new ScrSendUserInfoComposer());
        }
    }
}
