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
    class TextBox : GameObject
    {
        protected string displayText = "";
        protected int current_character = 0;
        protected int current_frame = 0; //used for timing delays

        public int character_delay = -1; //in frames

        public SpriteFont font;

        public TextBox(ComatoseGame gm)
            : base(gm)
        {
            font = game.Content.Load<SpriteFont>("art/fonts/segoeprint");
        }

        public void text(string message)
        {
            displayText = message;
            current_character = 0;
            if (character_delay < 0)
            {
                current_character = displayText.Length;
                current_frame = 0;
            } else {
                current_frame = character_delay;
            }
        }

        public override void Draw(Microsoft.Xna.Framework.GameTime gameTime)
        {
            if (current_frame > 0)
            {
                current_frame = current_frame - 1;
            }
            else
            {
                //see if we need to update our draw variables
                if (current_character < displayText.Length)
                {
                    current_character++;
                    current_frame = character_delay;
                }
            }

            //figure out screen position (same logic as GameObject base, but we aren't a
            //sprite, so we repeat here
            Vector2 draw_position = screen_position - game.camera;
            if (parent_object != null)
                draw_position += parent_object.screen_position;

            game.gameObjectBatch.DrawString(font, displayText.Substring(0, current_character), draw_position, sprite_color, rotation, rotation_origin, sprite_scale, SpriteEffects.None, z_index / 1000f);
        }


    }
}
