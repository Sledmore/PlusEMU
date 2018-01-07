using Plus.Database.Interfaces;
using Plus.HabboHotel.GameClients;


namespace Plus.Communication.Packets.Incoming.Avatar
{
    class SaveWardrobeOutfitEvent : IPacketEvent
    {
        public void Parse(GameClient session, ClientPacket packet)
        {
            int slotId = packet.PopInt();
            string look = packet.PopString();
            string gender = packet.PopString();

            look = PlusEnvironment.GetFigureManager().ProcessFigure(look, gender, session.GetHabbo().GetClothing().GetClothingParts, true);

            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT null FROM `user_wardrobe` WHERE `user_id` = @id AND `slot_id` = @slot");
                dbClient.AddParameter("id", session.GetHabbo().Id);
                dbClient.AddParameter("slot", slotId);

                if (dbClient.GetRow() != null)
                {
                    dbClient.SetQuery("UPDATE `user_wardrobe` SET `look` = @look, `gender` = @gender WHERE `user_id` = @id AND `slot_id` = @slot LIMIT 1");
                    dbClient.AddParameter("id", session.GetHabbo().Id);
                    dbClient.AddParameter("slot", slotId);
                    dbClient.AddParameter("look", look);
                    dbClient.AddParameter("gender", gender.ToUpper());
                    dbClient.RunQuery();
                }
                else
                {
                    dbClient.SetQuery("INSERT INTO `user_wardrobe` (`user_id`,`slot_id`,`look`,`gender`) VALUES (@id,@slot,@look,@gender)");
                    dbClient.AddParameter("id", session.GetHabbo().Id);
                    dbClient.AddParameter("slot", slotId);
                    dbClient.AddParameter("look", look);
                    dbClient.AddParameter("gender", gender.ToUpper());
                    dbClient.RunQuery();
                }
            }
        }
    }
}
