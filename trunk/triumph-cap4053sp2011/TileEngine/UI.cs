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
            }
            
            public void Update(GameTime gameTime, KeyboardState aKeyboardState, BaseUnit currentUnit, Cursor cursor, TileMap map, int counter, TurnManager turnManager, int screenWidth, int screenHeight, BaseUnit testUnit, BaseUnit testUnit2, Camera camera)
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
                                currentUnit.isDone = true;
                            }
                            counter--;
                            //checks if a unit has finsihed its turn, if it has then make the next unit the active unit
                            if (currentUnit.isDone)
                            {
                                counter = 100;
                                currentUnit.delay += currentUnit.SPD;
                                currentUnit.isDone = false;
                                turnManager.add(currentUnit);
                                currentUnit = turnManager.getNext();
                                cursor.location = currentUnit.position;
                            }

                            // TODO: Add your update logic here 
                            //int screenWidth = GraphicsDevice.Viewport.Width;
                            //int screenHeight = GraphicsDevice.Viewport.Height;

                            testUnit.update(gameTime, screenWidth, screenHeight, map);
                            testUnit2.update(gameTime, screenWidth, screenHeight, map);
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
            public void Draw(GameTime gameTime, SpriteBatch spriteBatch, int winWidth, int winHeight, TileMap map, Camera camera, Cursor cursor, BaseUnit testUnit, BaseUnit testUnit2, SpriteFont font, SpriteFont font2)
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
                            testUnit.draw(spriteBatch, camera);
                            testUnit2.draw(spriteBatch, camera);
                            break;
                        }

                    case Screen.Menu:
                        {
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
    }
}
