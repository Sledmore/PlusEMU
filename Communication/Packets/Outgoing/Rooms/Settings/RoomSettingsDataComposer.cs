using Plus.HabboHotel.Rooms;

namespace Plus.Communication.Packets.Outgoing.Rooms.Settings
{
    class RoomSettingsDataComposer : MessageComposer
    {
        public Room Room { get; }
        public RoomSettingsDataComposer(Room room)
            : base(ServerPacketHeader.RoomSettingsDataMessageComposer)
        {
            this.Room = room;
        }

        public override void Compose(ServerPacket packet)
        {
            packet.WriteInteger(Room.RoomId);
            packet.WriteString(Room.Name);
            packet.WriteString(Room.Description);
            packet.WriteInteger(RoomAccessUtility.GetRoomAccessPacketNum(Room.Access));
            packet.WriteInteger(Room.Category);
            packet.WriteInteger(Room.UsersMax);
            packet.WriteInteger(((Room.Model.MapSizeX * Room.Model.MapSizeY) > 100) ? 50 : 25);

            packet.WriteInteger(Room.Tags.Count);
            foreach (string Tag in Room.Tags.ToArray())
            {
                packet.WriteString(Tag);
            }

            packet.WriteInteger(Room.TradeSettings); //Trade
            packet.WriteInteger(Room.AllowPets); // allows pets in room - pet system lacking, so always off
            packet.WriteInteger(Room.AllowPetsEating);// allows pets to eat your food - pet system lacking, so always off
            packet.WriteInteger(Room.RoomBlockingEnabled);
            packet.WriteInteger(Room.Hidewall);
            packet.WriteInteger(Room.WallThickness);
            packet.WriteInteger(Room.FloorThickness);

            packet.WriteInteger(Room.ChatMode);//Chat mode
            packet.WriteInteger(Room.ChatSize);//Chat size
            packet.WriteInteger(Room.ChatSpeed);//Chat speed
            packet.WriteInteger(Room.ChatDistance);//Hearing Distance
            packet.WriteInteger(Room.ExtraFlood);//Additional Flood

            packet.WriteBoolean(true);

            packet.WriteInteger(Room.WhoCanMute); // who can mute
            packet.WriteInteger(Room.WhoCanKick); // who can kick
            packet.WriteInteger(Room.WhoCanBan); // who can ban
        }
    }
}