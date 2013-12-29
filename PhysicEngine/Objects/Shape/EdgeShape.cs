using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using PhysicEngine.Etc;
using PhysicEngine.Collision;

namespace PhysicEngine.Object.Shape
{
    class PolygonShape : Shape2D
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
        List<Vector2> normals;
        List<Vector2> currentNormals;

        public List<Vector2> CurrentNormals
        {
            get { return currentNormals; }
            set { currentNormals = value; }
        }

        /// <summary>
        /// gets the ObjectType
        /// </summary>
        public override EShapeType ShapeType
        {
            get { return EShapeType.PolygonShape; }
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
                for (int i = 0; i < normals.Count; ++i)
                {
                    currentNormals[i] = Helper.rotateVector2(normals[i], rotationCos, rotationSin);
                }
            }
        }

        public PolygonShape(Vector2[] corners, Vector2 position, bool moveable = true)
            : base(0, position, Vector2.Zero, moveable)
        {

            float area = 0.0f;
            Vector2 middlePoint = Vector2.Zero;
            for (int i = 0; i < corners.Length; ++i)
            {
                int j = (i + 1) % corners.Length;
                /// (xi * yi+1 - xi+1 * yi)
                float tmp = (corners[i].X * corners[j].Y) - (corners[j].X * corners[i].Y);
                area += tmp;
                middlePoint += (corners[i] + corners[j]) * tmp;
            }

            area /= 2f;
            area = Math.Abs(area);
            this.area = area;

            middlePoint /= 6 * area;
            this.MiddlePoint = new Vector2(Math.Abs(middlePoint.X), Math.Abs(middlePoint.Y));
            this.radius = MiddlePoint.Length();

            this.corners = new Vector2[corners.Length];
            this.currentCorners = new Vector2[corners.Length];
            this.normals = new List<Vector2>(corners.Length);
            this.currentNormals = new List<Vector2>(corners.Length);

            for (int i = 0; i < corners.Length; ++i)
            {
                this.corners[i] = new Vector2(corners[i].X, corners[i].Y) - MiddlePoint;
                this.currentCorners[i] = this.corners[i] + position;
            }
            for (int i = 1; i <= corners.Length; ++i)
            {
                Vector2 edge = this.corners[i - 1] - this.corners[i % corners.Length];
                Vector2 normal = Vector2.Normalize(new Vector2(-edge.Y, edge.X));
                if (!normals.Contains(normal))
                {
                    normals.Add(normal);
                    currentNormals.Add(normal);
                }
            }
        }

        public PolygonShape(Point size, Vector2 position, bool moveable = true)
            : base(new Vector2(size.X / 2, size.Y / 2).Length(), position, new Vector2(size.X / 2, size.Y / 2), moveable)
        {
            this.area = size.X * size.Y;
            this.corners = new Vector2[] { new Vector2(0, 0) - MiddlePoint, new Vector2(size.X, 0) - MiddlePoint, new Vector2(size.X, size.Y) - MiddlePoint, new Vector2(0, size.Y) - MiddlePoint };
            currentCorners = new Vector2[4];
            for (int i = 0; i < 4; ++i)
            {
                currentCorners[i] = position + corners[i];
            }
            this.normals = new List<Vector2>();
            normals.AddRange(new Vector2[] { Vector2.UnitY, Vector2.UnitX});
            this.currentNormals = new List<Vector2>();
            currentNormals.AddRange(new Vector2[] { Vector2.UnitY, Vector2.UnitX});
        }

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
        public override IntersectData intersects(PolygonShape o)
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
            foreach (Vector2 n in o.currentNormals)
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

            return new IntersectData(mtv);
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

        public static Vector2[] genCorners(Vector2 rectangleSize)
        {
            Vector2[] corners = new Vector2[4];

            corners[0] = new Vector2(0, 0);
            corners[1] = new Vector2(0, rectangleSize.Y);
            corners[2] = new Vector2(rectangleSize.X, rectangleSize.Y);
            corners[3] = new Vector2(rectangleSize.X, 0);

            return corners;
        }

    }
}
