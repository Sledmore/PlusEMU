using System;
using System.Linq;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Concurrent;

using Plus.HabboHotel.Rooms;
using Plus.HabboHotel.Users;
using Plus.Communication.Packets.Incoming;

namespace Plus.HabboHotel.Items.Wired.Boxes.Effects
{
    class TeleportBotToFurniBox : IWiredItem
    {
        public Room Instance { get; set; }
        public Item Item { get; set; }
        public WiredBoxType Type { get { return WiredBoxType.EffectTeleportBotToFurniBox; } }
        public ConcurrentDictionary<int, Item> SetItems { get; set; }
        public string StringData { get; set; }
        public bool BoolData { get; set; }
        public string ItemsData { get; set; }

        public TeleportBotToFurniBox(Room Instance, Item Item)
        {
            this.Instance = Instance;
            this.Item = Item;
            this.SetItems = new ConcurrentDictionary<int, Item>();
        }

        public void HandleSave(ClientPacket Packet)
        {
            int Unknown = Packet.PopInt();
            string BotName = Packet.PopString();

            if (this.SetItems.Count > 0)
                this.SetItems.Clear();

            int FurniCount = Packet.PopInt();
            for (int i = 0; i < FurniCount; i++)
            {
                Item SelectedItem = Instance.GetRoomItemHandler().GetItem(Packet.PopInt());
                if (SelectedItem != null)
                    SetItems.TryAdd(SelectedItem.Id, SelectedItem);
            }

            this.StringData = BotName;
        }

        public bool Execute(params object[] Params)
        {
            if (String.IsNullOrEmpty(this.StringData))
                return false;

            RoomUser User = this.Instance.GetRoomUserManager().GetBotByName(this.StringData);
            if (User == null)
                return false;

            Random rand = new Random();
            List<Item> Items = SetItems.Values.ToList();
            Items = Items.OrderBy(x => rand.Next()).ToList();

            if (Items.Count == 0)
                return false;

            Item Item = Items.First();
            if (Item == null)
                return false;

            if (!Instance.GetRoomItemHandler().GetFloor.Contains(Item))
            {
                SetItems.TryRemove(Item.Id, out Item);

                if (Items.Contains(Item))
                    Items.Remove(Item);

                if (SetItems.Count == 0 || Items.Count == 0)
                    return false;

                Item = Items.First();
                if (Item == null)
                    return false;
            }

            if (this.Instance.GetGameMap() == null)
                return false;

            this.Instance.GetGameMap().TeleportToItem(User, Item);
            this.Instance.GetRoomUserManager().UpdateUserStatusses();


            return true;
        }
    }
}