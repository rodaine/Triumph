using System;
using Microsoft.Xna.Framework;

namespace TileEngine
{
	/// <summary>
	/// Describes a unit map layer
	/// </summary>
	public class UnitLayer
	{

		#region Private Properties

		private int[,] layout;

		#endregion

		#region Initializers

		/// <summary>
		/// Creates an empty Unit Layer with specified dimensions
		/// </summary>
		/// <param name="width">Integer width (x-direction) of layer</param>
		/// <param name="height">Integer height (y-direction) of layer</param>
		public UnitLayer(int width, int height)
		{
			layout = new int[height, width];
			for (int y = 0; y < height; ++y)
			{
				for (int x = 0; x < width; ++x)
				{
					layout[y, x] = 0;
				}
			}
		}

		#endregion

		#region Dimensional Methods

		/// <summary>
		/// Returns the width in tiles of the Unit Layer
		/// </summary>
		private int widthInTiles
		{
			get { return layout.GetLength(1); }
		}

		/// <summary>
		/// Returns the height in tiles of the Unit Layer
		/// </summary>
		private int heightInTiles
		{
			get { return layout.GetLength(0); }
		}

		#endregion

		#region Map Methods

		/// <summary>
		/// Sets the index at a specified point in the Unit Layer
		/// </summary>
		/// <param name="point">Tile location where to set the index</param>
		/// <param name="unitIndex">Index to set the tile to</param>
		public void setUnitIndex(Point point, int unitIndex)
		{
			if (point.X < 0 || point.X >= widthInTiles || point.Y < 0 || point.Y >= heightInTiles)
				return;

			layout[point.Y, point.X] = unitIndex;

		}

		/// <summary>
		/// Relocate the specified index to a given point on the map
		/// </summary>
		/// <param name="unitIndex">Index to relocate on the map</param>
		/// <param name="point">Tile location to place the index</param>
		public void moveUnit(int unitIndex, Point point)
		{
			if (point.X < 0 || point.X >= widthInTiles || point.Y < 0 || point.Y >= heightInTiles)
				return;

			for (int X = 0; X < widthInTiles; ++X)
				for (int Y = 0; Y < heightInTiles; ++Y)
					if (Math.Abs(layout[Y, X]) == Math.Abs(unitIndex))
					{
						layout[Y, X] = 0;
					}

			layout[point.Y, point.X] = unitIndex;
		}
		
		/// <summary>
		/// Get the unit index at the specified tile location
		/// </summary>
		/// <param name="point">Tile location to check for a unit index</param>
		/// <returns>Returns the index at the given location; else, 0 for no unit present or -1 if point is out of range</returns>
		public int getTileUnitIndex(Point point)
		{
			if (point.X < 0 || point.X >= widthInTiles || point.Y < 0 || point.Y >= heightInTiles)
				return -1;

			return layout[point.Y, point.X];
		}

		#endregion

	}
}
