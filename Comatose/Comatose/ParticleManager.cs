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
using XNAGameConsole;
using Box2D.XNA;

namespace Comatose
{
    public class ParticleManager
    {
        private Action<Particle> updateParticle;
        private CircularParticleArray particleList;
        public int ParticleCount;
        ComatoseGame game;
        Random rand = new Random();

        public class Particle
        {
            public Texture2D Texture;
            public Vector2 Position;
            public float Orientation;

            public Vector2 Scale = Vector2.One;

            public Color Tint;
            public float Duration;
            public float PercentLife = 1f;
            public ParticleState State;
        }

        public ParticleManager(int capacity, Action<Particle> updateParticle, ComatoseGame root)
        {
            this.updateParticle = updateParticle;
            particleList = new CircularParticleArray(capacity);

            game = root;
            // Populate the list with empty particle objects, for reuse
            for (int i = 0; i < capacity; i++)
                particleList[i] = new Particle();
        }

        #region AddtionalFunctions
        private static void Swap(CircularParticleArray list, int index1, int index2)
        {
            var temp = list[index1];
            list[index1] = list[index2];
            list[index2] = temp;
        }
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

        #region CreateParticle
        public void CreateParticle(Texture2D texture, Vector2 position, Color tint, float duration, Vector2 scale, ParticleState state, float theta = 0)
        {
            Particle particle;
            if (particleList.Count == particleList.Capacity)
            {
                //Over write the oldest
                particle = particleList[0];
                particleList.Start++;
            }
            else
            {
                particle = particleList[particleList.Count];
                particleList.Count++;
            }

            particle.Texture = texture;
            particle.Position = position;
            particle.Tint = tint;

            particle.Duration = duration;
            particle.PercentLife = 1f;
            particle.Scale = scale;
            particle.Orientation = theta;
            particle.State = state;
        }
        #endregion

        #region ParticleArray
        private class CircularParticleArray
        {
            private int start;
            public int Start
            {
                get { return start; }
                set { start = value % list.Length; }
            }
            public int Count { get; set; }
            public int Capacity { get { return list.Length; } }
            private Particle[] list;

            public CircularParticleArray(int capacity)
            {
                list = new Particle[capacity];
            }

            public Particle this[int i]
            {
                get { return list[(start + i) % list.Length]; }
                set { list[(start + i) % list.Length] = value; }
            }
        }
        #endregion

        #region Update
        public void Update()
        {
            int removalCount = 0;
            for (int i = 0; i < particleList.Count; i++)
            {
                var particle = particleList[i];
                updateParticle(particle);
                particle.PercentLife -= 1f / particle.Duration;

                //Now its time to shift the deleted ones to the back of the list
                Swap(particleList, i - removalCount, i);
                ParticleCount = particleList.Count;
                // if the particle has expired, delete this particle
                if (particle.PercentLife < 0)
                    removalCount++;
            }
            particleList.Count -= removalCount;
        }
        #endregion

        #region Draw
        public void Draw(SpriteBatch spriteBatch)
        {
            for (int i = 0; i < particleList.Count; i++)
            {
                var particle = particleList[i];

                Vector2 origin = new Vector2(particle.Texture.Width / 2, particle.Texture.Height / 2);
                spriteBatch.Draw(particle.Texture, particle.Position - game.camera, null, particle.Tint, particle.Orientation, origin, particle.Scale, 0, 0.5f);

            }
        }
        #endregion

    }
}
