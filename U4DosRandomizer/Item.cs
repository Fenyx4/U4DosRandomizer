namespace U4DosRandomizer
{
    public class Item : ICoordinate
    {
        private byte x;
        private byte location;
        private byte y;

        public byte Location
        {
            get => location;
            set
            {
                Changed = true;
                location = value;
            }
        }
        public byte X
        {
            get => x;
            set
            {
                Changed = true;
                x = value;
            }
        }
        public byte Y
        {
            get => y;
            set
            {
                Changed = true;
                y = value;
            }
        }
        public bool Changed { get; private set; }

        public Item(byte location, byte x, byte y)
        {
            Location = location;
            X = x;
            Y = y;
            Changed = false;
        }
    }
}