namespace Plus.Communication.Packets.Outgoing.Catalog
{
    class ClubGiftsComposer : MessageComposer
    {
        public ClubGiftsComposer() 
            : base(ServerPacketHeader.ClubGiftsMessageComposer)
        {
            
        }

        public override void Compose(ServerPacket packet)
        {
            packet.WriteInteger(0);//Days until next gift.
            packet.WriteInteger(10);//Gifts available
            packet.WriteInteger(1);//Count?
            {
                packet.WriteInteger(14689);
                packet.WriteString("hc_arab_chair");
                packet.WriteBoolean(false);
                packet.WriteInteger(5);
                packet.WriteInteger(0);
                packet.WriteInteger(0);
                packet.WriteBoolean(true);
                packet.WriteInteger(1);//Count for some reason
                {
                    packet.WriteString("s");
                    packet.WriteInteger(6341);
                    packet.WriteString("");
                    packet.WriteInteger(1);
                    packet.WriteBoolean(false);
                }
                packet.WriteInteger(0);
                packet.WriteBoolean(false);
                packet.WriteBoolean(false);// TODO: Figure out
                packet.WriteString("");//previewImage -> e.g; catalogue/pet_lion.png
            }

            packet.WriteInteger(1);//Count
            {
                //int, bool, int, bool
                packet.WriteInteger(14689);//Maybe the item id?
                packet.WriteBoolean(true);//Can we get?
                packet.WriteInteger(-1);//idk
                packet.WriteBoolean(true);//idk
            }
        }
    }
}
