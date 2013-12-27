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
using PhysicEngine.Shape;
using PhysicEngine.Collision;

namespace PhysicEngine
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
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
            base.Initialize();
        }
        List<Object2D> objects;
        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            Helper.Initialize(GraphicsDevice);
            Shape2D shape = new CircleShape(40, new Vector2(399, 0));
            Shape2D shape1 = new CircleShape(40, new Vector2(400, 400));
            Shape2D shape2 = new CircleShape(40, new Vector2(600, 0));

            objects = new List<Object2D>();

            objects.Add(new Object2D(shape, Helper.genCircleTexture(80, Color.White, Color.Black, 2), 10f, new Vector2(40), 1));
            objects.Add(new Object2D(shape1, Helper.genCircleTexture(80, Color.White, Color.Black, 2), float.PositiveInfinity, new Vector2(20), 1));
            objects.Add(new Object2D(shape2, Helper.genCircleTexture(80, Color.White, Color.Black, 2), 3f, new Vector2(40), 1));
            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

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

            // TODO: Add your update logic here
            for (int i = 0; i < objects.Count; ++i)
            {
                objects[i].Update(gameTime);
            }
            for (int i = 0; i < objects.Count; ++i)
            {
                for (int j = i+1; j < objects.Count; ++j)
                {
                    CollisionManifold manifold = Object2D.checkCollision(objects[i],objects[j]);
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
            GraphicsDevice.Clear(Color.CornflowerBlue);
            Texture2D pixel = Helper.genRectangleTexture(1, 1, Color.Black, Color.Black);
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
