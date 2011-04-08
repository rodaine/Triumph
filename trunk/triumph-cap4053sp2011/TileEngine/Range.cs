using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace TileEngine
{
	/// <summary>
	/// Defines a range of tiles to highlight for movement, attack, etc...
	/// </summary>
	public class Range
	{
		private List<Point> rangePoints = new List<Point>();
		private Texture2D sprite;
		private string _currentRangeTypeName = null;
		private FrameAnimation _currentRangeType;		
		
		/// <summary>
		/// Whether or not the range should be drawn
		/// </summary>
		public bool isDrawing;
		/// <summary>
		/// Dictionary of range types
		/// </summary>
		public Dictionary<string, FrameAnimation> rangeTypes = new Dictionary<string, FrameAnimation>();

		/// <summary>
		/// Create a range object
		/// </summary>
		/// <param name="spriteTexture">Sprite map of range types</param>
		public Range(Texture2D spriteTexture)
		{
			this.sprite = spriteTexture;
			isDrawing = false;
		}

		/// <summary>
		/// Add tile points to the range
		/// </summary>
		/// <param name="points">Points to add to the range</param>
		public void addPoints(params Point[] points)
		{
			foreach (Point pt in points)
			{
				bool inRange = false;
				foreach (Point pnt in rangePoints)
					if (pt.X == pnt.X && pt.Y == pnt.Y)
					{
						inRange = true;
						break;
					}
				if (!inRange) rangePoints.Add(pt);
			}
		}

		/// <summary>
		/// Add tile points to the range
		/// </summary>
		/// <param name="points">List of points to add to the range</param>
		public void addPoints(List<Point> points)
		{
			foreach (Point pt in points)
			{
				bool inRange = false;
				foreach (Point pnt in rangePoints)
					if (pt.X == pnt.X && pt.Y == pnt.Y)
					{
						inRange = true;
						break;
					}
				if (!inRange) rangePoints.Add(pt);
			}
		}

		/// <summary>
		/// Remove tile points from the range
		/// </summary>
		/// <param name="points">Points to remove from the range</param>
		public void removePoints(params Point[] points)
		{
			foreach (Point pt in points)
			{
				foreach (Point pnt in rangePoints)
					if (pt.X == pnt.X && pt.Y == pnt.Y)
					{
						rangePoints.Remove(pnt);
						break;
					}
			}
		}

		/// <summary>
		/// Remove tile points from the range
		/// </summary>
		/// <param name="points">List of point to remove from the range</param>
		public void removePoints(List<Point> points)
		{
			foreach (Point pt in points)
			{
				foreach (Point pnt in rangePoints)
					if (pt.X == pnt.X && pt.Y == pnt.Y)
					{
						rangePoints.Remove(pnt);
						break;
					}
			}
		}

		/// <summary>
		/// Remove all points from the range
		/// </summary>
		public void clearPoints()
		{
			rangePoints.RemoveRange(0, rangePoints.Count);
		}
		
		/// <summary>
		/// Get the Current Range Type frame for drawing
		/// </summary>
		private FrameAnimation currentRangeType
		{
			get
			{
				if (!string.IsNullOrEmpty(_currentRangeTypeName))
					return rangeTypes[_currentRangeTypeName];
				else
					return null;
			}
		}

		/// <summary>
		/// Get or set the Current Range Type by name
		/// </summary>
		public string currentRangeTypeName
		{
			get { return _currentRangeTypeName; }
			set
			{
				if (rangeTypes.ContainsKey(value))
					_currentRangeTypeName = value;
			}
		}
		
		/// <summary>
		/// Draw the range on the map
		/// </summary>
		/// <param name="batch">Sprite Batch passed from Game</param>
		/// <param name="camera">Camera passed from Game</param>
		public void draw(SpriteBatch batch, Camera camera)
		{
			if (isDrawing)
			{
				batch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, camera.transformationMatrix);
				foreach (Point pt in rangePoints)
				{
					batch.Draw(sprite, Engine.convertTileToPosition(pt), currentRangeType.currentFrame, new Color(1f, 1f, 1f, 0.05f));
				}
				batch.End();
			}
		}



	}
}
