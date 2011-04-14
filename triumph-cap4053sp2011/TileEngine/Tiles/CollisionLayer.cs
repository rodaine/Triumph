using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;

namespace TileEngine
{
	/// <summary>
	/// Describes a collision map layer
	/// </summary>
	public class CollisionLayer
	{

		#region Private Properties

		private int[,] layout;

		#endregion

		#region Initializers & IO

		/// <summary>
		/// Creates an empty Collision Layer with specified dimensions
		/// </summary>
		/// <param name="width">Integer width (x-direction) of layer</param>
		/// <param name="height">Integer height (y-direction) of layer</param>
		public CollisionLayer(int width, int height)
		{
			layout = new int[height, width];
			for (int y = 0; y < height; ++y)
			{
				for (int x = 0; x < width; ++x)
				{
					layout[y, x] = -1;
				}
			}
		}

		/// <summary>
		/// Performs the IO on a .layer file to obtain a saved Collision Layer used by both fromFile() methods
		/// </summary>
		/// <param name="filename">Path (relative or absolute) to the .layer file</param>
		/// <returns>New Collision Layer</returns>
		public static CollisionLayer fromFile(string filename)
		{
			CollisionLayer layer;
			List<List<int>> tempLayout = new List<List<int>>();
			using (StreamReader reader = new StreamReader(filename))
			{
				bool readingLayout = false;
				while (!reader.EndOfStream)
				{
					string line = reader.ReadLine().Trim();

					if (string.IsNullOrEmpty(line))
						continue;

					if (line.Contains("[Layout]"))
					{
						readingLayout = true;
					}
					else if (readingLayout)
					{
						List<int> row = new List<int>();
						string[] cells = line.Split(' ');
						foreach (string c in cells)
						{
							if (!string.IsNullOrEmpty(c))
								row.Add(int.Parse(c));
						}
						tempLayout.Add(row);
					}
				}
			}

			int width = tempLayout[0].Count;
			int height = tempLayout.Count;
			layer = new CollisionLayer(width, height);

			for (int y = 0; y < height; ++y)
				for (int x = 0; x < width; ++x)
					layer.setTileCollisionIndex(x, y, tempLayout[y][x]);

			return layer;
		}

		#endregion

		#region Dimension Properties
	
			/// <summary>
			/// Returns the width in tiles of the Tile Layer
			/// </summary>
			public int widthInTiles
			{
				get { return layout.GetLength(1); }
			}
		
			/// <summary>
			/// Returns the height in tiles of the Tile Layer
			/// </summary>
			public int heightInTiles
			{
				get { return layout.GetLength(0); }
			}
	
		#endregion
	
		#region Map Methods

			/// <summary>
			/// Set a particular tile's collision index
			/// </summary>
			/// <param name="x">0-Based x-direction tile location</param>
			/// <param name="y">0-Based y-direction tile location</param>
			/// <param name="tileIndex">0-Based collision index; else, -1 for an empty tile</param>
			public void setTileCollisionIndex(int x, int y, int tileIndex)
			{
				if (x < 0 || x > widthInTiles - 1 || y < 0 || y > heightInTiles - 1)
					return;

				layout[y, x] = tileIndex;
			}

			/// <summary>
			/// Set a particular tile's collision index
			/// </summary>
			/// <param name="point">Point specifying the location of the tile</param>
			/// <param name="tileIndex">0-Based collision index; else, -1 for an empty tile</param>
			public void setTileCollisionIndex(Point point, int tileIndex)
			{
				if (point.X < 0 || point.X >= widthInTiles || point.Y < 0 || point.Y >= heightInTiles)
					return;

				layout[point.Y, point.X] = tileIndex;
			}

			/// <summary>
			/// Get the collision index at a particular tile
			/// </summary>
			/// <param name="x">0-Based x-direction tile location</param>
			/// <param name="y">0-Based y-direction tile location</param>
			/// <returns>0-Based collision index; else, -1 for an empty tile or -2 if x/y out of range</returns>
			public int getTileCollisionIndex(int x, int y)
			{
				if (x < 0 || x > widthInTiles - 1 || y < 0 || y > heightInTiles - 1)
					return -2;
				
				return layout[y, x];
			}

			/// <summary>
			/// Get the collision index at a particular tile
			/// </summary>
			/// <param name="point">Point specifying the location of the tile</param>
			/// <returns>0-Based collision index; else, -1 for an empty tile or -2 if x/y out of range</returns>
			public int getTileCollisionIndex(Point point)
			{
				if (point.X < 0 || point.X >= widthInTiles || point.Y < 0 || point.Y >= heightInTiles)
					return -2;

				return layout[point.Y, point.X];
			}

		#endregion

    }
}
