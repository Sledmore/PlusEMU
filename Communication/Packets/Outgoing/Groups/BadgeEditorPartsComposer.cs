using System.Collections.Generic;

using Plus.HabboHotel.Groups;

namespace Plus.Communication.Packets.Outgoing.Groups
{
    class BadgeEditorPartsComposer : ServerPacket
    {
        public BadgeEditorPartsComposer(ICollection<GroupBadgeParts> bases, ICollection<GroupBadgeParts> symbols, ICollection<GroupColours> baseColours, ICollection<GroupColours> symbolColours,
          ICollection<GroupColours> backgroundColours)
          : base(ServerPacketHeader.BadgeEditorPartsMessageComposer)
        {
            base.WriteInteger(bases.Count);
            foreach (GroupBadgeParts part in bases)
            {
                base.WriteInteger(part.Id);
                base.WriteString(part.AssetOne);
                base.WriteString(part.AssetTwo);
            }

            base.WriteInteger(symbols.Count);
            foreach (GroupBadgeParts part in symbols)
            {
                base.WriteInteger(part.Id);
                base.WriteString(part.AssetOne);
                base.WriteString(part.AssetTwo);
            }

            base.WriteInteger(baseColours.Count);
            foreach (GroupColours colour in baseColours)
            {
                base.WriteInteger(colour.Id);
                base.WriteString(colour.Colour);
            }

            base.WriteInteger(symbolColours.Count);
            foreach (GroupColours colour in symbolColours)
            {
                base.WriteInteger(colour.Id);
                base.WriteString(colour.Colour);
            }

            base.WriteInteger(backgroundColours.Count);
            foreach (GroupColours colour in backgroundColours)
            {
                base.WriteInteger(colour.Id);
                base.WriteString(colour.Colour);
            }
        }
    }
}