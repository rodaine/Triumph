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
	public class BaseUnit : ICloneable
    {
        #region baseUnit fields

		//Identification
        int index;
        string name;
        Faction faction;

		//Attributes
		private int _maxHP, _maxAP, _maxMP, _HP, _AP, _MP, _SPD;
        private bool _isDead, _isStunned;
        private List<Ability> moves;
        private List<Buff> itemsAndBuffs;
        private int unitAffinity;
        private static string[] affinities = { "Water", "Fire", "Earth", "Stone", "Wind", "Ice" };

		//Sprite and Movement
		private AnimatedSprite _unitSprite;
		private Point _location;
		private Vector2 _spritePosition, destination;
		private bool isWalking = false;
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
				_HP = (int)MathHelper.Clamp(value, 0, _maxHP);
				_isDead = (_HP == 0) ? true : false;
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
		public string affinityName
		{
			get { return affinities[unitAffinity]; }
		}

		/// <summary>
		/// Get the position (by tiles) of the unit
		/// </summary>
		public Point location
		{
			get { return _location; }
		}

		/// <summary>
		/// Get the position (by pixels) of the unit
		/// </summary>
		public Vector2 spritePosition
		{
			get { return _spritePosition; }
		}

		public AnimatedSprite unitSprite
		{
			get { return _unitSprite; }
			set { _unitSprite = value; }
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
			this.name = name;
			_HP = _maxHP = maxHP;
			_AP = _maxAP = maxAP;
			_MP = _maxMP = maxMP;
			_SPD = SPD;

			if (_HP > 0)
				_isDead = false;
			else
				_isDead = true;

			this.unitAffinity = unitAffinity;

			moves = new List<Ability>();
            for (int i = 0; i < abilities.Length; ++i)
                moves.Add(abilities[i]);

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
			this.name = name;
			_HP = _maxHP = maxHP;
			_AP = _maxAP = maxAP;
			_MP = _maxMP = maxMP;

			if (_HP > 0)
				_isDead = false;
			else
				_isDead = true;

			this.unitAffinity = unitAffinity;

			moves = new List<Ability>();
        }

		/// <summary>
		/// Create an empty baseUnit
		/// </summary>
        public BaseUnit()
        {
			this.name = "";
			_HP = _maxHP = 1;
			_AP = _maxAP = 1;
			_MP = _maxMP = 1;
			_SPD = 1;
			_isDead = false;

			this.unitAffinity = -1;
		
			moves = new List<Ability>();
        }

		/// <summary>
		/// Add one or more abilities to the unit
		/// </summary>
		/// <param name="abilities"></param>
		public void addAbility(params Ability[] abilities)
		{
			for (int i = 0; i < abilities.Length; ++i)
				moves.Add(abilities[i]);
		}

		#endregion

		#region use_ability

		//public Boolean use_ability(Ability action, BaseUnit target);

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

		#region Turn Checks

		public void run()
        {
            //adjust unit stats
            foreach (Buff item in itemsAndBuffs)
            {
                switch (item.getObjectType())
                {
                    case 1:
                        //heal
						HP += item.getObjectAmount();
                        break;
                    case 2:
                        //damage
						HP -= item.getObjectAmount();
                        break;
                    case 3:
                        //stun
						isStunned = item.getObjectFlip();
                        break;
                    case 4:
                        //increase MP
						MP += item.getObjectAmount();
                        break;
                    case 5:
                        //decrease MP
						MP -= item.getObjectAmount();
                        break;
                    case 6:
                        //increase AP
						AP += item.getObjectAmount();
                        break;
                    case 7:
                        //decrease AP
						AP -= item.getObjectAmount();
                        break;
                    default:
                        //not a valid choice
                        break;
                }

				item.decTurn();
				if (item.getTurnDuration() == 0)
					itemsAndBuffs.Remove(item);
            }

        }

        #endregion

		#region Sprite Updates

		public void update(GameTime gameTime, int screenWidth, int screenHeight, TileMap map)
		{

			//check if at destination and update as neccessary
			if (isWalking && _spritePosition.X == destination.X && _spritePosition.Y == destination.Y)
			{
				//if at final destination, stop walking!
				// get next destination
				if (path.Count == 0)
				{
					isWalking = false;
				}
				else 
				{
					Point tileDest = path.Pop();
					destination = new Vector2((float)tileDest.X * Engine.TILE_WIDTH, (float)tileDest.Y * Engine.TILE_HEIGHT);
				}
			}

			//update sprite location
			if (isWalking)
			{
				Vector2 motion = Vector2.Zero;
				motion = destination - _spritePosition;
				motion.Normalize();
				motion *= _unitSprite.speed;
				if (motion.Length() > (destination - _spritePosition).Length())
					motion = destination - _spritePosition;

				_spritePosition += motion;
				_unitSprite.position = _spritePosition;
			}

			//update sprite
			_unitSprite.update(gameTime, screenWidth, screenHeight, map);
		}

		public void goToTile(Point goal, TileMap map)
		{
			if (isWalking) return;

			path = map.getPath(_location, goal, new List<Point>());
			if (path.Count == 0)
				return;

			Point tileDest = path.Pop();
			
			destination = new Vector2((float)tileDest.X * Engine.TILE_WIDTH, (float)tileDest.Y * Engine.TILE_HEIGHT);
			_location = goal;
			isWalking = true;
			map.unitLayer.moveUnit(index, goal);
		}


		#endregion

		#region drawing player

		public void draw(SpriteBatch spriteBatch, Camera camera)
		{
			spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, SamplerState.LinearClamp, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, camera.transformationMatrix);
			unitSprite.Draw(spriteBatch);
			spriteBatch.End();
		}

        #endregion

		/// <summary>
		/// Clone the current unit object excluding buffs and items
		/// </summary>
		/// <returns></returns>
		public object Clone()
		{
			BaseUnit unit = new BaseUnit(name, _maxHP, _maxAP, _maxMP, _SPD, unitAffinity);

			List<Ability> abilities = new List<Ability>();
			foreach (Ability move in moves)
				abilities.Add(move);

			unit.moves = abilities;


			return unit;
		}


	}

}
