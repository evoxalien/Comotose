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
using Box2D.XNA;

namespace Comatose
{
    public class Input
    {
        private ComatoseGame game;
        private KeyboardState keyboardState, lastKeyboardState;
        private MouseState mouseState, lastMouseState;
        private GamePadState gamepadState, lastGamepadState;
        public bool GamePause = false;
        public bool DevMode = false;

        public Input(ComatoseGame game)
        {
            this.game = game;
        }

        private bool isAimingWithMouse = false;

        public Vector2 MousePosition { get { return new Vector2(mouseState.X, mouseState.Y); } }

        #region WasKeyOrButtonPressedOrReleased
        public bool WasKeyPressed(string key_string)
        {
            if (game.console.Opened)
                return false;
            Keys key = (Keys) Keys.Parse(typeof(Keys), key_string, true);
            return lastKeyboardState.IsKeyUp(key) && keyboardState.IsKeyDown(key);
        }

        public bool WasButtonPressed(string button_string)
        {
            if (game.console.Opened)
                return false;
            Buttons button = (Buttons)Buttons.Parse(typeof(Buttons), button_string, true);
            return lastGamepadState.IsButtonUp(button) && gamepadState.IsButtonDown(button);
        }

        public bool WasKeyReleased(string key_string)
        {
            if (game.console.Opened)
                return false;
            Keys key = (Keys)Keys.Parse(typeof(Keys), key_string, true);
            return lastKeyboardState.IsKeyDown(key) && keyboardState.IsKeyUp(key);
        }

        public bool IsKeyHeld(string key_string)
        {
            if (game.console.Opened)
                return false;
            Keys key = (Keys)Keys.Parse(typeof(Keys), key_string, true);
            return lastKeyboardState.IsKeyDown(key) && keyboardState.IsKeyDown(key);
        }

        public bool WasButtonReleased(string button_string)
        {
            if (game.console.Opened)
                return false;
            Buttons button = (Buttons)Buttons.Parse(typeof(Buttons), button_string, true);
            return lastGamepadState.IsButtonDown(button) && gamepadState.IsButtonUp(button);
        }
        #endregion

        #region Movement
        public Vector2 GetMovementDirection()
        {
            if (game.console.Opened)
                return new Vector2(0);

            Vector2 direction = gamepadState.ThumbSticks.Left;
            direction.Y *= -1; //invert the y-axis

            if (keyboardState.IsKeyDown(Keys.A))
                direction.X -= 1;
            if (keyboardState.IsKeyDown(Keys.D))
                direction.X += 1;
            if (keyboardState.IsKeyDown(Keys.W))
                direction.Y -= 1;
            if (keyboardState.IsKeyDown(Keys.S))
                direction.Y += 1;

            //Clamp the length of the vector to a maximum of 1.
            if (direction.LengthSquared() > 1)
                direction.Normalize();

            return direction;
        }

        public bool MovementDeadzone()
        {
            return !(gamepadState.ThumbSticks.Left.Length() > 0.1) && keyboardState.IsKeyUp(Keys.W) && keyboardState.IsKeyUp(Keys.A) && keyboardState.IsKeyUp(Keys.S) && keyboardState.IsKeyUp(Keys.D);
        }
        #endregion

        private Vector2 aim_center = new Vector2(0);
        public void setAimCenter(float x, float y)
        {
            aim_center = new Vector2(x * game.physics_scale - game.camera.X, y * game.physics_scale - game.camera.Y);
        }

        #region Aiming
        public Vector2 GetAimDirection()
        {
            if (game.console.Opened)
                return new Vector2(0);
            if (mouseState.LeftButton == ButtonState.Pressed || mouseState.RightButton == ButtonState.Pressed)
                return GetMouseAimDirection();

            Vector2 direction = gamepadState.ThumbSticks.Right;
            direction.Y *= -1;

            if (keyboardState.IsKeyDown(Keys.Left))
                direction.X -= 1;
            if (keyboardState.IsKeyDown(Keys.Right))
                direction.X += 1;
            if (keyboardState.IsKeyDown(Keys.Up))
                direction.Y -= 1;
            if (keyboardState.IsKeyDown(Keys.Down))
                direction.Y += 1;

            //If there's no aim input, return zero. Otherwise normalize the direction to have a length of 1.

            if (direction == Vector2.Zero)
                return Vector2.Zero;
            else
                return Vector2.Normalize(direction);
        }

        public Vector2 GetMouseAimDirection()
        {
            Vector2 direction = MousePosition - aim_center;

            if (direction == Vector2.Zero)
                return Vector2.Zero;
            else
                return Vector2.Normalize(direction);
        }

        public bool AimingDeadzone()
        {
            return !isAimingWithMouse && !(gamepadState.ThumbSticks.Right.Length() > 0.1) && keyboardState.IsKeyUp(Keys.Up) && keyboardState.IsKeyUp(Keys.Down) && keyboardState.IsKeyUp(Keys.Left) && keyboardState.IsKeyUp(Keys.Right);
        }

        public Vector2 GetMousePosition()
        {
            return new Vector2((float)mouseState.X + game.camera.X, (float)mouseState.Y + game.camera.Y);
        }
        #endregion

        #region GamePause
        public void GamePausePressed()
        {
            if (WasButtonPressed("Start") || WasKeyPressed("P"))
            {
                GamePause = !GamePause;
            }
        }
        #endregion

        public void DevModeButtonPressed()
        {
            if (WasButtonPressed("LeftShoulder") || WasKeyPressed("F3"))
                DevMode = !DevMode;
        }

        private void handleMouseClicks()
        {
            Vector2 mouse_position = GetMousePosition();
            Vector2 transformed_mouse = new Vector2((mouse_position.X / game.physics_scale), (mouse_position.Y / game.physics_scale));
            game.vm.DoString("mouse.x = " + transformed_mouse.X);
            game.vm.DoString("mouse.y = " + transformed_mouse.Y);

            List<PhysicsObject> objectsToHandle = new List<PhysicsObject>();
            if (mouseState.LeftButton == ButtonState.Pressed && lastMouseState.LeftButton == ButtonState.Released)
            {
                foreach (var o in game.game_objects)
                {
                    if (o.Value is PhysicsObject) {
                        PhysicsObject physics_object = (PhysicsObject)o.Value;
                        Fixture thing = physics_object.body.GetFixtureList();
                        while (thing != null)
                        {
                            if (thing.TestPoint(transformed_mouse))
                                objectsToHandle.Add(physics_object);
                            thing = thing.GetNext();
                        }
                    }
                }
                game.vm.DoString("if stage.click then stage.click(mouse.x, mouse.y) end");
                foreach (var o in objectsToHandle)
                {
                    game.vm.DoString("if objects[" + o.ID() + "].click then objects[" + o.ID() + "]:click(mouse.x - " + o.x + ", mouse.y - " + o.y + ") end");
                }
            }

            if (mouseState.RightButton == ButtonState.Pressed && lastMouseState.RightButton == ButtonState.Released)
            {
                foreach (var o in game.game_objects)
                {
                    if (o.Value is PhysicsObject) {
                        PhysicsObject physics_object = (PhysicsObject)o.Value;
                        Fixture thing = physics_object.body.GetFixtureList();
                        while (thing != null)
                        {
                            if (thing.TestPoint(transformed_mouse))
                                game.vm.DoString("if objects[" + physics_object.ID() + "].right_click then objects[" + physics_object.ID() + "]:right_click(mouse.x - " + physics_object.x + ", mouse.y - " + physics_object.y + ") end");
                            
                            thing = thing.GetNext();
                        }
                    }
                }
                game.vm.DoString("if stage.right_click then stage.right_click(mouse.x, mouse.y) end");
            }

            if (mouseState.ScrollWheelValue < lastMouseState.ScrollWheelValue) {
                game.vm.DoString("processEvent('scroll_down')");
            }

            if (mouseState.ScrollWheelValue > lastMouseState.ScrollWheelValue) {
                game.vm.DoString("processEvent('scroll_up')");
            }

            //scroll click
            if (mouseState.MiddleButton == ButtonState.Pressed && lastMouseState.MiddleButton == ButtonState.Released) {
                game.vm.DoString("processEvent('scroll_click')");
            }
        }

        #region Update
        public void Update()
        {
            lastKeyboardState = keyboardState;
            lastMouseState = mouseState;
            lastGamepadState = gamepadState;

            keyboardState = Keyboard.GetState();
            mouseState = Mouse.GetState();
            gamepadState = GamePad.GetState(PlayerIndex.One);

            // If the player pressed one of the arrow keys or is using a gamepad to aim, we want to disable mouse aiming. Otherwise,
            // if the player moves the mouse, enable mouse aiming.

            /*
            if (new[] { Keys.Left, Keys.Right, Keys.Up, Keys.Down }.Any(x => keyboardState.IsKeyDown(x)) || gamepadState.ThumbSticks.Right != Vector2.Zero)
                isAimingWithMouse = false;
            else if (MousePosition != new Vector2(lastMouseState.X, lastMouseState.Y))
                isAimingWithMouse = true;
             */

            isAimingWithMouse = (mouseState.LeftButton == ButtonState.Pressed || mouseState.RightButton == ButtonState.Pressed);

            GamePausePressed();
            DevModeButtonPressed();
            handleMouseClicks();
        }
        #endregion
    }
}
