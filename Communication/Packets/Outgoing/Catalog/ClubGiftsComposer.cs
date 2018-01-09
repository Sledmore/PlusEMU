namespace Plus.Communication.Packets.Outgoing.Catalog
{
    class ClubGiftsComposer : ServerPacket
    {
        public ClubGiftsComposer() 
            : base(ServerPacketHeader.ClubGiftsMessageComposer)
        {
            WriteInteger(0);//Days until next gift.
            WriteInteger(10);//Gifts available
            WriteInteger(1);//Count?
            {
                WriteInteger(14689);
                WriteString("hc_arab_chair");
                WriteBoolean(false);
                WriteInteger(5);
                WriteInteger(0);
                WriteInteger(0);
                WriteBoolean(true);
                WriteInteger(1);//Count for some reason
                {
                    WriteString("s");
                    WriteInteger(6341);
                    WriteString("");
                    WriteInteger(1);
                    WriteBoolean(false);
                }
                WriteInteger(0);
                WriteBoolean(false);
                WriteBoolean(false);// TODO: Figure out
                WriteString("");//previewImage -> e.g; catalogue/pet_lion.png
            }

            WriteInteger(1);//Count
            {
                //int, bool, int, bool
                WriteInteger(14689);//Maybe the item id?
                WriteBoolean(true);//Can we get?
                WriteInteger(-1);//idk
                WriteBoolean(true);//idk
            }
        }
    }
}
