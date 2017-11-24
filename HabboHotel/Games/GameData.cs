using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plus.HabboHotel.Games
{
    public class GameData
    {
        public int GameId { get; set; }
        public string GameName { get; set; }
        public string ColourOne { get; set; }
        public string ColourTwo { get; set; }
        public string ResourcePath { get; set; }
        public string StringThree { get; set; }
        public string GameSWF { get; set; }
        public string GameAssets { get; set; }
        public string GameServerHost { get; set; }
        public string GameServerPort { get; set; }
        public string SocketPolicyPort { get; set; }
        public bool GameEnabled { get; set; }

        public GameData(int GameId, string GameName, string ColourOne, string ColourTwo, string ResourcePath, string StringThree, string GameSWF, string GameAssets, string GameServerHost, string GameServerPort, string SocketPolicyPort, Boolean GameEnabled)
        {
            this.GameId = GameId;
            this.GameName = GameName;
            this.ColourOne = ColourOne;
            this.ColourTwo = ColourTwo;
            this.ResourcePath = ResourcePath;
            this.StringThree = StringThree;
            this.GameSWF = GameSWF;
            this.GameAssets = GameAssets;
            this.GameServerHost = GameServerHost;
            this.GameServerPort = GameServerPort;
            this.SocketPolicyPort = SocketPolicyPort;
            this.GameEnabled = GameEnabled;
        }
    }
}
