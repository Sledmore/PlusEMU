using System;

using Plus.HabboHotel.Rooms;
using Plus.HabboHotel.GameClients;

namespace Plus.Communication.Packets.Outgoing.Navigator
{
    class GetGuestRoomResultComposer : ServerPacket
    {
        public GetGuestRoomResultComposer(GameClient session, RoomData data, bool isLoading, bool checkEntry)
            : base(ServerPacketHeader.GetGuestRoomResultMessageComposer)
        {
            base.WriteBoolean(isLoading);
            base.WriteInteger(data.Id);
            base.WriteString(data.Name);
            base.WriteInteger(data.OwnerId);
            base.WriteString(data.OwnerName);
            base.WriteInteger(RoomAccessUtility.GetRoomAccessPacketNum(data.Access));
            base.WriteInteger(data.UsersNow);
            base.WriteInteger(data.UsersMax);
            base.WriteString(data.Description);
            base.WriteInteger(data.TradeSettings);
            base.WriteInteger(data.Score);
            base.WriteInteger(0);//Top rated room rank.
            base.WriteInteger(data.Category);

            base.WriteInteger(data.Tags.Count);
            foreach (string tag in data.Tags)
            {
                base.WriteString(tag);
            }

            if (data.Group != null && data.Promotion != null)
            {
                base.WriteInteger(62);

                base.WriteInteger(data.Group == null ? 0 : data.Group.Id);
                base.WriteString(data.Group == null ? "" : data.Group.Name);
                base.WriteString(data.Group == null ? "" : data.Group.Badge);

                base.WriteString(data.Promotion != null ? data.Promotion.Name : "");
                base.WriteString(data.Promotion != null ? data.Promotion.Description : "");
                base.WriteInteger(data.Promotion != null ? data.Promotion.MinutesLeft : 0);
            }
            else if (data.Group != null && data.Promotion == null)
            {
                base.WriteInteger(58);
                base.WriteInteger(data.Group == null ? 0 : data.Group.Id);
                base.WriteString(data.Group == null ? "" : data.Group.Name);
                base.WriteString(data.Group == null ? "" : data.Group.Badge);
            }
            else if (data.Group == null && data.Promotion != null)
            {
                base.WriteInteger(60);
                base.WriteString(data.Promotion != null ? data.Promotion.Name : "");
                base.WriteString(data.Promotion != null ? data.Promotion.Description : "");
                base.WriteInteger(data.Promotion != null ? data.Promotion.MinutesLeft : 0);
            }
            else
            {
                base.WriteInteger(56);
            }


            base.WriteBoolean(checkEntry);
            base.WriteBoolean(false);
            base.WriteBoolean(false);
            base.WriteBoolean(false);

            base.WriteInteger(data.WhoCanMute);
            base.WriteInteger(data.WhoCanKick);
            base.WriteInteger(data.WhoCanBan);

            base.WriteBoolean(session.GetHabbo().GetPermissions().HasRight("mod_tool") || data.OwnerName == session.GetHabbo().Username);
            base.WriteInteger(data.ChatMode);
            base.WriteInteger(data.ChatSize);
            base.WriteInteger(data.ChatSpeed);
            base.WriteInteger(data.ExtraFlood);
            base.WriteInteger(data.ChatDistance);
        }
    }
}