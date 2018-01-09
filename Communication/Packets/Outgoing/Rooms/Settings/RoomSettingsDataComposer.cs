using Plus.HabboHotel.Rooms;

namespace Plus.Communication.Packets.Outgoing.Rooms.Settings
{
    class RoomSettingsDataComposer : ServerPacket
    {
        public RoomSettingsDataComposer(Room room)
            : base(ServerPacketHeader.RoomSettingsDataMessageComposer)
        {
            WriteInteger(room.RoomId);
            WriteString(room.Name);
            WriteString(room.Description);
            WriteInteger(RoomAccessUtility.GetRoomAccessPacketNum(room.Access));
            WriteInteger(room.Category);
            WriteInteger(room.UsersMax);
            WriteInteger(((room.Model.MapSizeX * room.Model.MapSizeY) > 100) ? 50 : 25);

            WriteInteger(room.Tags.Count);
            foreach (string Tag in room.Tags.ToArray())
            {
                WriteString(Tag);
            }

            WriteInteger(room.TradeSettings); //Trade
            WriteInteger(room.AllowPets); // allows pets in room - pet system lacking, so always off
            WriteInteger(room.AllowPetsEating);// allows pets to eat your food - pet system lacking, so always off
            WriteInteger(room.RoomBlockingEnabled);
            WriteInteger(room.Hidewall);
            WriteInteger(room.WallThickness);
            WriteInteger(room.FloorThickness);

            WriteInteger(room.ChatMode);//Chat mode
            WriteInteger(room.ChatSize);//Chat size
            WriteInteger(room.ChatSpeed);//Chat speed
            WriteInteger(room.ChatDistance);//Hearing Distance
            WriteInteger(room.ExtraFlood);//Additional Flood

            WriteBoolean(true);

            WriteInteger(room.WhoCanMute); // who can mute
            WriteInteger(room.WhoCanKick); // who can kick
            WriteInteger(room.WhoCanBan); // who can ban
        }
    }
}