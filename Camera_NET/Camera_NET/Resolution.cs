namespace Camera_NET
{
    using System;
    using System.Runtime.CompilerServices;

    public class Resolution : IComparable<Resolution>, IEquatable<Resolution>
    {
        public Resolution(int width, int height)
        {
            this.Width = width;
            this.Height = height;
        }

        public Resolution Clone()
        {
            return new Resolution(this.Width, this.Height);
        }

        public int CompareTo(Resolution y)
        {
            Resolution resolution = this;
            if (resolution == null)
            {
                if (y == null)
                {
                    return 0;
                }
                return -1;
            }
            if (y == null)
            {
                return 1;
            }
            if (resolution.Width > y.Width)
            {
                return 1;
            }
            if (resolution.Width < y.Width)
            {
                return -1;
            }
            return resolution.Height.CompareTo(y.Height);
        }

        public bool Equals(Resolution other)
        {
            if (other == null)
            {
                return false;
            }
            return ((this.Height == other.Height) && (this.Width == other.Width));
        }

        public override string ToString()
        {
            return (this.Width.ToString() + "x" + this.Height.ToString());
        }

        public int Height { get; set; }

        public int Width { get; set; }
    }
}

