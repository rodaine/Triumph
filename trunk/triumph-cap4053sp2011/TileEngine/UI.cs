using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace TileEngine
{
    public class UI
    {
        #region UI Objects
            private enum Screen
            {
                Title,
                Main,
                Pause,
                Controls,
                End
            }
            Screen mCurrentScreen = Screen.Title;

            private enum Phase
            {
                Menu,
                Move,
                Attack,
                AbilityList,
                AbilityUse
            }
            Phase mCurrentPhase = Phase.Menu;

            private enum MenuOption
            {
                Move,
                Attack,
                Ability,
                EndTurn
            }
            MenuOption mCurrentOption = MenuOption.Move;
        
            Texture2D mTitleScreen;
            Texture2D mActive;
            Texture2D mEnemy;
            Texture2D mMenu;

            Texture2D mBlack;
            Texture2D mBlue;
            Texture2D mGreen;
            Texture2D mRed;

            Texture2D tTimer;
            Texture2D wTimer;

            SpriteFont font, font2, font3, font4, font5, font6, font7;
            SpriteFont experiment;

			AffinityIcon currentAffinity, targetAffinity;
			AffinityIcon currentType, targetType;

            ContentManager Content;

			SoundEffectInstance SFX;
			SoundEffect _menuMove;
			SoundEffect _menuCorrect;
			SoundEffect _menuWrong;

            int ab = 0;

            private float uiTimer = 0f, secondsPerOption = .13f;

            bool exit = false;

            bool hide = false;

        #endregion

            public void LoadContent(ContentManager c)
            {
                Content = c;

                mTitleScreen = Content.Load<Texture2D>("UI/SplashScreen");
                mActive = Content.Load<Texture2D>("UI/activeScroll");
                mEnemy = Content.Load<Texture2D>("UI/targetScroll");
                mMenu = Content.Load<Texture2D>("UI/menuScroll");

                mBlack = Content.Load<Texture2D>("UI/black");
                mBlue = Content.Load<Texture2D>("UI/blue");
                mGreen = Content.Load<Texture2D>("UI/green");
                mRed = Content.Load<Texture2D>("UI/red");

                wTimer = Content.Load<Texture2D>("UI/timer_white");

                font = Content.Load<SpriteFont>("UI/SpriteFont1");
                font2 = Content.Load<SpriteFont>("UI/SpriteFont2");
                font3 = Content.Load<SpriteFont>("UI/SpriteFont3");
                font4 = Content.Load<SpriteFont>("UI/SpriteFont4");
                font5 = Content.Load<SpriteFont>("UI/SpriteFont5");
                font6 = Content.Load<SpriteFont>("UI/SpriteFont6");
                font7 = Content.Load<SpriteFont>("UI/SpriteFont7");
                experiment = Content.Load<SpriteFont>("UI/experimentation");

				currentAffinity = new AffinityIcon(Content.Load<Texture2D>("UI/affinities"));
				targetAffinity = new AffinityIcon(Content.Load<Texture2D>("UI/affinities"));

				FrameAnimation Fire = new FrameAnimation(1, 34, 34, 0, 0);
				FrameAnimation Ice = new FrameAnimation(1, 34, 34, 34, 0);
				FrameAnimation Lightening = new FrameAnimation(1, 34, 34, 68, 0);
				FrameAnimation Water = new FrameAnimation(1, 34, 34, 102, 0);
				FrameAnimation Earth = new FrameAnimation(1, 34, 34, 136, 0);
				FrameAnimation Wind = new FrameAnimation(1, 34, 34, 170, 0);
				FrameAnimation Holy = new FrameAnimation(1, 34, 34, 204, 0);
				FrameAnimation Dark = new FrameAnimation(1, 34, 34, 238, 0);

				currentAffinity.frames.Add("Fire", Fire);
				currentAffinity.frames.Add("Ice", Ice);
				currentAffinity.frames.Add("Lightning", Lightening);
				currentAffinity.frames.Add("Water", Water);
				currentAffinity.frames.Add("Earth", Earth);
				currentAffinity.frames.Add("Wind", Wind);
				currentAffinity.frames.Add("Holy", Holy);
				currentAffinity.frames.Add("Dark", Dark);

				targetAffinity.frames.Add("Fire", (FrameAnimation) Fire.Clone());
				targetAffinity.frames.Add("Ice", (FrameAnimation) Ice.Clone());
				targetAffinity.frames.Add("Lightning", (FrameAnimation) Lightening.Clone());
				targetAffinity.frames.Add("Water", (FrameAnimation) Water.Clone());
				targetAffinity.frames.Add("Earth", (FrameAnimation) Earth.Clone());
				targetAffinity.frames.Add("Wind", (FrameAnimation) Wind.Clone());
				targetAffinity.frames.Add("Holy", (FrameAnimation) Holy.Clone());
				targetAffinity.frames.Add("Dark", (FrameAnimation) Dark.Clone());

				currentType = new AffinityIcon(Content.Load<Texture2D>("UI/unitType"));
				targetType = new AffinityIcon(Content.Load<Texture2D>("UI/unitType"));

				currentType.frames.Add("Melee", (FrameAnimation) Fire.Clone());
				currentType.frames.Add("Magic", (FrameAnimation) Ice.Clone());
				currentType.frames.Add("Range", (FrameAnimation) Lightening.Clone());

				targetType.frames.Add("Melee", (FrameAnimation)Fire.Clone());
				targetType.frames.Add("Magic", (FrameAnimation)Ice.Clone());
				targetType.frames.Add("Range", (FrameAnimation)Lightening.Clone());

				_menuMove = Content.Load<SoundEffect>("Music/click");
				_menuWrong = Content.Load<SoundEffect>("Music/wrong");
				_menuCorrect = Content.Load<SoundEffect>("Music/correct");

            }

            public bool readyToExit()
            {
                if (exit)
                    return true;

                return false;
            }
            
            public void Update(GameTime gameTime, KeyboardState aKeyboardState, BaseUnit currentUnit, BaseUnit targetUnit, Cursor cursor, TileMap map, int counter, TurnManager turnManager, int screenWidth, int screenHeight, BaseUnit[] testUnits, Camera camera, Range range, bool gameWon, ref bool inGame, bool unitBeingAttacked, Faction playerFaction)
            {
                switch (mCurrentScreen)
                {
                    #region Title Screen
                    case Screen.Title:
                        {
                            //If the user presses the "X" key while on the Title screen, start the game
                            //by switching the current state to the Main Screen
                            if (aKeyboardState.IsKeyDown(Keys.X) == true)
                            {
								playCorrect();
								inGame = true;
                                mCurrentScreen = Screen.Main;
                                mCurrentOption = MenuOption.Move;
                            }

                            if (aKeyboardState.IsKeyDown(Keys.Escape) == true)
                            {
                                exit = true;
                            }
                            break;
                        }
                    #endregion

                    #region Main Game
                    case Screen.Main:
                        {
                            if (!gameWon)
                            {

                                //If the user presses the "Q" key while in the main game screen, bring
                                //up the Menu options by switching the current state to Menu
                                if (aKeyboardState.IsKeyDown(Keys.Escape) == true)
                                {
									playCorrect();
                                    mCurrentScreen = Screen.Pause;
                                }

                                if (aKeyboardState.IsKeyDown(Keys.C) == true)
                                {
									playMove();
                                    mCurrentScreen = Screen.Controls;
                                }
                                
                                uiTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
                                if (uiTimer >= secondsPerOption)
                                {
                                    if (aKeyboardState.IsKeyDown(Keys.H) == true)
                                    {
										playMove();
                                        hide = !hide;
                                        uiTimer = 0;
                                    }

                                    if (currentUnit.faction == playerFaction)
                                    {
                                        switch (mCurrentPhase)
                                        {
                                            #region Menu
                                            case (Phase.Menu):
                                                {
                                                    switch (mCurrentOption)
                                                    {
                                                        #region Move
                                                        case (MenuOption.Move):
                                                            {
                                                                if (aKeyboardState.IsKeyDown(Keys.Enter))
                                                                {
																	playMove();
                                                                    mCurrentPhase = Phase.Move;
                                                                    range.clearPoints();
                                                                    range.addPoints(map.walkToPoints(currentUnit));
                                                                    range.isDrawing = true;

                                                                    uiTimer = 0;
                                                                }

                                                                if (aKeyboardState.IsKeyDown(Keys.W))
                                                                {
																	playMove();
                                                                    mCurrentOption = MenuOption.EndTurn;

                                                                    uiTimer = 0;
                                                                }

                                                                if (aKeyboardState.IsKeyDown(Keys.S))
                                                                {
																	playMove();
																	mCurrentOption = MenuOption.EndTurn;
                                                                    if (currentUnit.AP != 0)
                                                                        mCurrentOption = MenuOption.Attack;
                                                                    else
                                                                        mCurrentOption = MenuOption.EndTurn;

                                                                    uiTimer = 0;
                                                                }
                                                                break;
                                                            }
                                                        #endregion

                                                        #region Attack
                                                        case (MenuOption.Attack):
                                                            {
                                                                if (aKeyboardState.IsKeyDown(Keys.Enter))
                                                                {
																	playMove();
                                                                    mCurrentPhase = Phase.Attack;

                                                                    range.clearPoints();
                                                                    range.addPoints(map.attackPoints(currentUnit, currentUnit.range, false, true, false));
                                                                    range.isDrawing = true;

                                                                    uiTimer = 0;
                                                                }

                                                                if (aKeyboardState.IsKeyDown(Keys.W))
                                                                {
																	playMove();
                                                                    if (currentUnit.MP != 0)
                                                                        mCurrentOption = MenuOption.Move;
                                                                    else
                                                                        mCurrentOption = MenuOption.EndTurn;

                                                                    uiTimer = 0;
                                                                }

                                                                if (aKeyboardState.IsKeyDown(Keys.S))
                                                                {
																	playMove();
                                                                    if (currentUnit.AP != 0)
                                                                        mCurrentOption = MenuOption.Ability;
                                                                    else
                                                                        mCurrentOption = MenuOption.EndTurn;

                                                                    uiTimer = 0;
                                                                }
                                                                break;
                                                            }
                                                        #endregion

                                                        #region Ability
                                                        case (MenuOption.Ability):
                                                            {
                                                                if (aKeyboardState.IsKeyDown(Keys.Enter) && currentUnit.canAbility())
                                                                {
																	playMove();
                                                                    mCurrentPhase = Phase.AbilityList;

                                                                    uiTimer = 0;
                                                                }

                                                                if (aKeyboardState.IsKeyDown(Keys.W))
                                                                {
																	playMove();
                                                                    if (currentUnit.AP != 0)
                                                                        mCurrentOption = MenuOption.Attack;
                                                                    else if (currentUnit.MP != 0)
                                                                        mCurrentOption = MenuOption.Move;
                                                                    else
                                                                        mCurrentOption = MenuOption.EndTurn;

                                                                    uiTimer = 0;
                                                                }

                                                                if (aKeyboardState.IsKeyDown(Keys.S))
                                                                {
																	playMove();
                                                                    mCurrentOption = MenuOption.EndTurn;

                                                                    uiTimer = 0;
                                                                }
                                                                break;
                                                            }
                                                        #endregion

                                                        #region End Turn
                                                        case (MenuOption.EndTurn):
                                                            {
                                                                if (aKeyboardState.IsKeyDown(Keys.Enter))
                                                                {
																	playMove();
                                                                    currentUnit.isDone = true;
                                                                    mCurrentPhase = Phase.Menu;
                                                                    mCurrentOption = MenuOption.Move;

                                                                    uiTimer = 0;

                                                                }

                                                                if (aKeyboardState.IsKeyDown(Keys.W))
                                                                {
																	playMove();
                                                                    if (currentUnit.AP != 0)
                                                                        mCurrentOption = MenuOption.Ability;
                                                                    else if (currentUnit.MP != 0)
                                                                        mCurrentOption = MenuOption.Move;
                                                                    else
                                                                        mCurrentOption = MenuOption.EndTurn;

                                                                    uiTimer = 0;
                                                                }

                                                                if (aKeyboardState.IsKeyDown(Keys.S))
                                                                {
																	playMove();
                                                                    if (currentUnit.MP != 0)
                                                                        mCurrentOption = MenuOption.Move;
                                                                    else if (currentUnit.AP != 0)
                                                                        mCurrentOption = MenuOption.Attack;
                                                                    else
                                                                        mCurrentOption = MenuOption.EndTurn;

                                                                    uiTimer = 0;
                                                                }
                                                                break;
                                                            }
                                                        #endregion
                                                    }


                                                    break;
                                                }
                                            #endregion

                                            #region Move
                                            case (Phase.Move):
                                                {                                                   

                                                    if (Keyboard.GetState().IsKeyDown(Keys.Enter) && !currentUnit.isWalking)
                                                    {
														if (currentUnit.goToTile(Engine.convertPositionToTile(cursor.position), map))
															playCorrect();
														else
															playWrong();

                                                        range.clearPoints();
                                                        range.addPoints(map.walkToPoints(currentUnit));

                                                        uiTimer = 0;
                                                    }

                                                    if (!currentUnit.isWalking && currentUnit.MP == 0)
                                                    {
                                                        range.isDrawing = false;
                                                        mCurrentPhase = Phase.Menu;
                                                        if (currentUnit.AP != 0)
                                                            mCurrentOption = MenuOption.Attack;
                                                        else
                                                            mCurrentOption = MenuOption.EndTurn;

                                                        uiTimer = 0;
                                                    }

                                                    if (aKeyboardState.IsKeyDown(Keys.Back) == true && !currentUnit.isWalking)
                                                    {
														playMove();
                                                        range.isDrawing = false;
                                                        mCurrentPhase = Phase.Menu;
                                                        if (currentUnit.MP != 0)
                                                            mCurrentOption = MenuOption.Move;
                                                        else if (currentUnit.AP != 0)
                                                            mCurrentOption = MenuOption.Attack;
                                                        else
                                                            mCurrentOption = MenuOption.EndTurn;

                                                        uiTimer = 0;
                                                    }

                                                    break;
                                                }
                                            #endregion

                                            #region Attack
                                            case (Phase.Attack):
                                                {

                                                    if (!currentUnit.isAttacking && currentUnit.AP == 0&& !unitBeingAttacked)
                                                    {
                                                        range.isDrawing = false;
                                                        mCurrentPhase = Phase.Menu;
                                                        if (currentUnit.MP != 0)
                                                            mCurrentOption = MenuOption.Move;
                                                        else
                                                            mCurrentOption = MenuOption.EndTurn;

                                                        uiTimer = 0;
                                                    }

                                                    //attack does nothing if there is no targetted unit or
                                                    //the targetted unit is dead or the current unit
                                                    if (Keyboard.GetState().IsKeyDown(Keys.Enter) && counter < 0 && !currentUnit.isAttacking && (targetUnit == null || !targetUnit.isBeingHit))
                                                    {
														if (targetUnit != null && !targetUnit.faction.Equals(currentUnit.faction) && !targetUnit.isDead && currentUnit.withinRange(targetUnit)&&!unitBeingAttacked)
														{
															//return an int - -1 if invalid, 0 if miss, value of damage
															currentUnit.attack(targetUnit);

															range.clearPoints();
															range.addPoints(map.attackPoints(currentUnit, currentUnit.range, false, true, false));

															uiTimer = 0;
														}
														else
														{
															playWrong();
														}
                                                    }

                                                    if (aKeyboardState.IsKeyDown(Keys.Back) == true && !currentUnit.isAttacking && (targetUnit == null || !targetUnit.isBeingHit))
                                                    {
														playMove();
                                                        range.isDrawing = false;
                                                        mCurrentPhase = Phase.Menu;
                                                        if (currentUnit.MP != 0)
                                                            mCurrentOption = MenuOption.Move;
                                                        else if (currentUnit.AP != 0)
                                                            mCurrentOption = MenuOption.Attack;
                                                        else
                                                            mCurrentOption = MenuOption.EndTurn;

                                                        uiTimer = 0;
                                                    }

                                                    break;
                                                }
                                            #endregion

                                            #region Ability List
                                            case (Phase.AbilityList):
                                                {
                                                    //attack or ability
                                                    //attacks, does nothing if there is no targetted unit or
                                                    //the targetted unit is dead or the current unit
                                                    if (Keyboard.GetState().IsKeyDown(Keys.Enter) && counter < 0)
                                                    {
														playMove();
                                                        mCurrentPhase = Phase.AbilityUse;

                                                        range.clearPoints();
                                                        range.addPoints(map.attackPoints(currentUnit, currentUnit.moves[ab].attackRange, currentUnit.moves[ab].isFriendly, currentUnit.moves[ab].isHostile, currentUnit.moves[ab].isSelf));

                                                        uiTimer = 0;
                                                    }

                                                    if (aKeyboardState.IsKeyDown(Keys.Back) == true)
                                                    {
														playMove();
                                                        range.isDrawing = false;
                                                        mCurrentPhase = Phase.Menu;
                                                        if (currentUnit.MP != 0)
                                                            mCurrentOption = MenuOption.Move;
                                                        else if (currentUnit.AP != 0)
                                                            mCurrentOption = MenuOption.Attack;
                                                        else
                                                            mCurrentOption = MenuOption.EndTurn;

                                                        uiTimer = 0;
                                                    }

                                                    if (aKeyboardState.IsKeyDown(Keys.W) == true)
                                                    {
														playMove();
                                                        do
                                                        {
                                                            ab--;
                                                            if (ab < 0)
                                                                ab = currentUnit.moves.Count - 1;
                                                        } while (currentUnit.moves[ab].APCost > currentUnit.AP);
                                                        uiTimer = 0;   
                                                    }

                                                    if (aKeyboardState.IsKeyDown(Keys.S) == true)
                                                    {
														playMove();
                                                        do
                                                        {
                                                            ab++;
                                                            if (ab >= currentUnit.moves.Count)
                                                                ab = 0; 
                                                        } while (currentUnit.moves[ab].APCost > currentUnit.AP);
                                                        uiTimer = 0;
                                                    }

                                                    break;
                                                }
                                            #endregion

                                            #region Ability Use
                                            case (Phase.AbilityUse):
                                                {

                                                    if (!currentUnit.isAttacking && !currentUnit.canAbility())
                                                    {
                                                        range.isDrawing = false;
                                                        mCurrentPhase = Phase.Menu;
                                                        if (currentUnit.MP != 0)
                                                            mCurrentOption = MenuOption.Move;
                                                        else
                                                            mCurrentOption = MenuOption.EndTurn;

                                                        uiTimer = 0;
                                                    }

                                                    //ability does nothing if there is no targetted unit or
                                                    //the targetted unit is dead or the current unit
                                                    if (Keyboard.GetState().IsKeyDown(Keys.Enter) && counter < 0 && !currentUnit.isAttacking)
                                                    {
														if (currentUnit.moves[ab].APCost <= currentUnit.AP && currentUnit.canTargetAbility(currentUnit.moves[ab], targetUnit))
														{
															currentUnit.useAbility(currentUnit.moves[ab], targetUnit);
															range.clearPoints();
															range.addPoints(map.attackPoints(currentUnit, currentUnit.moves[ab].attackRange, currentUnit.moves[ab].isFriendly, currentUnit.moves[ab].isHostile, currentUnit.moves[ab].isSelf));
														}
														else
															playWrong();

                                                        uiTimer = 0;
                                                    }

													if (aKeyboardState.IsKeyDown(Keys.Back) == true && !currentUnit.isAttacking && (targetUnit == null || !targetUnit.isBeingHit))
                                                    {
														playMove();
                                                        range.isDrawing = false;
                                                        mCurrentPhase = Phase.Menu;
                                                        if (currentUnit.MP != 0)
                                                            mCurrentOption = MenuOption.Move;
                                                        else if (currentUnit.AP != 0)
                                                            mCurrentOption = MenuOption.Attack;
                                                        else
                                                            mCurrentOption = MenuOption.EndTurn;

                                                        uiTimer = 0;
                                                    }
                                                    break;
                                                }
                                            #endregion
                                        }
                                    }

                                    foreach (BaseUnit unit in testUnits)
                                    {
                                        unit.update(gameTime, map);
                                    }

                                    bool canMove = !currentUnit.isAttacking && !currentUnit.isWalking && !unitBeingAttacked;
                                    if (mCurrentPhase != Phase.Menu && mCurrentPhase != Phase.AbilityList)
                                        cursor.update(gameTime, screenWidth, screenHeight, map, canMove);

                                    camera.update(screenWidth, screenHeight, map);
                                }
                            }
                            else
                            {
                                mCurrentScreen = Screen.End;
                            }
                            break;
                        }
                        
                    #endregion

                    #region Pause Screen
                    case Screen.Pause:
                        {
                            if (aKeyboardState.IsKeyDown(Keys.Q) == true)
                            {
                                exit = true;
                            }

                            if (aKeyboardState.IsKeyDown(Keys.R) == true)
                            {
								playCorrect();
                                mCurrentScreen = Screen.Main;
                            }

                            if (aKeyboardState.IsKeyDown(Keys.C) == true)
                            {
								playMove();
                                mCurrentScreen = Screen.Controls;
                            }
                            break;
                        }
                    #endregion

                    #region Control List
                    case Screen.Controls:
                        {
                            if (aKeyboardState.IsKeyDown(Keys.R) == true || aKeyboardState.IsKeyDown(Keys.Escape) == true)
                            {
								playMove();
                                mCurrentScreen = Screen.Pause;
                            }
                            break;
                        }
                    #endregion

                    #region End
                    case (Screen.End):
                        {
                            uiTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
                            if (uiTimer >= secondsPerOption)
                            {
                                if (aKeyboardState.IsKeyDown(Keys.Escape))
                                {
                                    exit = true;
                                }

                                if (aKeyboardState.IsKeyDown(Keys.Enter))
                                {
									playCorrect();
                                    reset(testUnits, turnManager, currentUnit, map, ref inGame, playerFaction);
                                    mCurrentScreen = Screen.Title;
                                    uiTimer = 0;
                                }
                            }
                            break;
                        }
                    #endregion
                }

            }
        
            public void Draw(GameTime gameTime, SpriteBatch spriteBatch, int winWidth, int winHeight, TileMap map, Camera camera, Cursor cursor, BaseUnit[] testUnits, BaseUnit currentUnit, BaseUnit targetUnit, Range range, int winner, Faction playerFaction)
            {
                //spriteBatch.Begin();

                switch (mCurrentScreen)
                {
                    #region Title Screen
                    case Screen.Title:
                        {
                            spriteBatch.Begin();
                            spriteBatch.Draw(mTitleScreen, new Rectangle(0, 0, winWidth, winHeight), Color.White);
                            spriteBatch.DrawString(experiment, "X : Start", new Vector2(winWidth - 2 * Engine.TILE_WIDTH - Engine.TILE_WIDTH / 2, winHeight - 2*Engine.TILE_HEIGHT), Color.White);
                            spriteBatch.DrawString(experiment, "ESC : Exit", new Vector2(winWidth - 2 * Engine.TILE_WIDTH - Engine.TILE_WIDTH / 2, winHeight - Engine.TILE_HEIGHT), Color.White);
                            spriteBatch.End();
                            break;
                        }
                    #endregion

                    #region Main Game
                    case Screen.Main:
                        {
                            GameConsole.getInstanceOf().UpdateClockTime(gameTime.ElapsedGameTime.TotalSeconds);
                            map.draw(spriteBatch, camera);
                            range.draw(spriteBatch, camera);
                            cursor.Draw(spriteBatch, camera);
                            foreach (BaseUnit unit in testUnits)
                            {
                                unit.draw(spriteBatch, camera);
                            }

                            bool bottom = false;
                            if (cursor.location.Y > (30 - 8))
                            {
                                bottom = true;
                            }
                            else
                            {
                                bottom = false;
                            }

                            bool left = false;
                            if (cursor.location.X > (30 - 13))
                            {
                                left = true;
                            }
                            else
                            {
                                left = false;
                            }
                            
                            GameConsole.getInstanceOf().Draw(spriteBatch, winWidth, winHeight, left, bottom, hide);
                            
                            drawActiveInformation(spriteBatch, currentUnit, winHeight, winWidth, bottom);
                            drawTargetInformation(spriteBatch, targetUnit, currentUnit, winHeight, winWidth, bottom);


                            if (mCurrentPhase == Phase.AbilityList)
                            {
                                drawAbilityOption(spriteBatch, winHeight, winWidth, bottom, currentUnit);
                            }

                            if (mCurrentPhase == Phase.Menu && currentUnit.faction == playerFaction)
                            {
                                int m = 0, at = 0, ab = 0, et = 0;

                                switch (mCurrentOption)
                                {
                                    case (MenuOption.Move):
                                        m = 1;
                                        if (currentUnit.AP != 0)
                                        {
                                            ab = 2;
                                            at = 2;
                                        }
                                        break;
                                    case(MenuOption.Attack):
                                        at = 1;
                                        ab = 2;
                                        if (currentUnit.MP != 0)
                                        {
                                            m = 2;
                                        }
                                        break;
                                    case(MenuOption.Ability):
                                        ab = 1;
                                        at = 2;
                                        if (currentUnit.MP != 0)
                                        {
                                            m = 2;
                                        }
                                        break;
                                    case(MenuOption.EndTurn):
                                        et = 1;
                                        if (currentUnit.MP != 0)
                                            m = 2;
                                        if (currentUnit.AP != 0)
                                        {
                                            at = 2;
                                            ab = 2;
                                        }
                                        break;
                                }

                                drawMenu(spriteBatch, m, at, ab, et, winHeight, winWidth, bottom);
                            }
                            break;
                        }
                    #endregion

                    #region Pause Screen
                    case Screen.Pause:
                        {
                            map.draw(spriteBatch, camera);
                            spriteBatch.Begin();

                            spriteBatch.Draw(mTitleScreen, new Rectangle(0, 0, winWidth, winHeight), new Color(1f, 1f, 1f, 0.5f));
                            spriteBatch.DrawString(experiment, "C : Control List", new Vector2(winWidth - 3 * Engine.TILE_WIDTH - Engine.TILE_WIDTH/2, winHeight - 2*Engine.TILE_HEIGHT), new Color(1f, 1f, 1f, .75f));
                            spriteBatch.DrawString(experiment, "R : Resume", new Vector2(winWidth - 3 * Engine.TILE_WIDTH - Engine.TILE_WIDTH / 2, winHeight - (3 * Engine.TILE_HEIGHT) / 2), new Color(1f,1f,1f,.75f));
                            spriteBatch.DrawString(experiment, "Q : Quit", new Vector2(winWidth - 3 * Engine.TILE_WIDTH - Engine.TILE_WIDTH / 2, winHeight - Engine.TILE_HEIGHT), new Color(1f, 1f, 1f, .75f));

                            spriteBatch.End();
                            break;
                        }
                    #endregion

                    #region Control Screen
                    case Screen.Controls:
                        {
                            map.draw(spriteBatch, camera);
                            spriteBatch.Begin();

                            spriteBatch.Draw(mBlack, new Rectangle(0, 0, winWidth, winHeight), new Color(1f, 1f, 1f, .4f));
                            spriteBatch.Draw(mBlack, new Rectangle(winWidth / 4, winHeight / 4, winWidth / 2, winHeight / 2), new Color(1f, 1f, 1f, .6f));
                            spriteBatch.Draw(mRed, new Rectangle(winWidth / 4 + 10, winHeight / 4 + 10, winWidth / 2 - 20, winHeight / 2 - 20), new Color(1f, 1f, 1f, .1f));
                            spriteBatch.Draw(mBlack, new Rectangle(winWidth / 4 + 15, winHeight / 4 + 15, winWidth / 2 - 30, winHeight / 2 - 30), Color.White);

                            int offset = 30;

                            string str = "CONTROL LIST";
                            spriteBatch.DrawString(font, str, new Vector2(winWidth / 2 - 6*str.Length, winHeight / 4 + offset), Color.White);

                            str = "ESC : Pause Game";
                            spriteBatch.DrawString(font2, str, new Vector2(winWidth / 4 + 30, winHeight / 4 + 2 * offset), Color.White);

                            str = "W : Up";
                            spriteBatch.DrawString(font2, str, new Vector2(winWidth / 4 + 30, winHeight / 4 + 3 * offset), Color.White);

                            str = "A : Left";
                            spriteBatch.DrawString(font2, str, new Vector2(winWidth / 4 + 30, winHeight / 4 + 4 * offset), Color.White);

                            str = "S : Down";
                            spriteBatch.DrawString(font2, str, new Vector2(winWidth / 4 + 30, winHeight / 4 + 5 * offset), Color.White);

                            str = "D : Right";
                            spriteBatch.DrawString(font2, str, new Vector2(winWidth / 4 + 30, winHeight / 4 + 6 * offset), Color.White);

                            str = "ENTER : Confirm";
                            spriteBatch.DrawString(font2, str, new Vector2(winWidth / 2, winHeight / 4 + 2 * offset), Color.White);

                            str = "BACKSPACE : Cancel";
                            spriteBatch.DrawString(font2, str, new Vector2(winWidth / 2, winHeight / 4 + 3 * offset), Color.White);

                            str = "H: Toggle Console";
                            spriteBatch.DrawString(font2, str, new Vector2(winWidth / 2, winHeight / 4 + 4 * offset), Color.White);

                            str = "C : Controls";
                            spriteBatch.DrawString(font2, str, new Vector2(winWidth / 2, winHeight / 4 + 5 * offset), Color.White);

                            str = "R : Resume Game";
                            spriteBatch.DrawString(font2, str, new Vector2(winWidth / 2, winHeight / 4 + 6 * offset), Color.White);

                            spriteBatch.End();
                            break;
                        }
                    #endregion

                    #region End
                    case (Screen.End):
                        {
                            map.draw(spriteBatch, camera);
                            spriteBatch.Begin();

                            String msg1 = "", msg2 = "";

                            //winner = 2;

                            if (winner == 1)
                            {
                                msg1 = "Congratulations!";
                                msg2 = "You have won!";
                            }
                            else if (winner == 2)
                            {
                                msg1 = "Good try...";
                                msg2 = "The AI has won.";
                            }

                            spriteBatch.Draw(mTitleScreen, new Rectangle(0, 0, winWidth, winHeight), new Color(1f, 1f, 1f, .7f));
                            spriteBatch.DrawString(font7, msg1, new Vector2(winWidth / 2 - 19 * msg1.Length, 50), Color.White);
                            spriteBatch.DrawString(font7, msg2, new Vector2(winWidth / 2 - 19 * msg2.Length, 325), Color.White);

                            spriteBatch.DrawString(experiment, "Esc : Exit", new Vector2(winWidth - 3 * Engine.TILE_WIDTH - Engine.TILE_WIDTH / 2, winHeight - (3 * Engine.TILE_HEIGHT) / 2), new Color(1f, 1f, 1f, .75f));
                            spriteBatch.DrawString(experiment, "Enter : Begin New Game", new Vector2(winWidth - 6 * Engine.TILE_WIDTH, winHeight - Engine.TILE_HEIGHT), new Color(1f, 1f, 1f, .75f));

                            spriteBatch.End();
                            break;
                        }
                    #endregion
                }
            }

            private void drawActiveInformation(SpriteBatch spriteBatch, BaseUnit active, int winHeight, int winWidth, bool bottom)
            {
                String name = active.name;
                String fac = active.faction.name;
                int currHP = active.HP;
                int maxHP = active.maxHP;
                int currAP = active.AP;
                int maxAP = active.maxAP;
                int currMP = active.MP;
                int maxMP = active.maxMP;
                int range = active.range;

                spriteBatch.Begin();

                currentAffinity.currentAffinity = active.affinityName;
                currentAffinity.isDrawing = true;

                int c1 = Engine.TILE_WIDTH / 2;
                int c2;

                if (bottom)
                {
                    c2 = Engine.TILE_HEIGHT / 2;
                }
                else
                {
                    c2 = winHeight - winHeight / 4 - Engine.TILE_HEIGHT / 2;
                }

                int wid = winWidth / 3;
                int hei = winHeight / 4;


                spriteBatch.Draw(mActive, new Rectangle(c1, c2, wid, hei), Color.White);
                currentAffinity.position.X = c1 + wid - 32;
                currentAffinity.position.Y = c2 - 5;
                currentAffinity.Draw(spriteBatch);

                spriteBatch.DrawString(font4, name, new Vector2(c1 + 45, c2 + 15), active.faction.color);
                spriteBatch.DrawString(font5, fac, new Vector2(c1 + 100, c2 + 35), active.faction.color);

                spriteBatch.DrawString(font3, "Rng : " + range, new Vector2(c1 + 51, c2 + 50), Color.Black);

                spriteBatch.DrawString(font3, "HP : " + currHP + " / " + maxHP, new Vector2(c1 + 160, c2 + 65), Color.Black);
                spriteBatch.Draw(mBlack, new Rectangle(c1 + 50, c2 + 68, 102, 10), new Color(1f, 1f, 1f, 0.7f));
                if (maxHP != 0)
                    spriteBatch.Draw(mRed, new Rectangle(c1 + 51, c2 + 69, (100 * currHP) / maxHP, 8), new Color(1f, 1f, 1f, .7f));

                spriteBatch.DrawString(font3, "AP : " + currAP + " / " + maxAP, new Vector2(c1 + 160, c2 + 80), Color.Black);
                spriteBatch.Draw(mBlack, new Rectangle(c1 + 50, c2 + 83, 102, 10), new Color(1f, 1f, 1f, 0.7f));
                if (maxAP != 0)
                    spriteBatch.Draw(mGreen, new Rectangle(c1 + 51, c2 + 84, (100 * currAP) / maxAP, 8), new Color(1f, 1f, 1f, .7f));

                spriteBatch.DrawString(font3, "MP : " + currMP + " / " + maxMP, new Vector2(c1 + 160, c2 + 95), Color.Black);
                spriteBatch.Draw(mBlack, new Rectangle(c1 + 50, c2 + 98, 102, 10), new Color(1f, 1f, 1f, 0.7f));
                if (maxMP != 0)
                    spriteBatch.Draw(mBlue, new Rectangle(c1 + 51, c2 + 99, (100 * currMP) / maxMP, 8), new Color(1f, 1f, 1f, .7f));

                spriteBatch.End();
            }

            private void drawTargetInformation(SpriteBatch spriteBatch, BaseUnit target, BaseUnit active, int winHeight, int winWidth, bool bottom)
            {
                if (target != null)
                {
                    String name = target.name;
                    Faction tFac = target.faction;
                    Faction aFac = active.faction;
                    int currHP = target.HP;
                    int maxHP = target.maxHP;
                    int currAP = target.AP;
                    int maxAP = target.maxAP;
                    int currMP = target.MP;
                    int maxMP = target.maxMP;
                    Color facColor = Color.Black;

                    spriteBatch.Begin();

                    targetAffinity.currentAffinity = target.affinityName;
                    targetAffinity.isDrawing = true;

                    int c1 = winWidth - winWidth / 3 - Engine.TILE_WIDTH / 2;

                    int c2;

                    if (bottom)
                    {
                        c2 = Engine.TILE_HEIGHT / 2;
                    }
                    else
                    {
                        c2 = winHeight - winHeight / 4 - Engine.TILE_HEIGHT / 2;
                    }
                    int wid = winWidth / 3;
                    int hei = winHeight / 4;

                    spriteBatch.Draw(mEnemy, new Rectangle(c1, c2, wid, hei), Color.White);
                    targetAffinity.position.X = c1 - 5;
                    targetAffinity.position.Y = c2 - 5;
                    targetAffinity.Draw(spriteBatch);
                    
                    spriteBatch.DrawString(font4, name, new Vector2(c1 + 30, c2 + 15), tFac.color);
                    spriteBatch.DrawString(font5, tFac.name, new Vector2(c1 + 85, c2 + 35), tFac.color);

                    spriteBatch.DrawString(font3, "Rng : " + target.range.ToString(), new Vector2(c1 + 40, c2 + 50), Color.Black);

                    spriteBatch.DrawString(font3, "HP : " + currHP + " / " + maxHP, new Vector2(c1 + 35, c2 + 65), Color.Black);
                    spriteBatch.Draw(mBlack, new Rectangle(c1 + 105, c2 + 68, 102, 10), new Color(1f, 1f, 1f, 0.7f));
                    if (maxHP != 0)
                        spriteBatch.Draw(mRed, new Rectangle(c1 + 106, c2 + 69, (100 * currHP) / maxHP, 8), new Color(1f, 1f, 1f, .7f));

                    spriteBatch.DrawString(font3, "AP : " + currAP + " / " + maxAP, new Vector2(c1 + 50, c2 + 80), Color.Black);
                    spriteBatch.Draw(mBlack, new Rectangle(c1 + 105, c2 + 83, 102, 10), new Color(1f, 1f, 1f, 0.7f));
                    if (maxAP != 0)
                        spriteBatch.Draw(mGreen, new Rectangle(c1 + 106, c2 + 84, (100 * currAP) / maxAP, 8), new Color(1f, 1f, 1f, .7f));

                    spriteBatch.DrawString(font3, "MP : " + currMP + " / " + maxMP, new Vector2(c1 + 50, c2 + 95), Color.Black);
                    spriteBatch.Draw(mBlack, new Rectangle(c1 + 105, c2 + 98, 102, 10), new Color(1f, 1f, 1f, 0.7f));
                    if (maxMP != 0)
                        spriteBatch.Draw(mBlue, new Rectangle(c1 + 106, c2 + 99, (100 * currMP) / maxMP, 8), new Color(1f, 1f, 1f, .7f));

                    spriteBatch.End();
                }
            }

            private void drawMenu(SpriteBatch spriteBatch, int m, int at, int ab, int et, int winHeight, int winWidth, bool bottom)
            {
                spriteBatch.Begin();

                int c1 = winWidth / 2 - (winWidth / 6) + Engine.TILE_WIDTH;
                int c2;

                if (bottom)
                {
                    c2 = Engine.TILE_HEIGHT / 2;
                }
                else
                {
                    c2 = winHeight - winHeight / 4 - Engine.TILE_HEIGHT / 2;
                }

                spriteBatch.Draw(mMenu, new Rectangle(c1, c2, winWidth / 3 - Engine.TILE_WIDTH * 2, winHeight / 4), Color.White);

                Color mColor = Color.Black;
                Color atColor = Color.Black;
                Color abColor = Color.Black;
                Color etColor = Color.Black;

                switch (m)
                {
                    case(0):
                        mColor = Color.Gray;
                        break;
                    case(1):
                        mColor = Color.Green;
                        break;
                }
                switch (at)
                {
                    case(0):
                        atColor = Color.Gray;
                        break;
                    case(1):
                        atColor = Color.Green;
                        break;
                }
                switch (ab)
                {
                    case(0):
                        abColor = Color.Gray;
                        break;
                    case(1):
                        abColor = Color.Green;
                        break;
                }

                if (et == 1)
                {
                    etColor = Color.Green;
                }

                spriteBatch.DrawString(font6, "Move", new Vector2(winWidth / 2 - 20, c2 + 20), mColor);
                spriteBatch.DrawString(font6, "Attack", new Vector2(winWidth / 2 - 22, c2 + 40), atColor);
                spriteBatch.DrawString(font6, "Ability", new Vector2(winWidth / 2 - 23, c2 + 60), abColor);
                spriteBatch.DrawString(font6, "End Turn", new Vector2(winWidth / 2 - 30, c2 + 80), etColor);
                spriteBatch.End();
            }

            private void drawAbilityOption(SpriteBatch spriteBatch, int winHeight, int winWidth, bool bottom, BaseUnit active)
            {
                spriteBatch.Begin();

                int c1 = winWidth / 2 - (winWidth / 6) + Engine.TILE_WIDTH;
                int c2;

                if (bottom)
                {
                    c2 = Engine.TILE_HEIGHT / 2;
                }
                else
                {
                    c2 = winHeight - winHeight / 4 - Engine.TILE_HEIGHT / 2;
                }

                spriteBatch.Draw(mMenu, new Rectangle(c1, c2, winWidth / 3 - Engine.TILE_WIDTH * 2, winHeight / 4), Color.White);

                spriteBatch.DrawString(font6, "W", new Vector2(c1 + (winWidth/3 - Engine.TILE_WIDTH * 2) / 2, c2 + 10), Color.Black);
                spriteBatch.DrawString(font6, "S", new Vector2(c1 + (winWidth / 3 - Engine.TILE_WIDTH * 2) / 2, c2 + winHeight / 4 - 30), Color.Black);

                spriteBatch.DrawString(font6, active.moves[ab].name, new Vector2(winWidth / 2 - 3* (active.moves[ab].name.Length), c2 + 35), Color.Black);
                spriteBatch.DrawString(font6, "AP : " + active.moves[ab].APCost + " || Rng : " + active.moves[ab].attackRange, new Vector2(winWidth / 2 - 3 * ("AP : " + active.moves[ab].APCost + " || Rng : " + active.moves[ab].attackRange).Length, c2 + 60), Color.Black);                

                spriteBatch.End();
            }

			private void reset(BaseUnit[] testUnits, TurnManager turnManager, BaseUnit currentUnit, TileMap map, ref bool inGame, Faction playerFaction)
			{
				int i = 0, j = 0;
				foreach (BaseUnit unit in testUnits)
				{
					unit.HP = unit.maxHP;
					unit.MP = unit.maxMP;
					unit.AP = unit.maxAP;
					unit.delay = 0;
					unit.faction.numDead = 0;

					if (unit.faction == playerFaction)
					{
						unit.unitSprite.currentAnimationName = "Down";
						unit.randomPosition(new Point(20, 0), new Point(29, 9), map);
						++i;
					}
					else
					{
						unit.unitSprite.currentAnimationName = "Up";
						unit.randomPosition(new Point(8, 20), new Point(17, 29), map);
						++j;
					}
				}

				currentUnit.isDone = true;
                GameConsole.getInstanceOf().reset();
			}

			private void playCorrect()
			{
				SFX = _menuCorrect.CreateInstance();
				SFX.IsLooped = false;
				SFX.Volume = 1f;
				SFX.Play();
				mCurrentOption = MenuOption.EndTurn;
			}

			private void playWrong()
			{
				SFX = _menuWrong.CreateInstance();
				SFX.IsLooped = false;
				SFX.Volume = 1f;
				SFX.Play();
				mCurrentOption = MenuOption.EndTurn;
			}

			private void playMove()
			{
				SFX = _menuMove.CreateInstance();
				SFX.IsLooped = false;
				SFX.Volume = 1f;
				SFX.Play();
				mCurrentOption = MenuOption.EndTurn;
			}
            
    }
}
