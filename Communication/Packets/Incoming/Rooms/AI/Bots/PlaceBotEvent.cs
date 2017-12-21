using System;
using System.Data;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Plus.HabboHotel.Rooms;
using Plus.HabboHotel.Users.Inventory.Bots;
using Plus.Communication.Packets.Outgoing.Inventory.Bots;
using Plus.Database.Interfaces;
using Plus.HabboHotel.Rooms.AI.Speech;
using Plus.HabboHotel.Rooms.AI;
using Plus.HabboHotel.Rooms.AI.Responses;

namespace Plus.Communication.Packets.Incoming.Rooms.AI.Bots
{
    class PlaceBotEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            if (!Session.GetHabbo().InRoom)
                return;
            
            if (!PlusEnvironment.GetGame().GetRoomManager().TryGetRoom(Session.GetHabbo().CurrentRoomId, out Room room))
                return;

            if (!room.CheckRights(Session, true))
                return;

            int BotId = Packet.PopInt();
            int X = Packet.PopInt();
            int Y = Packet.PopInt();

            if (!room.GetGameMap().CanWalk(X, Y, false) || !room.GetGameMap().ValidTile(X, Y))
            {
                Session.SendNotification("You cannot place a bot here!");
                return;
            }
            
            if (!Session.GetHabbo().GetInventoryComponent().TryGetBot(BotId, out Bot bot))
                return;

            int BotCount = 0;
            foreach (RoomUser User in room.GetRoomUserManager().GetUserList().ToList())
            {
                if (User == null || User.IsPet || !User.IsBot)
                    continue;

                BotCount += 1;
            }

            if (BotCount >= 5 && !Session.GetHabbo().GetPermissions().HasRight("bot_place_any_override"))
            {
                Session.SendNotification("Sorry; 5 bots per room only!");
                return;
            }

            //TODO: Hmm, maybe not????
            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("UPDATE `bots` SET `room_id` = @roomId, `x` = @CoordX, `y` = @CoordY WHERE `id` = @BotId LIMIT 1");
                dbClient.AddParameter("roomId", room.RoomId);
                dbClient.AddParameter("BotId", bot.Id);
                dbClient.AddParameter("CoordX", X);
                dbClient.AddParameter("CoordY", Y);
                dbClient.RunQuery();
            }

            List<RandomSpeech> BotSpeechList = new List<RandomSpeech>();

            //TODO: Grab data?
            DataRow GetData = null;
            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT `ai_type`,`rotation`,`walk_mode`,`automatic_chat`,`speaking_interval`,`mix_sentences`,`chat_bubble` FROM `bots` WHERE `id` = @BotId LIMIT 1");
                dbClient.AddParameter("BotId", bot.Id);
                GetData = dbClient.GetRow();

                dbClient.SetQuery("SELECT `text` FROM `bots_speech` WHERE `bot_id` = @BotId");
                dbClient.AddParameter("BotId", bot.Id);
                DataTable BotSpeech = dbClient.GetTable();

                foreach (DataRow Speech in BotSpeech.Rows)
                {
                    BotSpeechList.Add(new RandomSpeech(Convert.ToString(Speech["text"]), bot.Id));
                }
            }

            RoomUser BotUser = room.GetRoomUserManager().DeployBot(new RoomBot(bot.Id, Session.GetHabbo().CurrentRoomId, Convert.ToString(GetData["ai_type"]), Convert.ToString(GetData["walk_mode"]), bot.Name, "", bot.Figure, X, Y, 0, 4, 0, 0, 0, 0, ref BotSpeechList, "", 0, bot.OwnerId, PlusEnvironment.EnumToBool(GetData["automatic_chat"].ToString()), Convert.ToInt32(GetData["speaking_interval"]), PlusEnvironment.EnumToBool(GetData["mix_sentences"].ToString()), Convert.ToInt32(GetData["chat_bubble"])), null);
            BotUser.Chat("Hello!", 0);

            room.GetGameMap().UpdateUserMovement(new System.Drawing.Point(X,Y), new System.Drawing.Point(X, Y), BotUser);


            if (!Session.GetHabbo().GetInventoryComponent().TryRemoveBot(BotId, out Bot ToRemove))
            {
                Console.WriteLine("Error whilst removing Bot: " + ToRemove.Id);
                return;
            }
            Session.SendPacket(new BotInventoryComposer(Session.GetHabbo().GetInventoryComponent().GetBots()));
        }
    }
}
