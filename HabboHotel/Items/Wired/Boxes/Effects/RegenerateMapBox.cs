using System;
using System.Collections.Concurrent;
using Plus.HabboHotel.Rooms;
using Plus.Communication.Packets.Incoming;

namespace Plus.HabboHotel.Items.Wired.Boxes.Effects
{
    internal class RegenerateMapsBox : IWiredItem
    {
        public Room Instance { get; set; }
        public Item Item { get; set; }

        public WiredBoxType Type
        {
            get { return WiredBoxType.EffectRegenerateMaps; }
        }

        public ConcurrentDictionary<int, Item> SetItems { get; set; }
        public string StringData { get; set; }
        public bool BoolData { get; set; }
        public string ItemsData { get; set; }

        public RegenerateMapsBox(Room Instance, Item Item)
        {
            this.Instance = Instance;
            this.Item = Item;
            StringData = "";
            SetItems = new ConcurrentDictionary<int, Item>();
        }

        public void HandleSave(ClientPacket Packet)
        {
            int Unknown = Packet.PopInt();
            string Unknown2 = Packet.PopString();
        }

        public bool Execute(params object[] Params)
        {
            if (Instance == null)
                return false;

            TimeSpan TimeSinceRegen = DateTime.Now - Instance.lastRegeneration;

            if (TimeSinceRegen.TotalMinutes > 1)
            {
                Instance.GetGameMap().GenerateMaps();
                Instance.lastRegeneration = DateTime.Now;
                return true;
            }
            
            return false;
        }
    }
}
