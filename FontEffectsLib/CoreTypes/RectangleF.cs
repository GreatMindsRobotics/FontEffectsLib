using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using System.Globalization;

namespace FontEffectsLib.CoreTypes
{
    /// <summary>
    /// Represents a rectangle created using floating point coordinates.
    /// </summary>
    public struct RectangleF
    {
        public float X;
        public float Y;
        public float Width;
        public float Height;

        public static explicit operator Rectangle(RectangleF instance){
            Rectangle rect = new Rectangle();
            rect.X = (int)instance.X;
            rect.Y = (int)instance.Y;
            rect.Width = (int)instance.Width;
            rect.Height = (int)instance.Height;
            return rect;
        }

        public static implicit operator RectangleF(Rectangle instance)
        {
            RectangleF rect = new RectangleF();
            rect.X = instance.X;
            rect.Y = instance.Y;
            rect.Width = instance.Width;
            rect.Height = instance.Height;
            return rect;
        }

        private static RectangleF _empty = new RectangleF(); // Assigns default field values so we're fine

        public static RectangleF Empty
        {
            get
            {
                return _empty;
            }
        }

        /// <summary>
        /// Gets the Y-coordinate of the bottom of this rectangle.
        /// </summary>
        public float Bottom
        {
            get
            {
                return this.Y + this.Height;
            }
        }

        /// <summary>Gets the Vector2 that specifies the center of the rectangle.</summary>
        public Vector2 Center
        {
            get
            {
                return new Vector2(X + (Width / 2), Y + (Height / 2));
            }
        }

        /// <summary>Gets a value that indicates whether the RectangleF is empty.</summary>
        public bool IsEmpty
        {
            get
            {
                if (this.Width != 0 || this.Height != 0 || this.X != 0)
                {
                    return false;
                }
                return this.Y == 0;
            }
        }

        /// <summary>Returns the X-coordinate of the left side of the rectangle.</summary>
        public float Left
        {
            get
            {
                return this.X;
            }
        }

        /// <summary>Gets or sets the upper-left value of the RectangeF.</summary>
        public Vector2 Location
        {
            get
            {
                return new Vector2(X, Y);
            }
            set
            {
                this.X = value.X;
                this.Y = value.Y;
            }
        }

        /// <summary>Returns the X-coordinate of the right side of the rectangle.</summary>
        public float Right
        {
            get
            {
                return this.X + this.Width;
            }
        }

        /// <summary>Returns the Y-coordinate of the top of the rectangle.</summary>
        public float Top
        {
            get
            {
                return this.Y;
            }
        }

        /// <summary>Initializes a new instance of Rectangle.</summary>
        /// <param name="x">The x-coordinate of the rectangle.</param>
        /// <param name="y">The y-coordinate of the rectangle.</param>
        /// <param name="width">Width of the rectangle.</param>
        /// <param name="height">Height of the rectangle.</param>
        public RectangleF(float x, float y, float width, float height)
        {
            this.X = x;
            this.Y = y;
            this.Width = width;
            this.Height = height;
        }

        /// <summary>Determines whether this RectangleF contains a specified point represented by its X- and Y-coordinates.</summary>
        /// <param name="x">The x-coordinate of the specified point.</param>
        /// <param name="y">The y-coordinate of the specified point.</param>
        public bool Contains(float x, float y)
        {
            if (this.X > x || x >= this.X + this.Width || this.Y > y)
            {
                return false;
            }
            return y < this.Y + this.Height;
        }

        /// <summary>Determines whether this RectangleF contains a specified Point.</summary>
        /// <param name="value">The Point to evaluate.</param>
        public bool Contains(Point value)
        {
            if (this.X > value.X || value.X >= this.X + this.Width || this.Y > value.Y)
            {
                return false;
            }
            return value.Y < this.Y + this.Height;
        }

        /// <summary>Determines whether this RectangleF contains a specified point represented by a floating point vector.</summary>
        /// <param name="value">The Vector2 to evaluate.</param>
        public bool Contains(Vector2 value)
        {
            if (this.X > value.X || value.X >= this.X + this.Width || this.Y > value.Y)
            {
                return false;
            }
            return value.Y < this.Y + this.Height;
        }

        /// <summary>Determines whether this Rectangle entirely contains a specified Rectangle.</summary>
        /// <param name="value">The Rectangle to evaluate.</param>
        public bool Contains(Rectangle value)
        {
            if (this.X > value.X || value.X + value.Width > this.X + this.Width || this.Y > value.Y)
            {
                return false;
            }
            return value.Y + value.Height <= this.Y + this.Height;
        }

        /// <summary>Determines whether this RectangleF entirely contains a specified RectangleF.</summary>
        /// <param name="value">The RectangleF to evaluate.</param>
        public bool Contains(RectangleF value)
        {
            if (this.X > value.X || value.X + value.Width > this.X + this.Width || this.Y > value.Y)
            {
                return false;
            }
            return value.Y + value.Height <= this.Y + this.Height;
        }

        /// <summary>Determines whether the specified Object is equal to the RectangleF.</summary>
        /// <param name="other">The Object to compare with the current RectangleF.</param>
        public bool Equals(RectangleF other)
        {
            if (this.X != other.X || this.Y != other.Y || this.Width != other.Width)
            {
                return false;
            }
            return this.Height == other.Height;
        }

        /// <summary>Determines whether the specified Object is equal to the Rectangle.</summary>
        /// <param name="other">The Object to compare with the current Rectangle.</param>
        public bool Equals(Rectangle other)
        {
            if (this.X != other.X || this.Y != other.Y || this.Width != other.Width)
            {
                return false;
            }
            return this.Height == other.Height;
        }

        /// <summary>Returns a value that indicates whether the current instance is equal to a specified object.</summary>
        /// <param name="obj">Object to make the comparison with.</param>
        public override bool Equals(object obj)
        {
            bool areEqual = Object.ReferenceEquals(obj, this);
            if (obj is RectangleF)
            {
                areEqual = this.Equals((RectangleF)obj);
            }else if (obj is Rectangle)
            {
                areEqual = this.Equals((Rectangle)obj); // Allow "soft equality"
            }
            return areEqual;
        }

        /// <summary>Gets the hash code for this object.</summary>
        public override int GetHashCode()
        {
            return this.X.GetHashCode() + this.Y.GetHashCode() + this.Width.GetHashCode() + this.Height.GetHashCode();
        }

        /// <summary>Pushes the edges of the Rectangle out by the horizontal and vertical values specified.</summary>
        /// <param name="horizontalAmount">Value to push the sides out by.</param>
        /// <param name="verticalAmount">Value to push the top and bottom out by.</param>
        public void Inflate(float horizontalAmount, float verticalAmount)
        {
            RectangleF x = this;
            x.X = x.X - horizontalAmount;
            RectangleF y = this;
            y.Y = y.Y - verticalAmount;
            RectangleF width = this;
            width.Width = width.Width + horizontalAmount * 2;
            RectangleF height = this;
            height.Height = height.Height + verticalAmount * 2;
        }

        /// <summary>Determines whether a specified Rectangle intersects with this RectangleF.</summary>
        /// <param name="value">The Rectangle to evaluate.</param>
        public bool Intersects(Rectangle value)
        {
            if (value.X >= this.X + this.Width || this.X >= value.X + value.Width || value.Y >= this.Y + this.Height)
            {
                return false;
            }
            return this.Y < value.Y + value.Height;
        }

        /// <summary>Determines whether a specified Rectangle intersects with this RectangleF.</summary>
        /// <param name="value">The RectangleF to evaluate.</param>
        public bool Intersects(RectangleF value)
        {
            if (value.X >= this.X + this.Width || this.X >= value.X + value.Width || value.Y >= this.Y + this.Height)
            {
                return false;
            }
            return this.Y < value.Y + value.Height;
        }

        /// <summary>Compares two rectangles for equality.</summary>
        /// <param name="a">Source rectangle.</param>
        /// <param name="b">Source rectangle.</param>
        public static bool operator ==(RectangleF a, RectangleF b)
        {
            if (a.X != b.X || a.Y != b.Y || a.Width != b.Width)
            {
                return false;
            }
            return a.Height == b.Height;
        }

        /// <summary>Compares two rectangles for inequality.</summary>
        /// <param name="a">Source rectangle.</param>
        /// <param name="b">Source rectangle.</param>
        public static bool operator !=(RectangleF a, RectangleF b)
        {
            if (a.X != b.X || a.Y != b.Y || a.Width != b.Width)
            {
                return true;
            }
            return a.Height != b.Height;
        }

        /// <summary>Retrieves a string representation of the current object.</summary>
        public override string ToString()
        {
            return this.ToString(CultureInfo.CurrentCulture);
        }

        /// <summary>Retrieves a string representation of the current object.</summary>
        public string ToString(IFormatProvider formatter)
        {
            object[] str = new object[] { this.X.ToString(formatter), this.Y.ToString(formatter), this.Width.ToString(formatter), this.Height.ToString(formatter) };
            return string.Format(formatter, "{{X:{0} Y:{1} Width:{2} Height:{3}}}", str);
        }

    }
}
