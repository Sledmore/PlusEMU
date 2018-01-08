namespace Plus.HabboHotel.LandingView.Promotions
{
    public class Promotion
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Text { get; set; }
        public string ButtonText { get; set; }
        public int ButtonType { get; set; }
        public string ButtonLink { get; set; }
        public string ImageLink { get; set; }

        public Promotion(int Id, string Title, string Text, string ButtonText, int ButtonType, string ButtonLink, string ImageLink)
        {
            this.Id = Id;
            this.Title = Title;
            this.Text = Text;
            this.ButtonText = ButtonText;
            this.ButtonType = ButtonType;
            this.ButtonLink = ButtonLink;
            this.ImageLink = ImageLink;
        }
    }
}
