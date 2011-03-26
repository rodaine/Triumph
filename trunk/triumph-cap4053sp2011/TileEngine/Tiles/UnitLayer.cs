using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace TileEngine
{
	/// <summary>
	/// Describes a collision map layer
	/// </summary>
	public class UnitLayer
	{
		/// <summary>
		/// 
		/// </summary>
		private int[,] layout;

		#region Initializers & IO

		/// <summary>
		/// Creates an empty Tile Layer with specified dimensions
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

		/// <summary>
		/// Performs the IO on a .layer file to obtain a saved Tile Layer used by both fromFile() methods
		/// </summary>
		/// <param name="filename">Path (relative or absolute) to the .layer file</param>
		/// <param name="textureNames">List of string texture names referenced by the layer</param>
		/// <returns>New Tile Layer</returns>
		public static UnitLayer fromFile(string filename)
		{
			UnitLayer layer;
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
			layer = new UnitLayer(width, height);

			for (int y = 0; y < height; ++y)
				for (int x = 0; x < width; ++x)
					layer.setUnitIndex(x, y, tempLayout[y][x]);

			return layer;
		}

		/// <summary>
		/// Saves a Tile Layer to file [used by Tile Editor]
		/// </summary>
		/// <param name="filename">Path (relative or absolute) where the .layer file should be saved</param>
		/// <param name="textureNames">String array of texture names (relative to a content root) used by the layer</param>
		public void save(string filename, string[] textureNames)
		{
			using (StreamWriter writer = new StreamWriter(filename))
			{
				writer.WriteLine("[Layout]");
				for (int y = 0; y < heightInTiles; ++y)
				{
					string line = string.Empty;

					for (int x = 0; x < widthInTiles; ++x)
					{
						line += layout[y, x].ToString() + " ";
					}

					writer.WriteLine(line);
				}

				writer.WriteLine();
			}
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

		public void setUnitIndex(int x, int y, int unitIndex)
		{
			if (x < 0 || x > widthInTiles - 1 || y < 0 || y > heightInTiles - 1)
				return;

			layout[y, x] = unitIndex;
		}

		public void setUnitIndex(Point point, int unitIndex)
		{
			if (point.X < 0 || point.X >= widthInTiles || point.Y < 0 || point.Y >= heightInTiles)
				return;

			layout[point.Y, point.X] = unitIndex;

		}

		public void moveUnit(int unitIndex, int x, int y)
		{
			if (x < 0 || x > widthInTiles - 1 || y < 0 || y > heightInTiles - 1)
				return;

			for (int X = 0; X < widthInTiles; ++X)
				for (int Y = 0; Y < heightInTiles; ++Y)
					if (layout[Y, X] == unitIndex)
					{
						layout[Y, X] = 0;
					}

			layout[y, x] = unitIndex;
		}

		public void moveUnit(int unitIndex, Point point)
		{
			if (point.X < 0 || point.X >= widthInTiles || point.Y < 0 || point.Y >= heightInTiles)
				return;

			for (int X = 0; X < widthInTiles; ++X)
				for (int Y = 0; Y < heightInTiles; ++Y)
					if (layout[Y, X] == unitIndex)
					{
						layout[Y, X] = 0;
					}

			layout[point.Y, point.X] = unitIndex;
		}
		
		/// <summary>
		/// Get the index of a texture at a particular tile
		/// </summary>
		/// <param name="x">0-Based x-direction tile location</param>
		/// <param name="y">0-Based y-direction tile location</param>
		/// <returns>0-Based texture index; else, -1 for an empty tile or -2 if x/y out of range</returns>
		public int getTileUnitIndex(int x, int y)
		{
			if (x < 0 || x > widthInTiles - 1 || y < 0 || y > heightInTiles - 1)
				return -1;

			return layout[y, x];
		}

		public int getTileUnitIndex(Point point)
		{
			if (point.X < 0 || point.X >= widthInTiles || point.Y < 0 || point.Y >= heightInTiles)
				return -1;

			return layout[point.Y, point.X];
		}

		#endregion

	}
}
