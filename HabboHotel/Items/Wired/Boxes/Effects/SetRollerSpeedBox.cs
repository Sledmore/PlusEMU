using System.Collections.Concurrent;

using Plus.Communication.Packets.Incoming;
using Plus.HabboHotel.Rooms;

namespace Plus.HabboHotel.Items.Wired.Boxes.Effects
{
    class SetRollerSpeedBox: IWiredItem
    {
        public Room Instance { get; set; }
        public Item Item { get; set; }
        public WiredBoxType Type { get { return WiredBoxType.EffectSetRollerSpeed; } }
        public ConcurrentDictionary<int, Item> SetItems { get; set; }
        public string StringData { get; set; }
        public bool BoolData { get; set; }
        public string ItemsData { get; set; }

        public SetRollerSpeedBox(Room Instance, Item Item)
        {
            this.Instance = Instance;
            this.Item = Item;
            SetItems = new ConcurrentDictionary<int, Item>();

            if (SetItems.Count > 0)
                SetItems.Clear();
        }

        public void HandleSave(ClientPacket Packet)
        {
            if (SetItems.Count > 0)
                SetItems.Clear();

            int Unknown = Packet.PopInt();
            string Message = Packet.PopString();

            StringData = Message;

            int Speed;
            if (!int.TryParse(StringData, out Speed))
            {
                StringData = "";
            }
        }

        public bool Execute(params object[] Params)
        {
            int Speed;
            if (int.TryParse(StringData, out Speed))
            {
                Instance.GetRoomItemHandler().SetSpeed(Speed);
            }
            return true;
        }
    }
}