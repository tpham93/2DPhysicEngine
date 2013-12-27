using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

namespace PhysicEngine.Etc
{
    struct VectorData
    {
        public Vector2 direction;
        public float length;

        public VectorData(Vector2 direction, float length)
        {
            this.direction = direction;
            this.length = length;
        }
        public VectorData(Vector2 scaledVector)
        {
            this.direction = Vector2.Normalize(scaledVector);
            this.length = scaledVector.Length();
        }
    }
}
