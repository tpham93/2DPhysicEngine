using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

using PhysicEngine.Collision;
using PhysicEngine.Etc;

namespace PhysicEngine.Object.Shape
{
    class RectangleShape : Shape2D
    {
        Vector2[] corners;

        public Vector2[] Corners
        {
            get { return corners; }
            set { corners = value; }
        }
        Vector2[] currentCorners;

        public Vector2[] CurrentCorners
        {
            get { return currentCorners; }
            set { currentCorners = value; }
        }

        Vector2[] normals;
        Vector2[] currentNormals;
        public Vector2[] CurrentNormals
        {
            get { return currentNormals; }
            set { currentNormals = value; }
        }

        /// <summary>
        /// gets the ObjectType
        /// </summary>
        public override EShapeType ShapeType
        {
            get { return EShapeType.RectangleShape; }
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
                for (int i = 0; i < corners.Length; ++i)
                {
                    currentCorners[i] = currentCorners[i] - position + value;
                }
                this.position = value;
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
                float rotationSin = (float)Math.Sin(rotation);
                float rotationCos = (float)Math.Cos(rotation);
                for (int i = 0; i < corners.Length; ++i)
                {
                    currentCorners[i] = Helper.rotateVector2(corners[i], rotationCos, rotationSin) + position;
                }
                for (int i = 0; i < normals.Length; ++i)
                {
                    currentNormals[i] = Helper.rotateVector2(normals[i], rotationCos, rotationSin);
                }
            }
        }

        public RectangleShape(Point size, Vector2 position, bool moveable = true)
            : base(new Vector2(size.X / 2, size.Y / 2).Length(), position, new Vector2(size.X / 2, size.Y / 2), moveable)
        {
            this.area = size.X * size.Y;
            this.corners = new Vector2[] { new Vector2(0, 0) - MiddlePoint, new Vector2(size.X, 0) - MiddlePoint, new Vector2(size.X, size.Y) - MiddlePoint, new Vector2(0, size.Y) - MiddlePoint };
            currentCorners = new Vector2[4];
            for (int i = 0; i < 4; ++i)
            {
                currentCorners[i] = position + corners[i];
            }
            this.normals = new Vector2[] { Vector2.UnitY, Vector2.UnitX, -Vector2.UnitY, -Vector2.UnitX, };
            this.currentNormals = new Vector2[] { Vector2.UnitY, Vector2.UnitX, -Vector2.UnitY, -Vector2.UnitX, };
        }

        public RectangleShape(Rectangle rect, Point position, bool moveable = true)
            : this(new Point(rect.Width,rect.Height), new Vector2(position.X,position.Y), moveable)
        {
        }

        //public RectangleShape(Point size, Vector2 middlePoint, Vector2 position, bool moveable = true)
        //    : base(middlePoint.Length(), position, middlePoint, moveable)
        //{
        //    this.area = size.X * size.Y;
        //    this.corners = new Vector2[] { new Vector2(0, 0) - middlePoint, new Vector2(size.X, 0) - middlePoint, new Vector2(size.X, size.Y) - middlePoint, new Vector2(0, size.Y) - middlePoint };
        //    currentCorners = new Vector2[4];
        //    for (int i = 0; i < 4; ++i)
        //    {
        //        currentCorners[i] = position + corners[i];
        //    }
        //    this.normals = new Vector2[] { Vector2.UnitY, Vector2.UnitX,-Vector2.UnitY,-Vector2.UnitX, };
        //    this.currentNormals = new Vector2[] { Vector2.UnitY, Vector2.UnitX, -Vector2.UnitY, -Vector2.UnitX, };
        //}

        /// <summary>
        /// calculate the range when projected to a vector
        /// </summary>
        /// <param name="vector">the vector to which the object is beeing projected</param>
        /// <returns>range of the projection</returns>
        public override Range getProjectionRange(Vector2 vector)
        {
            float min = float.PositiveInfinity;
            float max = float.NegativeInfinity;
            for (int i = 0; i < currentCorners.Length; ++i)
            {
                float value = Vector2.Dot(vector, currentCorners[i]);
                if (value < min)
                {
                    min = value;
                }
                if (value > max)
                {
                    max = value;
                }
            }
            return new Range(min, max);
        }

        /// <summary>
        /// checks if the object is intersecting with another CircleObject
        /// </summary>
        /// <param name="o">EdgeObject which is to be checked for an intersection</param>
        /// <returns>true if it intersects</returns>
        public override IntersectData intersects(EdgeShape o)
        {
            VectorData mtv = new VectorData();
            mtv.length = float.PositiveInfinity;

            foreach (Vector2 n in currentNormals)
            {
                Vector2 possibleMtv = n;

                Range r1 = getProjectionRange(possibleMtv);
                Range r2 = o.getProjectionRange(possibleMtv);

                float distance = Range.distance(r1, r2);

                if (distance >= 0)
                {
                    return new IntersectData();
                }
                else if (mtv.length > -distance)
                {
                    mtv.length = -distance;
                    mtv.direction = possibleMtv;
                }

            }
            foreach (Vector2 n in o.CurrentNormals)
            {
                Vector2 possibleMtv = n;

                Range r1 = getProjectionRange(possibleMtv);
                Range r2 = o.getProjectionRange(possibleMtv);

                float distance = Range.distance(r1, r2);

                if (distance >= 0)
                {
                    return new IntersectData();
                }
                else if (mtv.length > -distance)
                {
                    mtv.length = -distance;
                    mtv.direction = possibleMtv;
                }
            }


            if (Vector2.Dot(mtv.direction, o.Position - Position) < 0)
            {
                mtv.direction *= -1.0f;
            }

            return new IntersectData(mtv);
        }


        /// <summary>
        /// checks if the object is intersecting with another CircleObject
        /// </summary>
        /// <param name="o">EdgeObject which is to be checked for an intersection</param>
        /// <returns>true if it intersects</returns>
        public override IntersectData intersects(RectangleShape o)
        {
            VectorData mtv = new VectorData();
            mtv.length = float.PositiveInfinity;

            foreach (Vector2 n in currentNormals)
            {
                Vector2 possibleMtv = n;

                Range r1 = getProjectionRange(possibleMtv);
                Range r2 = o.getProjectionRange(possibleMtv);

                float distance = Range.distance(r1, r2);

                if (distance >= 0)
                {
                    return new IntersectData();
                }
                else if (mtv.length > -distance)
                {
                    mtv.length = -distance;
                    mtv.direction = possibleMtv;
                }

            }
            foreach (Vector2 n in o.CurrentNormals)
            {
                Vector2 possibleMtv = n;

                Range r1 = getProjectionRange(possibleMtv);
                Range r2 = o.getProjectionRange(possibleMtv);

                float distance = Range.distance(r1, r2);

                if (distance >= 0)
                {
                    return new IntersectData();
                }
                else if (mtv.length > -distance)
                {
                    mtv.length = -distance;
                    mtv.direction = possibleMtv;
                }
            }


            if (Vector2.Dot(mtv.direction, o.Position - Position) < 0)
            {
                mtv.direction *= -1.0f;
            }

            return new IntersectData(mtv);
        }

        /// <summary>
        /// checks if the object is intersecting with another CircleObject
        /// </summary>
        /// <param name="o">CircleObject which is to be checked for an intersection</param>
        /// <returns>true if it intersects</returns>
        public override IntersectData intersects(CircleShape o)
        {
            VectorData mtv = new VectorData();
            mtv.length = float.PositiveInfinity;

            for (int i = 0; i < currentCorners.Length; ++i)
            {
                Vector2 possibleMtv = Vector2.Normalize(currentCorners[i] - o.Position);

                Range r1 = getProjectionRange(possibleMtv);
                Range r2 = o.getProjectionRange(possibleMtv);

                float distance = Range.distance(r1, r2);

                if (distance > 0)
                {
                    return new IntersectData();
                }
                else if (mtv.length > -distance)
                {
                    mtv.length = -distance;
                    mtv.direction = possibleMtv;
                }
            }
            foreach (Vector2 n in currentNormals)
            {
                Vector2 possibleMtv = n;

                Range r1 = getProjectionRange(possibleMtv);
                Range r2 = o.getProjectionRange(possibleMtv);

                float distance = Range.distance(r1, r2);

                if (distance > 0)
                {
                    return new IntersectData();
                }
                else if (mtv.length > -distance)
                {
                    mtv.length = -distance;
                    mtv.direction = possibleMtv;
                }
            }


            if (Vector2.Dot(mtv.direction, o.Position - Position) < 0)
            {
                mtv.direction *= -1;
            }

            return new IntersectData(mtv/*, -mtv.direction * o.Radius + o.Position*/);
        }

        /// <summary>
        /// checks if a point is inside of the object
        /// </summary>
        /// <param name="point">Point which needs to be checked</param>
        /// <returns>true if point is inside of the object</returns>
        public override bool contains(Vector2 point)
        {
            throw new NotImplementedException();
        }

        ///// <summary>
        ///// generates a texture based on the input data
        ///// </summary>
        ///// <param name="corners">corners the shape should have in CW</param>
        ///// <param name="fillPoint">the point where the function begins to fill the texture</param>
        ///// <param name="width">the texture's width</param>
        ///// <param name="height">the texture's height</param>
        ///// <param name="color">the tecture's filling color</param>
        ///// <returns>a texture for an EdgeObject</returns>
        //public static Texture2D genTexture(Vector2[] corners, Vector2 fillPoint, int width, int height, Color color)
        //{
        //    Texture2D pixel = new Texture2D(graphicsDevice, 1, 1);
        //    pixel.SetData(new Color[] { Color.White });
        //    RenderTargetBinding[] originalRenderTarget = graphicsDevice.GetRenderTargets();

        //    RenderTarget2D renderTarget = new RenderTarget2D(graphicsDevice, width, height, false, SurfaceFormat.Color, DepthFormat.Depth24Stencil8);
        //    graphicsDevice.SetRenderTarget(renderTarget);

        //    graphicsDevice.Clear(Color.Transparent);

        //    SpriteBatch spriteBatch = new SpriteBatch(graphicsDevice);
        //    spriteBatch.Begin();
        //    for (int i = 1; i <= corners.Length; ++i)
        //    {
        //        Vector2 startPoint = corners[i - 1];
        //        Vector2 edge = corners[i % corners.Length] - corners[i - 1];
        //        spriteBatch.Draw(pixel, new Rectangle((int)startPoint.X, (int)startPoint.Y, (int)Math.Ceiling(edge.Length()), 1), null, color, (float)Helper.getAngleFromVector2(edge), Vector2.Zero, SpriteEffects.None, 0);
        //    }
        //    spriteBatch.End();

        //    graphicsDevice.SetRenderTargets(originalRenderTarget);

        //    Stack<Point> nextPoints = new Stack<Point>();
        //    nextPoints.Push(new Point((int)fillPoint.X, (int)fillPoint.Y));

        //    Color[] pixels = new Color[width * height];

        //    renderTarget.GetData<Color>(pixels);

        //    while (nextPoints.Count > 0)
        //    {
        //        Point currentPos = nextPoints.Pop();
        //        int index = currentPos.X + currentPos.Y * width;
        //        if (index < pixels.Length && pixels[index] == Color.Transparent)
        //        {
        //            pixels[index] = color;
        //            if (currentPos.X > 0 && pixels[index - 1] == Color.Transparent)
        //            {
        //                nextPoints.Push(new Point(currentPos.X - 1, currentPos.Y));
        //            }
        //            if (currentPos.X < width - 1 && pixels[index + 1] == Color.Transparent)
        //            {
        //                nextPoints.Push(new Point(currentPos.X + 1, currentPos.Y));
        //            }
        //            if (currentPos.Y > 0 && pixels[index - width] == Color.Transparent)
        //            {
        //                nextPoints.Push(new Point(currentPos.X, currentPos.Y - 1));
        //            }
        //            if (currentPos.Y < height - 1 && pixels[index + width] == Color.Transparent)
        //            {
        //                nextPoints.Push(new Point(currentPos.X, currentPos.Y + 1));
        //            }
        //        }
        //    }
        //    renderTarget.SetData<Color>(pixels);

        //    return renderTarget;
        //}


        public static Vector2[] genCorners(Vector2 rectangleSize)
        {
            Vector2[] corners = new Vector2[4];

            corners[0] = new Vector2(0, 0);
            corners[1] = new Vector2(0, rectangleSize.Y * 1);
            corners[2] = new Vector2(rectangleSize.X * 1, rectangleSize.Y * 1);
            corners[3] = new Vector2(rectangleSize.X * 1, 0);

            return corners;
        }
    }
}
