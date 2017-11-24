using System;
using System.Linq;
using System.Text;


using Plus.HabboHotel.Rooms;
using Plus.HabboHotel.GameClients;


namespace Plus.Communication.Packets.Outgoing.Rooms.Engine
{
    class AvatarAspectUpdateMessageComposer : ServerPacket
    {
        public AvatarAspectUpdateMessageComposer(string Figure, string Gender)
            : base(ServerPacketHeader.AvatarAspectUpdateMessageComposer)
        {
            base.WriteString(Figure);
            base.WriteString(Gender);


        }
    }
}