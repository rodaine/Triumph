using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace TileEngine
{
    public class AI
    {
        private enum AIState { startTurn, chooseTarget, chooseMoveTowards, chooseMoveAway, chooseAttack, moveToTarget, attackTarget, endTurn };

        #region data members
        private double waitTil;
        private BaseUnit _targetUnit;
        private Point _targetPoint;
        private Ability _abilityToUse;
        private AIState _myState;
        private int[] _APToRange;
        private float dmgWeight;
        private float distWeight;
        private float defWeight;
        private float statusWeight;
        #endregion

        #region constructor

        /// <summary>
        /// Comment
        /// </summary>
        /// <param name="dmgWeight">c</param>
        /// <param name="distWeight">c</param>
        /// <param name="defWeight">c</param>
        /// <param name="statusWeight">c</param>
        public AI(float dmgWeight, float distWeight, float defWeight, float statusWeight)
        {
            this.dmgWeight = dmgWeight;
            this.distWeight = distWeight;
            this.defWeight = defWeight;
            this.statusWeight = statusWeight;
            this.waitTil = 0;

            this.startNextTurn();
        }

        #endregion

        #region private methods

        /// <summary>
        /// Initializes all the flags for the next turn
        /// </summary>
        private void startNextTurn()
        {
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
            int maxRange = _APToRange[currentUnit.AP];
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

                    for (int i = -maxRange; i <= maxRange; ++i)
                    {
                        for(int j=maxRange-Math.Abs(i); j<=(maxRange-Math.Abs(i)); ++j)
                        {
                            int iOff = bu.position.X + i;
                            int jOff = bu.position.Y + j;
                            if(iOff >= 0 && iOff < map.getWidthInTiles() && jOff >= 0 && jOff < map.getHeightInTiles() &&
                                    map.isEmpty(new Point(iOff,jOff)) )
                            {
                                possibleTargets.Add(new Point(iOff, jOff));
                            }
                        }
                    }
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="currentUnit"></param>
        /// <param name="map"></param>
        /// <returns></returns>
        private bool chooseMoveAway(BaseUnit currentUnit, TileMap map)
        {
            Faction myFaction = currentUnit.faction;
            _targetPoint = currentUnit.faction.rallyPoint;
            
            return (_targetPoint.Equals(currentUnit.position));
        }

        /// <summary>
        /// Chooses the ability to use. Requires _targetUnit to exist
        /// </summary>
        /// <param name="currentUnit">The unit whose turn it is</param>
        /// <param name="map">The game map</param>
        private bool chooseAbilityToUse(BaseUnit currentUnit, TileMap map)
        {
            List<Ability> choices = new List<Ability>();
            if(currentUnit.withinRange(_targetUnit))
                choices.Add(null);
            foreach(Ability ab in currentUnit.moves)
            {
                if (currentUnit.canTargetAbility(ab, _targetUnit) && ab.APCost <= currentUnit.AP)
                {
                    choices.Add(ab);
                }
            }
            if (choices.Count < 1)
            {
                return false;
            }
            int abNum = RandomNumber.getInstance().getNext(0, choices.Count - 1);

            _abilityToUse = choices.ToArray()[abNum];
            return true;

        }


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
                if (!bu.isDead && myFaction != bu.faction && map.getDistance(currentUnit.position, bu.position) <= _APToRange[currentUnit.AP])
                {
                    _targetUnit = bu;
                    break;
                }
            }

            
            return (_targetUnit != null);
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
        public void update(GameTime gameTime, BaseUnit currentUnit, Cursor cursor, TileMap map, int viewWidth, int viewHeight, 
                            BaseUnit[] testUnits, Camera camera, bool somethingBeingAttacked)
        {

            if (!currentUnit.isWalking && !currentUnit.isAttacking && !somethingBeingAttacked && gameTime.TotalGameTime.TotalSeconds > waitTil) 
            {
                switch (_myState)
                {
                    #region startTurn
                    case AIState.startTurn:
                        {

                            waitTil = gameTime.TotalGameTime.TotalSeconds + 1;

                            _APToRange = new int[currentUnit.AP + 1];

                            for (int i = 1; i <= currentUnit.AP; ++i)
                            {
                                _APToRange[i] = currentUnit.range;
                                foreach (Ability ab in currentUnit.moves)
                                {
                                    if (ab.attackRange > _APToRange[i])
                                    {
                                        _APToRange[i] = ab.attackRange;
                                    }
                                }
                            }

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

                    #region chooseTarget
                    case AIState.chooseTarget:
                    {

                        chooseMoveIntoRange(currentUnit, map, testUnits);

                        if (chooseAttackTarget(currentUnit, map, testUnits) && currentUnit.AP > 0)
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

                    #region chooseAttack
                    case AIState.chooseAttack:
                    {
                        if (chooseAttackTarget(currentUnit, map, testUnits))
                        {
                            cursor.location = _targetUnit.position;
                            waitTil = gameTime.TotalGameTime.TotalSeconds + 1;
                            _myState = AIState.attackTarget;
                        }
                        else
                        {
                            _myState = AIState.endTurn;
                        }
                    }
                    break;
                    #endregion

                    #region chooseMoveAway
                    case AIState.chooseMoveAway:
                    {
                        chooseMoveAway(currentUnit, map);

                        Stack<Point> fullPath = map.getPath(currentUnit, _targetPoint, new List<Point>());
                        int cnt = (currentUnit.maxMP / 2) + 1;
                        Point shortTarg = currentUnit.position;
                        while (--cnt >= 0 && fullPath.Count > 0)
                        {
                            shortTarg = fullPath.Pop();
                        }
                        _targetPoint = shortTarg;

                        _myState = AIState.chooseMoveTowards;

                    }
                    break;
                    #endregion

                    #region chooseMoveTowards
                    case AIState.chooseMoveTowards:
                    {
                        Stack<Point> fullPath = map.getPath(currentUnit, _targetPoint, new List<Point>());
                        Stack<Point> shortPath = new Stack<Point>();
                        int cnt = currentUnit.MP + 1;
                        _targetPoint = currentUnit.position;
                        while (--cnt >= 0 && fullPath.Count > 0)
                        {
                            shortPath.Push(fullPath.Pop());
                        }

                        if (shortPath.Count > 0)
                        {
                            _targetPoint = shortPath.Pop();
                        }

                        while( !map.isEmpty(_targetPoint)  && shortPath.Count > 0 )
                        {
                            _targetPoint = shortPath.Pop();
                        }

                        if (_targetPoint != currentUnit.position)
                        {
                            cursor.location = _targetPoint;
                            waitTil = gameTime.TotalGameTime.TotalSeconds + 1;
                            _myState = AIState.moveToTarget;
                        }
                        else
                        {
                            _myState = AIState.endTurn;
                        }

                    }
                    break;
                    #endregion

                    #region attackTarget
                    case AIState.attackTarget:
                    {
                        if (chooseAbilityToUse(currentUnit, map))
                        {
                            if (_abilityToUse == null)
                            {
                                currentUnit.attack(_targetUnit);
                            }
                            else
                            {
                                currentUnit.useAbility(_abilityToUse, _targetUnit);
                            }

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
                            else
                            {
                                _myState = AIState.chooseTarget;
                            }
                        }
                        else
                        {
                            _myState = AIState.endTurn;
                        }
        
                    }
                    break;
                    #endregion

                    #region moveToTarget
                    case AIState.moveToTarget:
                    {
                        currentUnit.goToTile(_targetPoint, map, camera);

                        if (currentUnit.AP > 0)
                        {
                            _myState = AIState.chooseAttack;
                        }
                        else
                        {
                            _myState = AIState.endTurn;
                        }


                    }
                    break;
                    #endregion

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
            camera.setFocus(currentUnit);
            camera.update(viewWidth, viewHeight, map);
            camera.setFocus(cursor);
            //cursor.update(gameTime, viewWidth, viewHeight, map);

            foreach (BaseUnit bu in testUnits)
            {
                bu.update(gameTime, map, camera);
            }
            #endregion

        }
        #endregion


    }
}
