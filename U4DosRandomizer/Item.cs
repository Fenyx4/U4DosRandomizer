using System;
using System.Diagnostics.CodeAnalysis;

namespace U4DosRandomizer
{
    public class Item : ICoordinate, IEquatable<Item>
    {
        private byte x;
        private byte location;
        private byte y;
        private byte originalX;
        private byte originalLocation;
        private byte originalY;

        public byte Location
        {
            get => location;
            set
            {
                if (location != value)
                {
                    Changed = true;
                    location = value;
                }
            }
        }
        public byte X
        {
            get => x;
            set
            {
                if (x != value)
                {
                    Changed = true;
                    x = value;
                }
            }
        }
        public byte Y
        {
            get => y;
            set
            {
                if (y != value)
                {
                    Changed = true;
                    y = value;
                }
            }
        }
        public byte OriginalLocation
        {
            get => originalLocation;
        }
        public byte OriginalX
        {
            get => originalX;
        }
        public byte OriginalY
        {
            get => originalY;
        }
        public bool Changed { get; private set; }

        public Item(byte location, byte x, byte y)
        {
            Location = location;
            X = x;
            Y = y;

            originalLocation = location;
            originalX = x;
            originalY = y;

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