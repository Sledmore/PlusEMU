using System.Linq;
using System.Collections.Generic;

using MoreLinq;
using Plus.HabboHotel.Items;
using Plus.Communication.Packets.Outgoing.Inventory.Furni;

namespace Plus.Communication.Packets.Incoming.Inventory.Furni
{
    class RequestFurniInventoryEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            IEnumerable<Item> Items = Session.GetHabbo().GetInventoryComponent().GetWallAndFloor;

            int page = 0;
            int pages = ((Items.Count() - 1) / 700) + 1;

            if (!Items.Any())
            {
                Session.SendPacket(new FurniListComposer(Items.ToList(), 1, 0));
            }
            else
            {
                foreach (ICollection<Item> batch in Items.Batch(700))
                {
                    Session.SendPacket(new FurniListComposer(batch.ToList(), pages, page));

                    page++;
                }
            }
        }
    }
}
