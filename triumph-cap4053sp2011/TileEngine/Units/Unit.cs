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
using System.Collections;
using System.IO;
using TileEngine;



namespace TileEngine
{
    public enum unitTypes { archer, warrior, mage };
    public enum affinityTypes { Fire, Ice, Lightning, Water, Earth, Wind, Holy, Dark, none };
	public class BaseUnit
    {
        #region baseUnit fields

		//Identification
		public static int index_counter;
        int index;
        string _name;
        Faction _faction;

		//Attributes
        private unitTypes _type;
		private int _maxHP, _maxAP, _maxMP, _HP, _AP, _MP, _SPD, _delay, _range;
        private int _wAtk, _wDef, _mPow, _mRes, _evade; //stats used in FFT but not implemented yet here
        private int _stunLength;
        private bool _isDead, _isStunned, _isDone, _hasAttacked, _hasMoved;
        private List<Ability> _moves;
        private List<Buff> itemsAndBuffs;
        private int unitAffinity;
        private static affinityTypes[] affinities = { affinityTypes.Fire, affinityTypes.Ice, affinityTypes.Lightning, affinityTypes.Water, affinityTypes.Earth, affinityTypes.Wind, affinityTypes.Holy, affinityTypes.Dark };
        private static int[,] affinityMults = {  { 100, 115, 100, 85, 100, 100, 100, 100},
                                                 { 85, 100, 100, 100, 100, 115, 100, 100},
                                                 { 100, 100, 100, 115, 85, 100, 100, 100},
                                                 { 115, 100, 85, 100, 100, 100, 100, 100},
                                                 { 100, 100, 115, 100, 100, 85, 100, 100},
                                                 { 100, 85, 100, 100, 115, 100, 100, 100},
                                                 { 100, 100, 100, 100, 100, 100, 115, 115},
                                                 { 100, 100, 100, 100, 100, 100, 115, 115}};

        //info for updating unit after sprite is done attacking
        private int _dmgToBeTaken;
        private bool _wascrit;
        private BaseUnit _attacker;
        private String _msg;
        private Ability _prevAbility;
        private bool _isBeingHit;

		//Sprite and Movement
		private AnimatedSprite _unitSprite;
		private Point _position;
		private bool _isWalking = false;
        private bool _isAttacking = false;
        private int _attackCD;
		private Stack<Point> path;
		
        #endregion

        #region attribute properties

		/// <summary>
		/// Gets the maxHP of the unit.
		/// </summary>
		public int maxHP
		{
			get { return _maxHP; }
		}

		/// <summary>
		/// Gets the maxAP of the unit.
		/// </summary>
		public int maxAP
		{
			get { return _maxAP; }
		}

		/// <summary>
		/// Gets the maxMP of the unit.
		/// </summary>
		public int maxMP
		{
			get { return _maxMP; }
		}
		
		/// <summary>
		/// Get or set the HP of the unit inclusively between 0 and maxHP. If HP reaches zero, unit isDead = true; otherwise, unit isDead = false.
		/// </summary>
		public int HP
		{
			get { return _HP; }
			set
			{
                if (!_isDead)
                {
                    _HP = (int)MathHelper.Clamp(value, 0, _maxHP);
                    _isDead = (_HP == 0) ? true : false;
                    if (_isDead)
                    {
                        faction.numDead++;
                    }
                }
                else
                {
                    _HP = (int)MathHelper.Clamp(value, 0, _maxHP);
                    _isDead = (_HP == 0) ? true : false;
                    if (!_isDead)
                    {
                        faction.numDead--;
                    }
                }
			}
		}

		/// <summary>
		/// Get or set the AP of the unit inclusively between 0 and maxAP.
		/// </summary>
		public int AP
		{
			get { return _AP; }
			set { _AP = (int)MathHelper.Clamp(value, 0, _maxAP); }
		}

		/// <summary>
		/// Get or set the MP of the unit inclusively between 0 and maxMP.
		/// </summary>
		public int MP
		{
			get { return _MP; }
			set { _MP = (int)MathHelper.Clamp(value, 0, _maxMP); }
		}

		/// <summary>
		/// Get the base speed of the unit.
		/// </summary>
		public int SPD
		{
			get { return _SPD; }
		}
		
        /// <summary>
        /// Get the current delay of the unit
        /// </summary>
        public int delay
        {
            get { return _delay; }
            set { _delay = value; }
        }

		/// <summary>
		/// Get whether or not the unit isDead
		/// </summary>
		public bool isDead
		{
			get { return _isDead; }
		}

		/// <summary>
		/// Get or set whether or not the unit isStunned
		/// </summary>
		public bool isStunned
		{
			get { return _isStunned; }
			set { _isStunned = value; }
		}

        /// <summary>
        /// gets the remaining length of the stun
        /// </summary>
        public int stunLength
        {
            get { return _stunLength; }
            set 
            { 
                _stunLength = value;
                if (_stunLength <= 0)
                {
                    _isStunned = false;
                }
            }
        }

        ///<summary>
        /// Get or set whether a unit is done with its turn
        /// </summary>
        public bool isDone
        {
            get { return _isDone; }
            set { _isDone = value; }
        }

		/// <summary>
		/// Get whether or not a unit is walking
		/// </summary>
		public bool isWalking
		{
			get { return _isWalking; }
		}

        /// <summary>
        /// Get whether or not a unit is attacking
        /// </summary>
        public bool isAttacking
        {
            get { return _isAttacking; }
        }

        /// <summary>
        /// get whether or not an unit is in its being hit animation
        /// </summary>
        public bool isBeingHit
        {
            get { return _isBeingHit; }
        }
       
		/// <summary>
		/// Get the index of the unit's elemental affinity
		/// </summary>
		public int affinityIndex
		{
			get { return unitAffinity; }
			set
			{
				unitAffinity = (int)MathHelper.Clamp(value, -1, affinities.Length - 1);
			}
		}

		/// <summary>
		/// Get the name of the unit's elemental affinity
		/// </summary>
		public String affinityName
		{
            get { return affinities[unitAffinity].ToString() ; }
		}

        public affinityTypes affinityType
        {
            get { return affinities[unitAffinity]; }
        }

		/// <summary>
		/// Get the position (by tiles) of the unit
		/// </summary>
		public Point position
		{
			get { return _position; }
		}

		/// <summary>
		/// Get or set the Animated Sprite representing the unit
		/// </summary>
		public AnimatedSprite unitSprite
		{
			get { return _unitSprite; }
			set { _unitSprite = value; }
		}

        /// <summary>
        /// gets the unit index
        /// </summary>
		public int unitIndex
		{
			get 
			{
				return (isDead) ? -1 * index : index; 
			}
			set { index = value; }
		}


        /// <summary>
        /// gets the name of the object
        /// </summary>
		public string name
		{
			get { return _name; }
			set { _name = value; }
		}

        /// <summary>
        /// gets the faction of the unit
        /// </summary>
		public Faction faction
		{
			get { return _faction; }
			set { _faction = value; }
		}

        /// <summary>
        /// gets the range of the unit
        /// </summary>
        public int range
        {
            get { return _range; }
        }

        /// <summary>
        /// gets the list of moves for the unit
        /// </summary>
        public List<Ability> moves
        {
            get { return _moves; }
        }

        /// <summary>
        /// gets the type of unit this is
        /// </summary>
        public unitTypes type
        {
            get { return _type; }
        }


		#endregion

        #region constructors

		/// <summary>
		/// Create baseUnit object with abilities
		/// </summary>
		/// <param name="name">Name of unit</param>
		/// <param name="maxHP">Maximum Hit Points of unit</param>
		/// <param name="maxAP">Maximum Action Points of unit</param>
		/// <param name="maxMP">Maximum Movement Points of unit</param>
		/// <param name="SPD">Speed of unit</param>
		/// <param name="unitAffinity">Index of elemental affinity of unit</param>
		/// <param name="abilities">Abilities of unit</param>
        public BaseUnit(String name, int maxHP, int maxAP, int maxMP, int SPD, int unitAffinity, params Ability[] abilities)
        {
			_name = name;
			_HP = _maxHP = maxHP;
			_AP = _maxAP = maxAP;
			_MP = _maxMP = maxMP;
			_SPD = _delay = SPD;
            _isDone = false;
            _evade = _SPD/20;
            _wDef = 0;
            _wAtk = 0;
			if (_HP > 0)
				_isDead = false;
			else
				_isDead = true;

			this.unitAffinity = unitAffinity;

			_moves = new List<Ability>();
            for (int i = 0; i < abilities.Length; ++i)
                _moves.Add(abilities[i]);

			index = ++index_counter;


            //asign type
            if (this._wAtk < this._mPow)
                this._type = unitTypes.mage;
            else if (this._range > 1)
                this._type = unitTypes.archer;
            else
                this._type = unitTypes.warrior;
        }

		/// <summary>
		/// Create baseUnit object without abilities
		/// </summary>
		/// <param name="name">Name of unit</param>
		/// <param name="maxHP">Maximum Hit Points of unit</param>
		/// <param name="maxAP">Maximum Action Points of unit</param>
		/// <param name="maxMP">Maximum Movement Points of unit</param>
		/// <param name="SPD">Speed of unit</param>
		/// <param name="unitAffinity">Index of elemental affinity of unit</param>
        public BaseUnit(String name, int maxHP, int maxAP, int maxMP, int SPD, int unitAffinity)
        {
			this._name = name;
			_HP = _maxHP = maxHP;
			_AP = _maxAP = maxAP;
			_MP = _maxMP = maxMP;
            _SPD = _delay = SPD;
            _isDone = false;
            _evade = _SPD / 20;
            _wDef = 0;
            _wAtk = 0;
			if (_HP > 0)
				_isDead = false;
			else
				_isDead = true;

			this.unitAffinity = unitAffinity;

			_moves = new List<Ability>();

			index = ++index_counter;
            //asign type
            if (this._wAtk < this._mPow)
                this._type = unitTypes.mage;
            else if (this._range > 1)
                this._type = unitTypes.archer;
            else
                this._type = unitTypes.warrior;
        }

        /// <summary>
        /// creates an abilityless unit that has weapon attack, weapond def, magic power, magic resistance, attack range
        /// </summary>
        /// <param name="name"></param>
        /// <param name="maxHP"></param>
        /// <param name="maxAP"></param>
        /// <param name="maxMP"></param>
        /// <param name="SPD"></param>
        /// <param name="unitAffinity"></param>
        /// <param name="wAtk"></param>
        /// <param name="wDef"></param>
        /// <param name="mPow"></param>
        /// <param name="mRes"></param>
        public BaseUnit(String name, int maxHP, int maxAP, int maxMP, int SPD, int unitAffinity, int wAtk, int wDef, int mPow, int mRes, int range)
        {
            this._name = name;
            _HP = _maxHP = maxHP;
            _AP = _maxAP = maxAP;
            _MP = _maxMP = maxMP;
            _SPD = _delay = SPD;
            _isDone = false;
            _evade = _SPD / 20;
            _wAtk = wAtk;
            _wDef = wDef;
            _mPow = mPow;
            _mRes = mRes;
            _range = range;
            if (_HP > 0)
                _isDead = false;
            else
                _isDead = true;

            this.unitAffinity = unitAffinity;

            _moves = new List<Ability>();
            index = ++index_counter;

            //asign type
            if (this._wAtk < this._mPow)
                this._type = unitTypes.mage;
            else if (this._range > 1)
                this._type = unitTypes.archer;
            else
                this._type = unitTypes.warrior;
        }

		/// <summary>
		/// Create an empty baseUnit
		/// </summary>
        public BaseUnit()
        {
			this._name = "";
			_HP = _maxHP = 1;
			_AP = _maxAP = 1;
			_MP = _maxMP = 1;
			_SPD = _delay = 1;
			_isDead = false;
            _isDone = false;

			this.unitAffinity = -1;
		
			_moves = new List<Ability>();

			index = ++index_counter;

            //asign type
            if (this._wAtk < this._mPow)
                this._type = unitTypes.mage;
            else if (this._range > 1)
                this._type = unitTypes.archer;
            else
                this._type = unitTypes.warrior;
        }

		/// <summary>
		/// Add one or more abilities to the unit
		/// </summary>
		/// <param name="abilities"></param>
		public void addAbility(params Ability[] abilities)
		{
			for (int i = 0; i < abilities.Length; ++i)
				_moves.Add(abilities[i]);
		}

		public static Dictionary<string, BaseUnit> fromFile(ContentManager content, string filename, Dictionary<string, Ability>AbilityList)
		{
			Dictionary<string, BaseUnit> unitList = new Dictionary<string, BaseUnit>();

			using (StreamReader reader = new StreamReader(filename))
			{
				while (!reader.EndOfStream)
				{
					FrameAnimation up = new FrameAnimation(2, 32, 32, 0, 0);
					FrameAnimation down = new FrameAnimation(2, 32, 32, 64, 0);
					FrameAnimation left = new FrameAnimation(2, 32, 32, 128, 0);
					FrameAnimation right = new FrameAnimation(2, 32, 32, 192, 0);
					FrameAnimation dead = new FrameAnimation(1, 32, 32, 256, 0);
					up.framesPerSecond = down.framesPerSecond = left.framesPerSecond = right.framesPerSecond = 5;

					string line = reader.ReadLine().Trim();
					if (string.IsNullOrEmpty(line)) continue;
					if (line.Contains("///")) continue;
					string[] unitParams = line.Split(',');
                    BaseUnit unit = new BaseUnit(unitParams[0].Trim(), int.Parse(unitParams[1].Trim()), int.Parse(unitParams[2].Trim()), int.Parse(unitParams[3].Trim()), int.Parse(unitParams[4].Trim()), int.Parse(unitParams[5].Trim()), int.Parse(unitParams[6].Trim()) * 9 / 10, int.Parse(unitParams[7].Trim()) * 3 / 2, int.Parse(unitParams[8].Trim()), int.Parse(unitParams[9].Trim()), int.Parse(unitParams[10].Trim()));

					for (int i = 12; i < unitParams.Length; ++i)
					{
						unit.addAbility(AbilityList[unitParams[i].Trim()]);
					}


					unit.unitSprite = new AnimatedSprite(content.Load<Texture2D>(unitParams[11].Trim()));
					unit.unitSprite.animations.Add("Up", up);
					unit.unitSprite.animations.Add("Down", down);
					unit.unitSprite.animations.Add("Left", left);
					unit.unitSprite.animations.Add("Right", right);
					unit.unitSprite.animations.Add("Dead", dead);
					unit.unitSprite.speed = 1.75f;
					unit.unitSprite.currentAnimationName = "Down";
					unitList.Add(unit._name, unit);
				}
			}

			return unitList;
		}

		#endregion

        #region methods

        #region attacking
        /// <summary>
        /// tells the unit to attack another unit
        /// </summary>
        /// <param name="target"></param>
        public int attack(BaseUnit target)
        {
            if (_AP <= 0 || _isAttacking || _isWalking) return -1;
            _AP -= 1;
            _hasAttacked = true;
            _isAttacking = true;
            _attackCD = 50;
            int dmg = 0;
            dmg = _wAtk;

			_unitSprite.attack(this, target); 
            return target.takeDamage(dmg, affinityMults[this.affinityIndex, target.affinityIndex]*getWeatherMult()/100, this);

        }
        
        /// <summary>
        /// takes weapon damage
        /// </summary>
        /// <param name="amt"></param>
        /// <param name="mult"></param>
        /// <param name="attacker"></param>
        /// <returns></returns>
        public int takeDamage(int amt, int mult, BaseUnit attacker)
        {
            int hit = RandomNumber.getInstance().getNext(1, 100);
            if (hit > _evade) //hit
            {
                //apply random variance
                int dmg = Math.Max(amt - _wDef / 2, 1);
                int x = dmg / 10;
                dmg += RandomNumber.getInstance().getNext(0, 2 * x) - x;

                //check for critical hit
                if (RandomNumber.getInstance().getNext(1, 100) < SPD / 20)
                {
                    dmg += dmg / 2;
                    _wascrit = true;
                }

                //apply multiplier
                dmg = Math.Max(1,dmg * mult / 100);

                //deal damage
                _dmgToBeTaken = dmg;
                _attacker = attacker;

                return dmg;
            }
            else // dodged attack
            {
                _dmgToBeTaken = 0;
                _attacker = attacker;

                return 0;
            }
        }

        /// <summary>
        /// takes magic damage
        /// </summary>
        /// <param name="amt"></param>
        /// <param name="mult"></param>
        /// <param name="attacker"></param>
        public int takeMagicDamage(int amt, int mult, BaseUnit attacker)
        {
            //apply random variance
            int dmg = Math.Max(amt - _mRes / 2, 1);
            int x = dmg / 10;
            dmg += RandomNumber.getInstance().getNext(0, 2 * x) - x;

            //check for critical hit
            if (RandomNumber.getInstance().getNext(1, 100) < SPD / 20)
            {
                dmg += dmg / 2;
                _wascrit = true;
            }

            //apply multiplier
            dmg = Math.Max(1, dmg * mult / 100);

            //deal damage
            _dmgToBeTaken = dmg;
            _attacker = attacker;

            return dmg;
        }

        private int applyVariance(int amt, int percent)
        {
            int x = 100 / percent;
            return amt + RandomNumber.getInstance().getNext(0, 2 * x) - x;
        }

        /// <summary>
        /// Gets the weather multiplier
        /// </summary>
        /// <returns></returns>
        public int getWeatherMult()
        {
            WeatherTypes currentWeather = Weather.getInstance().currentWeather;
            #region weatherbuffs
            switch (currentWeather)
            {
                case WeatherTypes.cloudy:
                    return 100;
                    break;
                case WeatherTypes.dark:
                    if (this.affinityType == affinityTypes.Dark) return 110;
                    break;
                case WeatherTypes.rainy:
                    if (this.affinityType == affinityTypes.Lightning || this.affinityType == affinityTypes.Water) return 110;
                    break;
                case WeatherTypes.snowy:
                    if (this.affinityType == affinityTypes.Ice) return 110;
                    break;
                case WeatherTypes.sunny:
                    if (this.affinityType == affinityTypes.Holy || this.affinityType == affinityTypes.Earth) return 110;
                    break;
                case WeatherTypes.windy:
                    if (this.affinityType == affinityTypes.Wind || this.affinityType == affinityTypes.Fire) return 110;
                    break;
            }
            #endregion
            return 100;
        }


        /// <summary>
        /// checks if a target unit is within range
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public bool withinRange(BaseUnit target)
        {
            return (Math.Abs(this.position.X - target.position.X) + Math.Abs(this.position.Y - target.position.Y)) <= _range;
        }

        /// <summary>
        /// checks if a unit is within range of an ability
        /// </summary>
        /// <param name="ability"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        public bool canTargetAbility(Ability ability, BaseUnit target)
        {
            if(!((Math.Abs(this.position.X - target.position.X) + Math.Abs(this.position.Y - target.position.Y)) <= ability.attackRange))
                return false;
            if (ability.isFriendly)
            {
                if (target.faction != this.faction) return false;
            }
            if (ability.isHostile)
            {
                if (target.faction == this.faction) return false;
            }
            
            return true;
        }


        /// <summary>
        /// modift unit with nonattacking ability
        /// </summary>
        /// <param name="modifier"></param>
        /// <param name="ability"></param>
        public void modify(BaseUnit modifier, Ability ability)
        {
            //store attackers info and ability
            _attacker = modifier;
            _prevAbility = ability;
        }

        public void beStunned()
        {
            this._isBeingHit = true;
            this.unitSprite.beHit(this, this);
        }
        #endregion

        #region updates
        /// <summary>
        /// the unit ends its turn and resets values
        /// </summary>
        public void endTurn()
        {
            this.MP = this.maxMP;
            this.AP = this.maxAP;
            this.delay = 500;
            if (this._hasAttacked) delay -= 200;
            if (this._hasMoved) delay -= 300;
            _hasAttacked = _hasMoved = false;
            this.isDone = false;
        }

        /// <summary>
        /// this tells the unit to tick and get closer to its next turn
        /// </summary>
        public void tick()
        {
            this.delay += this.SPD;
        }
        #endregion

        #endregion

        #region comparers

        /// <summary>
        /// Allows units to be sorted by delay
        /// </summary>
        public class sortByDelay:IComparer
        {
            int IComparer.Compare(Object x, Object y)
            {
                BaseUnit a = (BaseUnit)x;
                BaseUnit b = (BaseUnit)y;
                if (a.delay > b.delay)
                {
                    return 1;
                }
                else if (a.delay == b.delay)
                {
                    return 0;
                }
                return -1;
            }
        }

        #endregion

        #region ability
        /// <summary>
        /// uses an ability against a target unit
        /// </summary>
        /// <param name="ability"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        public int useAbility(Ability ability, BaseUnit target)
        {
            if (Math.Abs(this.position.X - target.position.X) + Math.Abs(this.position.Y - target.position.Y) > ability.attackRange)
                return -1;
            if (target.faction != this.faction)
            {
                if (!ability.isHostile) return -1;
            }
            else if (target == this)
            {
                if (!ability.isSelf) return -1;
            }
            else if (target.faction == this.faction)
            {
                if (!ability.isFriendly) return -1;
            }
            if (!_moves.Contains(ability) || _AP - ability.APCost < 0 || _isAttacking) 
                return -1;

            _AP -= ability.APCost;
            _hasAttacked = true;
            _isAttacking = true;
            _attackCD = 50;
            _unitSprite.attack(this, target);
            if (ability.abilityType == EffectTypes.damage)
            {
                int dmg = _wAtk;
                //System.Console.WriteLine("Damage from ability " + ability.name + ": " + dmg);
                return target.takeDamage(dmg, ability.abilityAmount * affinityMults[this.affinityIndex, target.affinityIndex] * getWeatherMult() / 10000, this);
            }
            else if (ability.abilityType == EffectTypes.magicDamage)
            {
                int dmg = _mPow;
                //System.Console.WriteLine("Damage from ability " + ability.name + ": " + dmg);
                target.takeMagicDamage(dmg, ability.abilityAmount * affinityMults[this.affinityIndex, target.affinityIndex] * getWeatherMult() / 10000, this);
            }
            else
            {
                target.modify(this, ability);
            }
            return 0;
        }

        /// <summary>
        /// checks if an unit can use any ability
        /// </summary>
        /// <returns></returns>
        public bool canAbility()
        {
            bool ret = false;
            for (int i = 0; i < _moves.Count; i++)
            {
                if (this.AP >= _moves[i].APCost) ret = true;
            }
            return ret;
        }

		#endregion


		#region Sprite Updates

		/// <summary>
		/// Updates the units sprite representation based on flags.
		/// </summary>
		/// <param name="gameTime">GameTime object passed from Game class</param>
		/// <param name="map">Tile Map of play area</param>
		public void update(GameTime gameTime, TileMap map)
		{
			//check flags
			if (_unitSprite.isMoving)
				_isWalking = true;
			else
				_isWalking = false;

			if (isDead && unitSprite.currentAnimationName != "Dead")
				unitSprite.die();

			if (!_isAttacking)
				map.unitLayer.moveUnit(this.unitIndex, this.position);

			_unitSprite.update(gameTime, map);

			if (_isAttacking && !_unitSprite.isAttacking)
				_isAttacking = false;

			if (this._isBeingHit && !_unitSprite.isDefending)
			{
				this.HP -= _dmgToBeTaken;

				if (this.isDead)
					GameConsole.getInstanceOf().Update(this.name + " was killed by " + _attacker.name, _attacker.faction.color);

				_dmgToBeTaken = 0;
				_attacker = null;
				this._isBeingHit = false;
			}

            if (_attacker != null && !_attacker.isAttacking && !_isBeingHit)
            {
                String msg = "";
                if (_prevAbility != null)
                {
                    System.Console.WriteLine("sup");
                    if (_prevAbility.abilityType == EffectTypes.decAP)
                    {
                        this.AP -= _prevAbility.abilityAmount;
                        msg = _attacker.name + " has used " + _prevAbility.name + "to reduce " + this.name + "'s AP by " + _prevAbility.abilityAmount;
                    }
                    else if (_prevAbility.abilityType == EffectTypes.decMP)
                    {
                        this.MP -= _prevAbility.abilityAmount;
                        msg = _attacker.name + " has used " + _prevAbility.name + " to reduce " + this.name + "'s MP by " + _prevAbility.abilityAmount;
                    }
                    else if (_prevAbility.abilityType == EffectTypes.heal)
                    {
                        int amt = applyVariance(_prevAbility.abilityAmount,10);
                        this.HP += amt;
                        msg = _attacker.name + " has used " + _prevAbility.name + " to heal " + this.name + " by " + amt;
                    }
                    else if (_prevAbility.abilityType == EffectTypes.incAP)
                    {
                        this.AP += _prevAbility.abilityAmount;
                        msg = _attacker.name + " has used " + _prevAbility.name + " to increase " + this.name + "'s AP by " + _prevAbility.abilityAmount;
                    }
                    else if (_prevAbility.abilityType == EffectTypes.incMP)
                    {
                        this.MP += _prevAbility.abilityAmount;
                        msg = _attacker.name + " has used " + _prevAbility.name + " to increase " + this.name + "'s MP by " + _prevAbility.abilityAmount;
                    }
                    else if (_prevAbility.abilityType == EffectTypes.stun)
                    {
                        this._stunLength = _prevAbility.abilityAmount;
                        this.isStunned = true;
                        if(_prevAbility.abilityAmount != 1)
                            msg = _attacker.name + " has used " + _prevAbility.name + " to stun " + this.name + " for " + _prevAbility.abilityAmount + " turns";
                        else
                        {
                            msg = _attacker.name + " has used " + _prevAbility.name + " to stun " + this.name + " for " + _prevAbility.abilityAmount + " turn";
                        }
                    }
                    _unitSprite.beHit(this, _attacker);
                }
                else
                {

                    //Generate msg and perform animation
                    if (_wascrit)
                    {
                        msg = "Critical hit! " + _attacker.name + " has done " + _dmgToBeTaken + " damage to " + this.name;
                        _unitSprite.beCritHit(this, _attacker);
                    }
                    else if (_dmgToBeTaken == 0)
                    {
                        msg = _attacker.name + " has missed " + this.name;
                        _unitSprite.dodge(this, _attacker);
                    }
                    else
                    {
                        msg = _attacker.name + " has done " + _dmgToBeTaken + " damage to " + this.name;
                        _unitSprite.beHit(this, _attacker);
                    }
                }
				//Print msgs
                GameConsole.getInstanceOf().Update(msg, _attacker.faction.color);

                _wascrit = false;
                _prevAbility = null;
                this._isBeingHit = true;
            }
            
		}

		/// <summary>
		/// Move the unit to a specified tile.
		/// </summary>
		/// <param name="goal">Goal location of the </param>
		/// <param name="map">Tile Map of play area</param>
		public bool goToTile(Point goal, TileMap map)
		{
			if (_isWalking) return false;
            if (map.unitLayer.getTileUnitIndex(goal) != 0) return false;
			if (map.collisionLayer.getTileCollisionIndex(goal) != 0) return false;

			if (unitSprite.goToTile(this, goal, map, _MP))
			{
				MP -= map.getPath(this, goal, new List<Point>()).Count - 1;
				_position = goal;
				_isWalking = true;
				_hasMoved = true;
				map.unitLayer.moveUnit(unitIndex, goal);
				return true;
			}
			else
				return false;

		}

		/// <summary>
		/// Teleports the unit to any non-collision point on the map
		/// </summary>
		/// <param name="goal">Goal tile location to teleport to.</param>
		/// <param name="map">Tile Map of play area</param>
		public void teleportToTile(Point goal, TileMap map)
		{
			if (_isWalking) return;
			if (map.unitLayer.getTileUnitIndex(goal) != 0) return;
			if (map.collisionLayer.getTileCollisionIndex(goal) != 0) return;

			unitSprite.position = new Vector2((float)goal.X * Engine.TILE_WIDTH, (float)goal.Y * Engine.TILE_HEIGHT);
			_position = goal;
			map.unitLayer.moveUnit(unitIndex, goal);
		} 

		/// <summary>
		/// Randomly position a unit on a viable tile within a given range
		/// </summary>
		/// <param name="topLeft">Top left corner of range (inclusive)</param>
		/// <param name="topRight">Bottom right corner of range (inclusive)</param>
		public void randomPosition(Point topLeft, Point bottomRight, TileMap map)
		{
			Point p = Point.Zero;
			do
			{
				p.X = RandomNumber.getInstance().getNext(topLeft.X, bottomRight.X);
				p.Y = RandomNumber.getInstance().getNext(topLeft.Y, bottomRight.Y);
			} while (!map.isEmpty(p));

			teleportToTile(p, map);
		}

		#endregion

		#region drawing player

		/// <summary>
		/// Draws the unit on the map
		/// </summary>
		/// <param name="spriteBatch">SpriteBatch passed from Game class</param>
		/// <param name="camera">Camera object passed from Game class</param>
		public void draw(SpriteBatch spriteBatch, Camera camera)
		{
			spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, SamplerState.LinearClamp, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, camera.transformationMatrix);
			unitSprite.Draw(spriteBatch);
			spriteBatch.End();
		}

        #endregion

	}

}
