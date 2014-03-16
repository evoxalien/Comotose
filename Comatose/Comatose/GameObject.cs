﻿using System;
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

namespace Comatose
{
    public class GameObject
    {
        protected ComatoseGame game;
        private static int next_id = 0;
        private int id;

        protected Texture2D texture;
        public float rotation = 0f;
        protected Vector2 rotation_origin = new Vector2(0f);
        public float z_index = 0.0f;
        protected Vector2 sprite_scale = new Vector2(1f);
        protected Color sprite_color = Color.White;

        protected Vector2 screen_position = new Vector2(0);

        public int ID()
        {
            return id;
        }

        public virtual void sprite(string filename)
        {
            texture = game.Content.Load<Texture2D>("art/sprites/" + filename);
        }

        public void position(float x, float y)
        {
            screen_position = new Vector2(x, y);
        }

        public void color(int r, int g, int b, int a)
        {
            sprite_color = Color.FromNonPremultiplied(r, g, b, a);
        }

        public void scale(float sx, float sy)
        {
            sprite_scale = new Vector2(sx, sy);
        }

        public virtual void rotate(float angle)
        {
            rotation = angle;
        }

        public void origin(float x, float y)
        {
            rotation_origin = new Vector2(x, y);
        }
        
        public GameObject(ComatoseGame gm)
        {
            game = gm;
            id = next_id++;
            game.vm["object_to_bind"] = this;
        }

        public virtual void Draw(GameTime gameTime)
        {
            if (texture != null)
            {
                game.gameObjectBatch.Draw(texture, screen_position - game.camera, null, sprite_color, rotation, rotation_origin, sprite_scale, SpriteEffects.None, z_index);
            }
        }
        //center an object on the screen, used for bars
        public void Center()
        {
            screen_position=new Vector2((game.graphics.PreferredBackBufferWidth / 2)-(sprite_scale.X/2), screen_position.Y);
        }


    }
}
