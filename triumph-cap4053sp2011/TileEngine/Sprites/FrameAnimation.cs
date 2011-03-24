using System;
using Microsoft.Xna.Framework;

namespace TileEngine
{
	/// <summary>
	/// Describes the frame animation used by a sprite.
	/// This object does not contain the textures, just the rectangles used to pull frames out of them.
	/// </summary>
	/// <remarks>With this current model, frames must be located on the same line horizontally in the texture to render.</remarks>
	public class FrameAnimation : ICloneable
	{

		private Rectangle[] _frames;
		private int _currentFrameIndex = 0;
		private float _secondsPerFrame = 0.5f,
					  _timer = 0;

		/// <summary>
		/// Gets or sets the frames per second of the animation
		/// </summary>
		public int framesPerSecond
		{
			get { return (int) (1f / _secondsPerFrame); }
			set { _secondsPerFrame = (float)Math.Max(1f / (float)value, 0.001f); }
		}

		/// <summary>
		/// Gets the current frame rectangle in the animation
		/// </summary>
		public Rectangle currentFrame
		{
			get { return _frames[_currentFrameIndex]; }
		}

		/// <summary>
		/// Get or set the current frame index of the animation
		/// </summary>
		public int currentFrameIndex
		{
			get { return _currentFrameIndex; }
			set { _currentFrameIndex = (int)MathHelper.Clamp(value, 0, _frames.Length - 1); }
		}
		
		/// <summary>
		/// Creates a new Frame Animation 
		/// </summary>
		/// <param name="frameCount">Number of frames in the animation</param>
		/// <param name="frameWidth">Width in pixels of the frame rectangle</param>
		/// <param name="frameHeight">Height in pixels of the frame rectangle</param>
		/// <param name="xOffset">Offset in pixels in the x-direction of the first frame from the top-left corner of the texture</param>
		/// <param name="yOffset">Offset in pixels in the y-direction of the first frame from the top-left corner of the texture</param>
		public FrameAnimation(int frameCount, int frameWidth, int frameHeight, int xOffset, int yOffset)
		{
			_frames = new Rectangle[frameCount];

			for (int i = 0; i < frameCount; ++i)
			{
				Rectangle frame = new Rectangle();
				frame.Width = frameWidth;
				frame.Height = frameHeight;
				frame.X = xOffset + (i * frameWidth);
				frame.Y = yOffset;
				
				_frames[i] = frame;
			}
		}

		/// <summary>
		/// Creates a new FrameAnimation [used only by the Clone() method]
		/// </summary>
		private FrameAnimation(){}

		/// <summary>
		/// Clones the FrameAnimation
		/// </summary>
		/// <returns>Copy of the FrameAnimation</returns>
		/// <remarks>Frames are not cloned to conserve memory</remarks>
		public object Clone()
		{
			FrameAnimation anim = new FrameAnimation();
			anim._secondsPerFrame = _secondsPerFrame;
			anim._frames = _frames;
			return anim;
		}

		/// <summary>
		/// Updates the current frame based on elapsed game time since the last update
		/// </summary>
		/// <param name="gameTime">GameTime passed from the game where the animation is rendered</param>
		public void update(GameTime gameTime)
		{
			_timer += (float)gameTime.ElapsedGameTime.TotalSeconds;
			if (_timer >= _secondsPerFrame)
			{
				_timer = 0f;
				currentFrameIndex = (currentFrameIndex + 1) % _frames.Length;
			}
		}

	}
}
