using System;
using System.Collections;
using System.Collections.Generic;

using Plus.HabboHotel.GameClients;
using Plus.Communication.Packets.Incoming;

using Plus.Communication.Packets.Outgoing.Inventory.Badges;
using Plus.Communication.Packets.Outgoing.Inventory.Furni;

using Plus.Database.Interfaces;
using Plus.HabboHotel.Badges;

namespace Plus.HabboHotel.Users.Badges
{
    public class BadgeComponent
    {
        private readonly Habbo _player;
        private readonly Dictionary<string, Badge> _badges;

        public BadgeComponent(Habbo Player, UserData.UserData data)
        {
            this._player = Player;
            this._badges = new Dictionary<string, Badge>();

            foreach (Badge badge in data.badges)
            {
                BadgeDefinition BadgeDefinition = null;
                if (!PlusEnvironment.GetGame().GetBadgeManager().TryGetBadge(badge.Code, out BadgeDefinition) || BadgeDefinition.RequiredRight.Length > 0 && !Player.GetPermissions().HasRight(BadgeDefinition.RequiredRight))
                    continue;

                if (!this._badges.ContainsKey(badge.Code))
                    this._badges.Add(badge.Code, badge);
            }     
        }

        public int Count
        {
            get { return _badges.Count; }
        }

        public int EquippedCount
        {
            get
            {
                int i = 0;

                foreach (Badge Badge in _badges.Values)
                {
                    if (Badge.Slot <= 0)
                    {
                        continue;
                    }

                    i++;
                }

                return i;
            }
        }

        public ICollection<Badge> GetBadges()
        {
            return this._badges.Values;
        }

        public Badge GetBadge(string Badge)
        {
            if (_badges.ContainsKey(Badge))
                return (Badge)_badges[Badge];

            return null;
        }

        public bool TryGetBadge(string BadgeCode, out Badge Badge)
        {
            return this._badges.TryGetValue(BadgeCode, out Badge);
        }

        public bool HasBadge(string Badge)
        {
            return _badges.ContainsKey(Badge);
        }

        public void GiveBadge(string Badge, Boolean InDatabase, GameClient Session)
        {
            if (HasBadge(Badge))
                return;

            BadgeDefinition BadgeDefinition = null;
            if (!PlusEnvironment.GetGame().GetBadgeManager().TryGetBadge(Badge.ToUpper(), out BadgeDefinition) || BadgeDefinition.RequiredRight.Length > 0 && !Session.GetHabbo().GetPermissions().HasRight(BadgeDefinition.RequiredRight))
                return;

            if (InDatabase)
            {
                using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                {
                    dbClient.SetQuery("REPLACE INTO `user_badges` (`user_id`,`badge_id`,`badge_slot`) VALUES ('" + _player.Id + "', @badge, '" + 0 + "')");
                    dbClient.AddParameter("badge", Badge);
                    dbClient.RunQuery();
                }
            }

            _badges.Add(Badge, new Badge(Badge, 0));

            if (Session != null)
            {
                Session.SendPacket(new BadgesComposer(Session));
                Session.SendPacket(new FurniListNotificationComposer(1, 4));
            }
        }

        public void ResetSlots()
        {
            foreach (Badge Badge in _badges.Values)
            {
                Badge.Slot = 0;
            }
        }

        public void RemoveBadge(string Badge)
        {
            if (!HasBadge(Badge))
            {
                return;
            }

            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("DELETE FROM user_badges WHERE badge_id = @badge AND user_id = " + _player.Id + " LIMIT 1");
                dbClient.AddParameter("badge", Badge);
                dbClient.RunQuery();
            }

            if (_badges.ContainsKey(Badge))
                _badges.Remove(Badge);
        }
    }
}