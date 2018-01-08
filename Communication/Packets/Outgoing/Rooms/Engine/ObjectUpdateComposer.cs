using System;
using Plus.Utilities;
using Plus.HabboHotel.Items;


namespace Plus.Communication.Packets.Outgoing.Rooms.Engine
{
     class ObjectUpdateComposer : ServerPacket
    {
         public ObjectUpdateComposer(Item Item, int UserId)
             : base(ServerPacketHeader.ObjectUpdateMessageComposer)
         {
            WriteInteger(Item.Id);
            WriteInteger(Item.GetBaseItem().SpriteId);
            WriteInteger(Item.GetX);
            WriteInteger(Item.GetY);
            WriteInteger(Item.Rotation);
           WriteString(String.Format("{0:0.00}", TextHandling.GetString(Item.GetZ)));
           WriteString(String.Empty);

            if (Item.LimitedNo > 0)
            {
                WriteInteger(1);
                WriteInteger(256);
               WriteString(Item.ExtraData);
                WriteInteger(Item.LimitedNo);
                WriteInteger(Item.LimitedTot);
            }
            else
            {
                ItemBehaviourUtility.GenerateExtradata(Item, this);
            }
          
            WriteInteger(-1); // to-do: check
            WriteInteger((Item.GetBaseItem().Modes > 1) ? 1 : 0);
            WriteInteger(UserId);
        }
    }
}