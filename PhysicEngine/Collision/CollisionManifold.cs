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

                float minRestitution = Math.Min(A.MaterialData.Restitution, B.MaterialData.Restitution);
                float impulseMagnitude = -((1 + minRestitution) * velocityAlongMTV) / (A.MassData.IMass + B.MassData.IMass);
                Vector2 impulse = intersectData.Mtv * impulseMagnitude;
                if (A.MassData.IMass != 0)
                {
                    A.Velocity -= A.MassData.IMass * impulse;
                    A.Position -= intersectData.Mtv * intersectData.PenetrationDepth;
                }
                if (B.MassData.IMass != 0)
                {
                    B.Velocity += B.MassData.IMass * impulse;
                    B.Position += intersectData.Mtv * intersectData.PenetrationDepth;
                }
                positionalCorrection();
            }
        }

        private void positionalCorrection()
        {
            const float percent = 0.2f;
            const float slop = 0.01f;
            Vector2 correction = Math.Max(intersectData.PenetrationDepth - slop, 0.0f) / (A.MassData.IMass + B.MassData.IMass) * percent * intersectData.Mtv;
            if (A.MassData.IMass != 0)
            {
                A.Position += correction * A.MassData.IMass;
            }
            if (B.MassData.IMass != 0)
            {
                B.Position -= correction * B.MassData.IMass;
            }
        }
    }
}
