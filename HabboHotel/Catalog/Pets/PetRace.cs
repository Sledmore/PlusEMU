using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plus.HabboHotel.Catalog.Pets
{
    public class PetRace
    {
        private int _raceId;
        private int _primaryColour;
        private int _secondaryColour;
        public bool _hasPrimaryColour;
        public bool _hasSecondaryColour;

        public PetRace(int RaceId, int PrimaryColour, int SecondaryColour, bool HasPrimaryColour, bool HasSecondaryColour)
        {
            this._raceId = RaceId;
            this._primaryColour = PrimaryColour;
            this._secondaryColour = SecondaryColour;
            this._hasPrimaryColour = HasPrimaryColour;
            this._hasSecondaryColour = HasSecondaryColour;
        }

        public int RaceId
        {
            get { return this._raceId; }
            set { this._raceId = value; }
        }

        public int PrimaryColour
        {
            get { return this._primaryColour; }
            set { this._primaryColour = value; }
        }

        public int SecondaryColour
        {
            get { return this._secondaryColour; }
            set { this._secondaryColour = value; }
        }

        public bool HasPrimaryColour
        {
            get { return this._hasPrimaryColour; }
            set { this._hasPrimaryColour = value; }
        }

        public bool HasSecondaryColour
        {
            get { return this._hasSecondaryColour; }
            set { this._hasSecondaryColour = value; }
        }
    }
}
