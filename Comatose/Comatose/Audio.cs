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
    public class Audio : GameObject
    {
        private AudioEmitter emitter;
        private AudioListener listener;
        private SoundEffect soundEffect;
        private PhysicsObject parent_object = null;
        private PhysicsObject listener_object = null; //I expect this just to be the hero
        private SoundEffectInstance sfx;
        public bool looped = false;
        public float volume=100;

        public Audio(ComatoseGame gm)
            : base(gm)
        {
            listener = new AudioListener();
            emitter = new AudioEmitter();

        }

        public bool isPlaying
        {
            get
            {
                if(sfx!=null)
                {
                    if(sfx.State==SoundState.Playing)
                    {
                        return true;
                    }
                }
                return false;
            }
        }


        public void attach(int objectID)
        {
            if (objectID == -1)
                parent_object = null;

            if (game.game_objects.ContainsKey(objectID))
                parent_object = (PhysicsObject)game.game_objects[objectID];
        }
        public void attachListener(int objectID)
        {
            if (objectID == -1)
                listener_object = null;

            if (game.game_objects.ContainsKey(objectID))
                listener_object = (PhysicsObject)game.game_objects[objectID];
        }

        public void audioname(string filename)
        {
            soundEffect = game.Content.Load<SoundEffect>("sounds/" + filename);
        }

        public void play()
        {
            if (parent_object != null && listener_object != null)
            {
                sfx = soundEffect.CreateInstance();
                sfx.IsLooped = looped;
                sfx.Volume = volume / 100;
                sfx.Apply3D(listener, emitter); //needed before the first play
                Console.Write(sfx.Volume);
                sfx.Play();
            }
        }
        public void Volume(float f)
        {
            volume = f;
        }
        public void stop()
        {
            if(sfx!=null)
                sfx.Stop();
        }
        //this needs to be called everyframe so that the 3d works
        public void Calc3D()
        {
            if (isPlaying)
            {
                listener.Position = new Vector3(listener_object.body.Position, 0);
                emitter.Position = new Vector3(parent_object.body.Position, 0);
                sfx.Apply3D(listener, emitter);
            }
        }


    }
}
