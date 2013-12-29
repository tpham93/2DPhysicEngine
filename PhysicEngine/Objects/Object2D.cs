using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using PhysicEngine.Collision;
using PhysicEngine.Object.Shape;
using PhysicEngine.ObjectData.Objects;

namespace PhysicEngine.Objects
{
    class Object2D
    {

        /*
         * Object specific attributes
         */
        private MassData massData;
        private MaterialData materialData;
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

        public MassData MassData
        {
            get { return massData; }
        }
        public MaterialData MaterialData
        {
            get { return materialData; }
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

        /// <summary>
        /// constructor for a general object
        /// </summary>
        /// <param name="shape">the shape of the object with its current position</param>
        /// <param name="texture">the objects texture</param>
        /// <param name="mass">the mass in kg</param>
        /// <param name="restitution">defined as: v_after/v_before</param>
        public Object2D(Shape2D shape, Texture2D texture, MaterialData materialData)
        {
            this.shape = shape;
            this.texture = texture;
            this.materialData = materialData;
            this.massData = new MassData(shape, materialData.Density);
        }

        /// <summary>
        /// steps with the given elapsed time
        /// </summary>
        /// <param name="gameTime">the gameTime, the game uses</param>
        public void Update(TimeSpan elapsedTime)
        {
            float dT = (float)elapsedTime.TotalSeconds;
            velocity += Vector2.UnitY * MassData.Mass * 9.81f * dT * PhysicalConstants.PixelToMeter;
            Position += Velocity * dT;

        }

        /// <summary>
        /// draws the object to the given rendertarget of the spritebatch's graphicsdevice
        /// </summary>
        /// <param name="spriteBatch">the spritebatch used for drawing</param>
        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture, Position, null, Color.White, Rotation, CenterOfMass, 1, SpriteEffects.None, 0.0f);
        }

        /// <summary>
        /// checks if a collision happened
        /// </summary>
        /// <param name="a">a object</param>
        /// <param name="b">a object</param>
        /// <returns>the data to resolve the collision</returns>
        public static CollisionManifold checkCollision(Object2D a, Object2D b)
        {
            if (a.MassData.IMass != 0 || b.MassData.IMass != 0)
            {
                IntersectData data = a.Shape.intersects(b.Shape);
                return new CollisionManifold(a, b, data);
            }
            return new CollisionManifold(a, b, new IntersectData());
        }

        /*
         * generating methods
         */


        /// <summary>
        /// generates an object with a rectangle shape
        /// </summary>
        /// <param name="size">the size of the rectangle</param>
        /// <param name="position">the object's position</param>
        /// <param name="texture">the texture of the object</param>
        /// <param name="mass">the mass of the object</param>
        /// <param name="restitution">defined as: v_after/v_before</param>
        /// <returns>a object with a rectangle shape</returns>
        public static Object2D generateRectangleObject(Point size, Vector2 position, Texture2D texture, MaterialData material)
        {
            Vector2 centerOfMass = new Vector2(size.X / 2, size.Y / 2);
            Shape2D shape = new PolygonShape(size, position);
            return new Object2D(shape, texture, material);
        }

        /// <summary>
        /// generates an object with a polygon shape
        /// </summary>
        /// <param name="corners">corners of the polygon</param>
        /// <param name="position">the object's position</param>
        /// <param name="texture">the texture of the object</param>
        /// <param name="mass">the mass of the object</param>
        /// <param name="restitution">defined as: v_after/v_before</param>
        /// <returns>a object with a polygon shape</returns>
        public static Object2D generateEdgeObject(Vector2[] corners, Vector2 position, Texture2D texture, MaterialData material)
        {
            Vector2 size = Vector2.Zero;
            for (int i = 0; i < corners.Length; ++i)
            {
                size.X = Math.Max(corners[i].X, size.X);
                size.Y = Math.Max(corners[i].Y, size.Y);
            }

            Vector2 centerOfMass = Vector2.Zero;

            Shape2D shape = new PolygonShape(corners, position);
            return new Object2D(shape, texture, material);
        }

        /// <summary>
        /// generates an object with a circle shape
        /// </summary>
        /// <param name="radius">radius of the circle</param>
        /// <param name="position">the object's position</param>
        /// <param name="centerOfMass">the center of mass of the object</param>
        /// <param name="texture">the texture of the object</param>
        /// <param name="mass">the mass of the object</param>
        /// <param name="restitution">defined as: v_after/v_before</param>
        /// <returns>a object with a circle shape</returns>
        public static Object2D generateCircleObject(float radius, Vector2 position, Texture2D texture, MaterialData material)
        {
            Shape2D shape = new CircleShape(radius, position);
            return new Object2D(shape, texture, material);
        }

    }
}
