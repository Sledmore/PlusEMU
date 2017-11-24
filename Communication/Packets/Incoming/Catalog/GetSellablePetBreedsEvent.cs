using Plus.HabboHotel.GameClients;
using Plus.HabboHotel.Items;
using Plus.Communication.Packets.Outgoing.Catalog;

namespace Plus.Communication.Packets.Incoming.Catalog
{
    public class GetSellablePetBreedsEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            string Type = Packet.PopString();

            ItemData Item = PlusEnvironment.GetGame().GetItemManager().GetItemByName(Type);
            if (Item == null)
                return;

            int PetId = Item.BehaviourData;

            Session.SendPacket(new SellablePetBreedsComposer(Type, PetId, PlusEnvironment.GetGame().GetCatalog().GetPetRaceManager().GetRacesForRaceId(PetId)));
        }
    }
}