using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PhysicEngine.Objects;
using Microsoft.Xna.Framework;
namespace PhysicEngine.Collision
{
    struct CollisionManifold
    {
        private Object2D a;
        private Object2D b;
        private IntersectData intersectData;

        
        public Object2D A
        {
          get { return a; }
        }
        public Object2D B
        {
          get { return b; }
        }
        public IntersectData IntersectData
        {
          get { return intersectData; }
        }

        public CollisionManifold(Object2D a, Object2D b, IntersectData intersectData)
        {
            this.a = a;
            this.b = b;
            this.intersectData = intersectData;
        }

        public void resolveCollision()
        {
            if (intersectData.Intersects)
            {
                Vector2 relativeVelocity = B.Velocity - A.Velocity;
                float velocityAlongMTV = Vector2.Dot(relativeVelocity, IntersectData.Mtv);

                float minRestitution = Math.Min(A.Restitution, B.Restitution);
                float impulseMagnitude = -((1 + minRestitution) * velocityAlongMTV) / (A.IMass + B.IMass);
                Vector2 impulse = intersectData.Mtv * impulseMagnitude;
                A.Velocity -= A.IMass * impulse;
                B.Velocity += B.IMass * impulse;
                if (A.IMass != 0)
                    A.Position -= intersectData.Mtv * intersectData.PenetrationDepth;
                if (B.IMass != 0)
                    B.Position += intersectData.Mtv * intersectData.PenetrationDepth;
            }
        }
    }
}
