using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace ParticleLifeSim
{
    public enum ColorIndex
    {
        Red,
        Green,
        Blue,
        Yellow,
        Purple
    }

    public class Game1 : Game
    {
        public static readonly DiscreteStat<float> TimeScale = new([0.2f, 0.5f, 1f, 1.2f, 1.5f, 2f, 3f, 5f, 10f, 20f], 2);
        public static readonly BoolStat IsPaused = new(false);

        public static readonly Vector2 SCREEN_SIZE = new(900, 600);
        public static readonly Vector2 SCREEN_OFFSET_LEFT = new(300, 0);
        public static readonly Vector2 SCREEN_OFFSET_RIGHT = new();

        private readonly GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private SpriteFont _fontArial;
        // private SpriteFont _fontCourierNew;

        public static readonly Random rnd = new();

        private InputHandler _inputHandler = new();
        private ParticleHandler _particleHandler = new();

        private Texture2D _buttonTexture;
        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this)
            {
                PreferredBackBufferWidth = (int)(SCREEN_OFFSET_LEFT.X + SCREEN_SIZE.X + SCREEN_OFFSET_RIGHT.X),
                PreferredBackBufferHeight = (int)(SCREEN_OFFSET_LEFT.Y + SCREEN_SIZE.Y + SCREEN_OFFSET_RIGHT.Y)
            };
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        private void SetupControls()
        {
            _inputHandler.AddKeyAction(Keys.Q, () => TimeScale.StepForward());
            _inputHandler.AddKeyAction(Keys.A, () => TimeScale.StepBackward());
            _inputHandler.AddKeyAction(Keys.Z, () => TimeScale.ToStep(2));

            _inputHandler.AddKeyAction(Keys.Space, () => IsPaused.Toggle());
        }

        protected override void Initialize()
        {
            SetupControls();

            _particleHandler.GenParticles(800);

            /*
            float[][] atractionMatrix = [
                [10f, -10f, 10f, 0f, 0f],
                [0f, 10f, -10f, 10f, 0f],
                [0f, 0f, 10f, -10f, 10f],
                [10f, 0f, 0f, 10f, -10f],
                [-10f, 10f, 0f, 0f, 10f],
            ];
            _particleHandler.NewMatrix(atractionMatrix);
            */
            _particleHandler.NewMatrix();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _fontArial = Content.Load<SpriteFont>("Arial");
            // _fontCourierNew = Content.Load<SpriteFont>("CourierNew");
            _particleHandler.Texture = Content.Load<Texture2D>("particle");
            _buttonTexture = Content.Load<Texture2D>("button");
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            var kstate = Keyboard.GetState();

            _inputHandler.Update();

            if (!IsPaused.Value)
                _particleHandler.Update(gameTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(new Color(20, 20, 20));

            _spriteBatch.Begin();

            _particleHandler.Draw(_spriteBatch);

            _particleHandler.DrawMatrix(_spriteBatch, new Vector2(10, 10), _fontArial);

            TimeScale.DrawStat(_spriteBatch, _fontArial, new Vector2(10, 300), "Time Scale", ["Q: increase", "A: decrease", "Z: reset"]);

            IsPaused.DrawStat(_spriteBatch, _fontArial, new Vector2(10, 400), "Paused", ["Space: toggle"]);

            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
