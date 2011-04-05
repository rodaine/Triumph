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
                Menu
            }
            Screen mCurrentScreen = Screen.Title;

            private enum MenuOptions
            {
                Resume,
                ExitGame
            }
            MenuOptions mCurrentMenuOption = MenuOptions.Resume;

            Texture2D mTitleScreen;
            Texture2D mMainScreen;
            Texture2D mMenu;
            Texture2D mMenuOptions;
            Texture2D mActive;

            KeyboardState mPreviousKeyboardState;

            ContentManager Content;
        #endregion

            public void LoadContent(ContentManager c)
            {
                Content = c;

                mTitleScreen = Content.Load<Texture2D>("UI/Title");
                mMainScreen = Content.Load<Texture2D>("UI/MainScreen");
                mMenu = Content.Load<Texture2D>("UI/Menu");
                mMenuOptions = Content.Load<Texture2D>("UI/MenuOptions");
                mActive = Content.Load<Texture2D>("UI/ActiveUnitBox");
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
                            break;
                        }
                    case Screen.Main:
                        {
                            //If the user presses the "Q" key while in the main game screen, bring
                            //up the Menu options by switching the current state to Menu
                            if (aKeyboardState.IsKeyDown(Keys.Q) == true)
                            {
                                mCurrentScreen = Screen.Menu;
                            }                 

                            if (Keyboard.GetState().IsKeyDown(Keys.Enter))
                            {
                                currentUnit.goToTile(Engine.convertPositionToTile(cursor.position), map);
                            }

                            if (Keyboard.GetState().IsKeyDown(Keys.E) && counter < 0)
                            {
                                if (targetUnit != null)
                                {
                                    currentUnit.attack(targetUnit, random.getNext());
                                }
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
                    case Screen.Menu:
                        {
                            //Move the currently highlighted menu option 
                            //up and down depending on what key the user has pressed
                            if (aKeyboardState.IsKeyDown(Keys.Down) == true && mPreviousKeyboardState.IsKeyDown(Keys.Down) == false)
                            {
                                //Move selection down
                                if (mCurrentMenuOption == MenuOptions.Resume)
                                {
                                    mCurrentMenuOption = MenuOptions.ExitGame;

                                }
                            }

                            if (aKeyboardState.IsKeyDown(Keys.Up) == true && mPreviousKeyboardState.IsKeyDown(Keys.Up) == false)
                            {
                                if (mCurrentMenuOption == MenuOptions.ExitGame)
                                {
                                    mCurrentMenuOption = MenuOptions.Resume;
                                }
                            }

                            //If the user presses the "X" key, move the state to the 
                            //appropriate game state based on the current selection
                            if (aKeyboardState.IsKeyDown(Keys.X) == true)
                            {
                                switch (mCurrentMenuOption)
                                {
                                    //Return to the Main game screen and close the menu
                                    case MenuOptions.Resume:
                                        {
                                            mCurrentScreen = Screen.Main;
                                            break;
                                        }
                                    //Exit the game
                                    case MenuOptions.ExitGame:
                                        {
                                            //this.Exit();
                                            break;
                                        }
                                }

                                //Reset the selected menu option to Resume
                                mCurrentMenuOption = MenuOptions.Resume;
                            }
                            break;
                        }
                }

                //Store the Keyboard state
                mPreviousKeyboardState = aKeyboardState;

            }


            //called from update, draws screen
            public void Draw(GameTime gameTime, SpriteBatch spriteBatch, int winWidth, int winHeight, TileMap map, Camera camera, Cursor cursor, BaseUnit[] testUnits, BaseUnit currentUnit, BaseUnit targetUnit, SpriteFont font, SpriteFont font2)
            {
                //spriteBatch.Begin();

                switch (mCurrentScreen)
                {
                    case Screen.Title:
                        {
                            spriteBatch.Begin();
                            spriteBatch.Draw(mTitleScreen, new Rectangle(0, 0, winWidth, winHeight), Color.White);
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
                            drawActiveInformation(spriteBatch, currentUnit, winHeight, winWidth, font);
                            break;
                        }

                    case Screen.Menu:
                        {
                            map.draw(spriteBatch, camera);
                            spriteBatch.Begin();
                            //spriteBatch.Draw(mMainScreen, new Rectangle(0, 0, this.Window.ClientBounds.Width, this.Window.ClientBounds.Height), Color.Gray);
                            
                            spriteBatch.Draw(mMenu, new Rectangle(winWidth / 2 - mMenu.Width / 2, winHeight / 2 - mMenu.Height / 2, mMenu.Width, mMenu.Height), Color.White);

                            switch (mCurrentMenuOption)
                            {
                                case MenuOptions.Resume:
                                    {
                                        spriteBatch.DrawString(font2, "Main Menu", new Vector2(winWidth / 2 - 50, winHeight / 2 - mMenu.Height / 2 + 75), Color.Red);
                                        spriteBatch.DrawString(font, "Resume", new Vector2(winWidth / 2 - 50, winHeight / 2 - mMenu.Height / 2 + 150), Color.Gold);
                                        spriteBatch.DrawString(font, "Exit", new Vector2(winWidth / 2 - 50, winHeight / 2 - mMenu.Height / 2 + 250), Color.White);
                                        break;
                                    }

                                case MenuOptions.ExitGame:
                                    {
                                        spriteBatch.DrawString(font2, "Main Menu", new Vector2(winWidth / 2 - 50, winHeight / 2 - mMenu.Height / 2 + 75), Color.Red);
                                        spriteBatch.DrawString(font, "Resume", new Vector2(winWidth / 2 - 50, winHeight / 2 - mMenu.Height / 2 + 150), Color.White);
                                        spriteBatch.DrawString(font, "Exit", new Vector2(winWidth / 2 - 50, winHeight / 2 - mMenu.Height / 2 + 250), Color.Gold);
                                        break;
                                    }
                            }
                            spriteBatch.End();
                            break;
                        }
                }
            }

            private void drawActiveInformation(SpriteBatch spriteBatch, BaseUnit active, int winHeight, int winWidth, SpriteFont font)
            {
                String name = "NAME";
                String fac = "FACTION";
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
                int xOffset = 5;
                int yOffset = 5;

                spriteBatch.Draw(mActive, new Rectangle(c1, c2, c3, c4), Color.White);

                spriteBatch.DrawString(font, "Name : " + name, new Vector2(c1 + xOffset, c2 + yOffset), Color.Black);
                spriteBatch.DrawString(font, "Faction : " + fac, new Vector2(c1 + xOffset, c2 + yOffset * 5), Color.Black);
                spriteBatch.DrawString(font, "HP : " + currHP + "/" + maxHP, new Vector2(c1 + xOffset, c2 + yOffset * 10), Color.Black);
                spriteBatch.DrawString(font, "AP : " + currAP + "/" + maxAP, new Vector2(c1 + xOffset, c2 + yOffset * 15), Color.Black);
                spriteBatch.DrawString(font, "MP : " + currMP + "/" + maxMP, new Vector2(c1 + xOffset, c2 + yOffset * 20), Color.Black);

                spriteBatch.End();
            }
    }
}
