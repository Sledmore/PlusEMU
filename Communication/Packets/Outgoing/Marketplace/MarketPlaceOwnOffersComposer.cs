using System;
using System.Linq;
using System.Text;
using System.Data;
using System.Collections.Generic;

using Plus.Database.Interfaces;


namespace Plus.Communication.Packets.Outgoing.Marketplace
{
    class MarketPlaceOwnOffersComposer : ServerPacket
    {
        public MarketPlaceOwnOffersComposer(int UserId)
            : base(ServerPacketHeader.MarketPlaceOwnOffersMessageComposer)
        {
            int i = 0;
            DataTable table = null;
            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT timestamp, state, offer_id, item_type, sprite_id, total_price, limited_number, limited_stack FROM catalog_marketplace_offers WHERE user_id = '" + UserId + "'");
                table = dbClient.GetTable();

                dbClient.SetQuery("SELECT SUM(asking_price) FROM catalog_marketplace_offers WHERE state = '2' AND user_id = '" + UserId + "'");
                i = dbClient.GetInteger();

                base.WriteInteger(i);
                if (table != null)
                {
                    base.WriteInteger(table.Rows.Count);
                    foreach (DataRow row in table.Rows)
                    {
                        int num2 = Convert.ToInt32(Math.Floor((double)(((((double)row["timestamp"]) + 172800.0) - PlusEnvironment.GetUnixTimestamp()) / 60.0)));
                        int num3 = int.Parse(row["state"].ToString());
                        if ((num2 <= 0) && (num3 != 2))
                        {
                            num3 = 3;
                            num2 = 0;
                        }
                        base.WriteInteger(Convert.ToInt32(row["offer_id"]));
                        base.WriteInteger(num3);
                        base.WriteInteger(1);
                        base.WriteInteger(Convert.ToInt32(row["sprite_id"]));

                        base.WriteInteger(256);
                       base.WriteString("");
                        base.WriteInteger(Convert.ToInt32(row["limited_number"]));
                        base.WriteInteger(Convert.ToInt32(row["limited_stack"]));

                        base.WriteInteger(Convert.ToInt32(row["total_price"]));
                        base.WriteInteger(num2);
                        base.WriteInteger(Convert.ToInt32(row["sprite_id"]));
                    }
                }
                else
                {
                    base.WriteInteger(0);
                }
            }
        }
    }
}