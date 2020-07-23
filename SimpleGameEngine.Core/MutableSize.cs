using System;
using System.Windows;

namespace SimpleGameEngine.Core
{
    public class MutableSize
    {
        private double _width;
        private double _height;

        public MutableSize(double width, double height)
        {
            Width = width;
            Height = height;
        }

        public double Width
        {
            get => _width;
            set
            {
                if (value < 0.0)
                    throw new ArgumentException("Size cannot be negative");
                _width = value;
            }
        }

        public double Height
        {
            get => _height;
            set
            {
                if (value < 0.0)
                    throw new ArgumentException("Size cannot be negative");
                _height = value;
            }
        }

        public bool IsEmpty => Width < 0.0;
        
        protected bool Equals(MutableSize other)
        {
            return Width.Equals(other.Width) && Height.Equals(other.Height);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((MutableSize) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (Width.GetHashCode() * 397) ^ Height.GetHashCode();
            }
        }
        

        public static bool operator ==(MutableSize size1, MutableSize size2)
        {
            if (size1.Width == size2.Width)
                return size1.Height == size2.Height;
            return false;
        }

        public static bool operator !=(MutableSize size1, MutableSize size2)
        {
            return !(size1 == size2);
        }
        
        public static bool Equals(MutableSize size1, MutableSize size2)
        {
            if (size1.IsEmpty)
                return size2.IsEmpty;
            if (size1.Width.Equals(size2.Width))
                return size1.Height.Equals(size2.Height);
            return false;
        }
        
        public static bool operator ==(MutableSize size1, Size size2)
        {
            if (size1.Width == size2.Width)
                return size1.Height == size2.Height;
            return false;
        }

        public static bool operator !=(MutableSize size1, Size size2)
        {
            return !(size1 == size2);
        }
    }
}