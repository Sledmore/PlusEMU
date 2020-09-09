using System;
using System.Collections.Generic;

using Plus.HabboHotel.Rooms;

namespace Plus.Communication.Packets.Outgoing.Groups
{
    class GroupCreationWindowComposer : MessageComposer
    {
        public ICollection<RoomData> Rooms { get; }

        public GroupCreationWindowComposer(ICollection<RoomData> rooms)
            : base(ServerPacketHeader.GroupCreationWindowMessageComposer)
        {
            this.Rooms = rooms;
        }

        public override void Compose(ServerPacket packet)
        {
            packet.WriteInteger(Convert.ToInt32(PlusEnvironment.GetSettingsManager().TryGetValue("catalog.group.purchase.cost")));//Price

            packet.WriteInteger(Rooms.Count);//Room count that the user has.
            foreach (RoomData room in Rooms)
            {
                packet.WriteInteger(room.Id);//Room Id
                packet.WriteString(room.Name);//Room Name
                packet.WriteBoolean(false);//What?
            }

            packet.WriteInteger(5);
            packet.WriteInteger(5);
            packet.WriteInteger(11);
            packet.WriteInteger(4);

            packet.WriteInteger(6);
            packet.WriteInteger(11);
            packet.WriteInteger(4);

            packet.WriteInteger(0);
            packet.WriteInteger(0);
            packet.WriteInteger(0);

            packet.WriteInteger(0);
            packet.WriteInteger(0);
            packet.WriteInteger(0);

            packet.WriteInteger(0);
            packet.WriteInteger(0);
            packet.WriteInteger(0);
        }
    }
}