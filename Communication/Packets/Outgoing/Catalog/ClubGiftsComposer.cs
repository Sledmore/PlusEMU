namespace Plus.Communication.Packets.Outgoing.Catalog
{
    class ClubGiftsComposer : ServerPacket
    {
        public ClubGiftsComposer() 
            : base(ServerPacketHeader.ClubGiftsMessageComposer)
        {
            base.WriteInteger(0);//Days until next gift.
            base.WriteInteger(10);//Gifts available
            base.WriteInteger(1);//Count?
            {
                base.WriteInteger(14689);
                base.WriteString("hc_arab_chair");
                base.WriteBoolean(false);
                base.WriteInteger(5);
                base.WriteInteger(0);
                base.WriteInteger(0);
                base.WriteBoolean(true);
                base.WriteInteger(1);//Count for some reason
                {
                    base.WriteString("s");
                    base.WriteInteger(6341);
                    base.WriteString("");
                    base.WriteInteger(1);
                    base.WriteBoolean(false);
                }
                base.WriteInteger(0);
                base.WriteBoolean(false);
                base.WriteBoolean(false);// TODO: Figure out
                base.WriteString("");//previewImage -> e.g; catalogue/pet_lion.png
            }

            base.WriteInteger(1);//Count
            {
                //int, bool, int, bool
                base.WriteInteger(14689);//Maybe the item id?
                base.WriteBoolean(true);//Can we get?
                base.WriteInteger(-1);//idk
                base.WriteBoolean(true);//idk
            }
        }
    }
}
