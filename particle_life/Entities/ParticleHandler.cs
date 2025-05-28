using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using static ParticleLifeSim.Game1;

namespace ParticleLifeSim
{
    public class ParticleHandler
    {
        public List<Particle> Particles { get; } = [];
        public Dictionary<ColorIndex, Color> ColorMap = [];
        public Texture2D Texture;
        public ParticleProperties ParticleProperties = new();

        private AtractionMatrix _atractionMatrix;

        public ParticleHandler()
        {
            ColorMap.Add(ColorIndex.Red, new Color(255, 10, 15));
            ColorMap.Add(ColorIndex.Green, new Color(10, 255, 15));
            ColorMap.Add(ColorIndex.Blue, new Color(15, 10, 255));
            ColorMap.Add(ColorIndex.Yellow, new Color(255, 255, 10));
            ColorMap.Add(ColorIndex.Purple, new Color(255, 10, 255));
        }

        public void NewMatrix()
        {
            _atractionMatrix = new();
        }
        public void NewMatrix(float[][] matrix)
        {
            _atractionMatrix = new(matrix);
        }
        public void GenParticles(int count)
        {
            for (int i = 0; i < count; i++)
            {
                Vector2 randomPos = new(
                    rnd.Next((int)SCREEN_OFFSET_LEFT.X, (int)(SCREEN_SIZE.X + SCREEN_OFFSET_LEFT.X)),
                    rnd.Next((int)SCREEN_OFFSET_LEFT.Y, (int)(SCREEN_SIZE.Y + SCREEN_OFFSET_LEFT.Y))
                );
                Vector2 randomVel = new(0); //new(rnd.Next(-500, 500), rnd.Next(-500, 500)
                ColorIndex randomColor = (ColorIndex)rnd.Next(0, 5);
                Particles.Add(new Particle(randomPos, randomVel, randomColor));
            }
        }
        public void Update(GameTime gameTime)
        {
            Parallel.For(0, Particles.Count, i =>
                Particles[i].UpdForces(Particles, _atractionMatrix.Get(), ParticleProperties)
            );

            Parallel.For(0, Particles.Count, i =>
                Particles[i].UpdMov(gameTime, ParticleProperties)
            );
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            Particles.ForEach(p => p.Draw(spriteBatch, Texture, ColorMap[p.Color]));
        }
        public void DrawMatrix(SpriteBatch spriteBatch, Vector2 pos, SpriteFont font)
        {
            _atractionMatrix.DrawMatrix(spriteBatch, pos, font);
        }
    }
}
