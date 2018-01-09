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
            WriteInteger(bases.Count);
            foreach (GroupBadgeParts part in bases)
            {
                WriteInteger(part.Id);
                WriteString(part.AssetOne);
                WriteString(part.AssetTwo);
            }

            WriteInteger(symbols.Count);
            foreach (GroupBadgeParts part in symbols)
            {
                WriteInteger(part.Id);
                WriteString(part.AssetOne);
                WriteString(part.AssetTwo);
            }

            WriteInteger(baseColours.Count);
            foreach (GroupColours colour in baseColours)
            {
                WriteInteger(colour.Id);
                WriteString(colour.Colour);
            }

            WriteInteger(symbolColours.Count);
            foreach (GroupColours colour in symbolColours)
            {
                WriteInteger(colour.Id);
                WriteString(colour.Colour);
            }

            WriteInteger(backgroundColours.Count);
            foreach (GroupColours colour in backgroundColours)
            {
                WriteInteger(colour.Id);
                WriteString(colour.Colour);
            }
        }
    }
}