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
            if (intersectData.Intersects && !float.IsNaN(intersectData.Mtv.X) && !float.IsNaN(intersectData.Mtv.Y))
            {
                /*
                 * collision impulse
                 */

                Vector2 relativeVelocity = B.Velocity - A.Velocity;
                float minRestitution = Math.Min(A.MaterialData.Restitution, B.MaterialData.Restitution);
                float contactImpulseMagnitude = -((1 + minRestitution) * Vector2.Dot(relativeVelocity, IntersectData.Mtv)) / (A.MassData.IMass + B.MassData.IMass);
                Vector2 contactImpulse = intersectData.Mtv * contactImpulseMagnitude;
                if (A.MassData.IMass != 0)
                {
                    A.Velocity -= A.MassData.IMass * contactImpulse;
                    A.Position -= intersectData.Mtv * intersectData.PenetrationDepth;
                }
                if (B.MassData.IMass != 0)
                {
                    B.Velocity += B.MassData.IMass * contactImpulse;
                    B.Position += intersectData.Mtv * intersectData.PenetrationDepth;
                }

                positionalCorrection();

                /*
                 * friction
                 */

                relativeVelocity = B.Velocity - A.Velocity;
                Vector2 tangent = relativeVelocity - Vector2.Dot(relativeVelocity, intersectData.Mtv) * intersectData.Mtv;
                if (tangent != Vector2.Zero)
                {
                    tangent.Normalize();

                    float frictionImpulseMagnitude = -Vector2.Dot(relativeVelocity, tangent) / (A.MassData.IMass + B.MassData.IMass);
                    float staticFrictionCoefficient = new Vector2(A.MaterialData.StaticFriction, A.MaterialData.StaticFriction).Length();

                    Vector2 frictionImpulse = Vector2.Zero;
                    if (!float.IsNaN(staticFrictionCoefficient) && Math.Abs(frictionImpulseMagnitude) < Math.Abs(contactImpulseMagnitude * staticFrictionCoefficient))
                    {
                        frictionImpulse = tangent * frictionImpulseMagnitude;
                    }
                    else
                    {
                        float dynamicFrictionCoefficient = new Vector2(A.MaterialData.DynamicFriction, A.MaterialData.DynamicFriction).Length();
                        if (!float.IsNaN(dynamicFrictionCoefficient))
                            frictionImpulse = tangent * dynamicFrictionCoefficient * -contactImpulseMagnitude;
                    }

                    if (A.MassData.IMass != 0)
                    {
                        A.Velocity -= A.MassData.IMass * frictionImpulse;
                    }
                    if (B.MassData.IMass != 0)
                    {
                        B.Velocity += B.MassData.IMass * frictionImpulse;
                    }
                }


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
