using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

/*  Need to determine how we will handle objects blocking target    *
 *  and how we would like to determine distances                    *
 *  distanceTo(target) & isBlocked(target)                          */


namespace TileEngine
{
    class BaseUnit
    {
        #region baseUnit fields

        int index;
        String name;
        Faction faction;

        //Need location information

        int maxHP;
        int maxAP;
        int maxMP;
        int HP;
        int AP;
        int MP;
        Boolean isDead; //if true, then stop drawing object
        Boolean isStunned;
        List<Ability> moves;
        List<TriggerObject> itemsAndBuffs;

        int unitAffinity;
        static String[] affinities = { "Water", "Fire", "Earth", "Stone", "Wind", "Ice" };

        #endregion

        #region constructors

        public BaseUnit(String name, int maxHP, int maxAP, int maxMP, int unitAffinity, params Ability[] abilities)
        {
            this.name = name;
            this.maxHP = maxHP;
            HP = maxHP;
            this.maxAP = maxAP;
            AP = maxAP;
            this.maxMP = maxMP;
            MP = maxMP;

            isDead = false;
            this.unitAffinity = unitAffinity;

            //add list of abilities to player's move list
            for (int i = 0; i < abilities.Length; i++)
            {
                moves.Add(abilities[i]);
            }
        }

        public BaseUnit(String name, int maxHP, int maxAP, int maxMP, int unitAffinity)
        {
            this.name = name;
            this.maxHP = maxHP;
            HP = maxHP;
            this.maxAP = maxAP;
            AP = maxAP;
            this.maxMP = maxMP;
            MP = maxMP;

            isDead = false;
            this.unitAffinity = unitAffinity;

            moves = null;
        }

        public BaseUnit()
        {
            name = "";
            maxHP = 0;
            maxAP = 0;
            HP = 0;
            AP = 0;
            maxMP = 0;
            MP = 0;
            isDead = false;
            unitAffinity = -1;
            moves = null;
        }

        #endregion

        #region attribute mutators

        private void heal(int amount)
        {
            if (HP + amount < maxHP)
            {
                HP += amount;
            }
            else
            {
                HP = maxHP;
            }
        }

        private void damage(int amount)
        {
            if (HP - amount <= 0)
            {
                isDead = true;
            }
            else
            {
                HP -= amount;
            }
        }

        private void stun(Boolean input)
        {
            isStunned = input;
        }

        private void toggleStun()
        {
            isStunned = !isStunned;
        }

        private void incMP(int amount)
        {
            if (MP + amount < maxMP)
            {
                MP += amount;
            }
            else
            {
                MP = maxMP;
            }
        }

        private void decMP(int amount)
        {
            if (MP - amount <= 0)
            {
                MP = 0;
            }
            else
            {
                MP -= amount;
            }
        }

        private void incAP(int amount)
        {
            if (AP + amount < maxAP)
            {
                AP += amount;
            }
            else
            {
                AP = maxAP;
            }
        }

        private void decAP(int amount)
        {
            if (AP - amount <= 0)
            {
                AP = 0;
            }
            else
            {
                AP -= amount;
            }
        }

        #endregion

        #region use_ability

        public abstract Boolean use_ability(Ability action, BaseUnit target);

        /*public Boolean use_ability(Ability action, BasePlayer target)
        {
            //check that ability may be performed (enough ap and target within range is all for now)
            if (action.getAPCost() > AP) { return false; }

            if (action.getAttackRange() < distanceTo(target)) { return false; }

            if (isBlocked(target)) { return false; }

            //if all checks pass then carry out action
            //include buffs/debuffs/affinities here as well
            if (action.getAbilityType() == 0)                   //attack
            {
                target.damage(action.getAbilityAmount());
            }
            else if (action.getAbilityType() == 1)              //heal
            {
                target.heal(action.getAbilityAmount());
            }                                                   //more to come (buffs/debuffs)
            else
            {

            }

            return true;
        }*/

        #endregion

        #region run_checks

        public void run()
        {
            //adjust unit stats
            for (int i = 0; i < itemsAndBuffs.Count; i++)
            {
                switch (itemsAndBuffs[i].getObjectType())
                {
                    case 1:
                        //heal
                        heal(itemsAndBuffs[i].getObjectAmount());
                        itemsAndBuffs[i].decTurn();
                        break;
                    case 2:
                        //damage
                        damage(itemsAndBuffs[i].getObjectAmount());
                        itemsAndBuffs[i].decTurn();
                        break;
                    case 3:
                        //stun
                        stun(itemsAndBuffs[i].getObjectFlip());
                        itemsAndBuffs[i].decTurn();
                        break;
                    case 4:
                        //increase MP
                        incMP(itemsAndBuffs[i].getObjectAmount());
                        itemsAndBuffs[i].decTurn();
                        break;
                    case 5:
                        //decrease MP
                        decMP(itemsAndBuffs[i].getObjectAmount());
                        itemsAndBuffs[i].decTurn();
                        break;
                    case 6:
                        //increase AP
                        incAP(itemsAndBuffs[i].getObjectAmount());
                        itemsAndBuffs[i].decTurn();
                        break;
                    case 7:
                        //decrease AP
                        decAP(itemsAndBuffs[i].getObjectAmount());
                        itemsAndBuffs[i].decTurn();
                        break;
                    default:
                        //not a valid choice
                        break;
                }
            }

            //adjust items&buffs list
            for (int i = 0; i < itemsAndBuffs.Count; i++)
            {
                if (itemsAndBuffs[i].getTurnDuration() == 0)
                {
                    itemsAndBuffs.RemoveAt(i);
                }
            }

        }

        #endregion

        #region drawing player

        //enter sprite code here

        #endregion
    }

}
