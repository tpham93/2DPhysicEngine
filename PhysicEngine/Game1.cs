using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

using PhysicEngine.Etc;
using PhysicEngine.Objects;
using PhysicEngine.Object.Shape;
using PhysicEngine.Collision;
using PhysicEngine.ObjectData.Objects;

namespace PhysicEngine
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        Input input;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            graphics.SynchronizeWithVerticalRetrace = false;
            IsFixedTimeStep = false;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            IsMouseVisible = true;
            input = new Input(Window);
            base.Initialize();
        }
        List<Object2D> objects;


        Vector2[] corners = new Vector2[] { new Vector2(0, 6), new Vector2(0, 30), new Vector2(40, 30), new Vector2(40, 6), new Vector2(20, 0) };
        
        
        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            Helper.Initialize(GraphicsDevice);

            texture = Helper.genCircleTexture(8, Color.Transparent, Color.Red, 1);
            //texture = Helper.genPolygonTexture(corners, Color.Transparent, Color.Red, 1);
            objects = new List<Object2D>();

            objects.Add(Object2D.generateCircleObject(8, new Vector2(300, 1), Helper.genCircleTexture(8, Color.White, Color.Black, 1),MaterialData.Stone));
            objects.Add(Object2D.generateCircleObject(8, new Vector2(350, 1), Helper.genCircleTexture(8, Color.White, Color.Black, 1), MaterialData.Stone));
            objects.Add(Object2D.generateCircleObject(8, new Vector2(400, 1), Helper.genCircleTexture(8, Color.White, Color.Black, 1), MaterialData.Stone));
            objects.Add(Object2D.generateCircleObject(8, new Vector2(300, 0), Helper.genCircleTexture(8, Color.White, Color.Black, 1), MaterialData.Stone));
            objects.Add(Object2D.generateCircleObject(8, new Vector2(350, 10), Helper.genCircleTexture(8, Color.White, Color.Black, 1), MaterialData.Stone));
            objects.Add(Object2D.generateCircleObject(8, new Vector2(400, 30), Helper.genCircleTexture(8, Color.White, Color.Black, 1), MaterialData.Stone));
            objects.Add(Object2D.generateCircleObject(8, new Vector2(450, 50), Helper.genCircleTexture(8, Color.White, Color.Black, 1), MaterialData.Stone));

            MaterialData groundMaterial = new MaterialData(float.PositiveInfinity, 1, 0.05f, 0.03f);

            objects.Add(Object2D.generateRectangleObject(new Point(800, 20), new Vector2(400, 400), Helper.genRectangleTexture(800, 20, Color.Transparent, Color.White), groundMaterial));
            objects.Add(Object2D.generateRectangleObject(new Point(800, 20), new Vector2(400, -10), Helper.genRectangleTexture(800, 20, Color.Transparent, Color.White), groundMaterial));
            objects.Add(Object2D.generateRectangleObject(new Point(20, 800), new Vector2(810, 240), Helper.genRectangleTexture(20, 800, Color.Transparent, Color.White), groundMaterial));
            objects.Add(Object2D.generateRectangleObject(new Point(20, 800), new Vector2(-10, 240), Helper.genRectangleTexture(20, 800, Color.Transparent, Color.White), groundMaterial));
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }
        Texture2D texture;
        TimeSpan spawntime = TimeSpan.FromSeconds(0.25);
        TimeSpan lastSpawn = TimeSpan.Zero;
        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            Window.Title = "Objects: " + objects.Count + " FPS: " + (int)(1 / gameTime.ElapsedGameTime.TotalSeconds);

            input.Update();

            lastSpawn += gameTime.ElapsedGameTime;
            if(lastSpawn >= spawntime && input.mouseInsideWindow() && input.mouseButtonClicked(Input.EMouseButton.LeftButton))
            {
                objects.Add(Object2D.generateCircleObject(8, input.mousePositionV2(), texture, MaterialData.Stone));

                //objects.Add(Object2D.generateEdgeObject(corners,input.mousePositionV2(),texture,MaterialData.Stone));


                lastSpawn = TimeSpan.Zero;
            }


            for (int i = 0; i < objects.Count; ++i)
            {
                objects[i].Update(gameTime.ElapsedGameTime);
            }
            for (int i = 0; i < objects.Count; ++i)
            {
                for (int j = i + 1; j < objects.Count; ++j)
                {
                    CollisionManifold manifold = Object2D.checkCollision(objects[i], objects[j]);
                    manifold.resolveCollision();
                }
            }
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            Texture2D pixel = Helper.genRectangleTexture(1, 1, Color.White, Color.White);
            // TODO: Add your drawing code here
            spriteBatch.Begin();
            for (int i = 0; i < objects.Count; ++i)
            {
                objects[i].Draw(spriteBatch);
                spriteBatch.Draw(pixel, objects[i].Position, Color.White);
            }
            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
