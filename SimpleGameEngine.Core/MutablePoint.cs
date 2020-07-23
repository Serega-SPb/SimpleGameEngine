using System.Windows;

namespace SimpleGameEngine.Core
{
    public class MutablePoint
    {
        public MutablePoint(double x, double y)
        {
            X = x;
            Y = y;
        }

        public MutablePoint(Point point)
        {
            X = point.X;
            Y = point.Y;
        }
        
        public double X { get; set; }
        
        public double Y { get; set; }

        public Point ToPoint() => new Point(X, Y);

        protected bool Equals(MutablePoint other)
        {
            return X.Equals(other.X) && Y.Equals(other.Y);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((MutablePoint) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (X.GetHashCode() * 397) ^ Y.GetHashCode();
            }
        }

        public static bool Equals(MutablePoint point, MutablePoint other) => 
            point.X.Equals(other.X) && point.Y.Equals(other.Y);

        public static bool operator ==(MutablePoint point, MutablePoint other) => 
            point.X == other.X && point.Y == other.Y;

        public static bool operator !=(MutablePoint point, MutablePoint other) => 
            !(point == other);


        public static MutablePoint operator +(MutablePoint point, Vector vector)
        {
            point.X += vector.X;
            point.Y += vector.Y;
            return point;
        }
        
        public static MutablePoint operator -(MutablePoint point, Vector vector)
        {
            point.X -= vector.X;
            point.Y -= vector.Y;
            return point;
        }

        public static Vector operator -(MutablePoint point, MutablePoint other) => 
            new Vector(point.X - other.X, point.Y - other.Y);
    }
}