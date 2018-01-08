using Plus.Communication.Encryption;
using Plus.Communication.Encryption.Crypto.Prng;
using Plus.Communication.Packets.Outgoing.Handshake;
using Plus.HabboHotel.GameClients;

namespace Plus.Communication.Packets.Incoming.Handshake
{
    public class GenerateSecretKeyEvent : IPacketEvent
    {
        public void Parse(GameClient session, ClientPacket packet)
        {
            string cipherPublickey = packet.PopString();
           
            BigInteger sharedKey = HabboEncryptionV2.CalculateDiffieHellmanSharedKey(cipherPublickey);
            if (sharedKey != 0)
            {
                session.Rc4Client = new ARC4(sharedKey.getBytes());
                session.SendPacket(new SecretKeyComposer(HabboEncryptionV2.GetRsaDiffieHellmanPublicKey()));
            }
            else 
            {
                session.SendNotification("There was an error logging you in, please try again!");
            }
        }
    }
}