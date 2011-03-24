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
using TileEngine;

namespace Triumph
{
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
		Camera camera = new Camera();
		TileMap map = new TileMap();
		TileLayer fog;
		AnimatedSprite sprite;
		List<AnimatedSprite> NPCs = new List<AnimatedSprite>();
		List<AnimatedSprite> renderList = new List<AnimatedSprite>();
		Comparison<AnimatedSprite> renderSort =
			new Comparison<AnimatedSprite>(renderSpriteCompare);

		static int renderSpriteCompare(AnimatedSprite a, AnimatedSprite b)
		{
			return a.origin.Y.CompareTo(b.origin.Y);
		}

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            base.Initialize();
		
			FrameAnimation up = new FrameAnimation(2, 32, 32, 0, 0);
			up.framesPerSecond = 5;
			sprite.animations.Add("Up", up);

			FrameAnimation down = new FrameAnimation(2, 32, 32, 64, 0);
			down.framesPerSecond = 5;
			sprite.animations.Add("Down", down);

			FrameAnimation left = new FrameAnimation(2, 32, 32, 128, 0);
			left.framesPerSecond = 5;
			sprite.animations.Add("Left", left);

			FrameAnimation right = new FrameAnimation(2, 32, 32, 192, 0);
			right.framesPerSecond = 5;
			sprite.animations.Add("Right", right);

			Random rand = new Random();
			foreach (AnimatedSprite s in NPCs)
			{
				s.animations.Add("Up", (FrameAnimation)up.Clone());
				s.animations.Add("Down", (FrameAnimation)down.Clone());
				s.animations.Add("Left", (FrameAnimation)left.Clone());
				s.animations.Add("Right", (FrameAnimation)right.Clone());

				switch (rand.Next(3))
				{
					case 0:
						s.currentAnimationName = "Up";
						break;
					case 1:
						s.currentAnimationName = "Down";
						break;
					case 2:
						s.currentAnimationName = "Left";
						break;
					case 3:
					default:
						s.currentAnimationName = "Right";
						break;
				}

				s.position.X = rand.Next(map.getWidthInTiles() - 1) * Engine.TILE_WIDTH;
				s.position.Y = rand.Next(map.getHeightInTiles() - 1) * Engine.TILE_HEIGHT;
				s.isAnimating = true;
				s.originOffset = new Vector2(16f, 32f);
			}
			
			sprite.currentAnimationName = "Down";
			sprite.capturingKeyboard = true;
			sprite.speed = 5;
			sprite.originOffset = new Vector2(16, 32);
			camera.setFocus(sprite);

			renderList.Add(sprite);
			renderList.AddRange(NPCs);
			 
        }

        protected override void LoadContent()
        {

            spriteBatch = new SpriteBatch(GraphicsDevice);
			map.layers.Add(TileLayer.fromFile(Content, "Content/Layers/baseTiles.layer"));
			map.layers.Add(TileLayer.fromFile(Content, "Content/Layers/g2m1.layer"));
			map.layers.Add(TileLayer.fromFile(Content, "Content/Layers/g2m2.layer"));
			map.layers.Add(TileLayer.fromFile(Content, "Content/Layers/g2m3.layer"));
			map.layers.Add(TileLayer.fromFile(Content, "Content/Layers/fieldObjects.layer"));
			map.collisionLayer = CollisionLayer.fromFile("Content/Layers/Collision.layer");
			fog = TileLayer.fromFile(Content, "Content/Layers/fog.layer");
			sprite = new AnimatedSprite(Content.Load<Texture2D>("Sprites/mnt1"));
			NPCs.Add(new AnimatedSprite(Content.Load<Texture2D>("Sprites/mst1")));
			NPCs.Add(new AnimatedSprite(Content.Load<Texture2D>("Sprites/wmn1")));
			NPCs.Add(new AnimatedSprite(Content.Load<Texture2D>("Sprites/wmn2")));
			NPCs.Add(new AnimatedSprite(Content.Load<Texture2D>("Sprites/wmn3")));
			NPCs.Add(new AnimatedSprite(Content.Load<Texture2D>("Sprites/mnv1")));
        }

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

			if (Mouse.GetState().LeftButton == ButtonState.Pressed)
			{
				camera.unsetFocus();
			}
			if (Mouse.GetState().RightButton == ButtonState.Pressed)
			{
				camera.setFocus(sprite);
			}

            // TODO: Add your update logic here 
			int screenWidth = GraphicsDevice.Viewport.Width;
			int screenHeight = GraphicsDevice.Viewport.Height;

			sprite.update(gameTime, screenWidth, screenHeight, map);
			foreach (AnimatedSprite s in NPCs)
			{
				s.update(gameTime, screenWidth, screenHeight, map);

				if (AnimatedSprite.areColliding(sprite, s))
				{
					Vector2 d = Vector2.Normalize(s.origin - sprite.origin);
					sprite.position = s.position - d * (sprite.collisionRadius + s.collisionRadius);
				}
			}
			camera.update(screenWidth, screenHeight, map);

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            // TODO: Add your drawing code here
			map.draw(spriteBatch, camera);

			spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, SamplerState.LinearClamp, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, camera.transformationMatrix);
			renderList.Sort(renderSort);
			foreach (AnimatedSprite s in renderList)
			{
				s.Draw(spriteBatch);
			}
			spriteBatch.End();

			fog.draw(spriteBatch, camera);

            base.Draw(gameTime);
        }
    }
}
