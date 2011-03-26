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
		AnimatedSprite sprite, patrol;
		List<AnimatedSprite> NPCs = new List<AnimatedSprite>();
		List<AnimatedSprite> renderList = new List<AnimatedSprite>();
		List<Point> npcLocations = new List<Point>();
		Comparison<AnimatedSprite> renderSort =
			new Comparison<AnimatedSprite>(renderSpriteCompare);
		SoundEffect soundMusic;
		SoundEffectInstance soundMusicInstance;
		Cursor cursor;

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
			patrol.animations.Add("Up", up);

			FrameAnimation down = new FrameAnimation(2, 32, 32, 64, 0);
			down.framesPerSecond = 5;
			sprite.animations.Add("Down", down);
			patrol.animations.Add("Down", down);

			FrameAnimation left = new FrameAnimation(2, 32, 32, 128, 0);
			left.framesPerSecond = 5;
			sprite.animations.Add("Left", left);
			patrol.animations.Add("Left", left);

			FrameAnimation right = new FrameAnimation(2, 32, 32, 192, 0);
			right.framesPerSecond = 5;
			sprite.animations.Add("Right", right);
			patrol.animations.Add("Right", right);

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

				s.position.X = rand.Next(10) * Engine.TILE_WIDTH;
				s.position.Y = rand.Next(3) * Engine.TILE_HEIGHT;
				npcLocations.Add(Engine.convertPositionToTile(s.position));
				s.isAnimating = true;
				s.originOffset = new Vector2(16f, 32f);
			}
			
			sprite.currentAnimationName = "Down";
			//sprite.capturingKeyboard = true;
			sprite.speed = 5;
			sprite.originOffset = new Vector2(16, 32);
			//camera.setFocus(sprite);

			patrol.currentAnimationName = "Down";
			patrol.speed = 3;
			patrol.originOffset = new Vector2(16, 32);
			patrol.position.X = 320;
			patrol.position.Y = 0;

			renderList.Add(sprite);
			renderList.Add(patrol);
			renderList.AddRange(NPCs);

			soundMusicInstance.Volume = 0.75f;
			soundMusicInstance.IsLooped = true;
			soundMusicInstance.Play();

			cursor.animations.Add("Normal", new FrameAnimation(1, 32, 32, 0, 0));
			cursor.animations.Add("Collision", new FrameAnimation(1, 32, 32, 32, 0));
			cursor.animations.Add("SubSelect", new FrameAnimation(1, 32, 32, 64, 0));
			cursor.animations.Add("Hidden", new FrameAnimation(1, 32, 32, 96, 0));
			cursor.currentAnimationName = "Normal";
			cursor.position = new Vector2(32f, 32f);
			cursor.capturingKeyboard = true;
			camera.setFocus(cursor);

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
			patrol = new AnimatedSprite(Content.Load<Texture2D>("Sprites/mnt1"));
			NPCs.Add(new AnimatedSprite(Content.Load<Texture2D>("Sprites/mst1")));
			NPCs.Add(new AnimatedSprite(Content.Load<Texture2D>("Sprites/wmn1")));
			NPCs.Add(new AnimatedSprite(Content.Load<Texture2D>("Sprites/wmn2")));
			NPCs.Add(new AnimatedSprite(Content.Load<Texture2D>("Sprites/wmn3")));
			NPCs.Add(new AnimatedSprite(Content.Load<Texture2D>("Sprites/mnv1")));
			soundMusic = Content.Load<SoundEffect>("Music/POL-battle-march-long");
			soundMusicInstance = soundMusic.CreateInstance();
			cursor = new Cursor(Content.Load<Texture2D>("UI/cursor"));
        }

        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

			if (soundMusicInstance.State == SoundState.Paused || soundMusicInstance.State == SoundState.Paused)
			{
				soundMusicInstance.Volume = 0.75f;
				soundMusicInstance.IsLooped = true;
				soundMusicInstance.Play();
			}

            // TODO: Add your update logic here 
			int screenWidth = GraphicsDevice.Viewport.Width;
			int screenHeight = GraphicsDevice.Viewport.Height;

			sprite.update(gameTime, screenWidth, screenHeight, map);
			patrol.update(gameTime, screenWidth, screenHeight, map);
			cursor.update(gameTime, screenWidth, screenHeight, map);

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

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

			map.draw(spriteBatch, camera);

			cursor.Draw(spriteBatch, camera);

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
