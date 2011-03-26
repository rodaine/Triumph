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
		#region Game Objects
		GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
		Camera camera = new Camera();
		TileMap map = new TileMap();
		AnimatedSprite sprite;
		SoundEffect soundMusic;
		SoundEffectInstance soundMusicInstance;
		Cursor cursor;
		#endregion

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        //Set initial values for everything
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

			sprite.currentAnimationName = "Down";
			sprite.speed = 5;
			sprite.originOffset = new Vector2(16, 32);

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

        //load all images and outside files
        protected override void LoadContent()
        {

            spriteBatch = new SpriteBatch(GraphicsDevice);

			map.layers.Add(TileLayer.fromFile(Content, "Content/Layers/baseTiles.layer"));
			map.layers.Add(TileLayer.fromFile(Content, "Content/Layers/g2m1.layer"));
			map.layers.Add(TileLayer.fromFile(Content, "Content/Layers/g2m2.layer"));
			map.layers.Add(TileLayer.fromFile(Content, "Content/Layers/g2m3.layer"));
			map.layers.Add(TileLayer.fromFile(Content, "Content/Layers/fieldObjects.layer"));
			map.collisionLayer = CollisionLayer.fromFile("Content/Layers/Collision.layer");
			sprite = new AnimatedSprite(Content.Load<Texture2D>("Sprites/mnt1"));
			soundMusic = Content.Load<SoundEffect>("Music/POL-battle-march-long");
			soundMusicInstance = soundMusic.CreateInstance();
			cursor = new Cursor(Content.Load<Texture2D>("UI/cursor"));
        }
        
        //we do not currently use this!!
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        //every game tick prompts the following actions
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
			cursor.update(gameTime, screenWidth, screenHeight, map);

			camera.update(screenWidth, screenHeight, map);

            base.Update(gameTime);
        }

        //called from update, draws screen
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

			map.draw(spriteBatch, camera);

			cursor.Draw(spriteBatch, camera);

			spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, SamplerState.LinearClamp, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, camera.transformationMatrix);
			sprite.Draw(spriteBatch);
			spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
