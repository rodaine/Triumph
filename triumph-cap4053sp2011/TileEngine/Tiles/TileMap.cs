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
        #region dataMembers
        /// <summary>
		/// Collection of ordered Tile Layers to render
		/// </summary>
		public List<TileLayer> layers = new List<TileLayer>();

		/// <summary>
		/// Specifies locations on map that are collisions
		/// </summary>
		public CollisionLayer collisionLayer;

		/// <summary>
		/// Specifies locations and unit indeces of all player units
		/// </summary>
		public UnitLayer unitLayer;
        #endregion

        #region getBounds
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
        #endregion

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

		/// <summary>
		/// Gets the A* Optimal path between two tile points on the map.
		/// </summary>
		/// <param name="start">Starting Point</param>
		/// <param name="goal">Goal Point</param>
		/// <param name="otherCollisions">Other collisions (i.e., enemy sprites or objects) not on collision layer</param>
		/// <returns>Stack of tile points to travel through to reach goal</returns>
		public Stack<Point> getPath(Point start, Point goal, List<Point> otherCollisions)
		{
			Stack<Point> path = new Stack<Point>();
			List<KeyValuePair<Point, int[]>> openSet = new List<KeyValuePair<Point, int[]>>();
			List<KeyValuePair<Point, int[]>> closedSet = new List<KeyValuePair<Point, int[]>>();
			Comparison<KeyValuePair<Point, int[]>> compareHeuristic = new Comparison<KeyValuePair<Point, int[]>>(comparePathHeuristic);
			KeyValuePair<Point, int[]> currentTile = new KeyValuePair<Point,int[]>();
			bool foundGoal = false,
				isCollision = false,
				isOpen = false,
				isClosed = false,
				tentIsBetter = false;
			int openIndex = -1;

			//check input
			if (start.X < 0 || start.X >= getWidthInTiles() || start.Y < 0 || start.Y >= getHeightInTiles() ||
				goal.X < 0 || goal.X >= getWidthInTiles() || goal.Y < 0 || goal.Y >= getHeightInTiles())
				return path;

			if (!isEmpty(goal))
				return path;

			//add start to open set
			openSet.Add(new KeyValuePair<Point, int[]>(start, new int[] { 0, getDistance(start, goal), getDistance(start, goal), 0 }));

			while (openSet.Count > 0)
			{
				//get currentTile
				openSet.Sort(compareHeuristic);
				currentTile = openSet[0];
				
				closedSet.Add(currentTile);
				openSet.Remove(currentTile);

				if (currentTile.Key.X == goal.X && currentTile.Key.Y == goal.Y)
				{
					foundGoal = true;
					break;
				}

				foreach (KeyValuePair<Point, int[]> neighbor in getNeighbors(currentTile.Key))
				{
					isCollision = isOpen = isClosed = tentIsBetter = false;

					//check collision map
					if (collisionLayer.getTileCollisionIndex(neighbor.Key) == 1)
						continue;

					//check unit map
					if (unitLayer.getTileUnitIndex(neighbor.Key) > 0 && unitLayer.getTileUnitIndex(start) != unitLayer.getTileUnitIndex(neighbor.Key))
						continue;

					//check other collisions
					foreach (Point collision in otherCollisions)
					{
						if (collision.X == neighbor.Key.X && collision.Y == neighbor.Key.Y)
						{
							isCollision = true;
							break;
						}
					}
					if (isCollision) 
						continue;

					//check closed set
					foreach (KeyValuePair<Point, int[]> closed in closedSet)
					{
						if (closed.Key.X == neighbor.Key.X && closed.Key.Y == neighbor.Key.Y)
						{
							isClosed = true;
							break;
						}
					}
					if (isClosed)
						continue;

					neighbor.Value[0] = getDistance(start, currentTile.Key) + 1;
					neighbor.Value[1] = getDistance(goal, neighbor.Key);
					neighbor.Value[2] = neighbor.Value[0] + neighbor.Value[1];

					//check open set
					foreach (KeyValuePair<Point, int[]> open in openSet)
					{
						if (open.Key.X == neighbor.Key.X && open.Key.Y == neighbor.Key.Y)
						{
							if (neighbor.Value[0] < open.Value[0])
							{
								openIndex = openSet.IndexOf(open);
								tentIsBetter = true;
							}
				
							isOpen = true;
							break;
						}
					}
					if (isOpen)
					{
						if (tentIsBetter)
							openSet[openIndex] = neighbor;
						continue;
					}

					//add neighbor to open set
					openSet.Add(neighbor);
				}



			}

			if (!foundGoal)
				return path;

			//generate path stack
			Point pathPoint = currentTile.Key;
			while (closedSet.Count > 0)
			{
				path.Push(pathPoint);
				
				if (pathPoint.X == start.X && pathPoint.Y == start.Y)
					break;

				foreach (KeyValuePair<Point, int[]> closed in closedSet)
				{
					if (closed.Key.X == pathPoint.X && closed.Key.Y == pathPoint.Y)
					{
						switch (closed.Value[3])
						{
							case 1:
								pathPoint.X += 1;
								break;
							case 2:
								pathPoint.X -= 1;
								break;
							case 3:
								pathPoint.Y += 1;
								break;
							case 4:
								pathPoint.Y -= 1;
								break;
						}
						closedSet.Remove(closed);
						break;
					}
				}
			}


			return path;
		}

        /// <summary>
        /// Gets the A* Optimal path between two tile points on the map for a particular unit.
        /// </summary>
        /// <param name="currentUnit">The unit we are checking against.</param>
        /// <param name="start">Starting Point</param>
        /// <param name="goal">Goal Point</param>
        /// <param name="otherCollisions">Other collisions (i.e., enemy sprites or objects) not on collision layer</param>
        /// <returns>Stack of tile points to travel through to reach goal</returns>
        public Stack<Point> getPath(BaseUnit currentUnit, Point goal, List<Point> otherCollisions)
        {
            Stack<Point> path = new Stack<Point>();
            List<KeyValuePair<Point, int[]>> openSet = new List<KeyValuePair<Point, int[]>>();
            List<KeyValuePair<Point, int[]>> closedSet = new List<KeyValuePair<Point, int[]>>();
            Comparison<KeyValuePair<Point, int[]>> compareHeuristic = new Comparison<KeyValuePair<Point, int[]>>(comparePathHeuristic);
            KeyValuePair<Point, int[]> currentTile = new KeyValuePair<Point, int[]>();
            Point start = currentUnit.position;
            bool foundGoal = false,
                isCollision = false,
                isOpen = false,
                isClosed = false,
                tentIsBetter = false;
            int openIndex = -1;

            //check input
            if (start.X < 0 || start.X >= getWidthInTiles() || start.Y < 0 || start.Y >= getHeightInTiles() ||
                goal.X < 0 || goal.X >= getWidthInTiles() || goal.Y < 0 || goal.Y >= getHeightInTiles() || !canPass(currentUnit, goal))
                return path;

			//check goal
			if (!isEmpty(goal))
				return path;

            //add start to open set
            openSet.Add(new KeyValuePair<Point, int[]>(start, new int[] { 0, getDistance(start, goal), getDistance(start, goal), 0 }));

            while (openSet.Count > 0)
            {
                //get currentTile
                openSet.Sort(compareHeuristic);
                currentTile = openSet[0];

                closedSet.Add(currentTile);
                openSet.Remove(currentTile);

                if (currentTile.Key.X == goal.X && currentTile.Key.Y == goal.Y)
                {
                    foundGoal = true;
                    break;
                }

                foreach (KeyValuePair<Point, int[]> neighbor in getNeighbors(currentTile.Key))
                {
                    isCollision = isOpen = isClosed = tentIsBetter = false;

                    //check collision map
                    if (collisionLayer.getTileCollisionIndex(neighbor.Key) == 1)
                        continue;

                    //check unit map
					if (!canPass(currentUnit, neighbor.Key))
						continue;

                    //check other collisions
                    foreach (Point collision in otherCollisions)
                    {
                        if (collision.X == neighbor.Key.X && collision.Y == neighbor.Key.Y)
                        {
                            isCollision = true;
                            break;
                        }
                    }
                    if (isCollision)
                        continue;

                    //check closed set
                    foreach (KeyValuePair<Point, int[]> closed in closedSet)
                    {
                        if (closed.Key.X == neighbor.Key.X && closed.Key.Y == neighbor.Key.Y)
                        {
                            isClosed = true;
                            break;
                        }
                    }
                    if (isClosed)
                        continue;

                    neighbor.Value[0] = getDistance(start, currentTile.Key) + 1;
                    neighbor.Value[1] = getDistance(goal, neighbor.Key);
                    neighbor.Value[2] = neighbor.Value[0] + neighbor.Value[1];

                    //check open set
                    foreach (KeyValuePair<Point, int[]> open in openSet)
                    {
                        if (open.Key.X == neighbor.Key.X && open.Key.Y == neighbor.Key.Y)
                        {
                            if (neighbor.Value[0] < open.Value[0])
                            {
                                openIndex = openSet.IndexOf(open);
                                tentIsBetter = true;
                            }

                            isOpen = true;
                            break;
                        }
                    }
                    if (isOpen)
                    {
                        if (tentIsBetter)
                            openSet[openIndex] = neighbor;
                        continue;
                    }

                    //add neighbor to open set
                    openSet.Add(neighbor);
                }



            }

            if (!foundGoal)
                return path;

            //generate path stack
            Point pathPoint = currentTile.Key;
            while (closedSet.Count > 0)
            {
                path.Push(pathPoint);

                if (pathPoint.X == start.X && pathPoint.Y == start.Y)
                    break;

                foreach (KeyValuePair<Point, int[]> closed in closedSet)
                {
                    if (closed.Key.X == pathPoint.X && closed.Key.Y == pathPoint.Y)
                    {
                        switch (closed.Value[3])
                        {
                            case 1:
                                pathPoint.X += 1;
                                break;
                            case 2:
                                pathPoint.X -= 1;
                                break;
                            case 3:
                                pathPoint.Y += 1;
                                break;
                            case 4:
                                pathPoint.Y -= 1;
                                break;
                        }
                        closedSet.Remove(closed);
                        break;
                    }
                }
            }


            return path;
        }

		/// <summary>
		/// Get the points within walking range of a unit
		/// </summary>
		/// <param name="currentUnit">The unit currently attempting to walk</param>
		/// <param name="map">The tilemap</param>
		/// <returns>A list of points within walking range</returns>
		public List<Point> walkToPoints(BaseUnit currentUnit)
		{
			Stack<Point> openPoints = new Stack<Point>();
			List<Point> closedPoints = new List<Point>();
			List<Point> returnPoints = new List<Point>();

			if (currentUnit.MP == 0) return closedPoints;

			openPoints.Push(currentUnit.position);

			while (openPoints.Count > 0)
			{
				Point current = openPoints.Pop();

				//within walking range?
				if (getPath(currentUnit, current, new List<Point>()).Count - 1 > currentUnit.MP)
					continue;

				//is it empty?
				if (!canPass(currentUnit, current))
					continue;

				//check closedList
				bool isClosed = false;
				foreach (Point pt in closedPoints)
					if (current.X == pt.X && current.Y == pt.Y)
					{
						isClosed = true;
						break;
					}
				if (isClosed) continue;

				//good to go...get the neighbors and add to closedList
				List<KeyValuePair<Point, int[]>> neighbors = getNeighbors(current);
				foreach (KeyValuePair<Point, int[]> neighbor in neighbors)
					openPoints.Push(neighbor.Key);
				closedPoints.Add(current);
			}

			foreach (Point pt in closedPoints)
			{
				if (isEmpty(pt))
					returnPoints.Add(pt);
			}

			return returnPoints;
		}

		public List<Point> attackPoints(BaseUnit unit, int range, bool isFriendly, bool isHostile, bool isSelf)
		{
			Stack<Point> openPoints = new Stack<Point>();
			List<Point> closedPoints = new List<Point>();
			List<Point> ditchedPoints = new List<Point>();

			if (range == 0) return closedPoints;
			if (!isFriendly && !isHostile && !isSelf) return closedPoints;

			openPoints.Push(unit.position);

			while (openPoints.Count > 0)
			{
				Point current = openPoints.Pop();

				//within range?
				if (getDistance(unit.position, current) > range)
					continue;

				//check closedList
				bool isClosed = false;
				foreach (Point pt in closedPoints)
					if (current.X == pt.X && current.Y == pt.Y)
					{
						isClosed = true;
						break;
					}
				foreach (Point pt in ditchedPoints)
					if (current.X == pt.X && current.Y == pt.Y)
					{
						isClosed = true;
						break;
					}
				if (isClosed) continue;

				//get neighbors
				List<KeyValuePair<Point, int[]>> neighbors = getNeighbors(current);
				foreach (KeyValuePair<Point, int[]> neighbor in neighbors)
					openPoints.Push(neighbor.Key);

				if (unitLayer.getTileUnitIndex(current) == 0)
					continue;
				else
				{
					bool self = false, hostile = true;
					
					if (unitLayer.getTileUnitIndex(current) == unit.unitIndex)
						self = true;

					foreach (BaseUnit ut in unit.faction.units)
					{
						if (unitLayer.getTileUnitIndex(current) == ut.unitIndex)
							hostile = false;
					}

					if (self && isSelf)
					{
						closedPoints.Add(current);
						continue;
					}

					if (hostile && isHostile)
					{
						closedPoints.Add(current);
						continue;
					}

					if (!hostile && !self && isFriendly)
					{
						closedPoints.Add(current);
						continue;
					}

					ditchedPoints.Add(current);
				}
			}

			return closedPoints;
		}

		/// <summary>
		/// Comparison function used by the getPath() method
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <returns></returns>
		private int comparePathHeuristic(KeyValuePair<Point, int[]> a,KeyValuePair<Point, int[]>  b)
		{
			return a.Value[2].CompareTo(b.Value[2]);
		}

		/// <summary>
		/// Gets the neighbors of a point on the map
		/// </summary>
		/// <param name="point">Point from which to find neighbors</param>
		/// <returns>KeyValuePair of neighboring points and thier relative direction to the source Point</returns>
		/// <remarks>
		/// Collisions are not detected in this method. Used exclusively by the getPath() method.
		/// </remarks>
		private List<KeyValuePair<Point, int[]>> getNeighbors(Point point)
		{
			List<KeyValuePair<Point, int[]>> neighbors = new List<KeyValuePair<Point, int[]>>();

			//left neighbor
			if (point.X > 0)
				neighbors.Add(new KeyValuePair<Point, int[]>(new Point(point.X - 1, point.Y), new int[] { 0, 0, 0, 1 }));

			//right neighbor
			if (point.X < getWidthInTiles() - 1)
				neighbors.Add(new KeyValuePair<Point, int[]>(new Point(point.X + 1, point.Y), new int[] { 0, 0, 0, 2 }));

			//up neighbor
			if (point.Y > 0)
				neighbors.Add(new KeyValuePair<Point, int[]>(new Point(point.X, point.Y - 1), new int[] { 0, 0, 0, 3 }));

			//down neighbor
			if (point.Y < getHeightInTiles() - 1)
				neighbors.Add(new KeyValuePair<Point, int[]>(new Point(point.X, point.Y + 1), new int[] { 0, 0, 0, 4 }));

			return neighbors;
		}

		/// <summary>
		/// Gets the discrete tile-based distance between points on the map
		/// </summary>
		/// <param name="a">First Point</param>
		/// <param name="b">Second Point</param>
		/// <returns>Tile distance between points</returns>
		public int getDistance(Point a, Point b)
		{
			return Math.Abs(a.X - b.X) + Math.Abs(a.Y - b.Y);
		}

		/// <summary>
		/// Specifies whether a unit can pass through a given tile on the map. Units may pass over allies
		/// </summary>
		/// <param name="bu"></param>
		/// <param name="p"></param>
		/// <returns></returns>
        public bool canPass(BaseUnit bu, Point p)
        {
			if (isEmpty(p)) return true;
			if (this.unitLayer.getTileUnitIndex(p) < 0) return true;

			foreach (BaseUnit unit in bu.faction.units)
			{
				if (this.unitLayer.getTileUnitIndex(p) == unit.unitIndex)
					return true;
			}

			return false;
        }

        public bool isEmpty(Point p)
        {
            return (this.collisionLayer.getTileCollisionIndex(p) == 0 && this.unitLayer.getTileUnitIndex(p) == 0);
        }
	}

}
