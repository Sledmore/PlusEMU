using System.Linq;
using System.Collections.Generic;

using Plus.HabboHotel.Catalog.Pets;

namespace Plus.Communication.Packets.Outgoing.Catalog
{
    public class SellablePetBreedsComposer : ServerPacket
    {
        public SellablePetBreedsComposer(string PetType, int PetId, ICollection<PetRace> Races)
            : base(ServerPacketHeader.SellablePetBreedsMessageComposer)
        {
           WriteString(PetType);

            WriteInteger(Races.Count);
            foreach (PetRace Race in Races.ToList())
            {
                WriteInteger(PetId);
                WriteInteger(Race.PrimaryColour);
                WriteInteger(Race.SecondaryColour);
                WriteBoolean(Race.HasPrimaryColour);
                WriteBoolean(Race.HasSecondaryColour);
            }


        }
    }
}