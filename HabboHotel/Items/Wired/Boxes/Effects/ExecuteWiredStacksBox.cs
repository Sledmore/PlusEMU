using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Plus.Communication.Packets.Incoming;
using Plus.HabboHotel.Rooms;
using Plus.HabboHotel.Users;

namespace Plus.HabboHotel.Items.Wired.Boxes.Effects
{
    class ExecuteWiredStacksBox : IWiredItem
    {
        public Room Instance { get; set; }

        public Item Item { get; set; }

        public WiredBoxType Type { get { return WiredBoxType.EffectExecuteWiredStacks; } }

        public ConcurrentDictionary<int, Item> SetItems { get; set; }

        public string StringData { get; set; }

        public bool BoolData { get; set; }

        public string ItemsData { get; set; }

        public ExecuteWiredStacksBox(Room Instance, Item Item)
        {
            this.Instance = Instance;
            this.Item = Item;
            this.SetItems = new ConcurrentDictionary<int, Item>();
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
                if (SelectedItem != null)
                    SetItems.TryAdd(SelectedItem.Id, SelectedItem);
            }
        }

        public bool Execute(params object[] Params)
        {
            if (Params.Length != 1)
                return false;

            Habbo Player = (Habbo)Params[0];
            if (Player == null)
                return false;

            foreach (Item Item in this.SetItems.Values.ToList())
            {
                if (Item == null || !Instance.GetRoomItemHandler().GetFloor.Contains(Item) || !Item.IsWired)
                    continue;

                IWiredItem WiredItem;
                if(Instance.GetWired().TryGet(Item.Id, out WiredItem))
                {
                    if (WiredItem.Type == WiredBoxType.EffectExecuteWiredStacks)
                        continue;
                    else
                    {
                       ICollection<IWiredItem> Effects = Instance.GetWired().GetEffects(WiredItem);
                       if (Effects.Count > 0)
                       {
                           foreach (IWiredItem EffectItem in Effects.ToList())
                           {
                               if (SetItems.ContainsKey(EffectItem.Item.Id) && EffectItem.Item.Id != Item.Id)
                                   continue;
                               else if (EffectItem.Type == WiredBoxType.EffectExecuteWiredStacks)
                                   continue;
                               else
                                   EffectItem.Execute(Player);
                           }
                       }
                    }
                }
                else continue;
            }

            return true;
        }
    }
}