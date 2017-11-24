using System.Collections.Generic;
using Plus.HabboHotel.Catalog;
using Plus.HabboHotel.GameClients;

namespace Plus.Communication.Packets.Outgoing.Catalog
{
    public class CatalogIndexComposer : ServerPacket
    {
        public CatalogIndexComposer(GameClient Session, ICollection<CatalogPage> Pages, int Sub = 0)
            : base(ServerPacketHeader.CatalogIndexMessageComposer)
        {
            WriteRootIndex(Session, Pages);

            foreach (CatalogPage Parent in Pages)
            {
                if (Parent.ParentId != -1 || Parent.MinimumRank > Session.GetHabbo().Rank || (Parent.MinimumVIP > Session.GetHabbo().VIPRank && Session.GetHabbo().Rank == 1))
                    continue;

                WritePage(Parent, CalcTreeSize(Session, Pages, Parent.Id));

                foreach (CatalogPage child in Pages)
                {
                    if (child.ParentId != Parent.Id || child.MinimumRank > Session.GetHabbo().Rank || (child.MinimumVIP > Session.GetHabbo().VIPRank && Session.GetHabbo().Rank == 1))
                        continue;

                    if (child.Enabled)
                        WritePage(child, CalcTreeSize(Session, Pages, child.Id));
                    else
                        WriteNodeIndex(child, CalcTreeSize(Session, Pages, child.Id));
                    
                    foreach (CatalogPage SubChild in Pages)
                    {
                        if (SubChild.ParentId != child.Id || SubChild.MinimumRank > Session.GetHabbo().Rank)
                            continue;

                        WritePage(SubChild, 0);
                    }
                }
            }

            base.WriteBoolean(false);
            base.WriteString("NORMAL");
        }

        public void WriteRootIndex(GameClient session, ICollection<CatalogPage> pages)
        {
            base.WriteBoolean(true);
            base.WriteInteger(0);
            base.WriteInteger(-1);
            base.WriteString("root");
            base.WriteString(string.Empty);
            base.WriteInteger(0);
            base.WriteInteger(CalcTreeSize(session, pages, -1));
        }

        public void WriteNodeIndex(CatalogPage page, int treeSize)
        {
            base.WriteBoolean(page.Visible);
            base.WriteInteger(page.Icon);
            base.WriteInteger(-1);
            base.WriteString(page.PageLink);
            base.WriteString(page.Caption);
            base.WriteInteger(0);
            base.WriteInteger(treeSize);
        }

        public void WritePage(CatalogPage page, int treeSize)
        {
            base.WriteBoolean(page.Visible);
            base.WriteInteger(page.Icon);
            base.WriteInteger(page.Id);
            base.WriteString(page.PageLink);
            base.WriteString(page.Caption);

            base.WriteInteger(page.ItemOffers.Count);
            foreach (int i in page.ItemOffers.Keys)
            {
                base.WriteInteger(i);
            }

            base.WriteInteger(treeSize);
        }

        public int CalcTreeSize(GameClient Session, ICollection<CatalogPage> Pages, int ParentId)
        {
            int i = 0;
            foreach (CatalogPage Page in Pages)
            {
                if (Page.MinimumRank > Session.GetHabbo().Rank || (Page.MinimumVIP > Session.GetHabbo().VIPRank && Session.GetHabbo().Rank == 1) || Page.ParentId != ParentId)
                    continue;

                if (Page.ParentId == ParentId)
                    i++;
            }

            return i;
        }
    }
}