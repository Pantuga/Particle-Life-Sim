using System;
using System.Collections.Generic;
using System.DirectoryServices.ActiveDirectory;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace particle_life
{
    public enum ColorIndex
    {
        Red,
        Green,
        Blue,
        Yellow,
        Purple,
    }

    public class Particle (Vector2 pos, Vector2 vel, ColorIndex color)
    {
        public const float DISTANCE_MIN = 6f;
        public const float DISTANCE_MAX = 300f;

        public Vector2 Position = pos;
        public Vector2 Velocity = vel;
        public Vector2 Acceleration = Vector2.Zero;
        public ColorIndex Color = color;

        static float CalculateForce(float distance, float maxForce)
        {
            const float DISTANCE_MED = (DISTANCE_MIN + DISTANCE_MAX) / 2f;

            if (distance <= DISTANCE_MIN)
                return 2 * (distance - 2 * DISTANCE_MIN);
            else if (distance <= DISTANCE_MED)
                return (distance - DISTANCE_MIN) * maxForce / (DISTANCE_MED - DISTANCE_MIN);
            else if (distance <= DISTANCE_MAX)
                return maxForce - (distance - DISTANCE_MED) * maxForce / (DISTANCE_MAX - DISTANCE_MED);
            else
                return 0f;
        }

        public void UpdForces(List<Particle> particles, float[][] atractionMatrix)
        {
            Vector2 finalForce = Vector2.Zero;
            foreach (var p in particles)
            {
                if (p.Position == this.Position) continue; // check if 'p == this' and avoid division by 0 if not

                Vector2 direction = Vector2.Normalize(p.Position - this.Position);
                float distance = (p.Position - this.Position).Length();
                float maxForce = atractionMatrix[(int)this.Color][(int)p.Color];
                float force = CalculateForce(distance, maxForce);

                finalForce += force * direction;
            }
            Acceleration = finalForce;
        }

        public void UpdMov(GameTime gametime)
        {
            float deltaTime = (float)gametime.ElapsedGameTime.TotalSeconds;

            Velocity += Acceleration * deltaTime;
            Velocity *= 1f - Game1.FRICTION;

            if (Velocity.Y > Game1.MAX_VELOCITY) Velocity.Y = Game1.MAX_VELOCITY;
            if (Velocity.X > Game1.MAX_VELOCITY) Velocity.X = Game1.MAX_VELOCITY;

            Position += Velocity * deltaTime;

            if (Position.X > Game1.SCREEN_SIZE.X) {
                Position.X = Game1.SCREEN_SIZE.X;
                Velocity.X *= -1f;
            }
            if (Position.Y > Game1.SCREEN_SIZE.Y) {
                Position.Y = Game1.SCREEN_SIZE.Y;
                Velocity.Y *= -1f;
            }
            if (Position.X < 0)
            {
                Position.X = 0;
                Velocity.X *= -1f;
            }
            if (Position.Y < 0)
            {
                Position.Y = 0;
                Velocity.Y *= -1f;
            }
        }

        public void Draw (SpriteBatch spriteBatch, Texture2D texture, Color color)
        {
            spriteBatch.Draw(
                texture,
                Position,
                null,
                color,
                0f,
                new Vector2(texture.Width / 2, texture.Height / 2),
                Vector2.One,
                SpriteEffects.None,
                0f
            );
        }
    }
    public class Game1 : Game
    {
        private readonly Dictionary<ColorIndex, Color> ParticleColors = [];

        public const float MAX_VELOCITY = 500f;
        public const float FRICTION = 0.01f;
        public static readonly Vector2 SCREEN_SIZE = new(1200, 600);

        private readonly GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        public static readonly Random rnd = new();

        private readonly float[][] _atractionMatrix;
        private List<Particle> _particles = [];
        private Texture2D _particleTexture;

        private float[][] RandomMatrix(float maxForce = 10f)
        {
            float[][] matrix = new float[5][];
            Random rnd = new Random();

            int i;
            for (i = 0; i < 5; i++)
            {
                matrix[i] = new float[5];
                for (int j = 0; j < 5; j++)
                {
                    // Same-color particles always attract (positive force)
                    if (i == j)
                    {
                        matrix[i][j] = (float)rnd.NextDouble() * maxForce; // 0 to maxForce
                    }
                    // Different colors: random attraction/repulsion
                    else
                    {
                        matrix[i][j] = (float)(rnd.NextDouble() * 2 * maxForce - maxForce); // -maxForce to +maxForce
                    }
                }
            }

            return matrix;
        }
        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this)
            {
                PreferredBackBufferWidth = (int)SCREEN_SIZE.X,
                PreferredBackBufferHeight = (int)SCREEN_SIZE.Y
            };
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            ParticleColors.Add(ColorIndex.Red, new Color(255, 10, 15));
            ParticleColors.Add(ColorIndex.Green, new Color(10, 255, 15));
            ParticleColors.Add(ColorIndex.Blue, new Color(15, 10, 255));
            ParticleColors.Add(ColorIndex.Yellow, new Color(255, 255, 10));
            ParticleColors.Add(ColorIndex.Purple, new Color(255, 10, 255));

            /*
            _atractionMatrix = [
                [10f, -5f, 0f, 0f, 0f],
                [0f, 10f, -5f, 0f, 0f],
                [0f, 0f, 10f, -5f, 0f],
                [0f, 0f, 0f, 10f, -5f],
                [-5f, 0f, 0f, 0f, 10f],
            ];*/

            _atractionMatrix = RandomMatrix();
        }

        protected override void Initialize()
        {
            for (int i = 0; i < 200; i++)
            {
                Vector2 randomPos = new(rnd.Next(0, (int)SCREEN_SIZE.X), rnd.Next(0, (int)SCREEN_SIZE.Y));
                Vector2 randomVel = new(0); //new(rnd.Next(-500, 500), rnd.Next(-500, 500));
                ColorIndex randomColor = (ColorIndex)rnd.Next(0,5);
                _particles.Add(new Particle(randomPos, randomVel, randomColor));
            }

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            _particleTexture = Content.Load<Texture2D>("particle");
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            _particles.ForEach(p => p.UpdForces(_particles, _atractionMatrix));
            _particles.ForEach(p => p.UpdMov(gameTime));

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(new Color(20, 20, 20));

            _spriteBatch.Begin();

            _particles.ForEach(p => p.Draw(_spriteBatch, _particleTexture, ParticleColors[p.Color]));

            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
