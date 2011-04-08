using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace TileEngine
{
    public class AI
    {
        #region data members
        private bool _startNextTurn;
        private BaseUnit _target;
        private Point _targetPoint;
        private bool _hasAttacked;
        #endregion

        #region constructor
        public AI()
        {
            this.startNextTurn();
        }
        #endregion

        #region private methods

        /// <summary>
        /// Initializes all the flags for the next turn
        /// </summary>
        private void startNextTurn()
        {
            _startNextTurn = true;
            _target = null;
            _hasAttacked = false;
        }

        /// <summary>
        /// Should be run on the AI's first update after its turn starts. Chooses a target
        /// </summary>
        /// <param name="currentUnit">The AI unit whose turn it is</param>
        /// <param name="map">The game map</param>
        /// <param name="testUnits">All the units on the map</param>
        /// <returns>True if a target was found, false otherwise</returns>
        private bool chooseTarget(BaseUnit currentUnit, TileMap map, BaseUnit[] testUnits)
        {
            _target = null;
            Faction myFaction = currentUnit.faction;
            _targetPoint = currentUnit.position;
            int minDist = Int32.MaxValue;

            foreach (BaseUnit bu in testUnits)
            {
                if (!bu.isDead && myFaction != bu.faction)
                {
                    Point targetLoc = bu.position;
                    List<Point> possibleTargets = new List<Point>();
                    possibleTargets.Add(new Point(targetLoc.X + 1, targetLoc.Y));
                    possibleTargets.Add(new Point(targetLoc.X - 1, targetLoc.Y));
                    possibleTargets.Add(new Point(targetLoc.X, targetLoc.Y + 1));
                    possibleTargets.Add(new Point(targetLoc.X, targetLoc.Y - 1));
                    foreach (Point p in possibleTargets)
                    {

                        if (map.isEmpty(p) || currentUnit.position == p)
                        {
                            int dist = map.getPath(currentUnit, p, new List<Point>()).Count;
                            if (dist < minDist)
                            {
                                _target = bu;
                                minDist = dist;
                                _targetPoint = p;
                            }
                        }
                    }
                }
            }
            return (_target != null);
        }
        #endregion

        #region public methods
        /// <summary>
        /// 
        /// </summary>
        /// <param name="gameTime"></param>
        /// <param name="currentUnit"></param>
        /// <param name="cursor"></param>
        /// <param name="map"></param>
        /// <param name="viewWidth"></param>
        /// <param name="viewHeight"></param>
        /// <param name="testUnits"></param>
        /// <param name="camera"></param>
        public void update(GameTime gameTime, BaseUnit currentUnit, Cursor cursor, TileMap map, int viewWidth, int viewHeight, BaseUnit[] testUnits, Camera camera)
        {
            if (_startNextTurn)
            {
                if (chooseTarget(currentUnit, map, testUnits))
                {
                    _startNextTurn = false;
                }
                else
                {
                    this.startNextTurn();
                    currentUnit.isDone = true;
                }
            }
            else
            {
                if (!currentUnit.isWalking)
                {
                    //System.Console.Error.WriteLine(currentUnit.name + " is targeting " + target.name);
                    if (_hasAttacked)
                    {
                        //AI currently too dumb to do anything after attacking
                        this.startNextTurn();
                        currentUnit.isDone = true;
                    }
                    else if (!_hasAttacked && currentUnit.MP > 0 && !currentUnit.withinRange(_target))
                    {

                        Stack<Point> path = map.getPath(currentUnit.position, _targetPoint, new List<Point>());

                        Point walkTo = currentUnit.position;
                        int count = currentUnit.MP;
                        while (path.Count > 0 && count >= 0)
                        {
                            --count;
                            walkTo = path.Pop();
                        }
                        System.Console.Error.WriteLine("Trying to move from " + currentUnit.position + " to " + walkTo);
                        currentUnit.goToTile(walkTo, map);
                    }
                    else if (!_hasAttacked && currentUnit.withinRange(_target))
                    {
                        currentUnit.attack(_target);
                        this._hasAttacked = true;
                    }
                    else if (currentUnit.MP == 0)
                    {
                        this.startNextTurn();
                        currentUnit.isDone = true;
                    }
                }
            }

            camera.update(viewWidth, viewHeight, map);
            cursor.update(gameTime, viewWidth, viewHeight, map);
            foreach (BaseUnit bu in testUnits)
            {
                bu.update(gameTime, viewWidth, viewHeight, map);
            }
        }
#endregion


    }
}
