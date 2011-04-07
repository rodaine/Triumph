using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace TileEngine
{
    public class AI
    {


        public void update(BaseUnit currentUnit, TileMap map, BaseUnit[] testUnits, RandomNumber rand)
        {
            Faction myFaction = currentUnit.faction;
            BaseUnit target = null;
            Point targetPoint = currentUnit.position;
            int minDist = Int32.MaxValue;

            foreach(BaseUnit bu in testUnits)
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
                            if(dist < minDist)
                            {
                                target = bu;
                                minDist = dist;
                                targetPoint = p;
                            }
                        }
                    }
                }
            }
            if (target == null) //there are no living enemies
            {
                currentUnit.isDone = true;
            }
            else
            {
                System.Console.Error.WriteLine(currentUnit.name + " is targeting " + target.name);
                if (!currentUnit.withinRange(target))
                {

                    Stack<Point> path = map.getPath(currentUnit.position, targetPoint, new List<Point>());

                    while (path.Count > 0 && currentUnit.MP > 0)
                    {
                        currentUnit.goToTile(path.Pop(), map);
                    }
                }

                if (currentUnit.withinRange(target))
                {
                    currentUnit.attack(target, rand);
                }
                else
                {
                    currentUnit.isDone = true;
                }
                
            }
        }

    }
}
