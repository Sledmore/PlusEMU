using Plus.HabboHotel.Rooms;

namespace Plus.Communication.Packets.Outgoing.Rooms.Settings
{
    class RoomSettingsDataComposer : ServerPacket
    {
        public RoomSettingsDataComposer(Room room)
            : base(ServerPacketHeader.RoomSettingsDataMessageComposer)
        {
            base.WriteInteger(room.RoomId);
            base.WriteString(room.Name);
            base.WriteString(room.Description);
            base.WriteInteger(RoomAccessUtility.GetRoomAccessPacketNum(room.Access));
            base.WriteInteger(room.Category);
            base.WriteInteger(room.UsersMax);
            base.WriteInteger(((room.Model.MapSizeX * room.Model.MapSizeY) > 100) ? 50 : 25);

            base.WriteInteger(room.Tags.Count);
            foreach (string Tag in room.Tags.ToArray())
            {
                base.WriteString(Tag);
            }

            base.WriteInteger(room.TradeSettings); //Trade
            base.WriteInteger(room.AllowPets); // allows pets in room - pet system lacking, so always off
            base.WriteInteger(room.AllowPetsEating);// allows pets to eat your food - pet system lacking, so always off
            base.WriteInteger(room.RoomBlockingEnabled);
            base.WriteInteger(room.Hidewall);
            base.WriteInteger(room.WallThickness);
            base.WriteInteger(room.FloorThickness);

            base.WriteInteger(room.ChatMode);//Chat mode
            base.WriteInteger(room.ChatSize);//Chat size
            base.WriteInteger(room.ChatSpeed);//Chat speed
            base.WriteInteger(room.ChatDistance);//Hearing Distance
            base.WriteInteger(room.ExtraFlood);//Additional Flood

            base.WriteBoolean(true);

            base.WriteInteger(room.WhoCanMute); // who can mute
            base.WriteInteger(room.WhoCanKick); // who can kick
            base.WriteInteger(room.WhoCanBan); // who can ban
        }
    }
}