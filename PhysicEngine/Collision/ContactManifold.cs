using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

using PhysicEngine.Object.Shape;

namespace PhysicEngine.Collision
{

    class ContactManifold
    {
        private List<Vector2> contactPoints;

        public List<Vector2> ContactPoints
        {
            get { return contactPoints; }
        }

        public ContactManifold()
        {
            contactPoints = new List<Vector2>();
        }
        public ContactManifold(Shape2D shape1, Shape2D shape2, Vector2 n)
        {
            if (shape1.ShapeType == EShapeType.PolygonShape && shape2.ShapeType == EShapeType.PolygonShape)
            {
                contactPoints = getContactPoints((PolygonShape)shape1, (PolygonShape)shape2, n);
            }
            else if (shape1.ShapeType == EShapeType.CircleShape && shape2.ShapeType == EShapeType.PolygonShape)
            {
                contactPoints = getContactPoints((CircleShape)shape1, (PolygonShape)shape2, n);
            }
            else if (shape1.ShapeType == EShapeType.PolygonShape && shape2.ShapeType == EShapeType.CircleShape)
            {
                contactPoints = getContactPoints((CircleShape)shape2, (PolygonShape)shape1, -n);
            }
            else
            {
                contactPoints = getContactPoints((CircleShape)shape1, (CircleShape)shape2, n);
            }
        }

        public static List<Vector2> getContactPoints(CircleShape shape1, CircleShape shape2, Vector2 n)
        {
            List<Vector2> contactPoints = new List<Vector2>();

            contactPoints.Add(shape1.Position + Vector2.Normalize(shape2.Position - shape1.Position) * shape1.Radius);

            return contactPoints;
        }
        public static List<Vector2> getContactPoints(CircleShape shape1, PolygonShape shape2, Vector2 n)
        {
            List<Vector2> contactPoints = new List<Vector2>();

            contactPoints.Add(shape1.Position + n * shape1.Radius);

            return contactPoints;
        }

        public static List<Vector2> getContactPoints(PolygonShape shape1, PolygonShape shape2, Vector2 n)
        {
            Edge e1 = getBestEdge(shape1, n);
            Edge e2 = getBestEdge(shape2, -n);

            Edge refEdge;
            Edge incEdge;
            bool flipped = false;

            if (Vector2.Dot(e1.Direction, n) <= Vector2.Dot(e2.Direction, n))
            {
                refEdge = e1;
                incEdge = e2;
            }
            else
            {
                refEdge = e2;
                incEdge = e1;
                flipped = true;
            }

            Vector2 refDirection = refEdge.Direction;

            refDirection.Normalize();
            float o1 = Vector2.Dot(refDirection, refEdge.V1);
            List<Vector2> clippedPoints = clip(incEdge.V1, incEdge.V2, refDirection, o1);
            if (clippedPoints.Count < 2)
            {
                return new List<Vector2>();
            }

            float o2 = Vector2.Dot(refDirection, refEdge.V2);
            clippedPoints = clip(clippedPoints[0], clippedPoints[1], -refDirection, -o2);
            if (clippedPoints.Count < 2)
            {
                return new List<Vector2>();
            }

            Vector2 refNorm = new Vector2(-refDirection.Y, refDirection.X);
            if (flipped)
            {
                refNorm *= -1;
            }
            float max = Vector2.Dot(refNorm, refEdge.Max);

            if (Vector2.Dot(refNorm, clippedPoints[1]) - max < 0)
            {
                clippedPoints.RemoveAt(1);
            }
            if (Vector2.Dot(refNorm, clippedPoints[0]) - max < 0)
            {
                clippedPoints.RemoveAt(0);
            }

            return clippedPoints;
        }

        public static Edge getBestEdge(PolygonShape shape, Vector2 n)
        {
            Vector2[] currentCorners = shape.CurrentCorners;
            float max = Vector2.Dot(currentCorners[0],n);
            int index = 0;

            for (int i = 1; i < currentCorners.Length;++i )
            {
                float projection = Vector2.Dot(currentCorners[i], n);
                if (projection > max)
                {
                    index = i;
                    max = projection;
                }
            }

            Vector2 v = currentCorners[index];
            Vector2 v0 = currentCorners[(currentCorners.Length + index - 1) % currentCorners.Length];
            Vector2 v1 = currentCorners[(index + 1) % currentCorners.Length];

            Vector2 l = v - v1;
            Vector2 r = v - v0;

            if (Vector2.Dot(r, n) <= Vector2.Dot(l, n))
            {
                return new Edge(v,v0,v);
            }
            else
            {
                return new Edge(v, v, v1);
            }
        }

        public static List<Vector2> clip(Vector2 v1, Vector2 v2, Vector2 n, float o)
        {
            List<Vector2> clippedPoints = new List<Vector2>();

            float d1 = Vector2.Dot(v1, n) - o;
            float d2 = Vector2.Dot(v2, n) - o;

            if (d1 >= 0)
            {
                clippedPoints.Add(v1);
            }
            if (d2 >= 0)
            {
                clippedPoints.Add(v2);
            }

            if (d1 * d2 < 0.0f)
            {
                Vector2 e = v2 - v1;
                e *= d1 / (d1 - d2);
                e += v1;
                clippedPoints.Add(e);
            }

            return clippedPoints;
        }

    }
}
