using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using static ParticleLifeSim.Game1;

namespace ParticleLifeSim
{
    public struct ParticleProperties ()
    {
        public float MinDistance = 6f;
        public float MaxDistance = 100f;
        public float MaxVelocity = 500f;
        public float Friction = 0.01f;
    }

    public class Particle(Vector2 pos, Vector2 vel, ColorIndex color)
    {
        public const float MIN_DISTANCE = 6f;
        public const float MAX_DISTANCE = 100f;
        public const float MAX_VELOCITY = 500f;
        public static float Friction = 0.01f;

        public Vector2 Position = pos;
        public Vector2 Velocity = vel;
        public Vector2 Acceleration = Vector2.Zero;
        public ColorIndex Color = color;

        static float CalculateForce(float distance, float force, ParticleProperties pProps)
        {
            float MAX_RELEVANT_DISTANCE = pProps.MaxDistance * Math.Abs(force) * 0.1f; // because force value is in [-10, 10]
            float MID_DISTANCE = (pProps.MinDistance + MAX_RELEVANT_DISTANCE) / 2f;

            if (distance <= pProps.MinDistance)
                return 2 * (distance - 2 * pProps.MinDistance);
            else if (distance <= MID_DISTANCE)
                return (distance - pProps.MinDistance) * force / (MID_DISTANCE - pProps.MinDistance);
            else if (distance <= MAX_RELEVANT_DISTANCE)
                return force - (distance - MID_DISTANCE) * force / (MAX_RELEVANT_DISTANCE - MID_DISTANCE);
            else
                return 0f;
        }

        public void UpdForces(List<Particle> particles, float[][] atractionMatrix, ParticleProperties pProps)
        {
            Vector2 finalForce = Vector2.Zero;
            foreach (var p in particles)
            {
                if (p.Position == Position) continue; // check if 'p == this' and avoid division by 0 if not

                Vector2 positionDiference = p.Position - Position;

                float distance = positionDiference.Length();
                if (distance > pProps.MaxDistance) continue;  // Skip faraway particles

                Vector2 direction = positionDiference / distance; // Faster than Normalize()

                float forceFactor = atractionMatrix[(int)Color][(int)p.Color];
                float force = CalculateForce(distance, forceFactor, pProps);

                finalForce += force * direction;
            }
            Acceleration = finalForce;
        }

        public void UpdMov(GameTime gametime)
        {
            float deltaTime = (float)gametime.ElapsedGameTime.TotalSeconds * TimeScale.Value;

            Velocity += Acceleration * deltaTime;
            Velocity *= 1f - Friction;

            if (Velocity.Y > MAX_VELOCITY) Velocity.Y = MAX_VELOCITY;
            if (Velocity.X > MAX_VELOCITY) Velocity.X = MAX_VELOCITY;

            Position += Velocity * deltaTime;

            if (Position.X > SCREEN_SIZE.X + SCREEN_OFFSET_LEFT_UP.X)
            {
                Position.X = SCREEN_SIZE.X + SCREEN_OFFSET_LEFT_UP.X;
                Velocity.X *= -1f;
            }
            if (Position.Y > SCREEN_SIZE.Y + SCREEN_OFFSET_LEFT_UP.Y)
            {
                Position.Y = SCREEN_SIZE.Y + SCREEN_OFFSET_LEFT_UP.Y;
                Velocity.Y *= -1f;
            }
            if (Position.X < SCREEN_OFFSET_LEFT_UP.X)
            {
                Position.X = SCREEN_OFFSET_LEFT_UP.X;
                Velocity.X *= -1f;
            }
            if (Position.Y < SCREEN_OFFSET_LEFT_UP.Y)
            {
                Position.Y = SCREEN_OFFSET_LEFT_UP.Y;
                Velocity.Y *= -1f;
            }
        }

        public void Draw(SpriteBatch spriteBatch, Texture2D texture, Color color)
        {
            if (texture == null) return;

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
}