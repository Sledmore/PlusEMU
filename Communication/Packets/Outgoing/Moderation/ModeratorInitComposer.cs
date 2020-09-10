using System;
using System.Collections.Generic;

using Plus.Utilities;
using Plus.HabboHotel.Moderation;

namespace Plus.Communication.Packets.Outgoing.Moderation
{
    class ModeratorInitComposer : MessageComposer
    {
        public ICollection<string> UserPresets { get; }
        public ICollection<string> RoomPresets { get; }
        public ICollection<ModerationTicket> Tickets { get; }
        public ModeratorInitComposer(ICollection<string> UserPresets, ICollection<string> RoomPresets, ICollection<ModerationTicket> Tickets)
            : base(ServerPacketHeader.ModeratorInitMessageComposer)
        {
            this.UserPresets = UserPresets;
            this.RoomPresets = RoomPresets;
            this.Tickets = Tickets;
        }

        public override void Compose(ServerPacket packet)
        {
            packet.WriteInteger(Tickets.Count);
            foreach (ModerationTicket Ticket in Tickets)
            {
                packet.WriteInteger(Ticket.Id); // Id
                packet.WriteInteger(Ticket.GetStatus(Id)); // Tab ID
                packet.WriteInteger(Ticket.Type); // Type
                packet.WriteInteger(Ticket.Category); // Category
                packet.WriteInteger(Convert.ToInt32((DateTime.Now - UnixTimestamp.FromUnixTimestamp(Ticket.Timestamp)).TotalMilliseconds)); // This should fix the overflow?
                packet.WriteInteger(Ticket.Priority); // Priority
                packet.WriteInteger(Ticket.Sender == null ? 0 : Ticket.Sender.Id); // Sender ID
                packet.WriteInteger(1);
                packet.WriteString(Ticket.Sender == null ? string.Empty : Ticket.Sender.Username); // Sender Name
                packet.WriteInteger(Ticket.Reported == null ? 0 : Ticket.Reported.Id); // Reported ID
                packet.WriteString(Ticket.Reported == null ? string.Empty : Ticket.Reported.Username); // Reported Name
                packet.WriteInteger(Ticket.Moderator == null ? 0 : Ticket.Moderator.Id); // Moderator ID
                packet.WriteString(Ticket.Moderator == null ? string.Empty : Ticket.Moderator.Username); // Mod Name
                packet.WriteString(Ticket.Issue); // Issue
                packet.WriteInteger(Ticket.Room == null ? 0 : Ticket.Room.Id); // Room Id
                packet.WriteInteger(0);//LOOP
            }

            packet.WriteInteger(UserPresets.Count);
            foreach (string pre in UserPresets)
            {
                packet.WriteString(pre);
            }

            /*base.WriteInteger(UserActionPresets.Count);
            foreach (KeyValuePair<string, List<ModerationPresetActionMessages>> Cat in UserActionPresets.ToList())
            {
                base.WriteString(Cat.Key);
                base.WriteBoolean(true);
                base.WriteInteger(Cat.Value.Count);
                foreach (ModerationPresetActionMessages Preset in Cat.Value.ToList())
                {
                    base.WriteString(Preset.Caption);
                    base.WriteString(Preset.MessageText);
                    base.WriteInteger(Preset.BanTime); // Account Ban Hours
                    base.WriteInteger(Preset.IPBanTime); // IP Ban Hours
                    base.WriteInteger(Preset.MuteTime); // Mute in Hours
                    base.WriteInteger(0);//Trading lock duration
                    base.WriteString(Preset.Notice + "\n\nPlease Note: Avatar ban is an IP ban!");
                    base.WriteBoolean(false);//Show HabboWay
                }
            }*/

            // TODO: Figure out
            packet.WriteInteger(0);
            {
                //Loop a string.
            }

            packet.WriteBoolean(true); // Ticket right
            packet.WriteBoolean(true); // Chatlogs
            packet.WriteBoolean(true); // User actions alert etc
            packet.WriteBoolean(true); // Kick users
            packet.WriteBoolean(true); // Ban users
            packet.WriteBoolean(true); // Caution etc
            packet.WriteBoolean(true); // Love you, Tom

            packet.WriteInteger(RoomPresets.Count);
            foreach (string pre in RoomPresets)
            {
                packet.WriteString(pre);
            }
        }
    }
}
