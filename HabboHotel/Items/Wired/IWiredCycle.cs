namespace Plus.HabboHotel.Items.Wired
{
    interface IWiredCycle
    {
        int Delay { get; set; }
        int TickCount { get; set; }
        bool OnCycle();
    }
}
