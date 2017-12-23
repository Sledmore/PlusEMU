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
            base.WriteInteger(users.Count);
            foreach (RoomUser user in users.ToList())
            {
                base.WriteInteger(user.VirtualId);
                base.WriteInteger(user.X);
                base.WriteInteger(user.Y);
                base.WriteString(user.Z.ToString("0.00"));
                base.WriteInteger(user.RotHead);
                base.WriteInteger(user.RotBody);

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
                base.WriteString(StatusComposer.ToString());
            }
        }
    }
}