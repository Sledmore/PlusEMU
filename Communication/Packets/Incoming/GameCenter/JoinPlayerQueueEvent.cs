using System;
using System.Linq;
using System.Text;
using System.Data;
using System.Collections.Generic;

using Plus.HabboHotel.Games;
using Plus.Communication.Packets.Outgoing.GameCenter;
using Plus.Database.Interfaces;

namespace Plus.Communication.Packets.Incoming.GameCenter
{
    class JoinPlayerQueueEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            if (Session == null || Session.GetHabbo() == null)
                return;

            int GameId = Packet.PopInt();

            GameData GameData = null;
            if (PlusEnvironment.GetGame().GetGameDataManager().TryGetGame(GameId, out GameData))
            {
                string SSOTicket = "HABBOON-Fastfood-" + GenerateSSO(32) + "-" + Session.GetHabbo().Id;

                Session.SendPacket(new JoinQueueComposer(GameData.Id));
                Session.SendPacket(new LoadGameComposer(GameData, SSOTicket));
            }
        }

        private string GenerateSSO(int length)
        {
            Random random = new Random();
            string characters = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
            StringBuilder result = new StringBuilder(length);
            for (int i = 0; i < length; i++)
            {
                result.Append(characters[random.Next(characters.Length)]);
            }
            return result.ToString();
        }
    }
}
