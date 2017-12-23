namespace Plus.HabboHotel.Catalog.Pets
{
    public class PetRace
    {
        private int _raceId;
        private int _primaryColour;
        private int _secondaryColour;
        public bool _hasPrimaryColour;
        public bool _hasSecondaryColour;

        public PetRace(int raceId, int primaryColour, int secondaryColour, bool hasPrimaryColour, bool hasSecondaryColour)
        {
            _raceId = raceId;
            _primaryColour = primaryColour;
            _secondaryColour = secondaryColour;
            _hasPrimaryColour = hasPrimaryColour;
            _hasSecondaryColour = hasSecondaryColour;
        }

        public int RaceId
        {
            get { return _raceId; }
            set { _raceId = value; }
        }

        public int PrimaryColour
        {
            get { return _primaryColour; }
            set { _primaryColour = value; }
        }

        public int SecondaryColour
        {
            get { return _secondaryColour; }
            set { _secondaryColour = value; }
        }

        public bool HasPrimaryColour
        {
            get { return _hasPrimaryColour; }
            set { _hasPrimaryColour = value; }
        }

        public bool HasSecondaryColour
        {
            get { return _hasSecondaryColour; }
            set { _hasSecondaryColour = value; }
        }
    }
}
