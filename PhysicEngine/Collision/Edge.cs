using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

namespace PhysicEngine.Collision
{
    struct Edge
    {
        Vector2 max;
        Vector2 v1;
        Vector2 v2;

        public Vector2 Max
        {
            get { return max; }
        }
        public Vector2 V1
        {
            get { return v1; }
        }
        public Vector2 V2
        {
            get { return v2; }
        }
        public Vector2 Direction
        {
            get { return v2 - v1; }
        }



        public Edge(Vector2 max, Vector2 v1, Vector2 v2)
        {
            this.max = max;
            this.v1 = v1;
            this.v2 = v2;
        }
    }
}
