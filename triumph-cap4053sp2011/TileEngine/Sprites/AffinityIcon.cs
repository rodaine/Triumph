using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TileEngine
{
	public class AffinityIcon
	{
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

		public Dictionary<string, FrameAnimation> frames = new Dictionary<string, FrameAnimation>();

		public bool isDrawing;

		public string currentAffinity
		{
			get { return _currentAffinity; }
			set
			{
				if (frames.ContainsKey(value))
					_currentAffinity = value;
			}
		}

		public Vector2 position = Vector2.Zero;

		public AffinityIcon(Texture2D icons)
		{
			iconMap = icons;
		}

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

	}
}

