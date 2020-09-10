using System.Linq;
using System.Collections.Generic;

using Plus.HabboHotel.Games;
using Plus.HabboHotel.Users;

namespace Plus.Communication.Packets.Outgoing.GameCenter
{
    public class Game3WeeklyLeaderboardComposer : MessageComposer
    {
        public GameData GameData { get; }
        public ICollection<Habbo> Habbos { get; }

        public Game3WeeklyLeaderboardComposer(GameData GameData, ICollection<Habbo> Habbos)
            : base(ServerPacketHeader.Game3WeeklyLeaderboardMessageComposer)
        {
            this.GameData = GameData;
            this.Habbos = Habbos;
        }

        public override void Compose(ServerPacket packet)
        {
            packet.WriteInteger(2014);
            packet.WriteInteger(41);
            packet.WriteInteger(0);
            packet.WriteInteger(1);
            packet.WriteInteger(1581);

            //Used to generate the ranking numbers.
            int num = 0;

            packet.WriteInteger(Habbos.Count);//Count
            foreach (Habbo Habbo in Habbos.ToList())
            {
                num++;
                packet.WriteInteger(Habbo.Id);//Id
                packet.WriteInteger(Habbo.FastfoodScore);//Score
                packet.WriteInteger(num);//Rank
                packet.WriteString(Habbo.Username);//Username
                packet.WriteString(Habbo.Look);//Figure
                packet.WriteString(Habbo.Gender.ToLower());//Gender .ToLower()
            }

            packet.WriteInteger(0);//
            packet.WriteInteger(GameData.Id);//Game Id?
        }
    }
}
