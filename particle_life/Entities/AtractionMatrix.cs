using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ParticleLifeSim
{
    public struct TableElement(Vector2 pos, string val, Color color)
    {
        public Vector2 Position = pos;
        public string Value = val;
        public Color Color = color;
    }

    public class AtractionMatrix
    {
        private float[][] _matrix;
        private HashSet<TableElement> _renderedMatrix = null;

        public AtractionMatrix(float[][] matrix)
        {
            _matrix = matrix;
        }
        public AtractionMatrix()
        {
            _matrix = RandomMatrix(10f);
        }

        private static float[][] RandomMatrix(float maxForce)
        {
            float[][] matrix = new float[5][];
            Random rnd = new();

            int i;
            for (i = 0; i < 5; i++)
            {
                matrix[i] = new float[5];
                for (int j = 0; j < 5; j++)
                {
                    matrix[i][j] = (float)Math.Round((rnd.NextDouble() * 2 * maxForce - maxForce) * 10) / 10; // -maxForce to +maxForce
                }
            }

            return matrix;
        }

        private void RenderTable(Vector2 pos)
        {
            _renderedMatrix = [];

            string[] colors = ["Red", "Green", "Blue", "Yellow", "Purple"];

            for (int i = 0; i < colors.Length; i++)
            {
                _renderedMatrix.Add(new TableElement(
                    pos + new Vector2((i + 1) * 50, 0),
                    colors[i],
                    Color.White
                ));
                _renderedMatrix.Add(new TableElement(
                    pos + new Vector2(0, (i + 1) * 20),
                    colors[i],
                    Color.White
                ));
            }

            for (int i = 0; i < _matrix.Length; i++)
            {
                for (int j = 0; j < _matrix[i].Length; j++)
                {
                    float atractionPerCent = (_matrix[i][j] + 5) / 10;

                    _renderedMatrix.Add(new TableElement(
                        pos + new Vector2((j + 1) * 50, (i + 1) * 20),
                        _matrix[i][j].ToString(),
                        new Color(
                            2 - 2 * atractionPerCent,
                            2 * atractionPerCent,
                            0f
                        )
                    ));
                }
            }
        }

        public void DrawMatrix(SpriteBatch spriteBatch, Vector2 pos, SpriteFont font)
        {
            if (_renderedMatrix == null) RenderTable(pos);

            foreach (var element in _renderedMatrix)
                spriteBatch.DrawString(
                    font,
                    element.Value,
                    element.Position,
                    element.Color
                );
        }
        public float[][] Get() { return _matrix; }

    }
}
