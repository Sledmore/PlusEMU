using System;
using System.IO;
using log4net;
using Plus.Communication.Packets.Incoming;
using Plus.Communication.ConnectionManager;
using Plus.HabboHotel.GameClients;
using Plus.Utilities;

namespace Plus.Communication
{
    public class GamePacketParser : IDataParser
    {
        private static readonly ILog Log = LogManager.GetLogger("Plus.Communication.GamePacketParser");

        public delegate void HandlePacket(ClientPacket message);

        private readonly GameClient _client;

        private bool _halfDataRecieved;
        private byte[] _halfData;
        private bool _deciphered;

        public GamePacketParser(GameClient client)
        {
            _client = client;
        }

        public void HandlePacketData(byte[] data)
        {
            try
            {
                if (_client.Rc4Client != null && !_deciphered)
                {
                    _client.Rc4Client.Decrypt(ref data);
                    _deciphered = true;
                }

                if (_halfDataRecieved)
                {
                    byte[] fullDataRcv = new byte[_halfData.Length + data.Length];
                    Buffer.BlockCopy(_halfData, 0, fullDataRcv, 0, _halfData.Length);
                    Buffer.BlockCopy(data, 0, fullDataRcv, _halfData.Length, data.Length);

                    _halfDataRecieved = false; // mark done this round
                    HandlePacketData(fullDataRcv); // repeat now we have the combined array
                    return;
                }

                using (BinaryReader reader = new BinaryReader(new MemoryStream(data)))
                {
                    if (data.Length < 4)
                        return;

                    int msgLen = HabboEncoding.DecodeInt32(reader.ReadBytes(4));
                    if ((reader.BaseStream.Length - 4) < msgLen)
                    {
                        _halfData = data;
                        _halfDataRecieved = true;
                        return;
                    }

                    if (msgLen < 0 || msgLen > 5120)//TODO: Const somewhere.
                        return;

                    byte[] packet = reader.ReadBytes(msgLen);

                    using (BinaryReader r = new BinaryReader(new MemoryStream(packet)))
                    {
                        int header = HabboEncoding.DecodeInt16(r.ReadBytes(2));

                        byte[] content = new byte[packet.Length - 2];
                        Buffer.BlockCopy(packet, 2, content, 0, packet.Length - 2);

                        ClientPacket message = new ClientPacket(header, content);
                        OnNewPacket.Invoke(message);
                     
                        _deciphered = false;
                    }

                    if (reader.BaseStream.Length - 4 > msgLen)
                    {
                        byte[] extra = new byte[reader.BaseStream.Length - reader.BaseStream.Position];
                        Buffer.BlockCopy(data, (int)reader.BaseStream.Position, extra, 0, (int)(reader.BaseStream.Length - reader.BaseStream.Position));

                        _deciphered = true;
                        HandlePacketData(extra);
                    }
                }
            }
            catch (Exception)
            {
                //log.Error("Packet Error!", e);
            }
        }

        public void Dispose()
        {
            OnNewPacket = null;
            GC.SuppressFinalize(this);
        }

        public object Clone()
        {
            return new GamePacketParser(_client);
        }

        public event HandlePacket OnNewPacket;

        public void SetConnection(ConnectionInformation con)
        {
            // Connection information passes through, but we seemingly do nothing?
            OnNewPacket = null;
        }
    }
}