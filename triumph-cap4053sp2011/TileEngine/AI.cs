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
        private bool _hasMoved;
        private int _thinkDelay;
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
            _hasMoved = false;
            _thinkDelay = 0;
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
        /// AI tells the current unit what to do
        /// </summary>
        /// <param name="gameTime">Current gametime</param>
        /// <param name="currentUnit">The unit whose turn it is</param>
        /// <param name="cursor">The cursor</param>
        /// <param name="map">The map</param>
        /// <param name="viewWidth">The width of the viewport</param>
        /// <param name="viewHeight">The height of the viewport</param>
        /// <param name="testUnits">The list of all units</param>
        /// <param name="camera">The current camera</param>
        public void update(GameTime gameTime, BaseUnit currentUnit, Cursor cursor, TileMap map, int viewWidth, int viewHeight, BaseUnit[] testUnits, Camera camera)
        {
            if (_startNextTurn)
            {
                if (chooseTarget(currentUnit, map, testUnits))
                {
                    _startNextTurn = false;
                    _thinkDelay = 50;
                }
                else
                {
                    this.startNextTurn();
                    currentUnit.isDone = true;
                }
            }
            else
            {
                if (!currentUnit.isWalking && !currentUnit.isAttacking)
                {
                    //System.Console.Error.WriteLine(currentUnit.name + " is targeting " + target.name);
                    if (_thinkDelay > 0)
                    {
                        --_thinkDelay;
                    }
                    else if (_hasAttacked)
                    {
                        //AI currently too dumb to do anything after attacking
                        this.startNextTurn();
                        currentUnit.isDone = true;
                    }
                    else if (!_hasMoved && currentUnit.MP > 0 && !currentUnit.withinRange(_target))
                    {

                        Stack<Point> path = map.getPath(currentUnit, _targetPoint, new List<Point>());
                        Stack<Point> myPath = new Stack<Point>();
                        int count = currentUnit.MP;
                        while (path.Count > 0 && count >= 0)
                        {
                            --count;
                            myPath.Push(path.Pop());
                        }

                        if (myPath.Count > 0)
                        {
                            Point walkTo = myPath.Pop();
                            while (myPath.Count > 0 && map.getPath(currentUnit, walkTo, new List<Point>()).Count == 0)
                            {
                                walkTo = myPath.Pop();
                            }
                            //System.Console.Error.WriteLine("Trying to move from " + currentUnit.position + " to " + walkTo);
                            currentUnit.goToTile(walkTo, map);
                        }
                        _hasMoved = true;
                        this._thinkDelay = 25;
                    }
                    else if (!_hasAttacked && currentUnit.withinRange(_target))
                    {
                        currentUnit.attack(_target);
                        this._hasAttacked = true;
                        this._thinkDelay = 25;
                        
                    }
                    else if (_hasMoved)
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
