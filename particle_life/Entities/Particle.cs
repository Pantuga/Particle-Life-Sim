using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using static ParticleLifeSim.Game1;

namespace ParticleLifeSim
{
    public struct ParticleProperties(float minDist = 6f, float maxDist = 100f, short maxVel = 500, float friction = 0.01f)
    {
        public readonly float MinDistance = minDist;
        public readonly float MaxDistance = maxDist;
        public readonly short MaxVelocity = maxVel;
        public readonly float Friction = friction;
    }

    public class Particle(Vector2 pos, Vector2 vel, ColorIndex color)
    {
        public Vector2 Position = pos;
        public Vector2 Velocity = vel;
        public Vector2 Acceleration = new(0, 0);
        public ColorIndex Color = color;

        float CalculateForce(float distance, float force, ParticleProperties pProps)
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
            Vector2 finalForce = new(0, 0);
            foreach (var p in particles)
            {
                if (p.Position == Position) continue; // avoid division by 0

                Vector2 positionDiference = p.Position - Position;

                float distance = positionDiference.Length();
                if (distance > pProps.MaxDistance) continue;  // Skip faraway particles

                Vector2 direction = positionDiference / distance;

                float forceFactor = atractionMatrix[(int)Color][(int)p.Color];
                float force = CalculateForce(distance, forceFactor, pProps);

                finalForce += direction * force;
            }
            Acceleration = finalForce;
        }

        public void UpdMov(GameTime gametime, ParticleProperties pProps)
        {
            float deltaTime = (float)gametime.ElapsedGameTime.TotalSeconds * TimeScale.Value;

            Velocity += Acceleration * deltaTime;
            Velocity *= 1f - pProps.Friction;

            if (Velocity.Y > pProps.MaxVelocity) Velocity.Y = pProps.MaxVelocity;
            if (Velocity.X > pProps.MaxVelocity) Velocity.X = pProps.MaxVelocity;

            Position += Velocity * deltaTime;

            if (Position.X > SCREEN_SIZE.X + SCREEN_OFFSET_LEFT.X)
            {
                Position.X = (short)(SCREEN_SIZE.X + SCREEN_OFFSET_LEFT.X);
                Velocity.X *= -1;
            }
            if (Position.Y > SCREEN_SIZE.Y + SCREEN_OFFSET_LEFT.Y)
            {
                Position.Y = (short)(SCREEN_SIZE.Y + SCREEN_OFFSET_LEFT.Y);
                Velocity.Y *= -1;
            }
            if (Position.X < SCREEN_OFFSET_LEFT.X)
            {
                Position.X = (short)SCREEN_OFFSET_LEFT.X;
                Velocity.X *= -1;
            }
            if (Position.Y < SCREEN_OFFSET_LEFT.Y)
            {
                Position.Y = (short)SCREEN_OFFSET_LEFT.Y;
                Velocity.Y *= -1;
            }
        }

        public void Draw(SpriteBatch spriteBatch, Texture2D texture, Color color)
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
}
