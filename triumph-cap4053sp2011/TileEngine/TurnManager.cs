using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using Microsoft.Xna.Framework;

namespace TileEngine
{
    public class TurnManager
    {
        #region TurnManager fields

        private PriorityQueue pq = new PriorityQueue(new BaseUnit.sortByDelay());

        #endregion

        #region constructors 
        /// <summary>
        /// Constructor to create a turnmanager to manage a set of units
        /// </summary>
        /// <param name="units"></param>
        public TurnManager(BaseUnit[] units)
        {
            for(int i = 0; i < units.Length;i++)
            {
                 pq.Enqueue(units[i]);
            }
        }

        /// <summary>
        /// Empty constructor for turnmanager
        /// </summary>
        public TurnManager()
        {

        }

        #endregion


        #region Methods
        /// <summary>
        /// Gets the next unit in line
        /// </summary>
        /// <returns></returns>
        public BaseUnit getNext()
        {
            BaseUnit next = (BaseUnit)pq.Dequeue();
            while (next.isDead)
            {
                next = (BaseUnit)pq.Dequeue();
            }
            return next;
        }

        /// <summary>
        /// Puts a unit back into the line
        /// </summary>
        /// <param name="unit"></param>
        /// <returns></returns>
        public void add(BaseUnit unit)
        {
            if(!unit.isDead)
            {
                pq.Enqueue(unit);
            }
        }

        #endregion

    }
}
