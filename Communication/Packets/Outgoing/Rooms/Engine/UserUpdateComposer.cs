using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Plus.HabboHotel.Rooms;

namespace Plus.Communication.Packets.Outgoing.Rooms.Engine
{
    class UserUpdateComposer : MessageComposer
    {
        public ICollection<RoomUser> Users { get; }

        public UserUpdateComposer(ICollection<RoomUser> users)
            : base(ServerPacketHeader.UserUpdateMessageComposer)
        {
            this.Users = users;
        }

        public override void Compose(ServerPacket packet)
        {
            packet.WriteInteger(Users.Count);
            foreach (RoomUser user in Users.ToList())
            {
                packet.WriteInteger(user.VirtualId);
                packet.WriteInteger(user.X);
                packet.WriteInteger(user.Y);
                packet.WriteString(user.Z.ToString("0.00"));
                packet.WriteInteger(user.RotHead);
                packet.WriteInteger(user.RotBody);

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
                packet.WriteString(StatusComposer.ToString());
            }
        }
    }
}