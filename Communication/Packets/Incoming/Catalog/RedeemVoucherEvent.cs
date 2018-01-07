using System.Data;

using Plus.HabboHotel.GameClients;
using Plus.HabboHotel.Catalog.Vouchers;

using Plus.Communication.Packets.Outgoing.Catalog;
using Plus.Communication.Packets.Outgoing.Inventory.Purse;

using Plus.Database.Interfaces;

namespace Plus.Communication.Packets.Incoming.Catalog
{
    public class RedeemVoucherEvent : IPacketEvent
    {
        public void Parse(GameClient session, ClientPacket packet)
        {
            string code = packet.PopString().Replace("\r", "");

            if (!PlusEnvironment.GetGame().GetCatalog().GetVoucherManager().TryGetVoucher(code, out Voucher voucher))
            {
                session.SendPacket(new VoucherRedeemErrorComposer(0));
                return;
            }

            if (voucher.CurrentUses >= voucher.MaxUses)
            {
                session.SendNotification("Oops, this voucher has reached the maximum usage limit!");
                return;
            }

            DataRow row;
            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT * FROM `user_vouchers` WHERE `user_id` = @userId AND `voucher` = @Voucher LIMIT 1");
                dbClient.AddParameter("userId", session.GetHabbo().Id);
                dbClient.AddParameter("Voucher", code);
                row = dbClient.GetRow();
            }

            if (row != null)
            {
                session.SendNotification("You've already used this voucher code, one per each user, sorry!");
                return;
            }
            else
            {
                using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                {
                    dbClient.SetQuery("INSERT INTO `user_vouchers` (`user_id`,`voucher`) VALUES (@userId, @Voucher)");
                    dbClient.AddParameter("userId", session.GetHabbo().Id);
                    dbClient.AddParameter("Voucher", code);
                    dbClient.RunQuery();
                }
            }

            voucher.UpdateUses();

            if (voucher.Type == VoucherType.Credit)
            {
                session.GetHabbo().Credits += voucher.Value;
                session.SendPacket(new CreditBalanceComposer(session.GetHabbo().Credits));
            }
            else if (voucher.Type == VoucherType.Ducket)
            {
                session.GetHabbo().Duckets += voucher.Value;
                session.SendPacket(new HabboActivityPointNotificationComposer(session.GetHabbo().Duckets, voucher.Value));
            }

            session.SendPacket(new VoucherRedeemOkComposer());
        }
    }
}