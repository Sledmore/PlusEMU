using System;
using System.Data;
using Plus.Database.Interfaces;

namespace Plus.Communication.Packets.Outgoing.Avatar
{
    class WardrobeComposer : MessageComposer
    {
        public int UserId { get; }

        public WardrobeComposer(int userId)
            : base(ServerPacketHeader.WardrobeMessageComposer)
        {
            this.UserId = userId;
        }

        public override void Compose(ServerPacket packet)
        {
            packet.WriteInteger(1);
            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT `slot_id`,`look`,`gender` FROM `user_wardrobe` WHERE `user_id` = '" + UserId + "'");
                DataTable WardrobeData = dbClient.GetTable();

                if (WardrobeData == null)
                    packet.WriteInteger(0);
                else
                {
                    packet.WriteInteger(WardrobeData.Rows.Count);
                    foreach (DataRow Row in WardrobeData.Rows)
                    {
                        packet.WriteInteger(Convert.ToInt32(Row["slot_id"]));
                        packet.WriteString(Convert.ToString(Row["look"]));
                        packet.WriteString(Row["gender"].ToString().ToUpper());
                    }
                }
            }
        }
    }
}