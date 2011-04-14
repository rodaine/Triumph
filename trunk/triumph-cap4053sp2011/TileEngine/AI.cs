using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace TileEngine
{
    public class AI
    {
        private enum AIState { startTurn, chooseTarget, chooseMoveTowards, chooseMoveAway, chooseAttack, moveToTarget, attackTarget, endTurn };

        #region data members
        private bool _startNextTurn;
        private BaseUnit _targetUnit;
        private Point _targetPoint;
        private AIState _myState;
        private int[] _APToRange;
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
            _targetUnit = null;
            _myState = AIState.startTurn;
        }

        /// <summary>
        /// Should be run on the AI's first update after its turn starts. Chooses a target
        /// </summary>
        /// <param name="currentUnit">The AI unit whose turn it is</param>
        /// <param name="map">The game map</param>
        /// <param name="testUnits">All the units on the map</param>
        /// <returns>True if a target was found, false otherwise</returns>
        private bool chooseMoveIntoRange(BaseUnit currentUnit, TileMap map, BaseUnit[] testUnits)
        {
            Faction myFaction = currentUnit.faction;
            _targetPoint = currentUnit.position;
            int minDist = Int32.MaxValue;

            HashSet<Point> possibleTargets = new HashSet<Point>();

            //Generate list of 
            foreach (BaseUnit bu in testUnits)
            {
                if (!bu.isDead && myFaction != bu.faction)
                {
                    Point targetLoc = bu.position;
                    
                    //TODO fix ranges
                    possibleTargets.Add(new Point(targetLoc.X + 1, targetLoc.Y));
                    possibleTargets.Add(new Point(targetLoc.X - 1, targetLoc.Y));
                    possibleTargets.Add(new Point(targetLoc.X, targetLoc.Y + 1));
                    possibleTargets.Add(new Point(targetLoc.X, targetLoc.Y - 1));
                }
            }

            foreach (Point p in possibleTargets)
            {

                if (map.isEmpty(p) || currentUnit.position == p)
                {
                    int dist = map.getPath(currentUnit, p, new List<Point>()).Count;
                    if (dist < minDist)
                    {
                        minDist = dist;
                        _targetPoint = p;
                    }
                }
            }

            return (_targetPoint.Equals(currentUnit.position));
        }


        //TODO fix this
        /// <summary>
        /// Chooses who to attack from the set of units in attack range
        /// </summary>
        /// <param name="currentUnit"></param>
        /// <param name="map"></param>
        /// <param name="testUnits"></param>
        /// <returns></returns>
        private bool chooseAttackTarget(BaseUnit currentUnit, TileMap map, BaseUnit[] testUnits)
        {
            _targetUnit = null;
            Faction myFaction = currentUnit.faction;

            HashSet<BaseUnit> possibleTargets = new HashSet<BaseUnit>();

            //Generate list of 
            foreach (BaseUnit bu in testUnits)
            {
                if (!bu.isDead && myFaction != bu.faction && currentUnit.withinRange(bu))
                {
                    _targetUnit = bu;
                    break;
                }
            }

            
            return (_targetUnit != null);
        }

        /// <summary>
        /// Figures out which ability is the best to be used from this spot.
        /// </summary>
        /// <param name="currentUnit"></param>
        /// <param name="map"></param>
        /// <param name="testUnits"></param>
        /// <returns></returns>
        private Ability determineMostPrudentAbility(BaseUnit currentUnit, TileMap map, BaseUnit[] testUnits)
        {
            return null;
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
            if (!currentUnit.isWalking && !currentUnit.isAttacking)
            {
                switch (_myState)
                {
                    #region startTurn
                    case AIState.startTurn:
                        {
                            //TODO napsack problem to figure out best combination of abilities
                            /*
                            _APToRange = new int[currentUnit.AP + 1];
                            for (int i = 0; i <= currentUnit.AP; ++i)
                            {

                            }
                             */

                            if (currentUnit.AP > 0)
                            {
                                _myState = AIState.chooseTarget;
                            }
                            else if (currentUnit.MP > 0)
                            {
                                _myState = AIState.chooseMoveAway;
                            }
                            else
                            {
                                _myState = AIState.endTurn;
                            }
                        }
                    break;
                    #endregion

                    //TODO add functionality for going to moveAway
                    #region chooseTarget
                    case AIState.chooseTarget:
                    {
                        chooseMoveIntoRange(currentUnit, map, testUnits);

                        if (chooseAttackTarget(currentUnit, map, testUnits)  && currentUnit.AP > 0)
                        {
                            _myState = AIState.chooseAttack;
                        }
                        else if (currentUnit.MP > 0)
                        {
                            _myState = AIState.chooseMoveTowards;
                        }
                        else
                        {
                            _myState = AIState.endTurn;
                        }
                    }
                    break;
                    #endregion

                        //TODO Implement intelligent selection of abilities
                    #region chooseAttack
                    case AIState.chooseAttack:
                    {
                        if (chooseAttackTarget(currentUnit, map, testUnits))
                        {
                            _myState = AIState.attackTarget;
                        }
                        else
                        {
                            _myState = AIState.endTurn;
                        }
                    }
                    break;
                    #endregion

                        //TODO implement
                    #region chooseMoveAway
                    case AIState.chooseMoveAway:
                    {
                        _myState = AIState.endTurn;
                    }
                    break;
                    #endregion

                        //TODO 
                    #region chooseMoveTowards
                    case AIState.chooseMoveTowards:
                    {
                        Stack<Point> fullPath = map.getPath(currentUnit, _targetPoint, new List<Point>());
                        int cnt = currentUnit.MP;
                        Point shortTarg = currentUnit.position;
                        while (--cnt >= 0 && fullPath.Count > 0)
                        {
                            shortTarg = fullPath.Pop();
                        }
                        _targetPoint = shortTarg;

                        if (_targetPoint != currentUnit.position)
                        {
                            _myState = AIState.moveToTarget;
                        }
                        else
                        {
                            _myState = AIState.endTurn;
                        }

                    }
                    break;
                    #endregion

                        //TODO
                    #region attackTarget
                    case AIState.attackTarget:
                    {
                        currentUnit.attack(_targetUnit);
                        if (currentUnit.AP == 0)
                        {
                            if (currentUnit.MP > 0)
                            {
                                _myState = AIState.chooseMoveAway;
                            }
                            else
                            {
                                _myState = AIState.endTurn;
                            }
                        }
                        if (!_targetUnit.isDead && currentUnit.AP > 0)
                        {
                            _myState = AIState.chooseTarget;
                        }
                    }
                    break;
                    #endregion

                        //TODO
                    #region moveToTarget
                    case AIState.moveToTarget:
                    {
                        currentUnit.goToTile(_targetPoint, map);

                        //TODO delay

                        _myState = AIState.chooseAttack;


                    }
                    break;
                    #endregion

                    //TODO
                    #region endTurn
                    case AIState.endTurn:
                    {
                        currentUnit.isDone = true;
                        startNextTurn();
                    }
                    break;
                    #endregion

                }
            }

            //TODO gameclock?
            #region View Updates
            camera.update(viewWidth, viewHeight, map);
            //cursor.update(gameTime, viewWidth, viewHeight, map);

            foreach (BaseUnit bu in testUnits)
            {
                bu.update(gameTime, map);
            }
            #endregion

        }
#endregion


    }
}
