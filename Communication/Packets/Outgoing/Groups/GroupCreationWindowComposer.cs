using System;
using System.Collections.Generic;

using Plus.HabboHotel.Rooms;

namespace Plus.Communication.Packets.Outgoing.Groups
{
    class GroupCreationWindowComposer : ServerPacket
    {
        public GroupCreationWindowComposer(ICollection<RoomData> rooms)
            : base(ServerPacketHeader.GroupCreationWindowMessageComposer)
        {
            base.WriteInteger(Convert.ToInt32(PlusEnvironment.GetSettingsManager().TryGetValue("catalog.group.purchase.cost")));//Price

            base.WriteInteger(rooms.Count);//Room count that the user has.
            foreach (RoomData room in rooms)
            {
                base.WriteInteger(room.Id);//Room Id
                base.WriteString(room.Name);//Room Name
                base.WriteBoolean(false);//What?
            }

            base.WriteInteger(5);
            base.WriteInteger(5);
            base.WriteInteger(11);
            base.WriteInteger(4);

            base.WriteInteger(6);
            base.WriteInteger(11);
            base.WriteInteger(4);

            base.WriteInteger(0);
            base.WriteInteger(0);
            base.WriteInteger(0);

            base.WriteInteger(0);
            base.WriteInteger(0);
            base.WriteInteger(0);

            base.WriteInteger(0);
            base.WriteInteger(0);
            base.WriteInteger(0);
        }
    }
}