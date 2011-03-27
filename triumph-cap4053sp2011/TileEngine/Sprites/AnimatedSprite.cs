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
		private bool _isMoving = false;
		private Vector2 destination;
		private Stack<Point> path;

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

			updateAnimation(gameTime);
		}

		public void goToTile(Point goal, TileMap map)
		{
			if (_isMoving) return;

			path = map.getPath(Engine.convertPositionToTile(position), goal, new List<Point>());
			if (path.Count == 0)
				return;

			Point tileDest = path.Pop();

			destination = new Vector2((float)tileDest.X * Engine.TILE_WIDTH, (float)tileDest.Y * Engine.TILE_HEIGHT);
			_isMoving = true;
		}

		//private Vector2 adjustMotionForCollision(Vector2 motion,CollisionLayer collisionLayer)
		//{
		//    Point originTile = Engine.convertPositionToTile(origin);
		//    Point centerTile = Engine.convertPositionToTile(center);
		//    Point? up, down, left, right;
		//    Rectangle tileRect;
		//    int collIndex = collisionLayer.getTileCollisionIndex(originTile);

		//    //HANDLE NON-UNWALKABLE COLLISIONS HERE
		//    if (collIndex == 2)
		//        motion /= 2;

		//    //HANDLE UNWALKABLE COLLISIONS HERE
		//    if (centerTile.X != 0)
		//    {
		//        left = new Point(centerTile.X - 1, centerTile.Y);
		//        tileRect = Engine.createRectForTile(left.Value);
		//        if (collisionLayer.getTileCollisionIndex(left.Value) == 1 && tileRect.Intersects(bounds))
		//            motion.X = Math.Max(0, motion.X);

		//    }
		//    if (centerTile.X != collisionLayer.widthInTiles - 1)
		//    {
		//        right = new Point(centerTile.X + 1, centerTile.Y);
		//        tileRect = Engine.createRectForTile(right.Value);
		//        if (collisionLayer.getTileCollisionIndex(right.Value) == 1 && tileRect.Intersects(bounds))
		//            motion.X = Math.Min(0,motion.X);
		//    }

		//    if (centerTile.Y != 0)
		//    {
		//        up = new Point(centerTile.X, centerTile.Y - 1);
		//        tileRect = Engine.createRectForTile(up.Value);
		//        if (collisionLayer.getTileCollisionIndex(up.Value) == 1 && tileRect.Intersects(bounds))
		//            motion.Y = Math.Max(0, motion.Y);
		//    }

		//    if (centerTile.Y != collisionLayer.heightInTiles - 1)
		//    {
		//        down = new Point(centerTile.X, centerTile.Y + 1);
		//        tileRect = Engine.createRectForTile(down.Value);
		//        if (collisionLayer.getTileCollisionIndex(down.Value) == 1 && tileRect.Intersects(bounds))
		//            motion.Y = Math.Min(0, motion.Y);
		//    }

		//    return motion;
		//}

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
