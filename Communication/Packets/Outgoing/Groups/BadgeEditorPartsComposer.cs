using System.Collections.Generic;

using Plus.HabboHotel.Groups;

namespace Plus.Communication.Packets.Outgoing.Groups
{
    class BadgeEditorPartsComposer : MessageComposer
    {
        public ICollection<GroupBadgeParts> Bases { get; }
        public ICollection<GroupBadgeParts> Symbols { get; }
        public ICollection<GroupColours> BaseColours { get; }
        public ICollection<GroupColours> SymbolColours { get; }
        public ICollection<GroupColours> BackgroundColours { get; }

        public BadgeEditorPartsComposer(ICollection<GroupBadgeParts> bases, ICollection<GroupBadgeParts> symbols, ICollection<GroupColours> baseColours, ICollection<GroupColours> symbolColours,
          ICollection<GroupColours> backgroundColours)
          : base(ServerPacketHeader.BadgeEditorPartsMessageComposer)
        {
            this.Bases = bases;
            this.Symbols = symbols;
            this.BaseColours = baseColours;
            this.SymbolColours = symbolColours;
            this.BackgroundColours = BackgroundColours;
        }

        public override void Compose(ServerPacket packet)
        {
            packet.WriteInteger(Bases.Count);
            foreach (GroupBadgeParts part in Bases)
            {
                packet.WriteInteger(part.Id);
                packet.WriteString(part.AssetOne);
                packet.WriteString(part.AssetTwo);
            }

            packet.WriteInteger(Symbols.Count);
            foreach (GroupBadgeParts part in Symbols)
            {
                packet.WriteInteger(part.Id);
                packet.WriteString(part.AssetOne);
                packet.WriteString(part.AssetTwo);
            }

            packet.WriteInteger(BaseColours.Count);
            foreach (GroupColours colour in BaseColours)
            {
                packet.WriteInteger(colour.Id);
                packet.WriteString(colour.Colour);
            }

            packet.WriteInteger(SymbolColours.Count);
            foreach (GroupColours colour in SymbolColours)
            {
                packet.WriteInteger(colour.Id);
                packet.WriteString(colour.Colour);
            }

            packet.WriteInteger(BackgroundColours.Count);
            foreach (GroupColours colour in BackgroundColours)
            {
                packet.WriteInteger(colour.Id);
                packet.WriteString(colour.Colour);
            }
        }
    }
}