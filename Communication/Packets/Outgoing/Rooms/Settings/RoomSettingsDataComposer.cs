using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Plus.HabboHotel.Rooms;

namespace Plus.Communication.Packets.Outgoing.Rooms.Settings
{
    class RoomSettingsDataComposer : ServerPacket
    {
        public RoomSettingsDataComposer(Room Room)
            : base(ServerPacketHeader.RoomSettingsDataMessageComposer)
        {
            base.WriteInteger(Room.RoomId);
            base.WriteString(Room.Name);
            base.WriteString(Room.Description);
            base.WriteInteger(RoomAccessUtility.GetRoomAccessPacketNum(Room.Access));
            base.WriteInteger(Room.Category);
            base.WriteInteger(Room.UsersMax);
            base.WriteInteger(((Room.RoomData.Model.MapSizeX * Room.RoomData.Model.MapSizeY) > 100) ? 50 : 25);

            base.WriteInteger(Room.Tags.Count);
            foreach (string Tag in Room.Tags.ToArray())
            {
                base.WriteString(Tag);
            }

            base.WriteInteger(Room.TradeSettings); //Trade
            base.WriteInteger(Room.AllowPets); // allows pets in room - pet system lacking, so always off
            base.WriteInteger(Room.AllowPetsEating);// allows pets to eat your food - pet system lacking, so always off
            base.WriteInteger(Room.RoomBlockingEnabled);
            base.WriteInteger(Room.Hidewall);
            base.WriteInteger(Room.WallThickness);
            base.WriteInteger(Room.FloorThickness);

            base.WriteInteger(Room.chatMode);//Chat mode
            base.WriteInteger(Room.chatSize);//Chat size
            base.WriteInteger(Room.chatSpeed);//Chat speed
            base.WriteInteger(Room.chatDistance);//Hearing Distance
            base.WriteInteger(Room.extraFlood);//Additional Flood

            base.WriteBoolean(true);

            base.WriteInteger(Room.WhoCanMute); // who can mute
            base.WriteInteger(Room.WhoCanKick); // who can kick
            base.WriteInteger(Room.WhoCanBan); // who can ban
        }
    }
}