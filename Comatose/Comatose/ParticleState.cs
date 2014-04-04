using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;


namespace Comatose
{
    public enum ParticleType { None, Explosion, Fire, Bullet, IgnoreGravity }


    public struct ParticleState
    {
        static Random rand = new Random();
        public Vector2 Velocity;
        public ParticleType Type;
        public float LengthMultiplier;

        public ParticleState(Vector2 velocity, ParticleType type, float lengthMultiplier = 1f)
        {
            Velocity = velocity;
            Type = type;
            LengthMultiplier = lengthMultiplier;
        }

        public static ParticleState GetRandom(float minVel, float maxVel)
        {
            var state = new ParticleState();
            double theta = rand.NextDouble() * 2 * Math.PI;
            float length = (float)rand.NextDouble() * (minVel - maxVel) + minVel;

            state.Velocity = new Vector2(length * (float)Math.Cos(theta), length * (float)Math.Sin(theta));
            state.Type = ParticleType.None;
            state.LengthMultiplier = 1;

            return state;
        }

        #region UpdateParticle
        public static void UpdateParticle(ParticleManager.Particle particle)
        {
            var vel = particle.State.Velocity;
            float speed = vel.Length();
            float alpha = Math.Min(1, Math.Min(particle.PercentLife * 2, speed * 1f));
            alpha *= alpha;
            particle.Position += vel;
            particle.Orientation = (float)Math.Atan2(vel.Y, vel.X);

            if (particle.State.Type != ParticleType.IgnoreGravity)
            {
                /*
                foreach (var blackHole in EntityManager.BlackHoles)
                {
                    var dPos = blackHole.Position - particle.Position;
                    float distance = dPos.Length();
                    var n = dPos / distance;
                    vel += 10000 * n / (distance * distance + 10000);

                    // add tagentia acceleration for neary particles
                    if (distance < 400)
                        vel += 45 * new Vector2(n.Y, -n.X) / (distance + 100);

                }*/
            }

            particle.Tint.A = (byte)(255 * alpha);

            particle.Scale.X = particle.State.LengthMultiplier * Math.Min(Math.Min(1f, 0.2f * speed + 0.2f), alpha);
            particle.Scale.Y = particle.State.LengthMultiplier * Math.Min(Math.Min(1f, 0.2f * speed + 0.2f), alpha);

            //denormalized floats cause significant performance issues
            if (Math.Abs(vel.X) + Math.Abs(vel.Y) < 0.00000000001f)
                vel = Vector2.Zero;

            vel *= 0.97f;
            particle.State.Velocity = vel;
        }
        #endregion


    }

}
