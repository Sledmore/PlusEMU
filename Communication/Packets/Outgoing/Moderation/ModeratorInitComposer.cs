using System;
using System.Collections.Generic;

using Plus.Utilities;
using Plus.HabboHotel.Moderation;

namespace Plus.Communication.Packets.Outgoing.Moderation
{
    class ModeratorInitComposer : ServerPacket
    {
        public ModeratorInitComposer(ICollection<string> UserPresets, ICollection<string> RoomPresets, ICollection<ModerationTicket> Tickets)
            : base(ServerPacketHeader.ModeratorInitMessageComposer)
        {
            base.WriteInteger(Tickets.Count);
            foreach (ModerationTicket Ticket in Tickets)
            {
                base.WriteInteger(Ticket.Id); // Id
                base.WriteInteger(Ticket.GetStatus(Id)); // Tab ID
                base.WriteInteger(Ticket.Type); // Type
                base.WriteInteger(Ticket.Category); // Category
                base.WriteInteger(Convert.ToInt32((DateTime.Now - UnixTimestamp.FromUnixTimestamp(Ticket.Timestamp)).TotalMilliseconds)); // This should fix the overflow?
                base.WriteInteger(Ticket.Priority); // Priority
                base.WriteInteger(Ticket.Sender == null ? 0 : Ticket.Sender.Id); // Sender ID
                base.WriteInteger(1);
                base.WriteString(Ticket.Sender == null ? string.Empty : Ticket.Sender.Username); // Sender Name
                base.WriteInteger(Ticket.Reported == null ? 0 : Ticket.Reported.Id); // Reported ID
                base.WriteString(Ticket.Reported == null ? string.Empty : Ticket.Reported.Username); // Reported Name
                base.WriteInteger(Ticket.Moderator == null ? 0 : Ticket.Moderator.Id); // Moderator ID
                base.WriteString(Ticket.Moderator == null ? string.Empty : Ticket.Moderator.Username); // Mod Name
                base.WriteString(Ticket.Issue); // Issue
                base.WriteInteger(Ticket.Room == null ? 0 : Ticket.Room.Id); // Room Id
                base.WriteInteger(0);//LOOP
            }

            base.WriteInteger(UserPresets.Count);
            foreach (string pre in UserPresets)
            {
                base.WriteString(pre);
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
            base.WriteInteger(0);
            {
                //Loop a string.
            }

            base.WriteBoolean(true); // Ticket right
            base.WriteBoolean(true); // Chatlogs
            base.WriteBoolean(true); // User actions alert etc
            base.WriteBoolean(true); // Kick users
            base.WriteBoolean(true); // Ban users
            base.WriteBoolean(true); // Caution etc
            base.WriteBoolean(true); // Love you, Tom

            base.WriteInteger(RoomPresets.Count);
            foreach (string pre in RoomPresets)
            {
                base.WriteString(pre);
            }
        }
    }
}
