using System;
using Plus.HabboHotel.Items;
using Plus.HabboHotel.Rooms;
using Plus.Communication.Packets.Outgoing.Rooms.Furni.LoveLocks;
using Plus.Database.Interfaces;
using Plus.HabboHotel.GameClients;

namespace Plus.Communication.Packets.Incoming.Rooms.Furni.LoveLocks
{
    class ConfirmLoveLockEvent : IPacketEvent
    {
        public void Parse(GameClient session, ClientPacket packet)
        {
            int pId = packet.PopInt();
            bool isConfirmed = packet.PopBoolean();

            Room room = session.GetHabbo().CurrentRoom;
            if (room == null)
                return;

            Item item = room.GetRoomItemHandler().GetItem(pId);

            if (item == null || item.GetBaseItem() == null || item.GetBaseItem().InteractionType != InteractionType.LOVELOCK)
                return;

            int userOneId = item.InteractingUser;
            int userTwoId = item.InteractingUser2;

            RoomUser userOne = room.GetRoomUserManager().GetRoomUserByHabbo(userOneId);
            RoomUser userTwo = room.GetRoomUserManager().GetRoomUserByHabbo(userTwoId);

            if(userOne == null && userTwo == null)
            {
                item.InteractingUser = 0;
                item.InteractingUser2 = 0;
                session.SendNotification("Your partner has left the room or has cancelled the love lock.");
                return;
            }

            if(userOne.GetClient() == null || userTwo.GetClient() == null)
            {
                item.InteractingUser = 0;
                item.InteractingUser2 = 0;
                session.SendNotification("Your partner has left the room or has cancelled the love lock.");
                return;
            }

            if(userOne == null)
            {
                userTwo.CanWalk = true;
                userTwo.GetClient().SendNotification("Your partner has left the room or has cancelled the love lock.");
                userTwo.LLPartner = 0;
                item.InteractingUser = 0;
                item.InteractingUser2 = 0;
                return;
            }

            if(userTwo == null)
            {
                userOne.CanWalk = true;
                userOne.GetClient().SendNotification("Your partner has left the room or has cancelled the love lock.");
                userOne.LLPartner = 0;
                item.InteractingUser = 0;
                item.InteractingUser2 = 0;
                return;
            }

            if(item.ExtraData.Contains(Convert.ToChar(5).ToString()))
            {
                userTwo.CanWalk = true;
                userTwo.GetClient().SendNotification("It appears this love lock has already been locked.");
                userTwo.LLPartner = 0;

                userOne.CanWalk = true;
                userOne.GetClient().SendNotification("It appears this love lock has already been locked.");
                userOne.LLPartner = 0;

                item.InteractingUser = 0;
                item.InteractingUser2 = 0;
                return;
            }

            if(!isConfirmed)
            {
                item.InteractingUser = 0;
                item.InteractingUser2 = 0;

                userOne.LLPartner = 0;
                userTwo.LLPartner = 0;

                userOne.CanWalk = true;
                userTwo.CanWalk = true;
                return;
            }

            if(userOneId == session.GetHabbo().Id)
            {
                session.SendPacket(new LoveLockDialogueSetLockedMessageComposer(pId));
                userOne.LLPartner = userTwoId;
            }
            else if(userTwoId == session.GetHabbo().Id)
            {
                session.SendPacket(new LoveLockDialogueSetLockedMessageComposer(pId));
                userTwo.LLPartner = userOneId;
            }

            if (userOne.LLPartner == 0 || userTwo.LLPartner == 0)
                return;
            item.ExtraData = "1" + (char)5 + userOne.GetUsername() + (char)5 + userTwo.GetUsername() + (char)5 + userOne.GetClient().GetHabbo().Look + (char)5 + userTwo.GetClient().GetHabbo().Look + (char)5 + DateTime.Now.ToString("dd/MM/yyyy");

            item.InteractingUser = 0;
            item.InteractingUser2 = 0;

            userOne.LLPartner = 0;
            userTwo.LLPartner = 0;

            item.UpdateState(true, true);

            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("UPDATE `items` SET `extra_data` = @extraData WHERE `id` = @ID LIMIT 1");
                dbClient.AddParameter("extraData", item.ExtraData);
                dbClient.AddParameter("ID", item.Id);
                dbClient.RunQuery();
            }

            userOne.GetClient().SendPacket(new LoveLockDialogueCloseMessageComposer(pId));
            userTwo.GetClient().SendPacket(new LoveLockDialogueCloseMessageComposer(pId));

            userOne.CanWalk = true;
            userTwo.CanWalk = true;

            userOne = null;
            userTwo = null;
        }
    }
}
