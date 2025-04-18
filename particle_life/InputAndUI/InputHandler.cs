using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Input;

namespace ParticleLifeSim
{
    public enum Trigger
    {
        Never,
        KeyPressed,
        KeyReleased,
        KeyDown,
        KeyUp
    }
    public struct OnClickAction(Action action, Trigger trigger = Trigger.KeyPressed)
    {
        public Action Action = action;
        public Trigger Trigger = trigger;
    }

    public class InputHandler()
    {
        private Dictionary<Keys, OnClickAction> _keyActions = [];
        // private MouseState _previousMouseState;
        private KeyboardState _previousKeyboardState = Keyboard.GetState();

        public void AddKeyAction(Keys key, OnClickAction onClickAction)
        {
            if (!_keyActions.ContainsKey(key))
            {
                _keyActions.Add(key, onClickAction);
            }
            else
            {
                _keyActions[key] = new OnClickAction(
                    onClickAction.Action + _keyActions[key].Action,
                    onClickAction.Trigger
                );
            }
        }
        public void AddKeyAction(Keys key, Action action, Trigger trigger = Trigger.KeyPressed)
        {
            if (!_keyActions.ContainsKey(key))
            {
                _keyActions.Add(key, new OnClickAction(action, trigger));
            }
            else
            {
                _keyActions[key] = new OnClickAction(
                    action + _keyActions[key].Action,
                    trigger
                );
            }
        }

        public void RemoveKeyAction(Keys key)
        {
            if (_keyActions.ContainsKey(key))
                _keyActions.Remove(key);
        }

        public void ClearKeyActions()
        {
            _keyActions.Clear();
        }

        public void Update()
        {
            var keyboardState = Keyboard.GetState();

            foreach (var action in _keyActions)
            {
                Action invoke = action.Value.Action.Invoke;

                switch (action.Value.Trigger)
                {
                    case Trigger.KeyPressed:
                        if (IsKeyPressed(action.Key)) invoke();
                        break;

                    case Trigger.KeyReleased:
                        if (IsKeyReleased(action.Key)) invoke();
                        break;

                    case Trigger.KeyDown:
                        if (keyboardState.IsKeyDown(action.Key)) invoke();
                        break;

                    case Trigger.KeyUp:
                        if (!keyboardState.IsKeyDown(action.Key)) invoke();
                        break;

                    case Trigger.Never:
                    default: break;
                }
            }

            _previousKeyboardState = keyboardState;
        }

        // Helper methods for common checks
        public bool IsKeyPressed(Keys key)
        {
            return Keyboard.GetState().IsKeyDown(key) &&
                   !_previousKeyboardState.IsKeyDown(key);
        }

        public bool IsKeyReleased(Keys key)
        {
            return !Keyboard.GetState().IsKeyDown(key) &&
                   _previousKeyboardState.IsKeyDown(key);
        }
    }
}
