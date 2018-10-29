using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Concurrent;

using Plus.HabboHotel.Rooms.AI;
using Plus.HabboHotel.Items;
using Plus.HabboHotel.GameClients;
using Plus.HabboHotel.Users.Inventory.Pets;
using Plus.HabboHotel.Users.Inventory.Bots;
using Plus.Communication.Packets.Outgoing.Inventory.Furni;

using Plus.Database.Interfaces;

namespace Plus.HabboHotel.Users.Inventory
{
    public class InventoryComponent
    {
        private readonly int _userId;
        private GameClient _client;

        private readonly ConcurrentDictionary<int, Bot> _botItems;
        private readonly ConcurrentDictionary<int, Pet> _petsItems;
        private readonly ConcurrentDictionary<int, Item> _floorItems;
        private readonly ConcurrentDictionary<int, Item> _wallItems;

        public InventoryComponent(int userId, GameClient client)
        {
            _client = client;
            _userId = userId;

            _floorItems = new ConcurrentDictionary<int, Item>();
            _wallItems = new ConcurrentDictionary<int, Item>();
            _petsItems = new ConcurrentDictionary<int, Pet>();
            _botItems = new ConcurrentDictionary<int, Bot>();

            Init();
        }

        public void Init()
        {
            if (_floorItems.Count > 0)
                _floorItems.Clear();
            if (_wallItems.Count > 0)
                _wallItems.Clear();
            if (_petsItems.Count > 0)
                _petsItems.Clear();
            if (_botItems.Count > 0)
                _botItems.Clear();

            List<Item> items = ItemLoader.GetItemsForUser(_userId);
            foreach (Item item in items.ToList())
            {
                if (item.IsFloorItem)
                {
                    if (!_floorItems.TryAdd(item.Id, item))
                        continue;
                }
                else if (item.IsWallItem)
                {
                    if (!_wallItems.TryAdd(item.Id, item))
                        continue;
                }
                else
                    continue;
            }

            List<Pet> pets = PetLoader.GetPetsForUser(Convert.ToInt32(_userId));
            foreach (Pet pet in pets)
            {
                if (!_petsItems.TryAdd(pet.PetId, pet))
                {
                    Console.WriteLine("Error whilst loading pet x1: " + pet.PetId);
                }
            }

            List<Bot> bots = BotLoader.GetBotsForUser(Convert.ToInt32(_userId));
            foreach (Bot bot in bots)
            {
                if (!_botItems.TryAdd(bot.Id, bot))
                {
                    Console.WriteLine("Error whilst loading bot x1: " + bot.Id);
                }
            }
        }

        public void ClearItems()
        {
            UpdateItems(true);

            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.RunQuery("DELETE FROM items WHERE room_id='0' AND user_id = " + _userId); //Do join 
            }

            _floorItems.Clear();
            _wallItems.Clear();

            if (_client != null)
                _client.SendPacket(new FurniListUpdateComposer());
        }

        public void SetIdleState()
        {
            if (_botItems != null)
                _botItems.Clear();

            if (_petsItems != null)
                _petsItems.Clear();

            if (_floorItems != null)
                _floorItems.Clear();

            if (_wallItems != null)
                _wallItems.Clear();

            _client = null;
        }

        public void UpdateItems(bool fromDatabase)
        {
            if (fromDatabase)
                Init();

            if (_client != null)
            {
                _client.SendPacket(new FurniListUpdateComposer());
            }
        }

        public Item GetItem(int id)
        {
            if (_floorItems.ContainsKey(id))
                return _floorItems[id];

            return _wallItems.ContainsKey(id) ? _wallItems[id] : null;
        }

        public IEnumerable<Item> GetItems
        {
            get
            {
                return _floorItems.Values.Concat(_wallItems.Values);
            }
        }

        public Item AddNewItem(int id, int baseItem, string extraData, int group, bool toInsert, bool fromRoom, int limitedNumber, int limitedStack)
        {
            if (toInsert)
            {
                if (fromRoom)
                {
                    using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                    {
                        dbClient.RunQuery("UPDATE `items` SET `room_id` = '0', `user_id` = '" + _userId + "' WHERE `id` = '" + id + "' LIMIT 1");
                    }
                }
                else
                {
                    using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                    {
                        if (id > 0)
                            dbClient.RunQuery("INSERT INTO `items` (`id`,`base_item`, `user_id`, `limited_number`, `limited_stack`) VALUES ('" + id + "', '" + baseItem + "', '" + _userId + "', '" + limitedNumber + "', '" + limitedStack + "')");
                        else
                        {
                            dbClient.SetQuery("INSERT INTO `items` (`base_item`, `user_id`, `limited_number`, `limited_stack`) VALUES ('" + baseItem + "', '" + _userId + "', '" + limitedNumber + "', '" + limitedStack + "')");
                            id = Convert.ToInt32(dbClient.InsertQuery());
                        }

                        SendNewItems(Convert.ToInt32(id));

                        if (group > 0)
                            dbClient.RunQuery("INSERT INTO `items_groups` VALUES (" + id + ", " + group + ")");

                        if (!string.IsNullOrEmpty(extraData))
                        {
                            dbClient.SetQuery("UPDATE `items` SET `extra_data` = @extradata WHERE `id` = '" + id + "' LIMIT 1");
                            dbClient.AddParameter("extradata", extraData);
                            dbClient.RunQuery();
                        }
                    }
                }
            }

            Item itemToAdd = new Item(id, 0, baseItem, extraData, 0, 0, 0, 0, _userId, group, limitedNumber, limitedStack, string.Empty);
 
            if (UserHoldsItem(id))
                RemoveItem(id);

            if (itemToAdd.IsWallItem)
                _wallItems.TryAdd(itemToAdd.Id, itemToAdd);
            else
                _floorItems.TryAdd(itemToAdd.Id, itemToAdd);
            return itemToAdd;
        }

        private bool UserHoldsItem(int itemId)
        {
         
            if (_floorItems.ContainsKey(itemId))
                return true;
            if (_wallItems.ContainsKey(itemId))
                return true;
            return false;
        }

        public void RemoveItem(int id)
        {
            if (GetClient() == null)
                return;
            
            if(GetClient().GetHabbo() == null || GetClient().GetHabbo().GetInventoryComponent() == null)
                GetClient().Disconnect();

            if (_floorItems.ContainsKey(id))
            {
                _floorItems.TryRemove(id, out Item _);
            }

            if (_wallItems.ContainsKey(id))
            {
                _wallItems.TryRemove(id, out Item _);
            }

            GetClient().SendPacket(new FurniListRemoveComposer(id));
        }

        private GameClient GetClient()
        {
            return PlusEnvironment.GetGame().GetClientManager().GetClientByUserId(_userId);
        }

        public void SendNewItems(int id)
        {
            _client.SendPacket(new FurniListNotificationComposer(id, 1));
        }

        #region Pet Handling
        public ICollection<Pet> GetPets()
        {
            return _petsItems.Values;
        }

        public bool TryAddPet(Pet pet)
        {
            return _petsItems.TryAdd(pet.PetId, pet);
        }

        public bool TryRemovePet(int petId, out Pet petItem)
        {
            if (_petsItems.ContainsKey(petId))
                return _petsItems.TryRemove(petId, out petItem);
            else
            {
                petItem = null;
                return false;
            }
        }

        public bool TryGetPet(int petId, out Pet pet)
        {
            if (_petsItems.ContainsKey(petId))
                return _petsItems.TryGetValue(petId, out pet);
            else
            {
                pet = null;
                return false;
            }
        }
        #endregion

        #region Bot Handling
        public ICollection<Bot> GetBots()
        {
            return _botItems.Values;
        }

        public bool TryAddBot(Bot bot)
        {
            return _botItems.TryAdd(bot.Id, bot);
        }

        public bool TryRemoveBot(int botId, out Bot bot)
        {
            if (_botItems.ContainsKey(botId))
                return _botItems.TryRemove(botId, out bot);
            else
            {
                bot = null;
                return false;
            }
        }

        public bool TryGetBot(int botId, out Bot bot)
        {
            if (_botItems.ContainsKey(botId))
                return _botItems.TryGetValue(botId, out bot);
            else
            {
                bot = null;
                return false;
            }
        }
        #endregion

        public bool TryAddItem(Item item)
        {
            if (item.Data.Type.ToString().ToLower() == "s")// ItemType.FLOOR)
            {
                return _floorItems.TryAdd(item.Id, item);
            }
            else if (item.Data.Type.ToString().ToLower() == "i")//ItemType.WALL)
            {
                return _wallItems.TryAdd(item.Id, item);
            }
            else
            {
                throw new InvalidOperationException("Item did not match neither floor or wall item");
            }
        }

        public bool TryAddFloorItem(int itemId, Item item)
        {
            return _floorItems.TryAdd(itemId, item);
        }

        public bool TryAddWallItem(int itemId, Item item)
        {
            return _floorItems.TryAdd(itemId, item);
        }

        public ICollection<Item> GetFloorItems()
        {
            return _floorItems.Values;
        }

        public ICollection<Item> GetWallItems()
        {
            return _wallItems.Values;
        }

        public IEnumerable<Item> GetWallAndFloor
        {
            get
            {
                return _floorItems.Values.Concat(_wallItems.Values);
            }
        }
    }
}