using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PhysicEngine.Collision
{
    class Range
    {
        float min;
        float max;
        /// <summary>
        /// returns the minimum of the range
        /// </summary>
        public float Min
        {
            get { return min; }
        }
        /// <summary>
        /// returns the maximum of the range
        /// </summary>
        public float Max
        {
            get { return max; }
        }

        /// <summary>
        /// Constructor for the range
        /// </summary>
        /// <param name="min">minimum of the range</param>
        /// <param name="max">maximum of the range</param>
        public Range(float min, float max)
        {
            this.min = min;
            this.max = max;
        }

        /// <summary>
        /// calculate the distance of two ranges
        /// </summary>
        /// <param name="r1">1st range</param>
        /// <param name="r2">2nd range</param>
        /// <returns>distance between 2 ranges</returns>
        public static float distance(Range r1,Range r2)
        {
            return r1.max == r2.max?Math.Max(r1.min - r1.max,r2.min - r2.max) :r1.max > r2.max ? r1.min - r2.max : r2.min - r1.max;
        }

        /// <summary>
        /// checks if the value is inside of the range
        /// </summary>
        /// <param name="value">value which needs to be tested</param>
        /// <returns>true if the value is inside of the range</returns>
        public bool contains(float value)
        {
            return min <= value && value <= max;
        }
        /// <summary>
        /// checks if value intersects with this range
        /// </summary>
        /// <param name="value">value which needs to be tested</param>
        /// <returns>true if the value and this range intersects</returns>
        public bool intersects(Range value)
        {
            return distance(this, value) <= 0;
        }
        /// <summary>
        /// shows the minimum and maximum
        /// </summary>
        /// <returns>string which contains the minimum and maximum of the range</returns>
        public override string ToString()
        {
            return "{" + min + " : " + max + "}";
        }
    }
}
