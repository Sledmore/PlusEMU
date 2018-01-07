using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Concurrent;

using Plus.Communication.Packets.Incoming;
using Plus.HabboHotel.Rooms;
using System.Drawing;
using Plus.Communication.Packets.Outgoing.Rooms.Engine;


namespace Plus.HabboHotel.Items.Wired.Boxes.Effects
{
    internal class MoveFurniToUserBox : IWiredItem, IWiredCycle
    {
        public Room Instance { get; set; }
        public Item Item { get; set; }

        public WiredBoxType Type
        {
            get { return WiredBoxType.EffectMoveFurniToNearestUser; }
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

        public MoveFurniToUserBox(Room Instance, Item Item)
        {
            this.Instance = Instance;
            this.Item = Item;
            this.SetItems = new ConcurrentDictionary<int, Item>();
            this.TickCount = Delay;
            this.Requested = false;
        }

        public void HandleSave(ClientPacket Packet)
        {
            int Unknown = Packet.PopInt();
            string Unknown2 = Packet.PopString();

            if (this.SetItems.Count > 0)
                this.SetItems.Clear();

            int FurniCount = Packet.PopInt();
            for (int i = 0; i < FurniCount; i++)
            {
                Item SelectedItem = Instance.GetRoomItemHandler().GetItem(Packet.PopInt());

                if (SelectedItem != null && !Instance.GetWired().OtherBoxHasItem(this, SelectedItem.Id))
                    SetItems.TryAdd(SelectedItem.Id, SelectedItem);
            }

            int Delay = Packet.PopInt();
            this.Delay = Delay;
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

                    Point Point = Instance.GetGameMap().GetChaseMovement(Item);

                    Instance.GetWired().OnUserFurniCollision(Instance, Item);

                    if (!Instance.GetGameMap().ItemCanMove(Item, Point))
                        continue;

                    if (Instance.GetGameMap().CanRollItemHere(Point.X, Point.Y) && !Instance.GetGameMap().SquareHasUsers(Point.X, Point.Y))
                    {    
                        Double NewZ = Item.GetZ;
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
    }
}