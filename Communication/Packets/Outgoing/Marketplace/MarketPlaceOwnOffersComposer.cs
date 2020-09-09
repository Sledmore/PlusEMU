using System;
using System.Data;
using Plus.Database.Interfaces;


namespace Plus.Communication.Packets.Outgoing.Marketplace
{
    class MarketPlaceOwnOffersComposer : MessageComposer
    {
        public int UserId { get; }

        public MarketPlaceOwnOffersComposer(int UserId)
            : base(ServerPacketHeader.MarketPlaceOwnOffersMessageComposer)
        {
            this.UserId = UserId;
        }

        public override void Compose(ServerPacket packet)
        {
            int i = 0;
            DataTable table = null;
            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT timestamp, state, offer_id, item_type, sprite_id, total_price, limited_number, limited_stack FROM catalog_marketplace_offers WHERE user_id = '" + UserId + "'");
                table = dbClient.GetTable();

                dbClient.SetQuery("SELECT SUM(asking_price) FROM catalog_marketplace_offers WHERE state = '2' AND user_id = '" + UserId + "'");
                i = dbClient.GetInteger();

                packet.WriteInteger(i);
                if (table != null)
                {
                    packet.WriteInteger(table.Rows.Count);
                    foreach (DataRow row in table.Rows)
                    {
                        int num2 = Convert.ToInt32(Math.Floor((double)(((((double)row["timestamp"]) + 172800.0) - PlusEnvironment.GetUnixTimestamp()) / 60.0)));
                        int num3 = int.Parse(row["state"].ToString());
                        if ((num2 <= 0) && (num3 != 2))
                        {
                            num3 = 3;
                            num2 = 0;
                        }
                        packet.WriteInteger(Convert.ToInt32(row["offer_id"]));
                        packet.WriteInteger(num3);
                        packet.WriteInteger(1);
                        packet.WriteInteger(Convert.ToInt32(row["sprite_id"]));

                        packet.WriteInteger(256);
                        packet.WriteString("");
                        packet.WriteInteger(Convert.ToInt32(row["limited_number"]));
                        packet.WriteInteger(Convert.ToInt32(row["limited_stack"]));

                        packet.WriteInteger(Convert.ToInt32(row["total_price"]));
                        packet.WriteInteger(num2);
                        packet.WriteInteger(Convert.ToInt32(row["sprite_id"]));
                    }
                }
                else
                {
                    packet.WriteInteger(0);
                }
            }
        }
    }
}