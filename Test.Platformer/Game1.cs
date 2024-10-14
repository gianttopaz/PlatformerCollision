using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;

namespace Test.Platformer
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;


        private Dictionary<Vector2, int> tilemap;
        private Dictionary<Vector2, int> collisions;
        private List<Rectangle> textureStore;
        private Texture2D textureMap;
        private Texture2D collisionMap;
        private Vector2 camera;

        int tilesize = 16;


        // sprite

        private Sprite player;
        private List<Rectangle> intersections;

        private Texture2D rectangleTexture;

        private KeyboardState prevKBState;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            tilemap = LoadMap("../../../Data/map1_main.csv");
            collisions = LoadMap("../../../Data/map1_collisions.csv");
            camera = Vector2.Zero;
            intersections = new();

        }

        private Dictionary<Vector2, int> LoadMap(string filepath)
        {
            Dictionary<Vector2, int> result = new();

            StreamReader reader = new(filepath);

            int y = 0;
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                string[] items = line.Split(',');

                for (int x = 0; x < items.Length; x++)
                {
                    if (int.TryParse(items[x], out int value))
                    {
                        if (value > -1)
                        {
                            result[new Vector2(x, y)] = value;
                        }
                    }

                }
                y++;
            }
            return result;
        }
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            var animations = new Dictionary<string, Animation>()
            {
                {"RunDown", new Animation(Content.Load<Texture2D>("PlayerRunDown"), 4) },
                {"RunUp", new Animation(Content.Load<Texture2D>("PlayerRunUp"), 4) },
                {"RunLeft", new Animation(Content.Load<Texture2D>("PlayerRunLeft"), 6) },
                {"RunRight", new Animation(Content.Load<Texture2D>("PlayerRunRight"), 6) },
            };

            textureMap = Content.Load<Texture2D>("TexturePack");
            collisionMap = Content.Load<Texture2D>("TextureCollisionPack");

            player = new Sprite(animations, new Rectangle(0,0,16,32));
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();


                player.Update(Keyboard.GetState(), prevKBState, gameTime);
            player.isClinging = false;

            prevKBState = Keyboard.GetState();

            // add player's Velocity and grab the intersecting tiles
            player.rect.X += (int)player.velocity.X;
            intersections = getIntersectingTilesHorizontal(player.rect);

            foreach (var rect in intersections)
            {

                // handle collisions if the tile position exists in the tile map layer.
                if (collisions.TryGetValue(new Vector2(rect.X, rect.Y), out int _val))
                {

                    // create temp rect to handle collisions (not necessary, you can optimize!)
                    Rectangle collision = new(
                        rect.X * tilesize,
                        rect.Y * tilesize,
                        tilesize,
                        tilesize
                    );

                    if (!player.rect.Intersects(collision))
                    {
                        continue;
                    }

                    // handle collisions based on the direction the player is moving
                    if (player.velocity.X > 0.0f)
                    {
                        player.rect.X = collision.Left - player.rect.Width;
                        player.isClinging = true;
                    }
                    else if (player.velocity.X < 0.0f)
                    {
                        player.rect.X = collision.Right;
                        player.isClinging = true;
                    }

                }

            }

            // same as horizontal collisions

            player.rect.Y += (int)player.velocity.Y;
            intersections = getIntersectingTilesVertical(player.rect);
            player.isGrounded = false;

            foreach (var rect in intersections)
            {
                if (collisions.TryGetValue(new Vector2(rect.X, rect.Y), out int _val))
                {

                    // create temp rect to handle collisions (not necessary, you can optimize!)
                    Rectangle collision = new(
                        rect.X * tilesize,
                        rect.Y * tilesize,
                        tilesize,
                        tilesize
                    );

                    if (!player.rect.Intersects(collision))
                    {
                        continue;
                    }


                    // handle collisions based on the direction the player is moving
                    if (player.velocity.Y > 0.0f)
                    {
                        player.rect.Y = collision.Top - player.rect.Height;
                        player.velocity.Y = 2f;
                        player.isGrounded = true;
                    }
                    else if (player.velocity.Y < 0.0f)
                    {
                        player.rect.Y = collision.Bottom;
                        player.velocity.Y = -1.0f;

                    }

                }
            }

            base.Update(gameTime);
        }

        // wabs the intersecting tiles for a Rect. This grabs all tile positions where
        // an intersection is __possible__, not if a tile actually exists there.
        public List<Rectangle> getIntersectingTilesHorizontal(Rectangle target)
        {

            List<Rectangle> intersections = new();

            int widthInTiles = (target.Width - (target.Width % tilesize)) / tilesize;
            int heightInTiles = (target.Height - (target.Height % tilesize)) / tilesize;

            for (int x = 0; x <= widthInTiles; x++)
            {
                for (int y = 0; y <= heightInTiles; y++)
                {

                    intersections.Add(new Rectangle(

                        (target.X + x * tilesize) / tilesize,
                        (target.Y + y * (tilesize - 1)) / tilesize,
                        tilesize,
                        tilesize

                    ));

                }
            }

            return intersections;
        }
        public List<Rectangle> getIntersectingTilesVertical(Rectangle target)
        {

            List<Rectangle> intersections = new();

            int widthInTiles = (target.Width - (target.Width % tilesize)) / tilesize;
            int heightInTiles = (target.Height - (target.Height % tilesize)) / tilesize;

            for (int x = 0; x <= widthInTiles; x++)
            {
                for (int y = 0; y <= heightInTiles; y++)
                {

                    intersections.Add(new Rectangle(

                        (target.X + x * (tilesize - 1)) / tilesize,
                        (target.Y + y * tilesize) / tilesize,
                        tilesize,
                        tilesize

                    ));

                }
            }

            return intersections;
        }


        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            // TODO: Add your drawing code here
            _spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp);

            int numTilesPerRow = 4;

            foreach (var item in collisions)
            {
                Rectangle dest = new(
                    (int)item.Key.X * tilesize,
                    (int)item.Key.Y * tilesize,
                    tilesize,
                    tilesize
                );
                int x = item.Value % numTilesPerRow;
                int y = item.Value / numTilesPerRow;
                Rectangle src = new(
                    x * tilesize,
                    y * tilesize,
                    tilesize,
                    tilesize
                );

                _spriteBatch.Draw(collisionMap, dest, src, Color.White);
            }
            //repeat for background tiles
            foreach (var item in tilemap)
            {
                Rectangle dest = new(
                    (int)item.Key.X * tilesize,
                    (int)item.Key.Y * tilesize,
                    tilesize,
                    tilesize
                );
                int x = item.Value % numTilesPerRow;
                int y = item.Value / numTilesPerRow;
                Rectangle src = new(
                    x * tilesize,
                    y * tilesize,
                    tilesize,
                    tilesize
                );

                _spriteBatch.Draw(textureMap, dest, src, Color.White);
            }

            //foreach (var rect in intersections)
            //{

            //    DrawRectHollow(
            //        _spriteBatch,
            //        new Rectangle(
            //            rect.X * tilesize,
            //            rect.Y * tilesize,
            //            tilesize,
            //            tilesize
            //        ),
            //        4
            //    );

            //}

            player.Draw(_spriteBatch);

            //DrawRectHollow(_spriteBatch, player.rect, 4);

            _spriteBatch.End();

            //Console.WriteLine(player.stopwatch.Elapsed.ToString());
            Console.WriteLine(player.wallJump);

            base.Draw(gameTime);
        }

        //public void DrawRectHollow(SpriteBatch spriteBatch, Rectangle rect, int thickness)
        //{
        //    spriteBatch.Draw(
        //        rectangleTexture,
        //        new Rectangle(
        //            rect.X,
        //            rect.Y,
        //            rect.Width,
        //            thickness
        //        ),
        //        Color.White
        //    );
        //    spriteBatch.Draw(
        //        rectangleTexture,
        //        new Rectangle(
        //            rect.X,
        //            rect.Bottom - thickness,
        //            rect.Width,
        //            thickness
        //        ),
        //        Color.White
        //    );
        //    spriteBatch.Draw(
        //        rectangleTexture,
        //        new Rectangle(
        //            rect.X,
        //            rect.Y,
        //            thickness,
        //            rect.Height
        //        ),
        //        Color.White
        //    );
        //    spriteBatch.Draw(
        //        rectangleTexture,
        //        new Rectangle(
        //            rect.Right - thickness,
        //            rect.Y,
        //            thickness,
        //            rect.Height
        //        ),
        //        Color.White
        //    );
        //}

    }
}
