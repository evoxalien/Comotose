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

        public int sprite_width = 0;
        public int sprite_height = 0;

        public int animation = 0;
        public int current_frame = 0;
        public int frame_delay = 0; //one cell per frame
        protected int delay_timer = 0;

        public Vector2 screen_position = new Vector2(0);

        protected GameObject parent_object = null;

        #region Initialization
        public int ID()
        {
            return id;
        }

        public GameObject(ComatoseGame gm)
        {
            game = gm;
            id = next_id++;
            game.vm["object_to_bind"] = this;
        }
        #endregion

        #region Properties
        public virtual void sprite(string filename)
        {
            texture = game.Content.Load<Texture2D>("art/sprites/" + filename);
            sprite_width = texture.Width;
            sprite_height = texture.Height;
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
        #endregion

        public void attach(int objectID) 
        {
            if (objectID == -1)
                parent_object = null;

            if (game.game_objects.ContainsKey(objectID)) 
                parent_object = game.game_objects[objectID];
        }

        public virtual void Draw(GameTime gameTime)
        {
            if (texture != null)
            {
                Vector2 draw_position = screen_position - game.camera;
                if (parent_object != null)
                    draw_position += parent_object.screen_position;

                Rectangle source_rectangle = new Rectangle(current_frame * sprite_width, animation * sprite_height, sprite_width, sprite_height);
                
                game.gameObjectBatch.Draw(
                    texture, 
                    draw_position, 
                    source_rectangle, //source rectangle (spritesheets maybe?)
                    sprite_color, 
                    rotation, 
                    rotation_origin, sprite_scale, SpriteEffects.None, z_index / 1000f);

                delay_timer++;
                if (delay_timer >= frame_delay) {
                    current_frame++;
                    if (current_frame * sprite_width >= texture.Width)
                        current_frame = 0;
                    delay_timer = 0;
                }
            }
        }

        //center an object on the screen, used for bars
        public void Center()
        {
            screen_position=new Vector2((game.graphics.PreferredBackBufferWidth / 2)-(sprite_scale.X/2), screen_position.Y);
        }


    }
}
