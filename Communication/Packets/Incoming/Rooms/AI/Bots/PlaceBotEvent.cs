using System;
using System.Data;
using System.Linq;
using System.Collections.Generic;

using Plus.HabboHotel.Rooms;
using Plus.HabboHotel.Users.Inventory.Bots;
using Plus.Communication.Packets.Outgoing.Inventory.Bots;
using Plus.Database.Interfaces;
using Plus.HabboHotel.Rooms.AI.Speech;
using Plus.HabboHotel.Rooms.AI;

namespace Plus.Communication.Packets.Incoming.Rooms.AI.Bots
{
    class PlaceBotEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient session, ClientPacket packet)
        {
            if (!session.GetHabbo().InRoom)
                return;
            
            if (!PlusEnvironment.GetGame().GetRoomManager().TryGetRoom(session.GetHabbo().CurrentRoomId, out Room room))
                return;

            if (!room.CheckRights(session, true))
                return;

            int botId = packet.PopInt();
            int x = packet.PopInt();
            int y = packet.PopInt();

            if (!room.GetGameMap().CanWalk(x, y, false) || !room.GetGameMap().ValidTile(x, y))
            {
                session.SendNotification("You cannot place a bot here!");
                return;
            }
            
            if (!session.GetHabbo().GetInventoryComponent().TryGetBot(botId, out Bot bot))
                return;

            int botCount = 0;
            foreach (RoomUser user in room.GetRoomUserManager().GetUserList().ToList())
            {
                if (user == null || user.IsPet || !user.IsBot)
                    continue;

                botCount += 1;
            }

            if (botCount >= 5 && !session.GetHabbo().GetPermissions().HasRight("bot_place_any_override"))
            {
                session.SendNotification("Sorry; 5 bots per room only!");
                return;
            }

            //TODO: Hmm, maybe not????
            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("UPDATE `bots` SET `room_id` = @roomId, `x` = @CoordX, `y` = @CoordY WHERE `id` = @BotId LIMIT 1");
                dbClient.AddParameter("roomId", room.RoomId);
                dbClient.AddParameter("BotId", bot.Id);
                dbClient.AddParameter("CoordX", x);
                dbClient.AddParameter("CoordY", y);
                dbClient.RunQuery();
            }

            List<RandomSpeech> botSpeechList = new List<RandomSpeech>();

            //TODO: Grab data?
            DataRow getData;
            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT `ai_type`,`rotation`,`walk_mode`,`automatic_chat`,`speaking_interval`,`mix_sentences`,`chat_bubble` FROM `bots` WHERE `id` = @BotId LIMIT 1");
                dbClient.AddParameter("BotId", bot.Id);
                getData = dbClient.GetRow();

                dbClient.SetQuery("SELECT `text` FROM `bots_speech` WHERE `bot_id` = @BotId");
                dbClient.AddParameter("BotId", bot.Id);
                DataTable botSpeech = dbClient.GetTable();

                foreach (DataRow speech in botSpeech.Rows)
                {
                    botSpeechList.Add(new RandomSpeech(Convert.ToString(speech["text"]), bot.Id));
                }
            }

            RoomUser botUser = room.GetRoomUserManager().DeployBot(new RoomBot(bot.Id, session.GetHabbo().CurrentRoomId, Convert.ToString(getData["ai_type"]), Convert.ToString(getData["walk_mode"]), bot.Name, "", bot.Figure, x, y, 0, 4, 0, 0, 0, 0, ref botSpeechList, "", 0, bot.OwnerId, PlusEnvironment.EnumToBool(getData["automatic_chat"].ToString()), Convert.ToInt32(getData["speaking_interval"]), PlusEnvironment.EnumToBool(getData["mix_sentences"].ToString()), Convert.ToInt32(getData["chat_bubble"])), null);
            botUser.Chat("Hello!");

            room.GetGameMap().UpdateUserMovement(new System.Drawing.Point(x,y), new System.Drawing.Point(x, y), botUser);


            if (!session.GetHabbo().GetInventoryComponent().TryRemoveBot(botId, out Bot toRemove))
            {
                Console.WriteLine("Error whilst removing Bot: " + toRemove.Id);
                return;
            }
            session.SendPacket(new BotInventoryComposer(session.GetHabbo().GetInventoryComponent().GetBots()));
        }
    }
}
