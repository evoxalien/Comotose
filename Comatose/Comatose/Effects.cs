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
using NLua;
using Box2D.XNA;
using System.Collections;

namespace Comatose
{
    public class Effects
    {
        ComatoseGame game;
        Random rand = new Random();

        public Effects(ComatoseGame root)
        {
            game = root;
        }
        #region AddtionalFunctions

        #region NextFloat
        float NextFloat(Random rand, float minValue, float maxValue)
        {
            return (float)rand.NextDouble() * (maxValue - minValue) + minValue;
        }
        #endregion

        #region NextVector2
        Vector2 NextVector2(Random rand, float minLength, float maxLength)
        {
            double theta = rand.NextDouble() * 2 * Math.PI;
            float length = NextFloat(rand, minLength, maxLength);
            return new Vector2(length * (float)Math.Cos(theta), length * (float)Math.Sin(theta));
        }
        #endregion
        #endregion
        #region CreateCalls
        public void CreateExplosion(int x, int y, int count, int r, int b, int g)
        {

            Vector2 Position = new Vector2(x, y) * game.physics_scale;
            //Texture2D particleTexture = game.Content.Load<Texture2D>("art/particle");

            Texture2D particleTexture = game.Content.Load<Texture2D>("art/FlappyBird");
            for (int i = 0; i < count; i++)
            {
                float speed = 6f * (1f - 1 / NextFloat(rand, 1f, 10f));
                var state = new ParticleState()
                {
                    Velocity = NextVector2(rand, speed, speed),
                    Type = ParticleType.Explosion,
                    LengthMultiplier = 1f
                };
                game.ParticleManager.CreateParticle(particleTexture, Position, Color.FromNonPremultiplied(r, g, b, 255), 190f, new Vector2(1.0f), state);
            }
        }

        public void CreateFire(float x, float y, int intensity)
        {

            Vector2 Position;
            Texture2D particleTexture = game.Content.Load<Texture2D>("art/particles/Cloud1-128");

            //intensity = 100;
            //Red Particles
            int r = rand.Next(210, 250);
            int g = rand.Next(30, 90);
            int b = rand.Next(30, 50);

            for (int i = 0; i < intensity / 3; i++)
            {
                Position = new Vector2(x = NextFloat(rand, x + .75f, x - .75f), y = NextFloat(rand, y + .75f, y - .75f)) * game.physics_scale;
                float speed = 3f * (1f - 1 / NextFloat(rand, 1f, 20f));
                var state = new ParticleState()
                {
                    Velocity = NextVector2(rand, speed, speed / 4),
                    Type = ParticleType.Fire,
                    LengthMultiplier = 1.5f
                };
                game.ParticleManager.CreateParticle(particleTexture, Position, Color.FromNonPremultiplied(r, g, b, 255), 150f, new Vector2(1.0f), state);
            }

            // Yellow Particles
            r = rand.Next(220, 240);
            g = rand.Next(170, 225);
            b = rand.Next(0, 40);
            //particleTexture = game.Content.Load<Texture2D>("art/particles/Cloud2-128");
            for (int i = 0; i < intensity / 4; i++)
            {
                Position = new Vector2(x = NextFloat(rand, x + .5f, x - .5f), y = NextFloat(rand, y + .5f, y - .5f)) * game.physics_scale;
                float speed = 3f * (1f - 1 / NextFloat(rand, 1f, 5f));
                var state = new ParticleState()
                {
                    Velocity = NextVector2(rand, speed, speed / 7),
                    Type = ParticleType.Fire,
                    LengthMultiplier = 1.5f
                };
                game.ParticleManager.CreateParticle(particleTexture, Position, Color.FromNonPremultiplied(r, g, b, 255), 150f, new Vector2(NextFloat(rand, 0.5f, 1f)), state);
            }


            //particleTexture = game.Content.Load<Texture2D>("art/particles/Cloud3-128");
            //particleTexture = game.Content.Load<Texture2D>("art/particle");
            for (int i = 0; i < intensity / 7; i++)
            {
                Position = new Vector2(x = NextFloat(rand, x + .25f, x - .25f), y = NextFloat(rand, y + .25f, y - .25f)) * game.physics_scale;
                float speed = .5f * (1f - 1 / NextFloat(rand, 1f, 5f));
                var state = new ParticleState()
                {
                    Velocity = NextVector2(rand, speed * 3, speed / 5),
                    Type = ParticleType.Fire,
                    LengthMultiplier = 1.5f
                };
                game.ParticleManager.CreateParticle(particleTexture, Position, Color.FromNonPremultiplied(255, 255, 255, 200), 150f, new Vector2(NextFloat(rand, 0.5f, 1f)), state);
            }
        }

        public void CreateFireBall(float x, float y, int intensity)
        {

            Vector2 Position;
            Texture2D particleTexture = game.Content.Load<Texture2D>("art/particles/Cloud1-128");

            //intensity = 100;
            //Red Particles
            /*
            int r = rand.Next(210, 250);
            int g = rand.Next(30, 90);
            int b = rand.Next(30, 50);
            */
            int r = 225;
            int g = 55;
            int b = 40;
            float speed = 3f * (1f - 1 / NextFloat(rand, 1f, 20f));
            for (int i = 0; i < intensity / 3; i++)
            {
                Position = new Vector2(x = NextFloat(rand, x + .75f, x - .75f), y = NextFloat(rand, y + .75f, y - .75f)) * game.physics_scale;

                var state = new ParticleState()
                {
                    Velocity = NextVector2(rand, speed, speed / 3),
                    Type = ParticleType.Fire,
                    LengthMultiplier = 1.5f
                };
                game.ParticleManager.CreateParticle(particleTexture, Position, Color.FromNonPremultiplied(r, g, b, 255), 150f, new Vector2(1.0f), state);
            }

            // Yellow Particles
            /*r = rand.Next(220, 240);
            g = rand.Next(170, 225);
            b = rand.Next(0, 40);
            */

            r = 235;
            g = 195;
            b = 20;
            speed = 3f * (1f - 1 / NextFloat(rand, 1f, 5f));
            //particleTexture = game.Content.Load<Texture2D>("art/particles/Cloud2-12+8");
            for (int i = 0; i < intensity / 4; i++)
            {
                Position = new Vector2(x = NextFloat(rand, x + .5f, x - .5f), y = NextFloat(rand, y + .5f, y - .5f)) * game.physics_scale;
                //float speed = 3f * (1f - 1 / NextFloat(rand, 1f, 5f));
                var state = new ParticleState()
                {
                    Velocity = NextVector2(rand, speed, speed / 5),
                    Type = ParticleType.Fire,
                    LengthMultiplier = 1.5f
                };
                game.ParticleManager.CreateParticle(particleTexture, Position, Color.FromNonPremultiplied(r, g, b, 255), 150f, new Vector2(NextFloat(rand, 0.5f, 1f)), state);
            }


            //particleTexture = game.Content.Load<Texture2D>("art/particles/Cloud3-128");
            //particleTexture = game.Content.Load<Texture2D>("art/particle");
            speed = .5f * (1f - 1 / NextFloat(rand, 1f, 5f));
            for (int i = 0; i < intensity / 7; i++)
            {
                Position = new Vector2(x = NextFloat(rand, x + .25f, x - .25f), y = NextFloat(rand, y + .25f, y - .25f)) * game.physics_scale;
                speed = .5f * (1f - 1 / NextFloat(rand, 1f, 5f));
                var state = new ParticleState()
                {
                    Velocity = NextVector2(rand, speed * 2, speed / 4),
                    Type = ParticleType.Fire,
                    LengthMultiplier = 1.5f
                };
                game.ParticleManager.CreateParticle(particleTexture, Position, Color.FromNonPremultiplied(255, 255, 255, 200), 150f, new Vector2(NextFloat(rand, 0.5f, 1f)), state);
            }
        }
        #endregion
    }
}
