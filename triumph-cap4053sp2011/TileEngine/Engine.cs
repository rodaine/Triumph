using Microsoft.Xna.Framework;

namespace TileEngine
{
	/// <summary>
	/// Describes global Tile Engine constants and static methods used throughout the project
	/// </summary>
	public class Engine
	{
	
		/// <summary>
		/// Width in pixels of a tile rendered on the map
		/// </summary>
		public const int TILE_WIDTH = 32;

		/// <summary>
		/// Height in pixels of a tile rendered on the map
		/// </summary>
		public const int TILE_HEIGHT = 32;

		public static Point convertPositionToTile(Vector2 position)
		{
			return new Point(
				(int)(position.X/(float)TILE_WIDTH),
				(int)(position.Y/(float)TILE_HEIGHT)
				);
		}

		public static Rectangle createRectForTile(Point tile)
		{
			return new Rectangle(
				tile.X * TILE_WIDTH,
				tile.Y * TILE_HEIGHT,
				TILE_WIDTH,
				TILE_HEIGHT
				);
		}
	}
}
