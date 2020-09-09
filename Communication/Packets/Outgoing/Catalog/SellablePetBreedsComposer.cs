using System.Linq;
using System.Collections.Generic;

using Plus.HabboHotel.Catalog.Pets;

namespace Plus.Communication.Packets.Outgoing.Catalog
{
    public class SellablePetBreedsComposer : MessageComposer
    {
        public string PetType { get; }
        public int PetId { get; }
        public ICollection<PetRace> Races { get; }

        public SellablePetBreedsComposer(string PetType, int PetId, ICollection<PetRace> Races)
            : base(ServerPacketHeader.SellablePetBreedsMessageComposer)
        {
            this.PetType = PetType;
            this.PetId = PetId;
            this.Races = Races;
        }

        public override void Compose(ServerPacket packet)
        {
            packet.WriteString(PetType);

            packet.WriteInteger(Races.Count);
            foreach (PetRace Race in Races.ToList())
            {
                packet.WriteInteger(PetId);
                packet.WriteInteger(Race.PrimaryColour);
                packet.WriteInteger(Race.SecondaryColour);
                packet.WriteBoolean(Race.HasPrimaryColour);
                packet.WriteBoolean(Race.HasSecondaryColour);
            }
        }
    }
}