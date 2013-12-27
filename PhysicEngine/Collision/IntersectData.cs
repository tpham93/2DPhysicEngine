using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using PhysicEngine.Etc;

namespace PhysicEngine.Collision
{
    class IntersectData
    {
        private bool interects = false;
        private Vector2 mtv = Vector2.Zero;
        private float penetrationDepth = 0.0f;

        public bool Intersects
        {
            get { return interects; }
            set { interects = value; }
        }
        public Vector2 Mtv
        {
            get { return mtv; }
            set { mtv = value; }
        }
        public float PenetrationDepth
        {
            get { return penetrationDepth; }
            set { penetrationDepth = value; }
        }

        /// <summary>
        /// contstructor for not intersecting objects
        /// </summary>
        public IntersectData()
        {
            interects = false;
            this.mtv = Vector2.Zero;
            this.penetrationDepth = 0.0f;
        }

        /// <summary>
        /// contstructor for intersecting objects
        /// </summary>
        /// <param name="mtv">not normalized mtv</param>
        public IntersectData(VectorData mtv)
        {
            interects = true;
            this.penetrationDepth = mtv.length;
            this.mtv = mtv.direction;
        }

        /// <summary>
        /// contstructor for intersecting objects
        /// </summary>
        /// <param name="mtv">not normalized mtv</param>
        public IntersectData(Vector2 mtv)
        {
            interects = true;
            this.penetrationDepth = mtv.Length();
            this.mtv = mtv / penetrationDepth;
        }

        /// <summary>
        /// contstructor for intersecting objects
        /// </summary>
        /// <param name="mtv">the normalized direction of the minimum translation vector</param>
        /// <param name="penetrationDepth">the depth of the penetration</param>
        public IntersectData(Vector2 mtv, float penetrationDepth)
        {
            interects = true;
            this.mtv = mtv;
            this.penetrationDepth = penetrationDepth;
        }
    }
}
