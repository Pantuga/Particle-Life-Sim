using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ParticleLifeSim
{
    public class Stat<ValType>
    {
        public ValType Value { get; protected set; }
        protected readonly string Unit;

        public Stat(ValType initialValue, string unit = "")
        {
            Value = initialValue;
            Unit = unit;
        }

        public void DrawStat(SpriteBatch spriteBatch, SpriteFont font, Vector2 pos, string name, string[] notes, string warning = "")
        {
            if (warning == "")
                spriteBatch.DrawString(font, name + ": " + Value +" "+ Unit, pos, Color.White);
            else
                spriteBatch.DrawString(font, name + ": " + Value + " " + Unit, pos, Color.Yellow);

            for (int i = 0; i < notes.Length; i++)
            {
                spriteBatch.DrawString(font, notes[i], pos + new Vector2(0, (i + 1) * 20), Color.Gray);
            }

            if (warning != "")
                spriteBatch.DrawString(font,$"Warning: " + warning, pos + new Vector2(0, (notes.Length + 1) * 20), Color.Yellow);
        }

        public virtual void Set(ValType value)
        {
            Value = value;
        }
    }

    public class DiscreteStat<ValType> : Stat<ValType>
    {
        private readonly ValType[] _steps;
        private int _index;

        public DiscreteStat(ValType[] steps, int initialIndex = 0, string unit = "") : base(steps[initialIndex], unit)
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

        override public void Set(ValType value)
        {
            if (_steps.Contains(value))
                _index = Array.IndexOf(_steps, value);
            else
                throw new NullReferenceException($"the value of {this} cannot be set to {value} " +
                    "as it is not in the allowed discrete steps for this Stat object.\n" +
                    "Steps are:" + 
                    (string () => {
                        string outStr = "[ ";
                        foreach (var step in _steps)
                            outStr += step + ", ";
                        outStr += "]"; return outStr;
                    })
                );

            UpdateValue();
        }
    }

    public class ContinuousStat(float initValue, float defaultStep, string unit = "") : Stat<float>(initValue, unit)
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