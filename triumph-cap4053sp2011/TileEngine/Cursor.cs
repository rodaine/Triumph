using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace TileEngine
{
	/// <summary>
	/// Describes an animated sprite drawn on the screen
	/// </summary>
	public class Cursor
	{
		private string _currentAnimationName = null;
		private bool _isAnimating = true;
		private float _timer = 0f,
					secondsPerTile = 0.15f;
		private Texture2D spriteTexture;
		private Vector2 _originOffset = Vector2.Zero;

		/// <summary>
		/// Collection of FrameAnimations with assigned (arbitrary) names
		/// </summary>
		public Dictionary<string, FrameAnimation> animations = new Dictionary<string, FrameAnimation>();

		/// <summary>
		/// Position (in pixels) of the animated sprite on the map
		/// </summary>
		public Vector2 position = Vector2.Zero;

		/// <summary>
		/// Whether or not the sprite is currently controlled by the keyboard
		/// </summary>
		public bool capturingKeyboard = false;

		public Vector2 center
		{
			get
			{
				return new Vector2(
					position.X + (float)currentAnimation.currentFrame.Width / 2,
					position.Y + (float)currentAnimation.currentFrame.Height / 2
					);
			}
		}

		public Rectangle bounds
		{
			get
			{
				Rectangle rect = currentAnimation.currentFrame;
				rect.X = (int)position.X;
				rect.Y = (int)position.Y;
				return rect;
			}
		}

		/// <summary>
		/// Gets or sets whether or not the FrameAnimation of the sprite should be updating
		/// </summary>
		public bool isAnimating
		{
			get { return _isAnimating; }
			set { _isAnimating = value; }
		}

		/// <summary>
		/// Gets the current FrameAnimation used by the sprite
		/// </summary>
		public FrameAnimation currentAnimation
		{
			get
			{
				if (!string.IsNullOrEmpty(_currentAnimationName))
					return animations[_currentAnimationName];
				else
					return null;
			}
		}

		/// <summary>
		/// Gets or sets the name of the currentAnimation of the sprite
		/// </summary>
		/// <remarks>The name refers to the FrameAnimation's key in the animations Dictionary</remarks>
		public string currentAnimationName
		{
			get { return _currentAnimationName; }
			set
			{
				if (animations.ContainsKey(value))
					_currentAnimationName = value;
			}
		}

		/// <summary>
		/// Creates a new AnimatedSprite
		/// </summary>
		/// <param name="texture">2DTexture containing all the sprite animations</param>
		public Cursor(Texture2D texture)
		{
			spriteTexture = texture;
		}

		/// <summary>
		/// Updates the AnimatedSprite based on keyboard input, animation and collisions
		/// </summary>
		/// <param name="gameTime">GameTime passed from the game</param>
		/// <param name="screenWidth">Width in pixels of the viewport</param>
		/// <param name="screenHeight">Height in pixels of the viewport</param>
		/// <param name="map">TileMap currently being drawn in the viewport</param>
		public void update(GameTime gameTime, int screenWidth, int screenHeight, TileMap map)
		{
			if (!capturingKeyboard)
			{
				updateAnimation(gameTime, map);
				return;
			}

			KeyboardState keyState = Keyboard.GetState();
			Vector2 motion = Vector2.Zero;
			
			if (keyState.IsKeyDown(Keys.W))
				--motion.Y;
			else if (keyState.IsKeyDown(Keys.S))
				++motion.Y;
			else if (keyState.IsKeyDown(Keys.A))
				--motion.X;
			else if (keyState.IsKeyDown(Keys.D))
				++motion.X;

			_timer += (float)gameTime.ElapsedGameTime.TotalSeconds;
			if (_timer >= secondsPerTile && motion != Vector2.Zero)
				_timer = 0f;
			else if (_timer >= secondsPerTile)
				_timer = secondsPerTile;
			else
				motion = Vector2.Zero;


			if (motion != Vector2.Zero)
			{
				motion.Normalize();
				position += motion * 32;
				clampToArea(map.getWidthInPixels(), map.getHeightInPixels());
			}

			updateAnimation(gameTime, map);
		}

		/// <summary>
		/// If the sprite is currently animating, the FrameAnimation is updated
		/// </summary>
		/// <param name="gameTime">GameTime passed by the game</param>
		private void updateAnimation(GameTime gameTime, TileMap map)
		{
			if (!_isAnimating)
				return;

			if (map.collisionLayer.getTileCollisionIndex(Engine.convertPositionToTile(position)) == 1)
				currentAnimationName = "Collision";
			else
				currentAnimationName = "Normal";



			FrameAnimation animation = currentAnimation;
			if (animation == null)
			{
				if (animations.Count > 0)
				{
					string[] keys = new string[animations.Count];
					animations.Keys.CopyTo(keys, 0);
					currentAnimationName = keys[0];
				}
				else
					return;
			}

			animation.update(gameTime);
		}

		/// <summary>
		/// Clamps the sprite to only move within a specified area
		/// </summary>
		/// <param name="width">Width in pixels of the area</param>
		/// <param name="height">Hieght in pixels of the area</param>
		private void clampToArea(int width, int height)
		{
			if (position.X > width - currentAnimation.currentFrame.Width)
				position.X = width - currentAnimation.currentFrame.Width;
			if (position.Y > height - currentAnimation.currentFrame.Height)
				position.Y = height - currentAnimation.currentFrame.Height;

			if (position.X < 0)
				position.X = 0;
			if (position.Y < 0)
				position.Y = 0;
		}

		/// <summary>
		/// Draws the AnimatedSprite in the viewport
		/// </summary>
		/// <param name="batch">SpriteBatch used to render the sprite</param>
		public void Draw(SpriteBatch batch, Camera camera)
		{
			FrameAnimation animation = currentAnimation;
			if (animation != null)
			{
				batch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, camera.transformationMatrix);
				batch.Draw(
					spriteTexture,
					position,
					animation.currentFrame,
					new Color(1f, 1f, 1f, 0.05f));
				batch.End();
			}
		}

	}
}
