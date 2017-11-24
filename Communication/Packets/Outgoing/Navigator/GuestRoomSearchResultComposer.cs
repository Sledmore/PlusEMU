using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using Plus.HabboHotel.Rooms;

namespace Plus.Communication.Packets.Outgoing.Navigator
{
   class GuestRoomSearchResultComposer : ServerPacket
    {
       public GuestRoomSearchResultComposer(int Mode, string UserQuery, ICollection<RoomData> Rooms)
           : base(ServerPacketHeader.GuestRoomSearchResultMessageComposer)
       {
           base.WriteInteger(Mode);
          base.WriteString(UserQuery);
         
           base.WriteInteger(Rooms.Count);
           foreach (RoomData data in Rooms)
           {
               RoomAppender.WriteRoom(this, data, data.Promotion);
           }

           base.WriteBoolean(false);
       }
    }
}
