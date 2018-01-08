using System;
using System.Linq;
using System.Drawing;
using System.Collections.Generic;
using System.Collections.Concurrent;

using Plus.Utilities;
using Plus.Core;

using Plus.HabboHotel.Items;
using Plus.HabboHotel.GameClients;
using Plus.HabboHotel.Items.Wired;
using Plus.HabboHotel.Items.Data.Toner;
using Plus.HabboHotel.Items.Data.Moodlight;

using Plus.Communication.Packets.Outgoing.Rooms.Engine;

using Plus.Communication.Packets.Outgoing.Inventory.Furni;
using Plus.Communication.Packets.Outgoing;
using Plus.Database.Interfaces;
using Plus.HabboHotel.Rooms.PathFinding;

namespace Plus.HabboHotel.Rooms
{
    public class RoomItemHandling
    {
        private Room _room;

        public int HopperCount;
        private bool mGotRollers;
        private int mRollerSpeed;
        private int mRollerCycle;

        private ConcurrentDictionary<int, Item> _movedItems;

        private ConcurrentDictionary<int, Item> _rollers;
        private ConcurrentDictionary<int, Item> _wallItems = null;
        private ConcurrentDictionary<int, Item> _floorItems = null;

        private readonly List<int> rollerItemsMoved;
        private readonly List<int> rollerUsersMoved;
        private readonly List<ServerPacket> rollerMessages;

        private ConcurrentQueue<Item> _roomItemUpdateQueue;

        public RoomItemHandling(Room room)
        {
            _room = room;

            HopperCount = 0;
            mGotRollers = false;
            mRollerSpeed = 4;
            mRollerCycle = 0;

            _movedItems = new ConcurrentDictionary<int, Item>();

            _rollers = new ConcurrentDictionary<int, Item>();
            _wallItems = new ConcurrentDictionary<int, Item>();
            _floorItems = new ConcurrentDictionary<int, Item>();

            rollerItemsMoved = new List<int>();
            rollerUsersMoved = new List<int>();
            rollerMessages = new List<ServerPacket>();

            _roomItemUpdateQueue = new ConcurrentQueue<Item>();
        }

        public void TryAddRoller(int ItemId, Item Roller)
        {
            _rollers.TryAdd(ItemId, Roller);
        }

        public bool GotRollers
        {
            get { return mGotRollers; }
            set { mGotRollers = value; }
        }

        public void QueueRoomItemUpdate(Item item)
        {
            _roomItemUpdateQueue.Enqueue(item);
        }

        public void SetSpeed(int p)
        {
            mRollerSpeed = p;
        }

        public string WallPositionCheck(string wallPosition)
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

        public void LoadFurniture()
        {
            if (_floorItems.Count > 0)
                _floorItems.Clear();
            if (_wallItems.Count > 0)
                _wallItems.Clear();

            List<Item> Items = ItemLoader.GetItemsForRoom(_room.Id, _room);
            foreach (Item Item in Items.ToList())
            {
                if (Item == null)
                    continue;

                if (Item.UserID == 0)
                {
                    using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                    {
                        dbClient.SetQuery("UPDATE `items` SET `user_id` = @UserId WHERE `id` = @ItemId LIMIT 1");
                        dbClient.AddParameter("ItemId", Item.Id);
                        dbClient.AddParameter("UserId", _room.OwnerId);
                        dbClient.RunQuery();
                    }
                }

                if (Item.IsFloorItem)
                {
                    if (!_room.GetGameMap().ValidTile(Item.GetX, Item.GetY))
                    {
                        using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                        {
                            dbClient.RunQuery("UPDATE `items` SET `room_id` = '0' WHERE `id` = '" + Item.Id + "' LIMIT 1");
                        }

                        GameClient Client = PlusEnvironment.GetGame().GetClientManager().GetClientByUserId(Item.UserID);
                        if (Client != null)
                        {
                            Client.GetHabbo().GetInventoryComponent().AddNewItem(Item.Id, Item.BaseItem, Item.ExtraData, Item.GroupId, true, true, Item.LimitedNo, Item.LimitedTot);
                            Client.GetHabbo().GetInventoryComponent().UpdateItems(false);
                        }
                        continue;
                    }

                    if (!_floorItems.ContainsKey(Item.Id))
                        _floorItems.TryAdd(Item.Id, Item);
                }
                else if (Item.IsWallItem)
                {
                    if (string.IsNullOrWhiteSpace(Item.wallCoord))
                    {
                        using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                        {
                            dbClient.SetQuery("UPDATE `items` SET `wall_pos` = @WallPosition WHERE `id` = '" + Item.Id + "' LIMIT 1");
                            dbClient.AddParameter("WallPosition", ":w=0,2 l=11,53 l");
                            dbClient.RunQuery();
                        }

                        Item.wallCoord = ":w=0,2 l=11,53 l";
                    }

                    try
                    {
                        Item.wallCoord = WallPositionCheck(":" + Item.wallCoord.Split(':')[1]);
                    }
                    catch
                    {
                        using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                        {
                            dbClient.SetQuery("UPDATE `items` SET `wall_pos` = @WallPosition WHERE `id` = '" + Item.Id + "' LIMIT 1");
                            dbClient.AddParameter("WallPosition", ":w=0,2 l=11,53 l");
                            dbClient.RunQuery();
                        }

                        Item.wallCoord = ":w=0,2 l=11,53 l";
                    }

                    if (!_wallItems.ContainsKey(Item.Id))
                        _wallItems.TryAdd(Item.Id, Item);
                }
            }

            foreach (Item Item in _floorItems.Values.ToList())
            {
                if (Item.IsRoller)
                {
                    mGotRollers = true;
                }
                else if (Item.GetBaseItem().InteractionType == InteractionType.MOODLIGHT)
                {
                    if (_room.MoodlightData == null)
                        _room.MoodlightData = new MoodlightData(Item.Id);
                }
                else if (Item.GetBaseItem().InteractionType == InteractionType.TONER)
                {
                    if (_room.TonerData == null)
                        _room.TonerData = new TonerData(Item.Id);
                }
                else if (Item.IsWired)
                {
                    if (_room == null)
                        continue;

                    if (_room.GetWired() == null)
                        continue;

                    _room.GetWired().LoadWiredBox(Item);
                }
                else if (Item.GetBaseItem().InteractionType == InteractionType.HOPPER)
                    HopperCount++;
            }
        }

        public Item GetItem(int pId)
        {
            if (_floorItems != null && _floorItems.ContainsKey(pId))
            {
                Item Item = null;
                if (_floorItems.TryGetValue(pId, out Item))
                    return Item;
            }
            else if (_wallItems != null && _wallItems.ContainsKey(pId))
            {
                Item Item = null;
                if (_wallItems.TryGetValue(pId, out Item))
                    return Item;
            }

            return null;
        }

        public void RemoveFurniture(GameClient session, int id)
        {
            Item Item = GetItem(id);
            if (Item == null)
                return;

            if (Item.GetBaseItem().InteractionType == InteractionType.FOOTBALL_GATE)
                _room.GetSoccer().UnRegisterGate(Item);

            if (Item.GetBaseItem().InteractionType != InteractionType.GIFT)
                Item.Interactor.OnRemove(session, Item);

            if (Item.GetBaseItem().InteractionType == InteractionType.GUILD_GATE)
            {
                Item.UpdateCounter = 0;
                Item.UpdateNeeded = false;
            }

            RemoveRoomItem(Item);
        }

        private void RemoveRoomItem(Item Item)
        {
            if (Item.IsFloorItem)
                _room.SendPacket(new ObjectRemoveComposer(Item, Item.UserID));
            else if (Item.IsWallItem)
                _room.SendPacket(new ItemRemoveComposer(Item, Item.UserID));

            //TODO: Recode this specific part
            if (Item.IsWallItem)
                _wallItems.TryRemove(Item.Id, out Item);
            else
            {
                _floorItems.TryRemove(Item.Id, out Item);
                //mFloorItems.OnCycle();
                _room.GetGameMap().RemoveFromMap(Item);
            }

            RemoveItem(Item);
            _room.GetGameMap().GenerateMaps();
            _room.GetRoomUserManager().UpdateUserStatusses();
        }

        private List<ServerPacket> CycleRollers()
        {
            if (!mGotRollers)
                return new List<ServerPacket>();

            if (mRollerCycle >= mRollerSpeed || mRollerSpeed == 0)
            {
                rollerItemsMoved.Clear();
                rollerUsersMoved.Clear();
                rollerMessages.Clear();

                List<Item> ItemsOnRoller;
                List<Item> ItemsOnNext;

                foreach (Item Roller in _rollers.Values.ToList())
                {
                    if (Roller == null)
                        continue;

                    Point NextSquare = Roller.SquareInFront;

                    ItemsOnRoller = _room.GetGameMap().GetRoomItemForSquare(Roller.GetX, Roller.GetY, Roller.GetZ);
                    ItemsOnNext = _room.GetGameMap().GetAllRoomItemForSquare(NextSquare.X, NextSquare.Y).ToList();

                    if (ItemsOnRoller.Count > 10)
                        ItemsOnRoller = _room.GetGameMap().GetRoomItemForSquare(Roller.GetX, Roller.GetY, Roller.GetZ).Take(10).ToList();

                    bool NextSquareIsRoller = (ItemsOnNext.Count(x => x.GetBaseItem().InteractionType == InteractionType.ROLLER) > 0);
                    bool NextRollerClear = true;

                    double NextZ = 0.0;
                    bool NextRoller = false;

                    foreach (Item Item in ItemsOnNext.ToList())
                    {
                        if (Item.IsRoller)
                        {
                            if (Item.TotalHeight > NextZ)
                                NextZ = Item.TotalHeight;

                            NextRoller = true;
                        }
                    }

                    if (NextRoller)
                    {
                        foreach (Item Item in ItemsOnNext.ToList())
                        {
                            if (Item.TotalHeight > NextZ)
                                NextRollerClear = false;
                        }
                    }

                    if (ItemsOnRoller.Count > 0)
                    {
                        foreach (Item rItem in ItemsOnRoller.ToList())
                        {
                            if (rItem == null)
                                continue;

                            if (!rollerItemsMoved.Contains(rItem.Id) && _room.GetGameMap().CanRollItemHere(NextSquare.X, NextSquare.Y) && NextRollerClear && Roller.GetZ < rItem.GetZ && _room.GetRoomUserManager().GetUserForSquare(NextSquare.X, NextSquare.Y) == null)
                            {
                                if (!NextSquareIsRoller)
                                    NextZ = rItem.GetZ - Roller.GetBaseItem().Height;
                                else
                                    NextZ = rItem.GetZ;

                                rollerMessages.Add(UpdateItemOnRoller(rItem, NextSquare, Roller.Id, NextZ));
                                rollerItemsMoved.Add(rItem.Id);
                            }
                        }
                    }

                    RoomUser RollerUser = _room.GetGameMap().GetRoomUsers(Roller.Coordinate).FirstOrDefault();

                    if (RollerUser != null && !RollerUser.IsWalking && NextRollerClear && _room.GetGameMap().IsValidStep(new Vector2D(Roller.GetX, Roller.GetY), new Vector2D(NextSquare.X, NextSquare.Y), true, false, true) && _room.GetGameMap().CanRollItemHere(NextSquare.X, NextSquare.Y) && _room.GetGameMap().GetFloorStatus(NextSquare) != 0)
                    {
                        if (!rollerUsersMoved.Contains(RollerUser.HabboId))
                        {
                            if (!NextSquareIsRoller)
                                NextZ = RollerUser.Z - Roller.GetBaseItem().Height;
                            else
                                NextZ = RollerUser.Z;

                            RollerUser.isRolling = true;
                            RollerUser.rollerDelay = 1;

                            rollerMessages.Add(UpdateUserOnRoller(RollerUser, NextSquare, Roller.Id, NextZ));
                            rollerUsersMoved.Add(RollerUser.HabboId);
                        }
                    }
                }

                mRollerCycle = 0;
                return rollerMessages;
            }
            else
                mRollerCycle++;

            return new List<ServerPacket>();
        }

        public ServerPacket UpdateItemOnRoller(Item pItem, Point NextCoord, int pRolledID, Double NextZ)
        {
            var mMessage = new ServerPacket(ServerPacketHeader.SlideObjectBundleMessageComposer);
            mMessage.WriteInteger(pItem.GetX);
            mMessage.WriteInteger(pItem.GetY);

            mMessage.WriteInteger(NextCoord.X);
            mMessage.WriteInteger(NextCoord.Y);

            mMessage.WriteInteger(1);

            mMessage.WriteInteger(pItem.Id);

            mMessage.WriteString(TextHandling.GetString(pItem.GetZ));
            mMessage.WriteString(TextHandling.GetString(NextZ));

            mMessage.WriteInteger(pRolledID);

            SetFloorItem(pItem, NextCoord.X, NextCoord.Y, NextZ);

            return mMessage;
        }

        public ServerPacket UpdateUserOnRoller(RoomUser pUser, Point pNextCoord, int pRollerID, Double NextZ)
        {
            var mMessage = new ServerPacket(ServerPacketHeader.SlideObjectBundleMessageComposer);
            mMessage.WriteInteger(pUser.X);
            mMessage.WriteInteger(pUser.Y);

            mMessage.WriteInteger(pNextCoord.X);
            mMessage.WriteInteger(pNextCoord.Y);

            mMessage.WriteInteger(0);
            mMessage.WriteInteger(pRollerID);
            mMessage.WriteInteger(2);
            mMessage.WriteInteger(pUser.VirtualId);
            mMessage.WriteString(TextHandling.GetString(pUser.Z));
            mMessage.WriteString(TextHandling.GetString(NextZ));

            _room.GetGameMap().UpdateUserMovement(new Point(pUser.X, pUser.Y), new Point(pNextCoord.X, pNextCoord.Y), pUser);
            _room.GetGameMap().GameMap[pUser.X, pUser.Y] = 1;
            pUser.X = pNextCoord.X;
            pUser.Y = pNextCoord.Y;
            pUser.Z = NextZ;

            _room.GetGameMap().GameMap[pUser.X, pUser.Y] = 0;

            if (pUser != null && pUser.GetClient() != null && pUser.GetClient().GetHabbo() != null)
            {
                List<Item> Items = _room.GetGameMap().GetRoomItemForSquare(pNextCoord.X, pNextCoord.Y);
                foreach (Item IItem in Items.ToList())
                {
                    if (IItem == null)
                        continue;

                    _room.GetWired().TriggerEvent(WiredBoxType.TriggerWalkOnFurni, pUser.GetClient().GetHabbo(), IItem);
                }

                Item Item = _room.GetRoomItemHandler().GetItem(pRollerID);
                if (Item != null)
                {
                    _room.GetWired().TriggerEvent(WiredBoxType.TriggerWalkOffFurni, pUser.GetClient().GetHabbo(), Item);
                }
            }

            return mMessage;
        }

        private void SaveFurniture()
        {
            try
            {
                if (_movedItems.Count > 0)
                {
                    // TODO: Big string builder?
                    using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                    {
                        foreach (Item Item in _movedItems.Values.ToList())
                        {
                            if (!string.IsNullOrEmpty(Item.ExtraData))
                            {
                                dbClient.SetQuery("UPDATE `items` SET `extra_data` = @edata" + Item.Id + " WHERE `id` = '" + Item.Id + "' LIMIT 1");
                                dbClient.AddParameter("edata" + Item.Id, Item.ExtraData);
                                dbClient.RunQuery();
                            }

                            if (Item.IsWallItem && (!Item.GetBaseItem().ItemName.Contains("wallpaper_single") || !Item.GetBaseItem().ItemName.Contains("floor_single") || !Item.GetBaseItem().ItemName.Contains("landscape_single")))
                            {
                                dbClient.SetQuery("UPDATE `items` SET `wall_pos` = @wallPos WHERE `id` = '" + Item.Id + "' LIMIT 1");
                                dbClient.AddParameter("wallPos", Item.wallCoord);
                                dbClient.RunQuery();
                            }

                            dbClient.RunQuery("UPDATE `items` SET `x` = '" + Item.GetX + "', `y` = '" + Item.GetY + "', `z` = '" + Item.GetZ + "', `rot` = '" + Item.Rotation + "' WHERE `id` = '" + Item.Id + "' LIMIT 1");
                        }
                    }
                }
            }
            catch (Exception e)
            {
                ExceptionLogger.LogCriticalException(e);
            }
        }

        public bool SetFloorItem(GameClient Session, Item Item, int newX, int newY, int newRot, bool newItem, bool OnRoller, bool sendMessage, bool updateRoomUserStatuses = false, double height = -1)
        {
            bool NeedsReAdd = false;

            if (newItem)
            {
                if (Item.IsWired)
                {
                    if (Item.GetBaseItem().WiredType == WiredBoxType.EffectRegenerateMaps && _room.GetRoomItemHandler().GetFloor.Count(x => x.GetBaseItem().WiredType == WiredBoxType.EffectRegenerateMaps) > 0)
                        return false;
                }
            }

            List<Item> ItemsOnTile = GetFurniObjects(newX, newY);
            if (Item.GetBaseItem().InteractionType == InteractionType.ROLLER && ItemsOnTile.Count(x => x.GetBaseItem().InteractionType == InteractionType.ROLLER && x.Id != Item.Id)> 0)
                return false;

            if (!newItem)
                NeedsReAdd = _room.GetGameMap().RemoveFromMap(Item);

            Dictionary<int, ThreeDCoord> AffectedTiles = Gamemap.GetAffectedTiles(Item.GetBaseItem().Length, Item.GetBaseItem().Width, newX, newY, newRot);

            if (!_room.GetGameMap().ValidTile(newX, newY) || _room.GetGameMap().SquareHasUsers(newX, newY) && !Item.GetBaseItem().IsSeat)
            {
                if (NeedsReAdd)
                    _room.GetGameMap().AddToMap(Item);
                return false;
            }

            foreach (ThreeDCoord Tile in AffectedTiles.Values)
            {
                if (!_room.GetGameMap().ValidTile(Tile.X, Tile.Y) ||
                    (_room.GetGameMap().SquareHasUsers(Tile.X, Tile.Y) && !Item.GetBaseItem().IsSeat))
                {
                    if (NeedsReAdd)
                    {
                        _room.GetGameMap().AddToMap(Item);
                    }
                    return false;
                }
            }

            // Start calculating new Z coordinate
            double newZ = _room.GetGameMap().Model.SqFloorHeight[newX, newY];

            if (height == -1)
            {
                if (!OnRoller)
                {
                    // Make sure this tile is open and there are no users here
                    if (_room.GetGameMap().Model.SqState[newX, newY] != SquareState.Open && !Item.GetBaseItem().IsSeat)
                    {
                        return false;
                    }

                    foreach (ThreeDCoord Tile in AffectedTiles.Values)
                    {
                        if (_room.GetGameMap().Model.SqState[Tile.X, Tile.Y] != SquareState.Open &&
                            !Item.GetBaseItem().IsSeat)
                        {
                            if (NeedsReAdd)
                            {
                                //AddItem(Item);
                                _room.GetGameMap().AddToMap(Item);
                            }
                            return false;
                        }
                    }

                    // And that we have no users
                    if (!Item.GetBaseItem().IsSeat && !Item.IsRoller)
                    {
                        foreach (ThreeDCoord Tile in AffectedTiles.Values)
                        {
                            if (_room.GetGameMap().GetRoomUsers(new Point(Tile.X, Tile.Y)).Count > 0)
                            {
                                if (NeedsReAdd)
                                    _room.GetGameMap().AddToMap(Item);
                                return false;
                            }
                        }
                    }
                }

                // Find affected objects
                var ItemsAffected = new List<Item>();
                var ItemsComplete = new List<Item>();

                foreach (ThreeDCoord Tile in AffectedTiles.Values.ToList())
                {
                    List<Item> Temp = GetFurniObjects(Tile.X, Tile.Y);

                    if (Temp != null)
                    {
                        ItemsAffected.AddRange(Temp);
                    }
                }


                ItemsComplete.AddRange(ItemsOnTile);
                ItemsComplete.AddRange(ItemsAffected);

                if (!OnRoller)
                {
                    // Check for items in the stack that do not allow stacking on top of them
                    foreach (Item I in ItemsComplete.ToList())
                    {
                        if (I == null)
                            continue;

                        if (I.Id == Item.Id)
                            continue;

                        if (I.GetBaseItem() == null)
                            continue;

                        if (!I.GetBaseItem().Stackable)
                        {
                            if (NeedsReAdd)
                            {
                                //AddItem(Item);
                                _room.GetGameMap().AddToMap(Item);
                            }
                            return false;
                        }
                    }
                }

                //if (!Item.IsRoller)
                {
                    // If this is a rotating action, maintain item at current height
                    if (Item.Rotation != newRot && Item.GetX == newX && Item.GetY == newY)
                        newZ = Item.GetZ;

                    // Are there any higher objects in the stack!?
                    foreach (Item i in ItemsComplete.ToList())
                    {
                        if (i == null)
                            continue;
                        if (i.Id == Item.Id)
                            continue;

                        if (i.GetBaseItem().InteractionType == InteractionType.STACKTOOL)
                        {                       
                            newZ = i.GetZ;
                            break;
                        }
                        if (i.TotalHeight > newZ)
                        {
                            newZ = i.TotalHeight;
                        }
                    }
                }

                // Verify the rotation is correct
                if (newRot != 0 && newRot != 2 && newRot != 4 && newRot != 6 && newRot != 8 && !Item.GetBaseItem().ExtraRot)
                    newRot = 0;
            }
            else
                newZ = height;

            Item.Rotation = newRot;
            int oldX = Item.GetX;
            int oldY = Item.GetY;
            Item.SetState(newX, newY, newZ, AffectedTiles);

            if (!OnRoller && Session != null)
                Item.Interactor.OnPlace(Session, Item);


            if (newItem)
            {
                if (_floorItems.ContainsKey(Item.Id))
                {
                    if (Session != null)
                        Session.SendNotification(PlusEnvironment.GetLanguageManager().TryGetValue("room.item.already_placed"));
                    _room.GetGameMap().RemoveFromMap(Item);
                    return true;
                }

                if (Item.IsFloorItem && !_floorItems.ContainsKey(Item.Id))
                    _floorItems.TryAdd(Item.Id, Item);
                else if (Item.IsWallItem && !_wallItems.ContainsKey(Item.Id))
                    _wallItems.TryAdd(Item.Id, Item);

                if (sendMessage)
                    _room.SendPacket(new ObjectAddComposer(Item));
            }
            else
            {
                UpdateItem(Item);
                if (!OnRoller && sendMessage)
                    _room.SendPacket(new ObjectUpdateComposer(Item, _room.OwnerId));
            }
            _room.GetGameMap().AddToMap(Item);

            if (Item.GetBaseItem().IsSeat)
                updateRoomUserStatuses = true;

            if (updateRoomUserStatuses)
                _room.GetRoomUserManager().UpdateUserStatusses();

            if (Item.GetBaseItem().InteractionType == InteractionType.TENT || Item.GetBaseItem().InteractionType == InteractionType.TENT_SMALL)
            {
                _room.RemoveTent(Item.Id);
                _room.AddTent(Item.Id);
            }

            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.RunQuery("UPDATE `items` SET `room_id` = '" + _room.RoomId + "', `x` = '" + Item.GetX + "', `y` = '" + Item.GetY + "', `z` = '" + Item.GetZ + "', `rot` = '" + Item.Rotation + "' WHERE `id` = '" + Item.Id + "' LIMIT 1");
            }
            return true;
        }




        public List<Item> GetFurniObjects(int x, int y)
        {
            return _room.GetGameMap().GetCoordinatedItems(new Point(x, y));
        }

        public bool SetFloorItem(Item item, int newX, int newY, Double newZ)
        {
            if (_room == null)
                return false;

            _room.GetGameMap().RemoveFromMap(item);

            item.SetState(newX, newY, newZ, Gamemap.GetAffectedTiles(item.GetBaseItem().Length, item.GetBaseItem().Width, newX, newY, item.Rotation));
            if (item.GetBaseItem().InteractionType == InteractionType.TONER)
            {
                if (_room.TonerData == null)
                {
                    _room.TonerData = new TonerData(item.Id);
                }
            }
            UpdateItem(item);
            _room.GetGameMap().AddItemToMap(item);
            return true;
        }

        public bool SetWallItem(GameClient session, Item item)
        {
            if (!item.IsWallItem || _wallItems.ContainsKey(item.Id))
                return false;

            if (_floorItems.ContainsKey(item.Id))
            {
                session.SendNotification(PlusEnvironment.GetLanguageManager().TryGetValue("room.item.already_placed"));
                return true;
            }

            item.Interactor.OnPlace(session, item);
            if (item.GetBaseItem().InteractionType == InteractionType.MOODLIGHT)
            {
                if (_room.MoodlightData == null)
                {
                    _room.MoodlightData = new MoodlightData(item.Id);
                    item.ExtraData = _room.MoodlightData.GenerateExtraData();
                }
            }

            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("UPDATE `items` SET `room_id` = '" + _room.RoomId + "', `x` = '" + item.GetX + "', `y` = '" + item.GetY + "', `z` = '" + item.GetZ + "', `rot` = '" + item.Rotation + "', `wall_pos` = @WallPos WHERE `id` = '" + item.Id + "' LIMIT 1");
                dbClient.AddParameter("WallPos", item.wallCoord);
                dbClient.RunQuery();
            }

            _wallItems.TryAdd(item.Id, item);

            _room.SendPacket(new ItemAddComposer(item));

            return true;
        }

        public void UpdateItem(Item item)
        {
            if (item == null)
                return;
            if (!_movedItems.ContainsKey(item.Id))
                _movedItems.TryAdd(item.Id, item);
        }


        public void RemoveItem(Item item)
        {
            if (item == null)
                return;

            if (_movedItems.ContainsKey(item.Id))
                _movedItems.TryRemove(item.Id, out item);
            if (_rollers.ContainsKey(item.Id))
                _rollers.TryRemove(item.Id, out item);
        }

        public void OnCycle()
        {
            if (mGotRollers)
            {
                try
                {
                    _room.SendPacket(CycleRollers());
                }
                catch //(Exception e)
                {
                    // Logging.LogThreadException(e.ToString(), "rollers for room with ID " + room.RoomId);
                    mGotRollers = false;
                }
            }

            if (_roomItemUpdateQueue.Count > 0)
            {
                List<Item> addItems = new List<Item>();
                while (_roomItemUpdateQueue.Count > 0)
                {
                    var item = (Item)null;
                    if (_roomItemUpdateQueue.TryDequeue(out item))
                    {
                        item.ProcessUpdates();

                        if (item.UpdateCounter > 0)
                            addItems.Add(item);
                    }
                }

                foreach (Item item in addItems.ToList())
                {
                    if (item == null)
                        continue;

                    _roomItemUpdateQueue.Enqueue(item);
                }
            }
        }

        public List<Item> RemoveItems(GameClient session)
        {
            List<Item> items = new List<Item>();

            foreach (Item item in GetWallAndFloor.ToList())
            {
                if (item == null || item.UserID != session.GetHabbo().Id)
                    continue;

                if (item.IsFloorItem)
                {
                    _floorItems.TryRemove(item.Id, out Item I);
                    session.GetHabbo().GetInventoryComponent().TryAddFloorItem(item.Id, I);
                    _room.SendPacket(new ObjectRemoveComposer(item, item.UserID));                    
                }
                else if (item.IsWallItem)
                {
                    _wallItems.TryRemove(item.Id, out Item I);
                    session.GetHabbo().GetInventoryComponent().TryAddWallItem(item.Id, I);
                    _room.SendPacket(new ItemRemoveComposer(item, item.UserID));
                }
                
                session.SendPacket(new FurniListAddComposer(item));
            }

            _rollers.Clear();
            return items;
        }

        public ICollection<Item> GetFloor
        {
            get
            {
                return _floorItems.Values;
            }
        }

        public ICollection<Item> GetWall
        {
            get
            {
                return _wallItems.Values;
            }
        }

        public IEnumerable<Item> GetWallAndFloor
        {
            get
            {
                return _floorItems.Values.Concat(_wallItems.Values);
            }
        }


        public bool CheckPosItem(Item Item, int newX, int newY, int newRot)
        {
            try
            {
                Dictionary<int, ThreeDCoord> dictionary = Gamemap.GetAffectedTiles(Item.GetBaseItem().Length, Item.GetBaseItem().Width, newX, newY, newRot);
                if (!_room.GetGameMap().ValidTile(newX, newY))
                    return false;


                foreach (ThreeDCoord coord in dictionary.Values.ToList())
                {
                    if ((_room.GetGameMap().Model.DoorX == coord.X) && (_room.GetGameMap().Model.DoorY == coord.Y))
                        return false;
                }

                if ((_room.GetGameMap().Model.DoorX == newX) && (_room.GetGameMap().Model.DoorY == newY))
                    return false;


                foreach (ThreeDCoord coord in dictionary.Values.ToList())
                {
                    if (!_room.GetGameMap().ValidTile(coord.X, coord.Y))
                        return false;
                }

                double num = _room.GetGameMap().Model.SqFloorHeight[newX, newY];
                if ((((Item.Rotation == newRot) && (Item.GetX == newX)) && (Item.GetY == newY)) && (Item.GetZ != num))
                    return false;

                if (_room.GetGameMap().Model.SqState[newX, newY] != SquareState.Open)
                    return false;

                foreach (ThreeDCoord coord in dictionary.Values.ToList())
                {
                    if (_room.GetGameMap().Model.SqState[coord.X, coord.Y] != SquareState.Open)
                        return false;
                }
                if (!Item.GetBaseItem().IsSeat)
                {
                    if (_room.GetGameMap().SquareHasUsers(newX, newY))
                        return false;

                    foreach (ThreeDCoord coord in dictionary.Values.ToList())
                    {
                        if (_room.GetGameMap().SquareHasUsers(coord.X, coord.Y))
                            return false;
                    }
                }

                List<Item> furniObjects = GetFurniObjects(newX, newY);
                List<Item> collection = new List<Item>();
                List<Item> list3 = new List<Item>();
                foreach (ThreeDCoord coord in dictionary.Values.ToList())
                {
                    List<Item> list4 = GetFurniObjects(coord.X, coord.Y);
                    if (list4 != null)
                        collection.AddRange(list4);

                }

                if (furniObjects == null)
                    furniObjects = new List<Item>();

                list3.AddRange(furniObjects);
                list3.AddRange(collection);
                foreach (Item i in list3.ToList())
                {
                    if ((i.Id != Item.Id) && !i.GetBaseItem().Stackable)
                        return false;
                }
                return true;
            }
            catch
            {
                return false;
            }
        }


        public ICollection<Item> GetRollers()
        {
            return _rollers.Values;
        }

        public void Dispose()
        {
            SaveFurniture();

            foreach (Item Item in GetWallAndFloor.ToList())
            {
                if (Item == null)
                    continue;

                Item.Destroy();
            }

            _movedItems.Clear();
            _rollers.Clear();
            _wallItems.Clear();
            _floorItems.Clear();
            rollerItemsMoved.Clear();
            rollerUsersMoved.Clear();
            rollerMessages.Clear();
            _roomItemUpdateQueue = null;
        }
    }
}
 