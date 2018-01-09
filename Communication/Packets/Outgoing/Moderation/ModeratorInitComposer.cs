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
            WriteInteger(Tickets.Count);
            foreach (ModerationTicket Ticket in Tickets)
            {
                WriteInteger(Ticket.Id); // Id
                WriteInteger(Ticket.GetStatus(Id)); // Tab ID
                WriteInteger(Ticket.Type); // Type
                WriteInteger(Ticket.Category); // Category
                WriteInteger(Convert.ToInt32((DateTime.Now - UnixTimestamp.FromUnixTimestamp(Ticket.Timestamp)).TotalMilliseconds)); // This should fix the overflow?
                WriteInteger(Ticket.Priority); // Priority
                WriteInteger(Ticket.Sender == null ? 0 : Ticket.Sender.Id); // Sender ID
                WriteInteger(1);
                WriteString(Ticket.Sender == null ? string.Empty : Ticket.Sender.Username); // Sender Name
                WriteInteger(Ticket.Reported == null ? 0 : Ticket.Reported.Id); // Reported ID
                WriteString(Ticket.Reported == null ? string.Empty : Ticket.Reported.Username); // Reported Name
                WriteInteger(Ticket.Moderator == null ? 0 : Ticket.Moderator.Id); // Moderator ID
                WriteString(Ticket.Moderator == null ? string.Empty : Ticket.Moderator.Username); // Mod Name
                WriteString(Ticket.Issue); // Issue
                WriteInteger(Ticket.Room == null ? 0 : Ticket.Room.Id); // Room Id
                WriteInteger(0);//LOOP
            }

            WriteInteger(UserPresets.Count);
            foreach (string pre in UserPresets)
            {
                WriteString(pre);
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
            WriteInteger(0);
            {
                //Loop a string.
            }

            WriteBoolean(true); // Ticket right
            WriteBoolean(true); // Chatlogs
            WriteBoolean(true); // User actions alert etc
            WriteBoolean(true); // Kick users
            WriteBoolean(true); // Ban users
            WriteBoolean(true); // Caution etc
            WriteBoolean(true); // Love you, Tom

            WriteInteger(RoomPresets.Count);
            foreach (string pre in RoomPresets)
            {
                WriteString(pre);
            }
        }
    }
}
