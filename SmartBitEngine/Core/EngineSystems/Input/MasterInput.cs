
using System;
using System.Data;
using OpenTK;
using OpenTK.Input;
using Vector2 = System.Numerics.Vector2;

namespace DumBitEngine.Core.Util
{
    public static class MasterInput
    {
        private static bool[] keyPressed;
        private static bool[] keyPressedLastFrame;

        private static bool[] keyDown;
        private static bool[] keyDownCanBeActivated;
        
        private const int MAX_KEY_SIZE = (int) Key.NonUSBackSlash;

        private static KeyboardState keyboardState;
        private static KeyboardState keyboardStateLastFrame;

        private static MouseState mouseState;

        private static float lastMouseWheelPosition;
        private static float mouseWheelDelta;
        
        public static Vector2 MousePosition => new Vector2(mouseState.X, mouseState.Y);
        public static float MouseScroll => mouseWheelDelta;
        public static bool IsLeftMouseButtonDown => mouseState[0];

        public static void Init()
        {
            keyPressed = new bool[MAX_KEY_SIZE];
            keyDown = new bool[MAX_KEY_SIZE];
            keyPressedLastFrame = new bool[MAX_KEY_SIZE];
            keyDownCanBeActivated = new bool[MAX_KEY_SIZE];

            //by default, all keys can activated for one time press
            for (int i = 0; i < MAX_KEY_SIZE; i++)
            {
                keyDownCanBeActivated[i] = true;
            }

            keyboardStateLastFrame = Keyboard.GetState();
            
            lastMouseWheelPosition = 0.0f;
        }

        /// <summary>
        /// Update the state of the keyDown and keyPressed buffer
        /// </summary>
        public static void Update()
        {
            keyboardState = Keyboard.GetState();

            mouseWheelDelta = lastMouseWheelPosition - mouseState.WheelPrecise;
            lastMouseWheelPosition = mouseState.WheelPrecise;

            for (int i = 0; i < MAX_KEY_SIZE; i++)
            {
                keyPressed[i] = keyboardState.IsKeyDown((Key) i);

                if (keyPressed[i] == false)
                {
                    keyDown[i] = false;
                    keyDownCanBeActivated[i] = true;
                }
                else if (keyPressed[i])
                {
                    if(keyDown[i] == false && keyDownCanBeActivated[i])
                    {
                        keyDown[i] = true;
                        keyDownCanBeActivated[i] = false;
                    }
                    else if (keyDown[i])
                    {
                        keyDown[i] = false;
                    }
                }
            }
        }

        /// <summary>
        /// Checks if the key was pressed
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static bool IsKeyPressed(Key key)
        {
            return keyPressed[(int) key];
        }

        /// <summary>
        /// Checks if the key is down
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static bool IsKeyDown(Key key)
        {
            return keyDown[(int) key];
        }

        /// <summary>
        /// Retrieves the first key pressed during this frame
        /// </summary>
        /// <param name="key"></param>
        /// <returns>The key pressed during the frame</returns>
        public static bool GetKeyPressed(out Key key)
        {
            if (keyboardState.IsAnyKeyDown)
            {
                for (int i = 0; i < MAX_KEY_SIZE; i++)
                {
                    if (keyDown[i])
                    {
                        key = (Key) i;
                        return true;
                    }
                }
            }
            key = Key.A;
            return false;
        }

        public static void MouseEvent(MouseEventArgs args)
        {
            mouseState = args.Mouse;
        }
    }
}