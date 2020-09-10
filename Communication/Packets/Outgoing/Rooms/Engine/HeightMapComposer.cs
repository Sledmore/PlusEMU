using System;

namespace Plus.Communication.Packets.Outgoing.Rooms.Engine
{
    class HeightMapComposer : MessageComposer
    {
        public string Map { get; }

        public HeightMapComposer(string Map)
            : base(ServerPacketHeader.HeightMapMessageComposer)
        {
            this.Map = Map.Replace("\n", "");
        }

        public override void Compose(ServerPacket packet)
        {
            string[] Split = Map.Split('\r');
            packet.WriteInteger(Split[0].Length);
            packet.WriteInteger((Split.Length - 1) * Split[0].Length);
            int x = 0;
            int y = 0;
            for (y = 0; y < Split.Length - 1; y++)
            {
                for (x = 0; x < Split[0].Length; x++)
                {
                    char pos;

                    try
                    {
                        pos = Split[y][x];
                    }
                    catch { pos = 'x'; }

                    if (pos == 'x')
                        packet.WriteShort(-1);
                    else
                    {
                        int Height = 0;
                        if (int.TryParse(pos.ToString(), out Height))
                        {
                            Height = Height * 256;
                        }
                        else
                        {
                            Height = ((Convert.ToInt32(pos) - 87) * 256);
                        }
                        packet.WriteShort(Height);
                    }
                }
            }
        }
    }
}
