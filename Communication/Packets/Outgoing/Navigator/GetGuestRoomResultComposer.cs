using Plus.HabboHotel.Rooms;
using Plus.HabboHotel.GameClients;
using Plus.HabboHotel.Users;

namespace Plus.Communication.Packets.Outgoing.Navigator
{
    class GetGuestRoomResultComposer : MessageComposer
    {
        public Habbo Habbo { get; }
        public RoomData Data { get; }
        public bool IsLoading { get; }
        public bool CheckEntry { get; }

        public GetGuestRoomResultComposer(GameClient session, RoomData data, bool isLoading, bool checkEntry)
            : base(ServerPacketHeader.GetGuestRoomResultMessageComposer)
        {
            this.Habbo = session.GetHabbo();
            this.Data = data;
            this.IsLoading = isLoading;
            this.CheckEntry = checkEntry;
        }

        public override void Compose(ServerPacket packet)
        {
            packet.WriteBoolean(IsLoading);
            packet.WriteInteger(Data.Id);
            packet.WriteString(Data.Name);
            packet.WriteInteger(Data.OwnerId);
            packet.WriteString(Data.OwnerName);
            packet.WriteInteger(RoomAccessUtility.GetRoomAccessPacketNum(Data.Access));
            packet.WriteInteger(Data.UsersNow);
            packet.WriteInteger(Data.UsersMax);
            packet.WriteString(Data.Description);
            packet.WriteInteger(Data.TradeSettings);
            packet.WriteInteger(Data.Score);
            packet.WriteInteger(0);//Top rated room rank.
            packet.WriteInteger(Data.Category);

            packet.WriteInteger(Data.Tags.Count);
            foreach (string tag in Data.Tags)
            {
                packet.WriteString(tag);
            }

            if (Data.Group != null && Data.Promotion != null)
            {
                packet.WriteInteger(62);

                packet.WriteInteger(Data.Group == null ? 0 : Data.Group.Id);
                packet.WriteString(Data.Group == null ? "" : Data.Group.Name);
                packet.WriteString(Data.Group == null ? "" : Data.Group.Badge);

                packet.WriteString(Data.Promotion != null ? Data.Promotion.Name : "");
                packet.WriteString(Data.Promotion != null ? Data.Promotion.Description : "");
                packet.WriteInteger(Data.Promotion != null ? Data.Promotion.MinutesLeft : 0);
            }
            else if (Data.Group != null && Data.Promotion == null)
            {
                packet.WriteInteger(58);
                packet.WriteInteger(Data.Group == null ? 0 : Data.Group.Id);
                packet.WriteString(Data.Group == null ? "" : Data.Group.Name);
                packet.WriteString(Data.Group == null ? "" : Data.Group.Badge);
            }
            else if (Data.Group == null && Data.Promotion != null)
            {
                packet.WriteInteger(60);
                packet.WriteString(Data.Promotion != null ? Data.Promotion.Name : "");
                packet.WriteString(Data.Promotion != null ? Data.Promotion.Description : "");
                packet.WriteInteger(Data.Promotion != null ? Data.Promotion.MinutesLeft : 0);
            }
            else
            {
                packet.WriteInteger(56);
            }


            packet.WriteBoolean(CheckEntry);
            packet.WriteBoolean(false);
            packet.WriteBoolean(false);
            packet.WriteBoolean(false);

            packet.WriteInteger(Data.WhoCanMute);
            packet.WriteInteger(Data.WhoCanKick);
            packet.WriteInteger(Data.WhoCanBan);

            packet.WriteBoolean(Habbo.GetPermissions().HasRight("mod_tool") || Data.OwnerName == Habbo.Username);
            packet.WriteInteger(Data.ChatMode);
            packet.WriteInteger(Data.ChatSize);
            packet.WriteInteger(Data.ChatSpeed);
            packet.WriteInteger(Data.ExtraFlood);
            packet.WriteInteger(Data.ChatDistance);
        }
    }
}