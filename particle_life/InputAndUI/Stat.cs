using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ParticleLifeSim
{
    public class Stat<ValType>
    {
        public ValType Value { get; protected set; }

        public Stat(ValType initialValue)
        {
            Value = initialValue;
        }

        public void DrawStat(SpriteBatch spriteBatch, SpriteFont font, Vector2 pos, string name, string[] notes)
        {
            spriteBatch.DrawString(font, name + ": " + Value, pos, Color.White);

            for (int i = 0; i < notes.Length; i++)
            {
                spriteBatch.DrawString(font, notes[i], pos + new Vector2(0, (i + 1) * 20), Color.Gray);
            }
        }
    }

    public class DiscreteStat<ValType> : Stat<ValType>
    {
        private readonly ValType[] _steps;
        private int _index;

        public DiscreteStat(ValType[] steps, int initialIndex = 0) : base(steps[initialIndex])
        {
            if (steps == null || steps.Length == 0)
                throw new ArgumentException("Steps array cannot be null or empty");

            _steps = steps;
            _index = initialIndex;
            Value = steps[initialIndex]; // Set initial value
        }

        private void UpdateValue() { Value = _steps[_index]; }

        public void StepForward(int step = 1)
        {
            if (_index < _steps.Length - 1)
            {
                _index += step;
                UpdateValue();
            }
        }

        public void StepBackward(int step = 1)
        {
            if (_index > 0)
            {
                _index -= step;
                UpdateValue();
            }
        }

        public void ToStep(int index)
        {
            _index = index;
            UpdateValue();
        }
    }

    public class ContinuousStat(float initValue, float defaultStep) : Stat<float>(initValue)
    {
        private float _defaultStep = defaultStep;

        public void Increment(float increment = 0)
        {
            if (increment != 0) Value += increment;
            else Value += _defaultStep;
        }

        public void Decrement(float decrement = 0)
        {
            if (decrement != 0) Value -= decrement;
            else Value -= _defaultStep;
        }
    }

    public class BoolStat(bool initValue) : Stat<bool>(initValue)
    {
        public void Toggle() { Value = !Value; }
    }
}