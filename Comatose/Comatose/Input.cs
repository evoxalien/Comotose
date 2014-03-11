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

namespace Comatose
{
    public class Input
    {
        private KeyboardState keyboardState, lastKeyboardState;
        private MouseState mouseState, lastMouseState;
        private GamePadState gamepadState, lastGamepadState;
        public bool GamePause = false;
        public bool DevMode = false;

        private bool isAimingWithMouse = false;

        public Vector2 MousePosition { get { return new Vector2(mouseState.X, mouseState.Y); } }

        #region WasKeyOrButtonPressedOrReleased
        public bool WasKeyPressed(string key_string)
        {
            Keys key = (Keys) Keys.Parse(typeof(Keys), key_string, true);
            return lastKeyboardState.IsKeyUp(key) && keyboardState.IsKeyDown(key);
        }

        public bool WasButtonPressed(string button_string)
        {
            Buttons button = (Buttons)Buttons.Parse(typeof(Buttons), button_string, true);
            return lastGamepadState.IsButtonUp(button) && gamepadState.IsButtonDown(button);
        }

        public bool WasKeyReleased(string key_string)
        {
            Keys key = (Keys)Keys.Parse(typeof(Keys), key_string, true);
            return lastKeyboardState.IsKeyDown(key) && keyboardState.IsKeyUp(key);
        }

        public bool WasButtonReleased(string button_string)
        {
            Buttons button = (Buttons)Buttons.Parse(typeof(Buttons), button_string, true);
            return lastGamepadState.IsButtonDown(button) && gamepadState.IsButtonUp(button);
        }
        #endregion

        #region Movement
        public Vector2 GetMovementDirection()
        {
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
        #endregion

        public bool MovementDeadzone()
        {
            return !(gamepadState.ThumbSticks.Left.Length() > 0.1) && keyboardState.IsKeyUp(Keys.W) && keyboardState.IsKeyUp(Keys.A) && keyboardState.IsKeyUp(Keys.S) && keyboardState.IsKeyUp(Keys.D);
        }

        #region Aiming
        public Vector2 GetAimDirection(Vector2 origin)
        {

            if (isAimingWithMouse)
                if (mouseState.LeftButton == ButtonState.Pressed)
                    return GetMouseAimDirection(origin);

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

        public Vector2 GetMouseAimDirection(Vector2 origin)
        {
            Vector2 direction = MousePosition - origin;

            if (direction == Vector2.Zero)
                return Vector2.Zero;
            else
                return Vector2.Normalize(direction);
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

            if (new[] { Keys.Left, Keys.Right, Keys.Up, Keys.Down }.Any(x => keyboardState.IsKeyDown(x)) || gamepadState.ThumbSticks.Right != Vector2.Zero)
                isAimingWithMouse = false;
            else if (MousePosition != new Vector2(lastMouseState.X, lastMouseState.Y))
                isAimingWithMouse = true;

            GamePausePressed();
            DevModeButtonPressed();
        }
        #endregion
    }
}
