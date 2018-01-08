using System.Linq;
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
            WriteInteger(2014);
            WriteInteger(41);
            WriteInteger(0);
            WriteInteger(1);
            WriteInteger(1581);

            //Used to generate the ranking numbers.
            int num = 0;

            WriteInteger(Habbos.Count);//Count
            foreach (Habbo Habbo in Habbos.ToList())
            {
                num++;
                WriteInteger(Habbo.Id);//Id
                WriteInteger(Habbo.FastfoodScore);//Score
                WriteInteger(num);//Rank
               WriteString(Habbo.Username);//Username
               WriteString(Habbo.Look);//Figure
               WriteString(Habbo.Gender.ToLower());//Gender .ToLower()
            }

            WriteInteger(0);//
            WriteInteger(GameData.Id);//Game Id?
        }
    }
}
