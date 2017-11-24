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

        public Voucher(string Code, string Type, int Value, int CurrentUses, int MaxUses)
        {
            this._code = Code;
            this._type = VoucherUtility.GetType(Type);
            this._value = Value;
            this._currentUses = CurrentUses;
            this._maxUses = MaxUses;
        }

        public void UpdateUses()
        {
            this.CurrentUses += 1;
            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.RunQuery("UPDATE `catalog_vouchers` SET `current_uses` = `current_uses` + '1' WHERE `voucher` = '" + this._code + "' LIMIT 1");
            }
        }

        public string Code
        {
            get { return this._code; }
            set { this._code = value; }
        }

        public VoucherType Type
        {
            get { return this._type; }
            set { this._type = value; }
        }

        public int Value
        {
            get { return this._value; }
            set { this._value = value; }
        }

        public int CurrentUses
        {
            get { return this._currentUses; }
            set { this._currentUses = value; }
        }

        public int MaxUses
        {
            get { return this._maxUses; }
            set { this._maxUses = value; }
        }
    }
}
