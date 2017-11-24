using System;
using System.Data;

using Plus.Communication.Packets.Incoming;
using Plus.HabboHotel.GameClients;
using Plus.HabboHotel.Catalog.Vouchers;



using Plus.Communication.Packets.Outgoing.Catalog;
using Plus.Communication.Packets.Outgoing.Inventory.Purse;

using Plus.Database.Interfaces;

namespace Plus.Communication.Packets.Incoming.Catalog
{
    public class RedeemVoucherEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            string VoucherCode = Packet.PopString().Replace("\r", "");

            Voucher Voucher = null;
            if (!PlusEnvironment.GetGame().GetCatalog().GetVoucherManager().TryGetVoucher(VoucherCode, out Voucher))
            {
                Session.SendPacket(new VoucherRedeemErrorComposer(0));
                return;
            }

            if (Voucher.CurrentUses >= Voucher.MaxUses)
            {
                Session.SendNotification("Oops, this voucher has reached the maximum usage limit!");
                return;
            }

            DataRow GetRow = null;
            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT * FROM `user_vouchers` WHERE `user_id` = @userId AND `voucher` = @Voucher LIMIT 1");
                dbClient.AddParameter("userId", Session.GetHabbo().Id);
                dbClient.AddParameter("Voucher", VoucherCode);
                GetRow = dbClient.GetRow();
            }

            if (GetRow != null)
            {
                Session.SendNotification("You've already used this voucher code, one per each user, sorry!");
                return;
            }
            else
            {
                using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                {
                    dbClient.SetQuery("INSERT INTO `user_vouchers` (`user_id`,`voucher`) VALUES (@userId, @Voucher)");
                    dbClient.AddParameter("userId", Session.GetHabbo().Id);
                    dbClient.AddParameter("Voucher", VoucherCode);
                    dbClient.RunQuery();
                }
            }

            Voucher.UpdateUses();

            if (Voucher.Type == VoucherType.CREDIT)
            {
                Session.GetHabbo().Credits += Voucher.Value;
                Session.SendPacket(new CreditBalanceComposer(Session.GetHabbo().Credits));
            }
            else if (Voucher.Type == VoucherType.DUCKET)
            {
                Session.GetHabbo().Duckets += Voucher.Value;
                Session.SendPacket(new HabboActivityPointNotificationComposer(Session.GetHabbo().Duckets, Voucher.Value));
            }

            Session.SendPacket(new VoucherRedeemOkComposer());
        }
    }
}