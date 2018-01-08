using System.Linq;
using System.Collections.Generic;
using Plus.HabboHotel.Users.Clothing.Parts;

namespace Plus.Communication.Packets.Outgoing.Inventory.AvatarEffects
{
    class FigureSetIdsComposer : ServerPacket
    {
        public FigureSetIdsComposer(ICollection<ClothingParts> ClothingParts)
            : base(ServerPacketHeader.FigureSetIdsMessageComposer)
        {
            base.WriteInteger(ClothingParts.Count);
            foreach (ClothingParts Part in ClothingParts.ToList())
            {
                base.WriteInteger(Part.PartId);
            }

            base.WriteInteger(ClothingParts.Count);
            foreach (ClothingParts Part in ClothingParts.ToList())
            {
               base.WriteString(Part.Part);
            }
        }
    }
}
