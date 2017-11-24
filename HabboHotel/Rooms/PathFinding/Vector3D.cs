namespace Plus.HabboHotel.Rooms.PathFinding
{
    sealed class Vector3D
    {
        private int x;
        private int y;
        private double z;

        public int X
        {
            get
            {
                return this.x;
            }
            set
            {
                this.x = value;
            }
        }

        public int Y
        {
            get
            {
                return this.y;
            }
            set
            {
                this.y = value;
            }
        }

        public double Z
        {
            get
            {
                return this.z;
            }
            set
            {
                this.z = value;
            }
        }

        public Vector3D() { }

        public Vector3D(int x, int y, double z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public Vector2D ToVector2D()
        {
            return new Vector2D(this.x, this.y);
        }
    }
}
