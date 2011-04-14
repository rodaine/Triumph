using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TileEngine
{
	/// <summary>
	/// Describes an animated sprite drawn on the screen
	/// </summary>
	public class AnimatedSprite
	{

		#region Private Properties

		private string _currentAnimationName = null;

		private float _speed = 2f;
		private float _collisionRadius = 10f;
		private float timer;		

		private bool _isAnimating = false;
		private bool _isMoving = false;
		private bool _isAttacking = false;
		private bool _isHit = false;
		private bool _isDodging = false;
		private bool _isCritHit = false;
		private bool[] attackPhases = { false, false, false };
		private bool dodgePhase = false;
		
		private Texture2D spriteTexture;

		private Vector2 destination = Vector2.Zero;
		private Vector2 initial = Vector2.Zero;

		private Stack<Point> path;

		private FrameAnimation currentAnimation
		{
			get
			{
				if (!string.IsNullOrEmpty(_currentAnimationName))
					return animations[_currentAnimationName];
				else
					return null;
			}
		}

		#endregion

		#region Public Properties

		/// <summary>
		/// Collection of FrameAnimations with assigned (arbitrary) names
		/// </summary>
		public Dictionary<string, FrameAnimation> animations = new Dictionary<string, FrameAnimation>();

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
		/// Position (in pixels) of the animated sprite on the map
		/// </summary>
		public Vector2 position = Vector2.Zero;

		/// <summary>
		/// Gets or sets the movement speed of the sprite in pixels per update
		/// </summary>
		public float speed
		{
			get { return _speed; }
			set { _speed = (float)Math.Max(value, .1f); }
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
		/// Gets whether or not the Sprite is performing a walk animation
		/// </summary>
		public bool isMoving
		{
			get { return _isMoving; }
		}

		/// <summary>
		/// Gets whether or not the Sprite is performing an attack animation
		/// </summary>
		public bool isAttacking
		{
			get { return _isAttacking; }
		}

		/// <summary>
		/// Get whether or not the Sprite is performing a defensive animation (Hit, Dodge, Crit)
		/// </summary>
		public bool isDefending
		{
			get { return _isHit | _isDodging | _isCritHit; }
		}
		
		#endregion

		#region Initializers

		/// <summary>
		/// Creates a new AnimatedSprite
		/// </summary>
		/// <param name="texture">2DTexture containing all the sprite animations</param>
		public AnimatedSprite(Texture2D texture)
		{
			spriteTexture = texture;
		}

		#endregion

		#region Action Methods

		/// <summary>
		/// Walk the Sprite to a specified tile location
		/// </summary>
		/// <param name="unit">Unit to perform walking</param>
		/// <param name="goal">Goal tile location to walk to</param>
		/// <param name="map">TileMap passed from the game</param>
		/// <param name="maxDistance">Maximum allowed walking distance</param>
		/// <returns></returns>
		public bool goToTile(BaseUnit unit, Point goal, TileMap map, int maxDistance)
		{
			if (_isMoving || _isAttacking || _isDodging || _isHit) return false;

			path = map.getPath(unit, goal, new List<Point>());
			if (path.Count == 0 || path.Count > maxDistance + 1)
				return false;

			Point tileDest = path.Pop();

			destination = new Vector2((float)tileDest.X * Engine.TILE_WIDTH, (float)tileDest.Y * Engine.TILE_HEIGHT);
			_isMoving = true;
			return true;
		}

		/// <summary>
		/// Perform an attack animation
		/// </summary>
		/// <param name="unit">Unit performing the attack</param>
		/// <param name="target">Unit being attacked</param>
		/// <returns>True if attack animation will be performed; else false.</returns>
		public bool attack(BaseUnit unit, BaseUnit target)
		{
			if (_isMoving || _isAttacking || _isDodging || _isHit) return false;

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

		/// <summary>
		/// Perform the hit animation (after attacked)
		/// </summary>
		/// <param name="unit">Unit being hit</param>
		/// <param name="attackSource">Source Unit of the attack</param>
		/// <returns>True if hit animation will be performed; else false.</returns>
		public bool beHit(BaseUnit unit, BaseUnit attackSource)
		{
			if (_isMoving || _isAttacking || _isDodging || _isHit) return false;
			
			timer = 0f;
			initial.X = position.X;
			initial.Y = position.Y;

			int x = attackSource.position.X - unit.position.X;
			int y = attackSource.position.Y - unit.position.Y;

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
			}

			_isHit = true;

			return true;
		}

		/// <summary>
		/// Perform the critical hit animation (after attacked)
		/// </summary>
		/// <param name="unit">Unit being hit</param>
		/// <param name="attackSource">Source Unit of the attack</param>
		/// <returns>True if hit animation will be performed; else false.</returns>
		public bool beCritHit(BaseUnit unit, BaseUnit attackSource)
		{
			if (_isMoving || _isAttacking || _isDodging || _isHit) return false;
			_isCritHit = true;
			return beHit(unit, attackSource);
		}

		/// <summary>
		/// Perform the dodge animation (after attacked)
		/// </summary>
		/// <param name="unit">Unit dodging</param>
		/// <param name="attackSource">Source Unit of the attack</param>
		/// <returns>True if dodge animation will be performed; else false.</returns>
		public bool dodge(BaseUnit unit, BaseUnit attackSource)
		{
			if (_isMoving || _isAttacking || _isDodging || _isHit) return false;

			timer = 0f;
			initial.X = position.X;
			initial.Y = position.Y;

			int x = attackSource.position.X - unit.position.X;
			int y = attackSource.position.Y - unit.position.Y;

			//face unit...this favors X-Dir over Y-Dir if equivalent
			if (Math.Abs(y) > Math.Abs(x))
			{
				if (y > 0)
				{
					currentAnimationName = "Down";
					destination = position + new Vector2((float) Engine.TILE_WIDTH / 4f, 0);
				}
				else
				{
					currentAnimationName = "Up";
					destination = position - new Vector2((float)Engine.TILE_WIDTH / 4f, 0);
				}
			}
			else
			{
				if (x > 0)
				{
					currentAnimationName = "Right";
					destination = position + new Vector2(0, (float)Engine.TILE_HEIGHT / 4f);
				}
				else
				{
					currentAnimationName = "Left";
					destination = position - new Vector2(0, (float)Engine.TILE_HEIGHT / 4f);
				}
			}

			if (x == y && y == 0)
			{
				currentAnimationName = "Down";
				_isDodging = false;
			}

			dodgePhase = false;
			_isDodging = true;

			return true;
		}
	
		#endregion

		#region Update Methods

		/// <summary>
		/// Updates the AnimatedSprite based on moving, attacking, defending, etc...
		/// </summary>
		/// <param name="gameTime">GameTime passed from the game</param>
		/// <param name="map">TileMap currently being drawn in the viewport</param>
		public void update(GameTime gameTime, TileMap map)
		{
			updateWalking(map);
			updateAttacking();
			updateHit(gameTime);
			updateCrit(gameTime);
			updateDodge();
			updateAnimation(gameTime);
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

		private void updateAttacking()
		{
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

		private void updateHit(GameTime gameTime)
		{
			if (!_isHit || _isCritHit) return;
			if (timer < 1000f)
			{
				float radius = (float) RandomNumber.getInstance().getNext(0, Engine.TILE_WIDTH / 4);
				double angle = (double) MathHelper.ToRadians(RandomNumber.getInstance().getNext(0, 359));
				float x = radius * (float) Math.Cos(angle);
				float y = radius * (float) Math.Sin(angle);
				Vector2 motion = new Vector2(x, y);

				position = initial + motion;
				timer += (float) gameTime.ElapsedGameTime.TotalMilliseconds;
			}
			else
			{
				position = initial;
				_isHit = false;
			}
		}

		private void updateCrit(GameTime gameTime)
		{
			if (!_isHit && !_isCritHit) return;
			if (timer < 1500f)
			{
				float radius = (float)RandomNumber.getInstance().getNext(0, Engine.TILE_WIDTH / 2);
				double angle = (double)MathHelper.ToRadians(RandomNumber.getInstance().getNext(0, 359));
				float x = radius * (float)Math.Cos(angle);
				float y = radius * (float)Math.Sin(angle);
				Vector2 motion = new Vector2(x, y);

				position = initial + motion;
				timer += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
			}
			else
			{
				position = initial;
				_isHit = _isCritHit = false;
			}
		}

		private void updateDodge()
		{
			if (!_isDodging) return;
			Vector2 motion = Vector2.Zero;
			if (dodgePhase)
			{
				if (position.X == initial.X && position.Y == initial.Y)
				{
					_isDodging = false;
					return;
				}

				motion = destination - position;
				motion.Normalize();
				motion *= speed * 1.5f;
				if (motion.Length() > (destination - position).Length())
					motion = destination - position;
				if (motion != Vector2.Zero)
					position += motion;

			}
			else
			{
				if (position.X == destination.X && position.Y == initial.Y)
				{
					destination = initial;
					dodgePhase = true;
					return;
				}

				motion = destination - position;
				motion.Normalize();
				motion *= speed * 1.5f;
				if (motion.Length() > (destination - position).Length())
					motion = destination - position;
				if (motion != Vector2.Zero)
					position += motion;

			}
		}

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

		#endregion

		#region Draw Methods

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

		#endregion

	}
}
