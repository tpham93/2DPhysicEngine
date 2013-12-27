using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using PhysicEngine.Collision;
using PhysicEngine.Shape;

namespace PhysicEngine.Objects
{
    class Object2D
    {

        /*
         * Object specific attributes
         */
        private float mass;
        private float iMass;
        private float restitution;
        private Vector2 velocity;
        private Shape2D shape;

        public Vector2 Position
        {
            get { return shape.Position; }
            set { shape.Position = value; }
        }
        public float Rotation
        {
            get { return shape.Rotation; }
            set { shape.Rotation = value; }
        }
        public float Mass
        {
            get { return mass; }
            set
            {
                if (float.IsInfinity(value))
                {
                    mass = 0;
                    iMass = 0;
                }
                else
                {
                    mass = value;
                    iMass = 1.0f / mass;
                }
            }
        }
        public float IMass
        {
            get { return iMass; }
        }
        public float Restitution
        {
            get { return restitution; }
        }
        public Vector2 CenterOfMass
        {
            get { return shape.MiddlePoint; }
        }
        public Vector2 Velocity
        {
            get { return velocity; }
            set { velocity = value; }
        }
        public Shape2D Shape
        {
            get { return shape; }
        }

        /*
         * Drawing attributes
         */
        private Texture2D texture;

        public Texture2D Texture
        {
            get { return texture; }
        }

        public Object2D(Shape2D shape,Texture2D texture,float mass, Vector2 centerOfMass, float restitution)
        {
            this.shape = shape;
            this.texture = texture;
            this.Mass = mass;
            this.restitution = restitution;
        }

        public void Update(GameTime gameTime)
        {
            float dT = (float)gameTime.ElapsedGameTime.TotalSeconds*3;
            velocity += Vector2.UnitY * mass * 9.81f * dT;
            Position += Velocity * dT;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture, Position, null, Color.White, Rotation, CenterOfMass, 1, SpriteEffects.None, 0.0f);
        }
        
        public static CollisionManifold checkCollision(Object2D a, Object2D b)
        {
            IntersectData data = a.Shape.intersects(b.Shape);
            return new CollisionManifold(a,b,data);
        }


    }
}
