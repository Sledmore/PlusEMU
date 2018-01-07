using Plus.HabboHotel.GameClients;
using Plus.HabboHotel.Items;
using Plus.Communication.Packets.Outgoing.Catalog;

namespace Plus.Communication.Packets.Incoming.Catalog
{
    public class GetSellablePetBreedsEvent : IPacketEvent
    {
        public void Parse(GameClient session, ClientPacket packet)
        {
            string type = packet.PopString();

            ItemData item = PlusEnvironment.GetGame().GetItemManager().GetItemByName(type);
            if (item == null)
                return;

            int petId = item.BehaviourData;

            session.SendPacket(new SellablePetBreedsComposer(type, petId, PlusEnvironment.GetGame().GetCatalog().GetPetRaceManager().GetRacesForRaceId(petId)));
        }
    }
}