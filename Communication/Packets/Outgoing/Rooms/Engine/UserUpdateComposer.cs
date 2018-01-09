using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Plus.HabboHotel.Rooms;

namespace Plus.Communication.Packets.Outgoing.Rooms.Engine
{
    class UserUpdateComposer : ServerPacket
    {
        public UserUpdateComposer(ICollection<RoomUser> users)
            : base(ServerPacketHeader.UserUpdateMessageComposer)
        {
            WriteInteger(users.Count);
            foreach (RoomUser user in users.ToList())
            {
                WriteInteger(user.VirtualId);
                WriteInteger(user.X);
                WriteInteger(user.Y);
                WriteString(user.Z.ToString("0.00"));
                WriteInteger(user.RotHead);
                WriteInteger(user.RotBody);

                StringBuilder StatusComposer = new StringBuilder();
                StatusComposer.Append("/");

                foreach (KeyValuePair<string, string> Status in user.Statusses.ToList())
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
                WriteString(StatusComposer.ToString());
            }
        }
    }
}