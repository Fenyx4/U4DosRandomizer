using System;
using System.Diagnostics.CodeAnalysis;

namespace U4DosRandomizer
{
    public class Item : ICoordinate, IEquatable<Item>
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

        public override bool Equals(object obj)
        {
            return this.Equals(obj as Item);
        }

        public bool Equals([AllowNull] Item other)
        {
            if (other == null)
                return false;

            return this.X.Equals(other.X) && this.Y.Equals(other.Y) && this.Location.Equals(other.Location);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(x, y, location);
        }
    }
}