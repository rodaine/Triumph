using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace TileEngine
{

	/// <summary>
	/// Describes a collection of Tile Layers to be drawn together
	/// </summary>
	public class TileMap
	{
		/// <summary>
		/// Collection of ordered Tile Layers to render
		/// </summary>
		public List<TileLayer> layers = new List<TileLayer>();

		public CollisionLayer collisionLayer;

		/// <summary>
		/// Get the width (x-direction) in tiles of all Tile Layers in map
		/// </summary>
		/// <returns>Integer width in tiles of map</returns>
		public int getWidthInTiles()
		{
			int width = -1;

			foreach (TileLayer layer in layers)
			{
				width = (int)Math.Max(width, layer.widthInTiles);
			}

			return width;
		}

		/// <summary>
		/// Get the height (y-direction) in tiles of all Tile Layers in map
		/// </summary>
		/// <returns>Integer height in tiles of map</returns>
		public int getHeightInTiles()
		{
			int height = -1;

			foreach (TileLayer layer in layers)
			{
				height = (int)Math.Max(height, layer.heightInTiles);
			}

			return height;
		}

		/// <summary>
		/// Get the width (x-direction) in pixels of the entire map
		/// </summary>
		/// <returns>Integer width in pixels of map</returns>
		public int getWidthInPixels()
		{
			return getWidthInTiles() * Engine.TILE_WIDTH;
		}

		/// <summary>
		/// Get the height (y-direction) in pixels of the entire map
		/// </summary>
		/// <returns>Integer height in tiles of map</returns>
		public int getHeightInPixels()
		{
			return getHeightInTiles() * Engine.TILE_HEIGHT;
		}

		/// <summary>
		/// Draws the Tile Layers contained in the map together
		/// </summary>
		/// <param name="spriteBatch">SpriteBatch used to render layers</param>
		/// <param name="camera">Camera used to view map</param>
		/// <remarks>TileLayers are rendered in ascending order from index 0</remarks>
		public void draw(SpriteBatch spriteBatch, Camera camera)
		{
			foreach (TileLayer layer in layers)
				layer.draw(
					spriteBatch, 
					camera, 
					Engine.convertPositionToTile(camera.position), 
					Engine.convertPositionToTile(camera.position + new Vector2(
						spriteBatch.GraphicsDevice.Viewport.Width + Engine.TILE_WIDTH,
						spriteBatch.GraphicsDevice.Viewport.Height + Engine.TILE_HEIGHT)
						)
					);

		}

	}







	
}
