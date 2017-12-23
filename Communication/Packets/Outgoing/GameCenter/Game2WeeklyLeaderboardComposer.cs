using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Plus.HabboHotel.Games;
using Plus.HabboHotel.Users;

namespace Plus.Communication.Packets.Outgoing.GameCenter
{
    public class Game2WeeklyLeaderboardComposer : ServerPacket
    {
        public Game2WeeklyLeaderboardComposer(GameData GameData, ICollection<Habbo> Habbos)
            : base(ServerPacketHeader.Game2WeeklyLeaderboardMessageComposer)
        {
            base.WriteInteger(2014);
            base.WriteInteger(41);
            base.WriteInteger(0);
            base.WriteInteger(1);
            base.WriteInteger(1581);

            //Used to generate the ranking numbers.
            int num = 0;

            base.WriteInteger(Habbos.Count);//Count
            foreach (Habbo Habbo in Habbos.ToList())
            {
                num++;
                base.WriteInteger(Habbo.Id);//Id
                base.WriteInteger(Habbo.FastfoodScore);//Score
                base.WriteInteger(num);//Rank
               base.WriteString(Habbo.Username);//Username
               base.WriteString(Habbo.Look);//Figure
               base.WriteString(Habbo.Gender.ToLower());//Gender .ToLower()
            }

            base.WriteInteger(0);//
            base.WriteInteger(GameData.Id);//Game Id?
        }
    }
}
