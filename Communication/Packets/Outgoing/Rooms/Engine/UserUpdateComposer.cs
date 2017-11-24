using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Plus.HabboHotel.Rooms;

namespace Plus.Communication.Packets.Outgoing.Rooms.Engine
{
    class UserUpdateComposer : ServerPacket
    {
        public UserUpdateComposer(ICollection<RoomUser> RoomUsers)
            : base(ServerPacketHeader.UserUpdateMessageComposer)
        {
            base.WriteInteger(RoomUsers.Count);
            foreach (RoomUser User in RoomUsers.ToList())
            {
                base.WriteInteger(User.VirtualId);
                base.WriteInteger(User.X);
                base.WriteInteger(User.Y);
                base.WriteString(User.Z.ToString("0.00"));
                base.WriteInteger(User.RotHead);
                base.WriteInteger(User.RotBody);

                StringBuilder StatusComposer = new StringBuilder();
                StatusComposer.Append("/");

                foreach (KeyValuePair<string, string> Status in User.Statusses.ToList())
                {
                    StatusComposer.Append(Status.Key);

                    if (!String.IsNullOrEmpty(Status.Value))
                    {
                        StatusComposer.Append(" ");
                        StatusComposer.Append(Status.Value);
                    }

                    StatusComposer.Append("/");
                }

                StatusComposer.Append("/");
                base.WriteString(StatusComposer.ToString());
            }
        }
    }
}