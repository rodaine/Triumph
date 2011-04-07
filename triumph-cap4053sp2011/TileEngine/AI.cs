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
            int minDist = Int32.MaxValue;
            foreach(BaseUnit bu in testUnits)
            {
                if (!bu.isDead && myFaction != bu.faction)
                {
                    int dist = map.getDistance(currentUnit.position, bu.position);
                    if (dist < minDist)
                    {
                        minDist = dist;
                        target = bu;
                    }
                }
            }
            if (target == null) //there are no living enemies
            {
                currentUnit.isDone = true;
            }
            else
            {
                Point targetPoint = target.position;
                int dX = currentUnit.position.X - targetPoint.X;
                int dY = currentUnit.position.Y - targetPoint.Y;
                if (Math.Abs(dX) > Math.Abs(dY))
                {
                    targetPoint.X = targetPoint.X + (dX / Math.Abs(dX));
                }
                else if (Math.Abs(dY) > 0)
                {
                    targetPoint.Y = targetPoint.Y + (dY / Math.Abs(dY));
                }

                System.Console.Error.WriteLine(currentUnit.name + " is targeting " + target.name + " and wants to move to " + targetPoint.ToString());
                
                currentUnit.goToTile(targetPoint, map);
                
                if (currentUnit.withinRange(target))
                {
                    currentUnit.attack(target, rand.getNext(1, 20));
                }
                else
                {
                    currentUnit.isDone = true;
                }
                
            }
        }

    }
}
