using Plus.Database.Interfaces;

namespace Plus.HabboHotel.Catalog.Vouchers
{
    public class Voucher
    {
        private string _code;
        private VoucherType _type;
        private int _value;
        private int _currentUses;
        private int _maxUses;

        public Voucher(string code, string type, int value, int currentUses, int maxUses)
        {
            _code = code;
            _type = VoucherUtility.GetType(type);
            _value = value;
            _currentUses = currentUses;
            _maxUses = maxUses;
        }

        public void UpdateUses()
        {
            CurrentUses += 1;
            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.RunQuery("UPDATE `catalog_vouchers` SET `current_uses` = `current_uses` + '1' WHERE `voucher` = '" + _code + "' LIMIT 1");
            }
        }

        public string Code
        {
            get { return _code; }
            set { _code = value; }
        }

        public VoucherType Type
        {
            get { return _type; }
            set { _type = value; }
        }

        public int Value
        {
            get { return _value; }
            set { _value = value; }
        }

        public int CurrentUses
        {
            get { return _currentUses; }
            set { _currentUses = value; }
        }

        public int MaxUses
        {
            get { return _maxUses; }
            set { _maxUses = value; }
        }
    }
}
