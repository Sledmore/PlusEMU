using System;

namespace Plus.Communication.Packets.Outgoing.Rooms.Engine
{
    class HeightMapComposer : ServerPacket
    {
        public HeightMapComposer(string Map)
            : base(ServerPacketHeader.HeightMapMessageComposer)
        {
            Map = Map.Replace("\n", "");
            string[] Split = Map.Split('\r');
            base.WriteInteger(Split[0].Length);
            base.WriteInteger((Split.Length - 1) * Split[0].Length);
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
                        base.WriteShort(-1);
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
                        base.WriteShort(Height);
                    }
                }
            }
        }
    }
}
