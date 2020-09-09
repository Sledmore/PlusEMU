using System.Collections.Generic;
using Plus.HabboHotel.Catalog;
using Plus.HabboHotel.GameClients;
using Plus.HabboHotel.Users;

namespace Plus.Communication.Packets.Outgoing.Catalog
{
    public class CatalogIndexComposer : MessageComposer
    {
        public Habbo Habbo { get; }
        public ICollection<CatalogPage> Pages { get; }

        public CatalogIndexComposer(GameClient sesion, ICollection<CatalogPage> pages)
            : base(ServerPacketHeader.CatalogIndexMessageComposer)
        {
            this.Habbo = sesion.GetHabbo();
            this.Pages = pages;
        }

        public void WriteRootIndex(Habbo Habbo, ICollection<CatalogPage> pages, ServerPacket packet)
        {
            packet.WriteBoolean(true);
            packet.WriteInteger(0);
            packet.WriteInteger(-1);
            packet.WriteString("root");
            packet.WriteString(string.Empty);
            packet.WriteInteger(0);
            packet.WriteInteger(CalcTreeSize(Habbo, pages, -1));
        }

        public void WriteNodeIndex(CatalogPage page, int treeSize, ServerPacket packet)
        {
            packet.WriteBoolean(page.Visible);
            packet.WriteInteger(page.Icon);
            packet.WriteInteger(-1);
            packet.WriteString(page.PageLink);
            packet.WriteString(page.Caption);
            packet.WriteInteger(0);
            packet.WriteInteger(treeSize);
        }

        public void WritePage(CatalogPage page, int treeSize, ServerPacket packet)
        {
            packet.WriteBoolean(page.Visible);
            packet.WriteInteger(page.Icon);
            packet.WriteInteger(page.Id);
            packet.WriteString(page.PageLink);
            packet.WriteString(page.Caption);

            packet.WriteInteger(page.ItemOffers.Count);
            foreach (int i in page.ItemOffers.Keys)
            {
                packet.WriteInteger(i);
            }

            packet.WriteInteger(treeSize);
        }

        public int CalcTreeSize(Habbo Habbo, ICollection<CatalogPage> Pages, int ParentId)
        {
            int i = 0;
            foreach (CatalogPage Page in Pages)
            {
                if (Page.MinimumRank > Habbo.Rank || (Page.MinimumVIP > Habbo.VIPRank && Habbo.Rank == 1) || Page.ParentId != ParentId)
                    continue;

                if (Page.ParentId == ParentId)
                    i++;
            }

            return i;
        }

        public override void Compose(ServerPacket packet)
        {
            WriteRootIndex(Habbo, Pages, packet);

            foreach (CatalogPage Parent in Pages)
            {
                if (Parent.ParentId != -1 || Parent.MinimumRank > Habbo.Rank || (Parent.MinimumVIP > Habbo.VIPRank && Habbo.Rank == 1))
                    continue;

                WritePage(Parent, CalcTreeSize(Habbo, Pages, Parent.Id), packet);

                foreach (CatalogPage child in Pages)
                {
                    if (child.ParentId != Parent.Id || child.MinimumRank > Habbo.Rank || (child.MinimumVIP > Habbo.VIPRank && Habbo.Rank == 1))
                        continue;

                    if (child.Enabled)
                        WritePage(child, CalcTreeSize(Habbo, Pages, child.Id), packet);
                    else
                        WriteNodeIndex(child, CalcTreeSize(Habbo, Pages, child.Id), packet);

                    foreach (CatalogPage SubChild in Pages)
                    {
                        if (SubChild.ParentId != child.Id || SubChild.MinimumRank > Habbo.Rank)
                            continue;

                        WritePage(SubChild, 0, packet);
                    }
                }
            }

            packet.WriteBoolean(false);
            packet.WriteString("NORMAL");
        }
    }
}