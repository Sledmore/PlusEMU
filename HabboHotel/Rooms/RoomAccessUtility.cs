namespace Plus.HabboHotel.Rooms
{
    public static class RoomAccessUtility
    {
        public static int GetRoomAccessPacketNum(RoomAccess Access)
        {
            switch (Access)
            {
                default:
                case RoomAccess.OPEN:
                    return 0;

                case RoomAccess.DOORBELL:
                    return 1;

                case RoomAccess.PASSWORD:
                    return 2;

                case RoomAccess.INVISIBLE:
                    return 3;
            }
        }

        public static RoomAccess ToRoomAccess(string Id)
        {
            switch (Id)
            {
                default:
                case "open":
                    return RoomAccess.OPEN;

                case "locked":
                    return RoomAccess.DOORBELL;

                case "password":
                    return RoomAccess.PASSWORD;

                case "invisible":
                    return RoomAccess.INVISIBLE;
            }
        }

        public static RoomAccess ToRoomAccess(int Id)
        {
            switch (Id)
            {
                default:
                case 0:
                    return RoomAccess.OPEN;

                case 1:
                    return RoomAccess.DOORBELL;

                case 2:
                    return RoomAccess.PASSWORD;

                case 3:
                    return RoomAccess.INVISIBLE;
            }
        }
    }
}
