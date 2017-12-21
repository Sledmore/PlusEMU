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
        private static readonly ILog log = LogManager.GetLogger("Plus.Communication.GamePacketParser");

        public delegate void HandlePacket(ClientPacket message);

        private readonly GameClient _currentClient;
        private ConnectionInformation _con;

        private bool _halfDataRecieved = false;
        private byte[] _halfData = null;
        private bool _deciphered = false;

        public GamePacketParser(GameClient me)
        {
            _currentClient = me;
        }

        public void HandlePacketData(byte[] data)
        {
            try
            {
                if (this._currentClient.RC4Client != null && !this._deciphered)
                {
                    this._currentClient.RC4Client.Decrypt(ref data);
                    this._deciphered = true;
                }

                if (this._halfDataRecieved)
                {
                    byte[] FullDataRcv = new byte[this._halfData.Length + data.Length];
                    Buffer.BlockCopy(this._halfData, 0, FullDataRcv, 0, this._halfData.Length);
                    Buffer.BlockCopy(data, 0, FullDataRcv, this._halfData.Length, data.Length);

                    this._halfDataRecieved = false; // mark done this round
                    HandlePacketData(FullDataRcv); // repeat now we have the combined array
                    return;
                }

                using (BinaryReader Reader = new BinaryReader(new MemoryStream(data)))
                {
                    if (data.Length < 4)
                        return;

                    int MsgLen = HabboEncoding.DecodeInt32(Reader.ReadBytes(4));
                    if ((Reader.BaseStream.Length - 4) < MsgLen)
                    {
                        this._halfData = data;
                        this._halfDataRecieved = true;
                        return;
                    }
                    else if (MsgLen < 0 || MsgLen > 5120)//TODO: Const somewhere.
                        return;

                    byte[] Packet = Reader.ReadBytes(MsgLen);

                    using (BinaryReader R = new BinaryReader(new MemoryStream(Packet)))
                    {
                        int Header = HabboEncoding.DecodeInt16(R.ReadBytes(2));

                        byte[] Content = new byte[Packet.Length - 2];
                        Buffer.BlockCopy(Packet, 2, Content, 0, Packet.Length - 2);

                        ClientPacket Message = new ClientPacket(Header, Content);
                        onNewPacket.Invoke(Message);
                     
                        this._deciphered = false;
                    }

                    if (Reader.BaseStream.Length - 4 > MsgLen)
                    {
                        byte[] Extra = new byte[Reader.BaseStream.Length - Reader.BaseStream.Position];
                        Buffer.BlockCopy(data, (int)Reader.BaseStream.Position, Extra, 0, (int)(Reader.BaseStream.Length - Reader.BaseStream.Position));

                        this._deciphered = true;
                        HandlePacketData(Extra);
                    }
                }
            }
            catch (Exception e)
            {
                //log.Error("Packet Error!", e);
            }
        }

        public void Dispose()
        {
            onNewPacket = null;
            GC.SuppressFinalize(this);
        }

        public object Clone()
        {
            return new GamePacketParser(_currentClient);
        }

        public event HandlePacket onNewPacket;

        public void SetConnection(ConnectionInformation con)
        {
            _con = con;
            onNewPacket = null;
        }
    }
}