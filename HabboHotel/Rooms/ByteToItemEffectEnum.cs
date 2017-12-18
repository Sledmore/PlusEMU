namespace Plus.HabboHotel.Rooms
{
    public static class ByteToItemEffectEnum
    {
        public static ItemEffectType Parse(byte number)
        {
            switch (number)
            {
                case 0:
                    return ItemEffectType.None;
                case 1:
                    return ItemEffectType.Swim;
                case 2:
                    return ItemEffectType.Normalskates;
                case 3:
                    return ItemEffectType.Iceskates;
                case 4:
                    return ItemEffectType.SwimLow;
                case 5:
                    return ItemEffectType.SwimHalloween;
                default:
                    return ItemEffectType.None;
            }
        }
    }
}
