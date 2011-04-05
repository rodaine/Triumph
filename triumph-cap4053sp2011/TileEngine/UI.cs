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
                Pause
            }
            Screen mCurrentScreen = Screen.Title;
        
            Texture2D mTitleScreen;
            Texture2D mActive;
            Texture2D mEnemy;

            Texture2D mBlack;
            Texture2D mBlue;
            Texture2D mGreen;
            Texture2D mRed;

            SpriteFont font, font2, font3, font4, font5;
            SpriteFont experiment;

            KeyboardState mPreviousKeyboardState;

            ContentManager Content;

            bool exit = false;
        #endregion

            public void LoadContent(ContentManager c)
            {
                Content = c;

                mTitleScreen = Content.Load<Texture2D>("UI/SplashScreen");
                mActive = Content.Load<Texture2D>("UI/activeScroll");
                mEnemy = Content.Load<Texture2D>("UI/targetScroll");

                mBlack = Content.Load<Texture2D>("UI/black");
                mBlue = Content.Load<Texture2D>("UI/blue");
                mGreen = Content.Load<Texture2D>("UI/green");
                mRed = Content.Load<Texture2D>("UI/red");

                font = Content.Load<SpriteFont>("UI/SpriteFont1");
                font2 = Content.Load<SpriteFont>("UI/SpriteFont2");
                font3 = Content.Load<SpriteFont>("UI/SpriteFont3");
                font4 = Content.Load<SpriteFont>("UI/SpriteFont4");
                font5 = Content.Load<SpriteFont>("UI/SpriteFont5");
                experiment = Content.Load<SpriteFont>("UI/experimentation");
            }

            public bool readyToExit()
            {
                if (exit)
                    return true;

                return false;
            }
            
            public void Update(GameTime gameTime, KeyboardState aKeyboardState, BaseUnit currentUnit, BaseUnit targetUnit, Cursor cursor, TileMap map, int counter, TurnManager turnManager, int screenWidth, int screenHeight, BaseUnit[] testUnits, Camera camera, RandomNumber random)
            {
                switch (mCurrentScreen)
                {
                    case Screen.Title:
                        {
                            //If the user presses the "X" key while on the Title screen, start the game
                            //by switching the current state to the Main Screen
                            if (aKeyboardState.IsKeyDown(Keys.X) == true)
                            {
                                mCurrentScreen = Screen.Main;
                            }

                            if (aKeyboardState.IsKeyDown(Keys.Escape) == true)
                            {
                                exit = true;
                            }
                            break;
                        }
                    case Screen.Main:
                        {
                            //If the user presses the "Q" key while in the main game screen, bring
                            //up the Menu options by switching the current state to Menu
                            if (aKeyboardState.IsKeyDown(Keys.Escape) == true)
                            {
                                mCurrentScreen = Screen.Pause;
                            }                 

                            if (Keyboard.GetState().IsKeyDown(Keys.Enter))
                            {
                                currentUnit.goToTile(Engine.convertPositionToTile(cursor.position), map);
                            }

                            //attacks, does nothing if there is no targetted unit or
                            //the targetted unit is dead or the current unit
                            if (Keyboard.GetState().IsKeyDown(Keys.E) && counter < 0)
                            {
                                if (targetUnit != null && !targetUnit.faction.Equals(currentUnit.faction) && !targetUnit.isDead && currentUnit.withinRange(targetUnit))
                                {
                                    currentUnit.attack(targetUnit, random.getNext(1,20));
                                    System.Console.WriteLine(targetUnit.HP);
                                    currentUnit.isDone = true;
                                }
                            }

                            //ends turn without attacking
                            if (Keyboard.GetState().IsKeyDown(Keys.R) && counter < 0)
                            {
                                currentUnit.isDone = true;
                            }
   

                            // TODO: Add your update logic here 
                            //int screenWidth = GraphicsDevice.Viewport.Width;
                            //int screenHeight = GraphicsDevice.Viewport.Height;
                            foreach (BaseUnit unit in testUnits)
                            {
                                unit.update(gameTime, screenWidth, screenHeight, map);
                            }
                            cursor.update(gameTime, screenWidth, screenHeight, map);

                            camera.update(screenWidth, screenHeight, map);
                            break;
                        }
                    case Screen.Pause:
                        {
                            if (aKeyboardState.IsKeyDown(Keys.Q) == true)
                            {
                                exit = true;
                            }

                            if (aKeyboardState.IsKeyDown(Keys.R) == true)
                            {
                                mCurrentScreen = Screen.Main;
                            }
                            break;
                        }
                }

                //Store the Keyboard state
                mPreviousKeyboardState = aKeyboardState;

            }


            //called from update, draws screen
            public void Draw(GameTime gameTime, SpriteBatch spriteBatch, int winWidth, int winHeight, TileMap map, Camera camera, Cursor cursor, BaseUnit[] testUnits, BaseUnit currentUnit, BaseUnit targetUnit)
            {
                //spriteBatch.Begin();

                switch (mCurrentScreen)
                {
                    case Screen.Title:
                        {
                            spriteBatch.Begin();
                            spriteBatch.Draw(mTitleScreen, new Rectangle(0, 0, winWidth, winHeight), Color.White);
                            spriteBatch.DrawString(experiment, "X : Start", new Vector2(winWidth - 2 * Engine.TILE_WIDTH - Engine.TILE_WIDTH / 2, winHeight - 2*Engine.TILE_HEIGHT), Color.White);
                            spriteBatch.DrawString(experiment, "ESC : Exit", new Vector2(winWidth - 2 * Engine.TILE_WIDTH - Engine.TILE_WIDTH / 2, winHeight - Engine.TILE_HEIGHT), Color.White);
                            spriteBatch.End();
                            break;
                        }

                    case Screen.Main:
                        {
                            map.draw(spriteBatch, camera);
                            cursor.Draw(spriteBatch, camera);
                            foreach (BaseUnit unit in testUnits)
                            {
                                unit.draw(spriteBatch, camera);
                            }
                            drawActiveInformation(spriteBatch, currentUnit, winHeight, winWidth);
                            drawTargetInformation(spriteBatch, targetUnit, currentUnit, winHeight, winWidth);
                            break;
                        }

                    case Screen.Pause:
                        {
                            map.draw(spriteBatch, camera);
                            spriteBatch.Begin();

                            spriteBatch.Draw(mTitleScreen, new Rectangle(0, 0, winWidth, winHeight), new Color(1f, 1f, 1f, 0.5f));

                            spriteBatch.End();
                            break;
                        }
                }
            }

            private void drawActiveInformation(SpriteBatch spriteBatch, BaseUnit active, int winHeight, int winWidth)
            {
                String name = active.name;
                String fac = active.faction.name;
                int currHP = active.HP;
                int maxHP = active.maxHP;
                int currAP = active.AP;
                int maxAP = active.maxAP;
                int currMP = active.MP;
                int maxMP = active.maxMP;

                spriteBatch.Begin();

                int c1 = Engine.TILE_WIDTH / 2;
                int c2 = winHeight - winHeight / 4 - Engine.TILE_HEIGHT / 2;
                int c3 = winWidth / 3;
                int c4 = winHeight / 4;


                spriteBatch.Draw(mActive, new Rectangle(c1, c2, c3, c4), Color.White);

                spriteBatch.DrawString(font4, name, new Vector2(c1 + 45, c2 + 20), Color.Blue);
                spriteBatch.DrawString(font5, fac, new Vector2(c1 + 90, c2 + 40), Color.Blue);

                spriteBatch.DrawString(font3, "HP : " + currHP + " / " + maxHP, new Vector2(c1 + 160, c2 + 60), Color.Black);
                spriteBatch.Draw(mBlack, new Rectangle(c1 + 50, c2 + 63, 102, 10), new Color(1f, 1f, 1f, 0.7f));
                if (maxHP != 0)
                    spriteBatch.Draw(mRed, new Rectangle(c1 + 51, c2 + 64, (100 * currHP) / maxHP, 8), new Color(1f, 1f, 1f, .7f));

                spriteBatch.DrawString(font3, "AP : " + currAP + " / " + maxAP, new Vector2(c1 + 160, c2 + 75), Color.Black);
                spriteBatch.Draw(mBlack, new Rectangle(c1 + 50, c2 + 78, 102, 10), new Color(1f, 1f, 1f, 0.7f));
                if (maxAP != 0)
                    spriteBatch.Draw(mGreen, new Rectangle(c1 + 51, c2 + 79, (100 * currAP) / maxAP, 8), new Color(1f, 1f, 1f, .7f));

                spriteBatch.DrawString(font3, "MP : " + currMP + " / " + maxMP, new Vector2(c1 + 160, c2 + 90), Color.Black);
                spriteBatch.Draw(mBlack, new Rectangle(c1 + 50, c2 + 93, 102, 10), new Color(1f, 1f, 1f, 0.7f));
                if (maxMP != 0)
                    spriteBatch.Draw(mBlue, new Rectangle(c1 + 51, c2 + 94, (100 * currMP) / maxMP, 8), new Color(1f, 1f, 1f, .7f));

                spriteBatch.End();
            }

            private void drawTargetInformation(SpriteBatch spriteBatch, BaseUnit target, BaseUnit active, int winHeight, int winWidth)
            {
                if (target != null)
                {
                    String name = target.name;
                    String tFac = target.faction.name;
                    String aFac = active.faction.name;
                    int currHP = target.HP;
                    int maxHP = target.maxHP;
                    int currAP = target.AP;
                    int maxAP = target.maxAP;
                    int currMP = target.MP;
                    int maxMP = target.maxMP;
                    Color facColor = Color.Black;

                    spriteBatch.Begin();

                    int c1 = winWidth - winWidth / 3 - Engine.TILE_WIDTH / 2;
                    int c2 = winHeight - winHeight / 4 - Engine.TILE_HEIGHT / 2;
                    int c3 = winWidth / 3;
                    int c4 = winHeight / 4;

                    if (tFac == aFac)
                    {
                        spriteBatch.Draw(mEnemy, new Rectangle(c1, c2, c3, c4), Color.White);
                        facColor = Color.Blue;
                    }
                    else
                    {
                        spriteBatch.Draw(mEnemy, new Rectangle(c1, c2, c3, c4), Color.White);
                        facColor = Color.Red;
                    }
                    
                    spriteBatch.DrawString(font4, name, new Vector2(c1 + 30, c2 + 20), facColor);
                    spriteBatch.DrawString(font5, tFac, new Vector2(c1 + 75, c2 + 40), facColor);

                    spriteBatch.DrawString(font3, "HP : " + currHP + " / " + maxHP, new Vector2(c1 + 35, c2 + 60), Color.Black);
                    spriteBatch.Draw(mBlack, new Rectangle(c1 + 105, c2 + 63, 102, 10), new Color(1f, 1f, 1f, 0.7f));
                    if (maxHP != 0)
                        spriteBatch.Draw(mRed, new Rectangle(c1 + 106, c2 + 64, (100 * currHP) / maxHP, 8), new Color(1f, 1f, 1f, .7f));

                    spriteBatch.DrawString(font3, "AP : " + currAP + " / " + maxAP, new Vector2(c1 + 50, c2 + 75), Color.Black);
                    spriteBatch.Draw(mBlack, new Rectangle(c1 + 105, c2 + 78, 102, 10), new Color(1f, 1f, 1f, 0.7f));
                    if (maxAP != 0)
                        spriteBatch.Draw(mGreen, new Rectangle(c1 + 106, c2 + 79, (100 * currAP) / maxAP, 8), new Color(1f, 1f, 1f, .7f));

                    spriteBatch.DrawString(font3, "MP : " + currMP + " / " + maxMP, new Vector2(c1 + 50, c2 + 90), Color.Black);
                    spriteBatch.Draw(mBlack, new Rectangle(c1 + 105, c2 + 93, 102, 10), new Color(1f, 1f, 1f, 0.7f));
                    if (maxMP != 0)
                        spriteBatch.Draw(mBlue, new Rectangle(c1 + 106, c2 + 94, (100 * currMP) / maxMP, 8), new Color(1f, 1f, 1f, .7f));

                    spriteBatch.End();
                }
            }
    }
}
