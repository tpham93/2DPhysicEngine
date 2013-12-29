using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using PhysicEngine.Collision;
using PhysicEngine.Etc;

namespace PhysicEngine.Object.Shape
{
    class CircleShape : Shape2D
    {
        /// <summary>
        /// gets & sets the radius
        /// </summary>
        public float Radius
        {
            get { return radius; }
        }

        /// <summary>
        /// gets the position
        /// </summary>
        public override EShapeType ShapeType
        {
            get { return EShapeType.CircleShape; }
        }

        /// <summary>
        /// gets & sets the position
        /// </summary>
        public override Vector2 Position
        {
            get
            {
                return position;
            }
            set
            {
                position = value;
            }
        }

        /// <summary>
        /// gets & sets the rotation
        /// </summary>
        public override float Rotation
        {
            get
            {
                return rotation;
            }
            set
            {
                this.rotation = value;
            }
        }

        public CircleShape(float radius, Vector2 position, bool moveable = true)
            : base(radius, position, new Vector2(radius), moveable)
        {
            this.area = MathHelper.Pi * radius * radius;
        }


        /// <summary>
        /// projects the object to a given vector
        /// </summary>
        /// <param name="vector">the vector where the object needs to be projected</param>
        /// <returns>the Range of the projection</returns>
        public override Range getProjectionRange(Vector2 vector)
        {
            float baseValue = Vector2.Dot(vector, Position);
            return new Range(baseValue - radius, baseValue + radius);
        }

        /// <summary>
        /// checks if the object is intersecting with another EdgeObject
        /// </summary>
        /// <param name="o">EdgeObject which is to be checked for an intersection</param>
        /// <returns>true if it intersects</returns>
        public override IntersectData intersects(PolygonShape o)
        {
            IntersectData intersectData = o.intersects(this);
            if (intersectData.Intersects)
            {

                return intersectData;

            }
            else return new IntersectData();
        }

        /// <summary>
        /// checks if the object is intersecting with another CircleObject
        /// </summary>
        /// <param name="o">CircleObject which is to be checked for an intersection</param>
        /// <returns>true if it intersects</returns>
        public override IntersectData intersects(CircleShape o)
        {
            float minDistance = (radius + o.radius);
            Vector2 mtv = (Position - o.Position);
            float realDistance = mtv.Length();
            bool intersects = realDistance < minDistance;
            mtv /= realDistance;
            if (!intersects)
            {
                return new IntersectData();
            }

            return new IntersectData(new VectorData(mtv, minDistance - realDistance));
        }

        /// <summary>
        /// checks if a point is inside of the object
        /// </summary>
        /// <param name="point">Point which needs to be checked</param>
        /// <returns>true if point is inside of the object</returns>
        public override bool contains(Vector2 point)
        {
            return (Position - point).LengthSquared() <= radius * radius;
        }

        /// <summary>
        /// generates a circle texture
        /// </summary>
        /// <param name="radius">the texture's radius</param>
        /// <param name="color">the texture's color</param>
        /// <returns>a circle texture</returns>
        public static Texture2D genTexture(float r, Color color)
        {
            int radius = (int)Math.Ceiling(r);
            Texture2D texture = new Texture2D(graphicsDevice, 2 * radius, 2 * radius);
            Color[] pixels = new Color[2 * 2 * radius * radius];

            for (int y = 0; y < 2 * radius; ++y)
            {
                for (int x = 0; x < 2 * radius; ++x)
                {
                    int index = x + y * 2 * radius;
                    int rx = x - radius;
                    int ry = y - radius;
                    if (rx * rx + ry * ry <= r * r)
                    {
                        pixels[index] = color;
                    }
                    else
                    {
                        pixels[index] = Color.Transparent;
                    }
                    if (x == radius)
                        pixels[index] = Color.Black;
                }
            }

            texture.SetData<Color>(pixels);

            return texture;
        }
    }
}
