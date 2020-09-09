using System.Collections;

namespace Plus.Communication.Packets.Outgoing.Navigator
{
    class FavouritesComposer : MessageComposer
    {
        public ArrayList FavouriteIds { get; }

        public FavouritesComposer(ArrayList favouriteIds)
            : base(ServerPacketHeader.FavouritesMessageComposer)
        {
            this.FavouriteIds = favouriteIds;
        }

        public override void Compose(ServerPacket packet)
        {
            packet.WriteInteger(50);
            packet.WriteInteger(FavouriteIds.Count);

            foreach (int id in FavouriteIds.ToArray())
            {
                packet.WriteInteger(id);
            }
        }
    }
}
