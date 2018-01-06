using System.Collections;

namespace Plus.Communication.Packets.Outgoing.Navigator
{
    class FavouritesComposer : ServerPacket
    {
        public FavouritesComposer(ArrayList favouriteIDs)
            : base(ServerPacketHeader.FavouritesMessageComposer)
        {
            WriteInteger(50);
            WriteInteger(favouriteIDs.Count);

            foreach (int id in favouriteIDs.ToArray())
            {
                WriteInteger(id);
            }
        }
    }
}
