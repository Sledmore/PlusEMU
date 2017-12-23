using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.Collections.Concurrent;

using Plus.Communication.Packets.Incoming;
using Plus.HabboHotel.Rooms;
using Plus.HabboHotel.Users;
using System.Drawing;
using System.Security.Cryptography;
using Plus.Communication.Packets.Outgoing.Rooms.Engine;
using Plus.Utilities;


namespace Plus.HabboHotel.Items.Wired.Boxes.Effects
{
    internal class MoveAndRotateBox : IWiredItem, IWiredCycle
    {
        public Room Instance { get; set; }
        public Item Item { get; set; }

        public WiredBoxType Type
        {
            get { return WiredBoxType.EffectMoveAndRotate; }
        }

        public ConcurrentDictionary<int, Item> SetItems { get; set; }
        public string StringData { get; set; }
        public bool BoolData { get; set; }

        public int Delay
        {
            get { return this._delay; }
            set
            {
                this._delay = value;
                this.TickCount = value + 1;
            }
        }

        public int TickCount { get; set; }
        public string ItemsData { get; set; }
        private bool Requested;
        private int _delay = 0;
        private long _next = 0;

        public MoveAndRotateBox(Room Instance, Item Item)
        {
            this.Instance = Instance;
            this.Item = Item;
            this.SetItems = new ConcurrentDictionary<int, Item>();
            this.TickCount = Delay;
            this.Requested = false;
        }

        public void HandleSave(ClientPacket Packet)
        {
            if (this.SetItems.Count > 0)
                this.SetItems.Clear();

            int Unknown = Packet.PopInt();
            int Movement = Packet.PopInt();
            int Rotation = Packet.PopInt();

            string Unknown1 = Packet.PopString();

            int FurniCount = Packet.PopInt();
            for (int i = 0; i < FurniCount; i++)
            {
                Item SelectedItem = Instance.GetRoomItemHandler().GetItem(Packet.PopInt());

                if (SelectedItem != null && !Instance.GetWired().OtherBoxHasItem(this, SelectedItem.Id))
                    SetItems.TryAdd(SelectedItem.Id, SelectedItem);
            }

            this.StringData = Movement + ";" + Rotation;
            this.Delay = Packet.PopInt();
        }

        public bool Execute(params object[] Params)
        {
            if (this.SetItems.Count == 0)
                return false;

            if (this._next == 0 || this._next < PlusEnvironment.Now())
                this._next = PlusEnvironment.Now() + this.Delay;

            if (!Requested)
            {
                this.TickCount = this.Delay;
                this.Requested = true;
            }
            return true;
        }

        public bool OnCycle()
        {
            if (Instance == null || !Requested || _next == 0)
                return false;

            long Now = PlusEnvironment.Now();
            if (_next < Now)
            {
                foreach (Item Item in this.SetItems.Values.ToList())
                {
                    if (Item == null)
                        continue;

                    if (!Instance.GetRoomItemHandler().GetFloor.Contains(Item))
                        continue;

                    Item toRemove = null;

                    if (Instance.GetWired().OtherBoxHasItem(this, Item.Id))
                        this.SetItems.TryRemove(Item.Id, out toRemove);
   

                    Point Point = HandleMovement(Convert.ToInt32(this.StringData.Split(';')[0]),new Point(Item.GetX, Item.GetY));
                    int newRot = HandleRotation(Convert.ToInt32(this.StringData.Split(';')[1]), Item.Rotation);

                    Instance.GetWired().OnUserFurniCollision(Instance, Item);

                    if (!Instance.GetGameMap().ItemCanMove(Item, Point))
                        continue;

                    if (Instance.GetGameMap().CanRollItemHere(Point.X, Point.Y) && !Instance.GetGameMap().SquareHasUsers(Point.X, Point.Y))
                    {
                        double NewZ = Instance.GetGameMap().GetHeightForSquareFromData(Point);
                        bool CanBePlaced = true;

                        List<Item> Items = Instance.GetGameMap().GetCoordinatedItems(Point);
                        foreach (Item IItem in Items.ToList())
                        {
                            if (IItem == null || IItem.Id == Item.Id)
                                continue;

                            if (!IItem.GetBaseItem().Walkable)
                            {
                                _next = 0;
                                CanBePlaced = false;
                                break;
                            }

                            if (IItem.TotalHeight > NewZ)
                                NewZ = IItem.TotalHeight;

                            if (CanBePlaced == true && !IItem.GetBaseItem().Stackable)
                                CanBePlaced = false;
                        }

                        if (newRot != Item.Rotation)
                        {
                            Item.Rotation = newRot;
                            Item.UpdateState(false, true);
                        }

                        if (CanBePlaced && Point != Item.Coordinate)
                        {
                            Instance.SendPacket(new SlideObjectBundleComposer(Item.GetX, Item.GetY, Item.GetZ, Point.X,
                                Point.Y, NewZ, 0, 0, Item.Id));
                            Instance.GetRoomItemHandler().SetFloorItem(Item, Point.X, Point.Y, NewZ);
                        }
                    }
                }

                _next = 0;
                return true;
            }
            return false;
        }

        private int HandleRotation(int mode, int rotation)
        {
            switch (mode)
            {
                case 1:
                {
                    rotation += 2;
                    if (rotation > 6)
                    {
                        rotation = 0;
                    }
                    break;
                }

                case 2:
                {
                    rotation -= 2;
                    if (rotation < 0)
                    {
                        rotation = 6;
                    }
                    break;
                }

                case 3:
                {
                    if (RandomNumber.GenerateRandom(0, 2) == 0)
                    {
                        rotation += 2;
                        if (rotation > 6)
                        {
                            rotation = 0;
                        }
                    }
                    else
                    {
                        rotation -= 2;
                        if (rotation < 0)
                        {
                            rotation = 6;
                        }
                    }
                    break;
                }
            }
            return rotation;
        }

        private Point HandleMovement(int Mode, Point Position)
        {
            Point NewPos = new Point();
            switch (Mode)
            {
                case 0:
                {
                    NewPos = Position;
                    break;
                }
                case 1:
                {
                    switch (RandomNumber.GenerateRandom(1, 4))
                    {
                        case 1:
                            NewPos = new Point(Position.X + 1, Position.Y);
                            break;
                        case 2:
                            NewPos = new Point(Position.X - 1, Position.Y);
                            break;
                        case 3:
                            NewPos = new Point(Position.X, Position.Y + 1);
                            break;
                        case 4:
                            NewPos = new Point(Position.X, Position.Y - 1);
                            break;
                    }
                    break;
                }
                case 2:
                {
                    if (RandomNumber.GenerateRandom(0, 2) == 1)
                    {
                        NewPos = new Point(Position.X - 1, Position.Y);
                    }
                    else
                    {
                        NewPos = new Point(Position.X + 1, Position.Y);
                    }
                    break;
                }
                case 3:
                {
                    if (RandomNumber.GenerateRandom(0, 2) == 1)
                    {
                        NewPos = new Point(Position.X, Position.Y - 1);
                    }
                    else
                    {
                        NewPos = new Point(Position.X, Position.Y + 1);
                    }
                    break;
                }
                case 4:
                {
                    NewPos = new Point(Position.X, Position.Y - 1);
                    break;
                }
                case 5:
                {
                    NewPos = new Point(Position.X + 1, Position.Y);
                    break;
                }
                case 6:
                {
                    NewPos = new Point(Position.X, Position.Y + 1);
                    break;
                }
                case 7:
                {
                    NewPos = new Point(Position.X - 1, Position.Y);
                    break;
                }
            }

            return NewPos;
        }
    }
}