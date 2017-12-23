namespace Plus.HabboHotel.Games
{
    public class GameData
    {
        public int Id { get; private set; }
        public string Name { get; private set; }
        public string ColourOne { get; private set; }
        public string ColourTwo { get; private set; }
        public string ResourcePath { get; private set; }
        public string StringThree { get; private set; }
        public string SWF { get; private set; }
        public string Assets { get; private set; }
        public string ServerHost { get; private set; }
        public string ServerPort { get; private set; }
        public string SocketPolicyPort { get; private set; }
        public bool Enabled { get; private set; }

        public GameData(int gameId, string name, string colourOne, string colourTwo, string resourcePath, string stringThree, string gameSWF, string gameAssets, string gameServerHost, string gameServerPort, string socketPolicyPort, bool enabled)
        {
            Id = gameId;
            Name = name;
            ColourOne = colourOne;
            ColourTwo = colourTwo;
            ResourcePath = resourcePath;
            StringThree = stringThree;
            SWF = gameSWF;
            Assets = gameAssets;
            ServerHost = gameServerHost;
            ServerPort = gameServerPort;
            SocketPolicyPort = socketPolicyPort;
            Enabled = enabled;
        }
    }
}
