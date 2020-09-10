using System.Linq;
using System.Collections.Generic;
using Plus.HabboHotel.Users.Clothing.Parts;

namespace Plus.Communication.Packets.Outgoing.Inventory.AvatarEffects
{
    class FigureSetIdsComposer : MessageComposer
    {
        public ICollection<ClothingParts> ClothingParts { get; }

        public FigureSetIdsComposer(ICollection<ClothingParts> ClothingParts)
            : base(ServerPacketHeader.FigureSetIdsMessageComposer)
        {
            this.ClothingParts = ClothingParts;
        }

        public override void Compose(ServerPacket packet)
        {
            packet.WriteInteger(ClothingParts.Count);
            foreach (ClothingParts Part in ClothingParts.ToList())
            {
                packet.WriteInteger(Part.PartId);
            }

            packet.WriteInteger(ClothingParts.Count);
            foreach (ClothingParts Part in ClothingParts.ToList())
            {
                packet.WriteString(Part.Part);
            }
        }
    }
}
