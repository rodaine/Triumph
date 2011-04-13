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
	public class AnimatedSprite
	{
		private string _currentAnimationName = null;	
		private bool _isAnimating = false;
		private float _speed = 2f,
					  _collisionRadius = 10f;
		private Texture2D spriteTexture;
		private Vector2 _originOffset = Vector2.Zero;
		private bool _isMoving = false, _isAttacking = false, _isHit = false, _isDodging = false, _isCritHit = false;
		private bool[] attackPhases = {false, false, false};
		private Vector2 destination;
		private Vector2 initial;
		private Stack<Point> path;
		private float timer;

		/// <summary>
		/// Collection of FrameAnimations with assigned (arbitrary) names
		/// </summary>
		public Dictionary<string, FrameAnimation> animations = new Dictionary<string, FrameAnimation>();

		/// <summary>
		/// Position (in pixels) of the animated sprite on the map
		/// </summary>
		public Vector2 position = Vector2.Zero;

		/// <summary>
		/// Gets or sets the offset of the sprite's origin (e.g., feet) from the sprite's position
		/// </summary>
		public Vector2 originOffset
		{
			get { return _originOffset; }
			set
			{
				_originOffset.X = Math.Max(value.X, 0f);
				_originOffset.Y = Math.Max(value.Y, 0f);
			}
		}

		/// <summary>
		/// Gets the origin (e.g., feet) of the sprite
		/// </summary>
		public Vector2 origin
		{
			get { return position + _originOffset; }
		}

		public Vector2 center
		{
			get { return new Vector2(
				position.X + (float)currentAnimation.currentFrame.Width/2, 
				position.Y + (float)currentAnimation.currentFrame.Height/2
				); }
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

		public bool isMoving
		{
			get { return _isMoving; }
		}

		public bool isAttacking
		{
			get { return _isAttacking; }
		}

		/// <summary>
		/// Gets or sets the movement speed of the sprite in pixels per update
		/// </summary>
		public float speed
		{
			get { return _speed; }
			set { _speed = (float)Math.Max(value, .1f); }
		}

		/// <summary>
		/// Gets or sets the circular collision radius of the sprite
		/// </summary>
		public float collisionRadius
		{
			get { return _collisionRadius; }
			set { _collisionRadius = (float)Math.Max(value, 1f); }
		}

		/// <summary>
		/// Determines whether or not two sprites are within each other's collision radius
		/// </summary>
		/// <param name="a">First AnimatedSprite</param>
		/// <param name="b">Second AnimatedSprite</param>
		/// <returns>True if the sprites are colliding; else, false.</returns>
		public static bool areColliding(AnimatedSprite a, AnimatedSprite b)
		{
			Vector2 d = b.origin - a.origin;
			return d.Length() < b.collisionRadius + a.collisionRadius;
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
		public AnimatedSprite(Texture2D texture)
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

			updateWalking(map);
			updateAttacking();
			updateAnimation(gameTime);
		}

		private void updateAttacking()
		{
			if (_isMoving) return;

			if (_isAttacking)
			{
				isAnimating = true;
				Vector2 motion = Vector2.Zero;

				if (attackPhases[2])
				{
					//walk back to initial location

					//at destination
					if (position.X == destination.X && position.Y == destination.Y)
					{
						isAnimating = false;
						_isAttacking = false;
					}
					else //not at destination
					{
						motion = destination - position;
						motion.Normalize();
						motion *= speed * 1.5f;
						if (motion.Length() > (destination - position).Length())
							motion = destination - position;
						if (motion != Vector2.Zero)
							position += motion;
					}
				}
				else if (attackPhases[1])
				{
					//run foward

					//at destination
					if (position.X == destination.X && position.Y == destination.Y)
					{
						attackPhases[2] = true;
						destination = initial;
					}
					else //not at destination
					{
						motion = destination - position;
						motion.Normalize();
						motion *= speed * 2f;
						if (motion.Length() > (destination - position).Length())
							motion = destination - position;
						if (motion != Vector2.Zero)
							position += motion;
					}
				} 
				else if (attackPhases[0])
				{
					//step backward

					//at destination
					if (position.X == destination.X && position.Y == destination.Y)
					{
						attackPhases[1] = true;
						switch (currentAnimationName)
						{
							case "Up":
								destination = this.position - new Vector2(0f, (float)(Engine.TILE_HEIGHT * .75));
								break;
							case "Down":
								destination = this.position + new Vector2(0f, (float)(Engine.TILE_HEIGHT * .75));
								break;
							case "Left":
								destination = this.position - new Vector2((float)(Engine.TILE_WIDTH * .75), 0f);
								break;
							case "Right":
								destination = this.position + new Vector2((float)(Engine.TILE_WIDTH * .75), 0f);
								break;
							default:
								_isAnimating = false;
								_isAttacking = false;
								break;

						}
					}
					else //not at destination
					{
						motion = destination - position;
						motion.Normalize();
						motion *= speed * 0.25f;
						if (motion.Length() > (destination - position).Length())
							motion = destination - position;
						if (motion != Vector2.Zero)
							position += motion;
					}

				} 
				else 
				{
					//initialize attack

					initial = new Vector2(position.X, position.Y);
					attackPhases[0] = true;
					switch (currentAnimationName)
					{
						case "Up":
							destination = this.position + new Vector2(0f, (float)(Engine.TILE_HEIGHT * 0.25)); 
							break;
						case "Down":
							destination = this.position - new Vector2(0f, (float)(Engine.TILE_HEIGHT * 0.25)); 
							break;
						case "Left":
							destination = this.position + new Vector2((float)(Engine.TILE_WIDTH * 0.25), 0f); 
							break;
						case "Right":
							destination = this.position - new Vector2((float)(Engine.TILE_WIDTH * 0.25), 0f); 
							break;
						default:
							_isAnimating = false;
							_isAttacking = false;
							break;

					}
				}
			}
			else
				_isAnimating = false;
		}

		private void updateWalking(TileMap map)
		{
			Vector2 motion = Vector2.Zero;
			if (_isMoving && position.X == destination.X && position.Y == destination.Y)
			{
				//if at final destination, stop walking!
				// get next destination
				if (path.Count == 0)
				{
					_isMoving = false;
				}
				else
				{
					Point tileDest = path.Pop();
					destination = new Vector2((float)tileDest.X * Engine.TILE_WIDTH, (float)tileDest.Y * Engine.TILE_HEIGHT);
				}
			}

			//update motion vector
			if (_isMoving)
			{
				motion = destination - position;
				motion.Normalize();
				motion *= speed;
				if (motion.Length() > (destination - position).Length())
					motion = destination - position;
			}

			//change to appropriate current animation
			if (motion != Vector2.Zero)
			{
				isAnimating = true;

				if (motion.X > 0)
				{
					currentAnimationName = "Right";
				}
				else if (motion.Y > 0)
				{
					currentAnimationName = "Down";
				}
				else if (motion.Y < 0)
				{
					currentAnimationName = "Up";
				}
				else
				{
					currentAnimationName = "Left";
				}

				//motion = adjustMotionForCollision(motion, map.collisionLayer);
				position += motion;

				clampToArea(map.getWidthInPixels(), map.getHeightInPixels());

			}
			else
				isAnimating = false;
		}

		private void updateHit()
		{

		}

		private void updateDodge()
		{

		}

		private void updateCrit()
		{

		}

		public bool goToTile(BaseUnit unit, Point goal, TileMap map, int maxDistance)
		{
			if (_isMoving || _isAttacking) return false;

			path = map.getPath(unit, goal, new List<Point>());
			if (path.Count == 0 || path.Count > maxDistance + 1)
				return false;

			Point tileDest = path.Pop();

			destination = new Vector2((float)tileDest.X * Engine.TILE_WIDTH, (float)tileDest.Y * Engine.TILE_HEIGHT);
			_isMoving = true;
			return true;
		}

		public bool attack(BaseUnit unit, BaseUnit target)
		{
			if (_isMoving) return false;

			attackPhases[0] =
			attackPhases[1] =
			attackPhases[2] = false;
			
			_isAttacking = true;

			int x = target.position.X - unit.position.X;
			int y = target.position.Y - unit.position.Y;

			//face unit...this favors X-Dir over Y-Dir if equivalent
			if (Math.Abs(y) > Math.Abs(x))
			{
				if (y > 0)
					currentAnimationName = "Down";
				else
					currentAnimationName = "Up";
			}
			else
			{
				if (x > 0)
					currentAnimationName = "Right";
				else
					currentAnimationName = "Left";
			}

			if (x == y && y == 0)
			{
				currentAnimationName = "Down";
				_isAttacking = false;
			}

			return true;
		}

		public bool beHit(BaseUnit unit, BaseUnit attackSource)
		{
			return true;
		}

		public bool dodge(BaseUnit unit, BaseUnit attackSource)
		{
			return true;
		}

		public bool beCritHit(BaseUnit unit, BaseUnit attackSource)
		{
			return true;
		}

		/// <summary>
		/// If the sprite is currently animating, the FrameAnimation is updated
		/// </summary>
		/// <param name="gameTime">GameTime passed by the game</param>
		private void updateAnimation(GameTime gameTime)
		{
			if (!_isAnimating)
				return;

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
		public void Draw(SpriteBatch batch)
		{
			FrameAnimation animation = currentAnimation;
			if (animation != null)
				batch.Draw(
					spriteTexture,
					position,
					animation.currentFrame,
					Color.White);
		}
	
	}
}
