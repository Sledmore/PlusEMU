using System;
using System.Collections.Generic;
using System.Linq;
using Plus.HabboHotel.Rooms;
using Plus.HabboHotel.Items;
using Plus.Communication.Packets.Outgoing.Rooms.Notifications;
using Plus.HabboHotel.GameClients;
using Plus.HabboHotel.Items.Data.Moodlight;
using Plus.HabboHotel.Items.Data.Toner;

namespace Plus.Communication.Packets.Incoming.Rooms.Engine
{
    class PlaceObjectEvent : IPacketEvent
    {
        public void Parse(GameClient session, ClientPacket packet)
        {
            if (session == null || session.GetHabbo() == null || !session.GetHabbo().InRoom)
                return;

            if (!PlusEnvironment.GetGame().GetRoomManager().TryGetRoom(session.GetHabbo().CurrentRoomId, out Room room))
                return;

            string rawData = packet.PopString();
            var data = rawData.Split(' ');

            if (!int.TryParse(data[0], out int itemId))
                return;

            bool hasRights = room.CheckRights(session, false, true);

            if (!hasRights)
            {
                session.SendPacket(new RoomNotificationComposer("furni_placement_error", "message", "${room.error.cant_set_not_owner}"));
                return;
            }

            Item item = session.GetHabbo().GetInventoryComponent().GetItem(itemId);
            if (item == null)
                return;

            if (room.GetRoomItemHandler().GetWallAndFloor.Count() > Convert.ToInt32(PlusEnvironment.GetSettingsManager().TryGetValue("room.item.placement_limit")))
            {
                session.SendNotification("You cannot have more than " + Convert.ToInt32(PlusEnvironment.GetSettingsManager().TryGetValue("room.item.placement_limit")) + " items in a room!");
                return;
            }

            if (item.Data.InteractionType == InteractionType.EXCHANGE && room.OwnerId != session.GetHabbo().Id && !session.GetHabbo().GetPermissions().HasRight("room_item_place_exchange_anywhere"))
            {
                session.SendNotification("You cannot place exchange items in other people's rooms!");
                return;
            }

            //TODO: Make neat.
            switch (item.GetBaseItem().InteractionType)
            {
                #region Interaction Types
                case InteractionType.MOODLIGHT:
                    {
                        MoodlightData moodData = room.MoodlightData;
                        if (moodData != null && room.GetRoomItemHandler().GetItem(moodData.ItemId) != null)
                        {
                            session.SendNotification("You can only have one background moodlight per room!");
                            return;
                        }
                        break;
                    }
                case InteractionType.TONER:
                    {
                        TonerData tonerData = room.TonerData;
                        if (tonerData != null && room.GetRoomItemHandler().GetItem(tonerData.ItemId) != null)
                        {
                            session.SendNotification("You can only have one background toner per room!");
                            return;
                        }
                        break;
                    }
                case InteractionType.HOPPER:
                    {
                        if (room.GetRoomItemHandler().HopperCount > 0)
                        {
                            session.SendNotification("You can only have one hopper per room!");
                            return;
                        }
                        break;
                    }

                case InteractionType.TENT:
                case InteractionType.TENT_SMALL:
                    {
                        room.AddTent(item.Id);
                        break;
                    }
                #endregion
            }

            if (!item.IsWallItem)
            {
                if (data.Length < 4)
                    return;

                if (!int.TryParse(data[1], out int x)) { return; }
                if (!int.TryParse(data[2], out int y)) { return; }
                if (!int.TryParse(data[3], out int rotation)) { return; }

                Item roomItem = new Item(item.Id, room.RoomId, item.BaseItem, item.ExtraData, x, y, 0, rotation, session.GetHabbo().Id, item.GroupId, item.LimitedNo, item.LimitedTot, string.Empty, room);
                if (room.GetRoomItemHandler().SetFloorItem(session, roomItem, x, y, rotation, true, false, true))
                {
                    session.GetHabbo().GetInventoryComponent().RemoveItem(itemId);

                    if (session.GetHabbo().Id == room.OwnerId)
                        PlusEnvironment.GetGame().GetAchievementManager().ProgressAchievement(session, "ACH_RoomDecoFurniCount", 1);

                    if (roomItem.IsWired)
                    {
                        try { room.GetWired().LoadWiredBox(roomItem); }
                        catch { Console.WriteLine(item.GetBaseItem().InteractionType); }
                    }
                }
                else
                {
                    session.SendPacket(new RoomNotificationComposer("furni_placement_error", "message", "${room.error.cant_set_item}"));
                }
            }
            else if (item.IsWallItem)
            {
                string[] correctedData = new string[data.Length - 1];

                for (int i = 1; i < data.Length; i++)
                {
                    correctedData[i - 1] = data[i];
                }

                if (TrySetWallItem( correctedData, out string wallPos))
                {
                    try
                    {
                        Item roomItem = new Item(item.Id, room.RoomId, item.BaseItem, item.ExtraData, 0, 0, 0, 0, session.GetHabbo().Id, item.GroupId, item.LimitedNo, item.LimitedTot, wallPos, room);

                        if (room.GetRoomItemHandler().SetWallItem(session, roomItem))
                        {
                            session.GetHabbo().GetInventoryComponent().RemoveItem(itemId);
                            if (session.GetHabbo().Id == room.OwnerId)
                                PlusEnvironment.GetGame().GetAchievementManager().ProgressAchievement(session, "ACH_RoomDecoFurniCount", 1);
                        }
                    }
                    catch
                    {
                        session.SendPacket(new RoomNotificationComposer("furni_placement_error", "message", "${room.error.cant_set_item}"));
                    }
                }
                else
                {
                    session.SendPacket(new RoomNotificationComposer("furni_placement_error", "message", "${room.error.cant_set_item}"));
                }
            }
        }

        private static bool TrySetWallItem(IReadOnlyList<string> data, out string position)
        {
            if (data.Count != 3 || !data[0].StartsWith(":w=") || !data[1].StartsWith("l=") || (data[2] != "r" && data[2] != "l"))
            {
                position = null;
                return false;
            }

            string wBit = data[0].Substring(3, data[0].Length - 3);
            string lBit = data[1].Substring(2, data[1].Length - 2);

            if (!wBit.Contains(",") || !lBit.Contains(","))
            {
                position = null;
                return false;
            }

            int.TryParse(wBit.Split(',')[0], out int w1);
            int.TryParse(wBit.Split(',')[1], out int w2);
            int.TryParse(lBit.Split(',')[0], out int l1);
            int.TryParse(lBit.Split(',')[1], out int l2);
            //
            //if (!Habbo.HasFuse("super_admin") && (w1 < 0 || w2 < 0 || l1 < 0 || l2 < 0 || w1 > 200 || w2 > 200 || l1 > 200 || l2 > 200))
            //{
            //    position = null;
            //    return false;
            //}



            string wallPos = ":w=" + w1 + "," + w2 + " l=" + l1 + "," + l2 + " " + data[2];

            position = WallPositionCheck(wallPos);

            return (position != null);
        }

        private static string WallPositionCheck(string wallPosition)
        {
            //:w=3,2 l=9,63 l
            try
            {
                if (wallPosition.Contains(Convert.ToChar(13)))
                {
                    return null;
                }
                if (wallPosition.Contains(Convert.ToChar(9)))
                {
                    return null;
                }

                string[] posD = wallPosition.Split(' ');
                if (posD[2] != "l" && posD[2] != "r")
                    return null;

                string[] widD = posD[0].Substring(3).Split(',');
                int widthX = int.Parse(widD[0]);
                int widthY = int.Parse(widD[1]);
                if (widthX < -1000 || widthY < -1 || widthX > 700 || widthY > 700)
                    return null;

                string[] lenD = posD[1].Substring(2).Split(',');
                int lengthX = int.Parse(lenD[0]);
                int lengthY = int.Parse(lenD[1]);
                if (lengthX < -1 || lengthY < -1000 || lengthX > 700 || lengthY > 700)
                    return null;

                return ":w=" + widthX + "," + widthY + " " + "l=" + lengthX + "," + lengthY + " " + posD[2];
            }
            catch
            {
                return null;
            }
        }
    }
}