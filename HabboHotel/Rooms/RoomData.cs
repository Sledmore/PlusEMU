using System;
using System.Collections.Generic;
using System.Data;

using Plus.HabboHotel.Groups;
using Plus.Database.Interfaces;

namespace Plus.HabboHotel.Rooms
{
    public class RoomData
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ModelName { get; set; }
        public string OwnerName { get; set; }
        public int OwnerId { get; set; }
        public string Password { get; set; }
        public int Score { get; set; }
        public RoomAccess Access { get; set; }
        public string Type { get; set; }
        public int UsersMax { get; set; }
        public int UsersNow { get; set; }
        public int Category { get; set; }
        public string Description { get; set; }
        public string Floor { get; set; }
        public string Landscape { get; set; }
        public int AllowPets { get; set; }
        public int AllowPetsEating { get; set; }
        public int RoomBlockingEnabled { get; set; }
        public int Hidewall { get; set; }
        public int WallThickness { get; set; }
        public int FloorThickness { get; set; }
        public string Wallpaper { get; set; }
        public int WhoCanMute { get; set; }
        public int WhoCanBan { get; set; }
        public int WhoCanKick { get; set; }
        public int chatMode { get; set; }
        public int ChatSize { get; set; }
        public int ChatSpeed { get; set; }
        public int ExtraFlood { get; set; }
        public int ChatDistance { get; set; }
        public int TradeSettings { get; set; }
        public bool PushEnabled { get; set; }
        public bool PullEnabled { get; set; }
        public bool SuperPushEnabled { get; set; }
        public bool SuperPullEnabled { get; set; }
        public bool EnablesEnabled { get; set; }
        public bool RespectNotificationsEnabled { get; set; }
        public bool PetMorphsAllowed { get; set; }
        public int SalePrice { get; set; }
        public bool ReverseRollers { get; set; }
        public bool LayEnabled { get; set; }

        public List<string> Tags;

        private Group _group;
        private RoomPromotion _promotion;

        private RoomModel _model;


        public RoomModel Model
        {
            get { return this._model; }
            set { this._model = value; }
        }

        public RoomData(int id, string caption, string modelName, string ownerName, int ownerId, string password, int score, string type, string access, int usersNow, int usersMax, int category, string description,
            string tags, string floor, string landscape, int allowPets, int allowPetsEating, int roomBlockingEnabled, int hidewall, int wallThickness, int floorThickness, string wallpaper, int muteSettings,
            int banSettings, int kickSettings, int chatMode, int chatSize, int chatSpeed, int extraFlood, int chatDistance, int tradeSettings, bool pushEnabled, bool pullEnabled, bool superPushEnabled,
            bool superPullEnabled, bool enablesEnabled, bool respectedNotificationsEnabled, bool petMorphsAllowed, int groupId, int salePrice, bool layEnabled, RoomModel model)
        {
            this.Id = id;
            this.Name = caption;
            this.ModelName = modelName;
            this.OwnerName = ownerName;
            this.OwnerId = ownerId;
            this.Password = password;
            this.Score = score;
            this.Type = type;
            this.Access = RoomAccessUtility.ToRoomAccess(access);
            this.UsersNow = usersNow;
            this.UsersMax = usersMax;
            this.Category = category;
            this.Description = description;

            this.Tags = new List<string>();
            foreach (string Tag in tags.ToString().Split(','))
            {
                Tags.Add(Tag);
            }

            this.Floor = floor;
            this.Landscape = landscape;
            this.AllowPets = allowPets;
            this.AllowPetsEating = allowPetsEating;
            this.RoomBlockingEnabled = roomBlockingEnabled;
            this.Hidewall = hidewall;
            this.WallThickness = wallThickness;
            this.FloorThickness = floorThickness;
            this.Wallpaper = wallpaper;
            this.WhoCanMute = muteSettings;
            this.WhoCanBan = banSettings;
            this.WhoCanKick = kickSettings;
            this.chatMode = chatMode;
            this.ChatSize = chatSize;
            this.ChatSpeed = chatSpeed;
            this.ExtraFlood = extraFlood;
            this.ChatDistance = chatDistance;
            this.TradeSettings = tradeSettings;
            this.PushEnabled = pushEnabled;
            this.PullEnabled = pullEnabled;
            this.SuperPushEnabled = superPushEnabled;
            this.SuperPullEnabled = superPullEnabled;
            this.EnablesEnabled = enablesEnabled;
            this.RespectNotificationsEnabled = respectedNotificationsEnabled;
            this.PetMorphsAllowed = petMorphsAllowed;
            this.SalePrice = salePrice;
            this.ReverseRollers = false;
            this.LayEnabled = layEnabled;

            if (groupId > 0)
                PlusEnvironment.GetGame().GetGroupManager().TryGetGroup(groupId, out this._group);

            LoadPromotions();

            this._model = model;
        }

        public RoomPromotion Promotion
        {
            get { return this._promotion; }
            set { this._promotion = value; }
        }

        public Group Group
        {
            get { return this._group; }
            set { this._group = value; }
        }

        public RoomData(RoomData data)
        {
            this.Id = data.Id;
            this.Name = data.Name;
            this.ModelName = data.ModelName;
            this.OwnerName = data.OwnerName;
            this.OwnerId = data.OwnerId;
            this.Password = data.Password;
            this.Score = data.Score;
            this.Type = data.Type;
            this.Access = data.Access;
            this.UsersNow = data.UsersNow;
            this.UsersMax = data.UsersMax;
            this.Category = data.Category;
            this.Description = data.Description;
            this.Tags = data.Tags;
            this.Floor = data.Floor;
            this.Landscape = data.Landscape;
            this.AllowPets = data.AllowPets;
            this.AllowPetsEating = data.AllowPetsEating;
            this.RoomBlockingEnabled = data.RoomBlockingEnabled;
            this.Hidewall = data.Hidewall;
            this.WallThickness = data.WallThickness;
            this.FloorThickness = data.FloorThickness;
            this.Wallpaper = data.Wallpaper;
            this.WhoCanMute = data.WhoCanMute;
            this.WhoCanBan = data.WhoCanBan;
            this.WhoCanKick = data.WhoCanKick;
            this.chatMode = data.chatMode;
            this.ChatSize = data.ChatSize;
            this.ChatSpeed = data.ChatSpeed;
            this.ExtraFlood = data.ExtraFlood;
            this.ChatDistance = data.ChatDistance;
            this.TradeSettings = data.TradeSettings;
            this.PushEnabled = data.PushEnabled;
            this.PullEnabled = data.PullEnabled;
            this.SuperPushEnabled = data.SuperPushEnabled;
            this.SuperPullEnabled = data.SuperPullEnabled;
            this.RespectNotificationsEnabled = data.RespectNotificationsEnabled;
            this.PetMorphsAllowed = data.PetMorphsAllowed;
            this.Group = data.Group;
            this.SalePrice = data.SalePrice;
            this.EnablesEnabled = data.EnablesEnabled;
            this.ReverseRollers = data.ReverseRollers;
            this.LayEnabled = data.LayEnabled;

            this.Model = data.Model;
        }

        public void LoadPromotions()
        {
            DataRow GetPromotion = null;
            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT * FROM `room_promotions` WHERE `room_id` = " + Id + " LIMIT 1;");
                GetPromotion = dbClient.GetRow();

                if (GetPromotion != null)
                {
                    if (Convert.ToDouble(GetPromotion["timestamp_expire"]) > Plus.Utilities.UnixTimestamp.GetNow())
                        this._promotion = new RoomPromotion(Convert.ToString(GetPromotion["title"]), Convert.ToString(GetPromotion["description"]), Convert.ToDouble(GetPromotion["timestamp_start"]), Convert.ToDouble(GetPromotion["timestamp_expire"]), Convert.ToInt32(GetPromotion["category_id"]));
                }
            }
        }

        public bool HasActivePromotion
        {
            get { return this.Promotion != null; }
        }

        public void EndPromotion()
        {
            if (!this.HasActivePromotion)
                return;

            this.Promotion = null;
        }
    }
}