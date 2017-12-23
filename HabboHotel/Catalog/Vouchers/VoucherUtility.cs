namespace Plus.HabboHotel.Catalog.Vouchers
{
    public static class VoucherUtility
    {
        public static VoucherType GetType(string type)
        {
            switch (type)
            {
                default:
                case "credit":
                    return VoucherType.Credit;
                case "ducket":
                    return VoucherType.Ducket;
            }
        }

        public static string FromType(VoucherType type)
        {
            switch (type)
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
