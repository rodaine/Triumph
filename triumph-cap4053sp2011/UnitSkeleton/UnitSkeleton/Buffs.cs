using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnitSkeleton
{
     class TriggerObject
    {
        #region triggerobject fields

        String name;
        String description;
        int turnDuration;
        int objectAmount;
        int objectType;         
        /*
         * heal = 1
         * damage = 2
         * stun = 3
         * incMP = 4
         * decMP = 5
         * incAP = 6
         * decAP = 7
         * 
         * */
        Boolean objectFlip;

        #endregion

        #region constructors

        public TriggerObject(String name, int objectType, int objectAmount, int turnDuration, Boolean objectFlip, String description)
        {
            this.name = name;
            this.objectType = objectType;
            this.objectAmount = objectAmount;
            this.turnDuration = turnDuration;
            this.objectFlip = objectFlip;
            this.description = description;
        }

        public TriggerObject(String name, int objectType, int objectAmount, int turnDuration, Boolean objectFlip)
        {
            this.name = name;
            this.objectType = objectType;
            this.objectAmount = objectAmount;
            this.turnDuration = turnDuration;
            this.objectFlip = objectFlip;
            description = "No description provided.";
        }

        public TriggerObject()
        {
            name = "";
            objectType = -1;
            objectAmount = 0;
            turnDuration = 0;
            objectFlip = false;
            description = "";
        }

        #endregion

        #region get methods
        
        public int getObjectType() { return objectType; }
        public int getObjectAmount() { return objectAmount; }
        public int getTurnDuration() { return turnDuration; }
        public String getName() { return name; }
        public String getDescription() { return description; }
        public Boolean getObjectFlip() { return objectFlip; }

        #endregion

        #region methods

        public void decTurn()
        {
            turnDuration--;
        }

        #endregion

    }
}
