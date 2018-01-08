using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Plus.HabboHotel.Rooms;
using Plus.HabboHotel.Users;
using Plus.Communication.Packets.Incoming;

namespace Plus.HabboHotel.Items.Wired.Boxes.Triggers
{
    internal class UserFurniCollision : IWiredItem
    {
        public Room Instance { get; set; }
        public Item Item { get; set; }

        public WiredBoxType Type
        {
            get { return WiredBoxType.TriggerUserFurniCollision; }
        }

        public ConcurrentDictionary<int, Item> SetItems { get; set; }
        public string StringData { get; set; }
        public bool BoolData { get; set; }
        public string ItemsData { get; set; }

        public UserFurniCollision(Room instance, Item item)
        {
            Instance = instance;
            Item = item;
            StringData = "";
            SetItems = new ConcurrentDictionary<int, Item>();
        }

        public void HandleSave(ClientPacket packet)
        {
            int Unknown = packet.PopInt();
            string Unknown2 = packet.PopString();
        }

        public bool Execute(params object[] Params)
        {
            Instance.GetWired().OnEvent(this.Item);

            Habbo Player = (Habbo)Params[0];
            if (Player == null)
                return false;

            Item Item = (Item)Params[1];
            if (Item == null)
                return false;

            ICollection<IWiredItem> Effects = Instance.GetWired().GetEffects(this);
            ICollection<IWiredItem> Conditions = Instance.GetWired().GetConditions(this);

            foreach (IWiredItem Condition in Conditions.ToList())
            {
                if (!Condition.Execute(Player))
                    return false;

                if (Instance != null)
                    Instance.GetWired().OnEvent(Condition.Item);
            }

            //Check the ICollection to find the random addon effect.
            bool HasRandomEffectAddon = Effects.Count(x => x.Type == WiredBoxType.AddonRandomEffect) > 0;
            if (HasRandomEffectAddon)
            {
                //Okay, so we have a random addon effect, now lets get the IWiredItem and attempt to execute it.
                IWiredItem RandomBox = Effects.FirstOrDefault(x => x.Type == WiredBoxType.AddonRandomEffect);
                if (!RandomBox.Execute())
                    return false;

                //Success! Let's get our selected box and continue.
                IWiredItem SelectedBox = Instance.GetWired().GetRandomEffect(Effects.ToList());
                if (!SelectedBox.Execute())
                    return false;

                //Woo! Almost there captain, now lets broadcast the update to the room instance.
                if (Instance != null)
                {
                    Instance.GetWired().OnEvent(RandomBox.Item);
                    Instance.GetWired().OnEvent(SelectedBox.Item);
                }
            }
            else
            {
                foreach (IWiredItem Effect in Effects.ToList())
                {
                    if (!Effect.Execute(Player))
                        return false;

                    if (Instance != null)
                        Instance.GetWired().OnEvent(Effect.Item);
                }
            }

            return true;
        }
    }
}
