using System.Collections.Generic;

using Plus.HabboHotel.GameClients;
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
            _player = Player;
            _badges = new Dictionary<string, Badge>();

            foreach (Badge badge in data.badges)
            {
                BadgeDefinition BadgeDefinition = null;
                if (!PlusEnvironment.GetGame().GetBadgeManager().TryGetBadge(badge.Code, out BadgeDefinition) || BadgeDefinition.RequiredRight.Length > 0 && !Player.GetPermissions().HasRight(BadgeDefinition.RequiredRight))
                    continue;

                if (!_badges.ContainsKey(badge.Code))
                    _badges.Add(badge.Code, badge);
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

                foreach (Badge badge in _badges.Values)
                {
                    if (badge.Slot <= 0)
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
            return _badges.Values;
        }

        public Badge GetBadge(string badge)
        {
            if (_badges.ContainsKey(badge))
                return _badges[badge];

            return null;
        }

        public bool TryGetBadge(string code, out Badge badge)
        {
            return _badges.TryGetValue(code, out badge);
        }

        public bool HasBadge(string badge)
        {
            return _badges.ContainsKey(badge);
        }

        public void GiveBadge(string code, bool inDatabase, GameClient session)
        {
            if (HasBadge(code))
                return;

            BadgeDefinition badge = null;
            if (!PlusEnvironment.GetGame().GetBadgeManager().TryGetBadge(code.ToUpper(), out badge) || badge.RequiredRight.Length > 0 && !session.GetHabbo().GetPermissions().HasRight(badge.RequiredRight))
                return;

            if (inDatabase)
            {
                using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                {
                    dbClient.SetQuery("REPLACE INTO `user_badges` (`user_id`,`badge_id`,`badge_slot`) VALUES ('" + _player.Id + "', @badge, '" + 0 + "')");
                    dbClient.AddParameter("badge", code);
                    dbClient.RunQuery();
                }
            }

            _badges.Add(code, new Badge(code, 0));

            if (session != null)
            {
                session.SendPacket(new BadgesComposer(session.GetHabbo().GetBadgeComponent().GetBadges()));
                session.SendPacket(new FurniListNotificationComposer(1, 4));
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