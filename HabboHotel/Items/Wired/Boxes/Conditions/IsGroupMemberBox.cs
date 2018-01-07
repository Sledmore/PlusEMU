using System.Collections.Concurrent;

using Plus.Communication.Packets.Incoming;
using Plus.HabboHotel.Rooms;
using Plus.HabboHotel.Users;

namespace Plus.HabboHotel.Items.Wired.Boxes.Conditions
{
    class IsGroupMemberBox : IWiredItem
    {
        public Room Instance { get; set; }
        public Item Item { get; set; }
        public WiredBoxType Type { get { return WiredBoxType.ConditionIsGroupMember; } }
        public ConcurrentDictionary<int, Item> SetItems { get; set; }
        public string StringData { get; set; }
        public bool BoolData { get; set; }
        public string ItemsData { get; set; }

        public IsGroupMemberBox(Room Instance, Item Item)
        {
            this.Instance = Instance;
            this.Item = Item;
            this.SetItems = new ConcurrentDictionary<int, Item>();
        }

        public void HandleSave(ClientPacket Packet)
        {
            int Unknown = Packet.PopInt();
            string Unknown2 = Packet.PopString();
        }

        public bool Execute(params object[] Params)
        {
            if (Params.Length == 0)
                return false;

            Habbo Player = (Habbo)Params[0];
            if (Player == null)
                return false;

            if (Instance.Group == null)
                return false;

            if (!Instance.Group.IsMember(Player.Id))
                return false;

            return true;
        }
    }
}