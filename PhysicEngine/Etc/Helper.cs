using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PhysicEngine.Etc
{
    class Helper
    {

        static GraphicsDevice graphicsDevice;

        public static void Initialize(GraphicsDevice graphicsDevice)
        {
            Helper.graphicsDevice = graphicsDevice;
        }

        /// <summary>
        /// rotates a vector
        /// </summary>
        /// <param name="vector">vector which needs to be rotated</param>
        /// <param name="rotation">angle in rad</param>
        /// <returns>rotated vector</returns>
        public static Vector2 rotateVector2(Vector2 vector, float rotation)
        {
            // multplicate vector with rotationmatrix
            // (cos(a) -sin(a))
            // (sin(a)  cos(a))
            return rotateVector2(vector, (float)Math.Cos(rotation), (float)Math.Sin(rotation));
        }

        /// <summary>
        /// rotates a vector
        /// </summary>
        /// <param name="vector">vector which needs to be rotated</param>
        /// <param name="rotationCos">cos of the angle</param>
        /// <param name="rotationSin">sin of the angle</param>
        /// <returns>rotated vector</returns>
        public static Vector2 rotateVector2(Vector2 vector, float rotationCos, float rotationSin)
        {
            // multplicate vector with rotationmatrix
            // (cos(a) -sin(a))
            // (sin(a)  cos(a))
            Vector2 tmp = Vector2.Normalize(new Vector2((vector.X * rotationCos + vector.Y * -rotationSin), (vector.X * rotationSin + vector.Y * rotationCos)));
            return tmp * vector.Length();
        }

        // calculates the angle of an Vector relatively to the y axis
        /// <summary>
        /// extracts the angle of a vector relatively to the y-axis
        /// </summary>
        /// <param name="v">the vector, from which the angle needs to be extracted</param>
        /// <returns>angle of the vector relatively to the y axis</returns>
        public static double getAngleFromVector2(Vector2 v)
        {
            return Math.Atan2(v.Y, v.X);
        }

        /// <summary>
        /// calculates the crossproduct of 2 dimensional vectors
        /// </summary>
        /// <param name="a">1st vector</param>
        /// <param name="b">2nd vector</param>
        /// <returns>scalar of the crossproduct</returns>
        public static float crossProduct(Vector2 a, Vector2 b)
        {
            return a.X * b.Y - a.Y * b.X;
        }

        /// <summary>
        /// generates a rectangle texture
        /// </summary>
        /// <param name="width">width of the rectangle</param>
        /// <param name="height">height of the rectangle</param>
        /// <param name="color">color of the rectangle</param>
        /// <param name="outline">outline color of the rectangle</param>
        /// <returns>a rectangle texture</returns>
        public static Texture2D genRectangleTexture(int width, int height, Color color, Color outline)
        {
            RenderTarget2D renderTarget = new RenderTarget2D(graphicsDevice, width, height);

            Color[] pixel = new Color[width * height];

            for (int y = 0; y < height; ++y)
            {
                for (int x = 0; x < width; ++x)
                {
                    if (x == 0 || y == 0 || x == width - 1 || y == height - 1)
                    {
                        pixel[x + y * width] = outline;
                    }
                    else
                    {
                        pixel[x + y * width] = color;
                    }
                }
            }
            renderTarget.SetData<Color>(pixel);
            return renderTarget;
        }

        /// <summary>
        /// generate a texture, with given corners
        /// </summary>
        /// <param name="corners">corners of the polygon</param>
        /// <param name="color">the inner color</param>
        /// <param name="outline">the color outline</param>
        /// <param name="outlineThickness">the width of the outline</param>
        /// <returns>a texture for the given corners</returns>
        public static Texture2D genPolygonTexture(Vector2[] corners, Color color, Color outline, int outlineWidth = 1)
        {

            Point maxPoint = Point.Zero;
            Point minPoint = new Point(Int32.MaxValue, Int32.MaxValue);

            for (int i = 0; i < corners.Length; ++i)
            {
                minPoint.X = Math.Min((int)corners[i].X, minPoint.X);
                minPoint.Y = Math.Min((int)corners[i].Y, minPoint.Y);

                maxPoint.X = Math.Max((int)corners[i].X, maxPoint.X);
                maxPoint.Y = Math.Max((int)corners[i].Y, maxPoint.Y);
            }

            Rectangle r = new Rectangle(minPoint.X, minPoint.Y, maxPoint.X - minPoint.X, maxPoint.Y - minPoint.Y);

            return genPolygonTexture(corners, r.Center, r.Right, r.Bottom, color, outline, outlineWidth);
        }

        /// <summary>
        /// generates a texture based on the input data
        /// </summary>
        /// <param name="corners">corners the shape should have in CW</param>
        /// <param name="fillPoint">the point where the function begins to fill the texture</param>
        /// <param name="width">the texture's width</param>
        /// <param name="height">the texture's height</param>
        /// <param name="color">the tecture's filling color</param>
        /// <param name="outline">the color outline</param>
        /// <param name="outlineThickness">the width of the outline</param>
        /// <returns>a texture for an EdgeObject</returns>
        public static Texture2D genPolygonTexture(Vector2[] corners, Point fillPoint, int width, int height, Color color, Color outline, int outlineWidth)
        {
            Texture2D pixel = new Texture2D(graphicsDevice, 1, 1);
            pixel.SetData(new Color[] { Color.White });
            RenderTargetBinding[] originalRenderTarget = graphicsDevice.GetRenderTargets();

            RenderTarget2D renderTarget = new RenderTarget2D(graphicsDevice, width, height, false, SurfaceFormat.Color, DepthFormat.Depth24Stencil8);
            graphicsDevice.SetRenderTarget(renderTarget);

            graphicsDevice.Clear(Color.Transparent);

            SpriteBatch spriteBatch = new SpriteBatch(graphicsDevice);
            spriteBatch.Begin();
            for (int i = 1; i <= corners.Length; ++i)
            {
                Vector2 startPoint = corners[i - 1];
                Vector2 edge = corners[i % corners.Length] - corners[i - 1];
                spriteBatch.Draw(pixel, new Rectangle((int)startPoint.X, (int)startPoint.Y, (int)Math.Ceiling(edge.Length()), outlineWidth), null, outline, (float)Helper.getAngleFromVector2(edge), Vector2.Zero, SpriteEffects.None, 0);
            }
            spriteBatch.End();

            graphicsDevice.SetRenderTargets(originalRenderTarget);

            Stack<Point> nextPoints = new Stack<Point>();
            nextPoints.Push(new Point((int)fillPoint.X, (int)fillPoint.Y));

            bool[] checkedPixel = new bool[width * height];
            Color[] pixels = new Color[width * height];

            renderTarget.GetData<Color>(pixels);

            while (nextPoints.Count > 0)
            {
                Point currentPos = nextPoints.Pop();
                int index = currentPos.X + currentPos.Y * width;
                checkedPixel[index] = true;
                if (pixels[index] != outline)
                {
                    pixels[index] = color;
                    if (currentPos.X > 0 && !checkedPixel[index - 1])
                    {
                        nextPoints.Push(new Point(currentPos.X - 1, currentPos.Y));
                    }
                    if (currentPos.X < width - 1 && !checkedPixel[index + 1])
                    {
                        nextPoints.Push(new Point(currentPos.X + 1, currentPos.Y));
                    }
                    if (currentPos.Y > 0 && !checkedPixel[index - width])
                    {
                        nextPoints.Push(new Point(currentPos.X, currentPos.Y - 1));
                    }
                    if (currentPos.Y < height - 1 && !checkedPixel[index + width])
                    {
                        nextPoints.Push(new Point(currentPos.X, currentPos.Y + 1));
                    }
                }
            }
            renderTarget.SetData<Color>(pixels);

            return renderTarget;
        }


        private static int inCircle(int radius, Point location, Point middlePoint)
        {
            int x = location.X - middlePoint.X;
            int y = location.Y - middlePoint.Y;

            int distance = x * x + y * y;
            return distance - radius * radius;
        }

        public static Texture2D genCircleTexture(int size, Color color, int outlineWidth = 1)
        {
            return genCircleTexture(size, color, color, outlineWidth);
        }
        public static Texture2D genCircleTexture(int radius, Color basicColor, Color borderColor, int outlineWidth)
        {
            int size = 2 * radius;
            Texture2D texture = new Texture2D(graphicsDevice, size, size);
            Color[] data = new Color[size * size];
            Point middlePoint = new Point(radius, radius);

            for (int x = 0; x < size; x++)
            {
                for (int y = 0; y < size; y++)
                {
                    int circleState = 0;

                    circleState = inCircle(radius, new Point(x, y), middlePoint);
                    if (circleState <= 0)
                    {
                        data[x + y * size] = borderColor;
                    }

                    circleState = inCircle(radius - outlineWidth, new Point(x, y), middlePoint);
                    if (circleState <= 0)
                    {
                        data[x + y * size] = basicColor;
                    }
                }
            }

            texture.SetData<Color>(data);

            return texture;
        }
    }
}
