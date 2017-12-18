namespace Plus.HabboHotel.Catalog.Vouchers
{
    public static class VoucherUtility
    {
        public static VoucherType GetType(string Type)
        {
            switch (Type)
            {
                default:
                case "credit":
                    return VoucherType.Credit;
                case "ducket":
                    return VoucherType.Ducket;
            }
        }

        public static string FromType(VoucherType Type)
        {
            switch (Type)
            {
                default:
                case VoucherType.Credit:
                    return "credit";
                case VoucherType.Ducket:
                    return "ducket";
            }
        }
    }
}
