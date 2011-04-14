using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TileEngine
{
	/// <summary>
	/// Defines the affinity icon object for drawing unit affinities in the UI
	/// </summary>
	public class AffinityIcon
	{

		#region Private Properties

		private string _currentAffinity = null;

		private Texture2D iconMap;

		private FrameAnimation currentFrame
		{
			get
			{
				if (!string.IsNullOrEmpty(_currentAffinity))
					return frames[_currentAffinity];
				else
					return null;
			}
		}

		#endregion

		#region Public Properties

		/// <summary>
		/// List of affinity frames
		/// </summary>
		public Dictionary<string, FrameAnimation> frames = new Dictionary<string, FrameAnimation>();

		/// <summary>
		/// Get or set whether or not to draw the affinity icons on screen
		/// </summary>
		public bool isDrawing;

		/// <summary>
		/// Get or set the current affinity 
		/// </summary>
		public string currentAffinity
		{
			get { return _currentAffinity; }
			set
			{
				if (frames.ContainsKey(value))
					_currentAffinity = value;
			}
		}

		/// <summary>
		/// The position of the icon relative to the viewport
		/// </summary>
		public Vector2 position = Vector2.Zero;

		#endregion

		#region Initializers

		/// <summary>
		/// Create an AffinityIcon object
		/// </summary>
		/// <param name="icons">Sprite map of icons</param>
		public AffinityIcon(Texture2D icons)
		{
			iconMap = icons;
		}

		#endregion

		#region Draw Methods

		/// <summary>
		/// Draw the icon onscreen
		/// </summary>
		/// <param name="batch">Sprite Batch used to render the icon</param>
		public void Draw(SpriteBatch batch)
		{
			FrameAnimation frame = currentFrame;
			if (frame != null && isDrawing)
				batch.Draw(
					iconMap,
					position,
					frame.currentFrame,
					Color.White);
		}

		#endregion
	
	}
}

