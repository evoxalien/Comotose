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

        public int character_delay = -1; //in frames

        public SpriteFont sprite_font;

        private SoundEffect chatter;

        public TextBox(ComatoseGame gm)
            : base(gm)
        {
            sprite_font = game.Content.Load<SpriteFont>("art/fonts/segoeprint");
            chatter = game.Content.Load<SoundEffect>("sounds/chatter");
            layer = "ui";
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

            //process line endings
            textWrap();
        }

        public void font(string name)
        {
            sprite_font = game.Content.Load<SpriteFont>("art/fonts/" + name);
        }

        public override void Draw(Microsoft.Xna.Framework.GameTime gameTime)
        {
            if (displayMessage.Count <= 0)
            {
                return; //bail if there's nothing to display
            }
            if (current_frame > 0)
            {
                current_frame = current_frame - 1;
            }
            else
            {
                //Only process animations if we're not finished with them yet
                if (current_character < displayText.Length)
                {
                    current_character++;
                    current_frame = character_delay;

                    int tline = currentLine();
                    int tcharacter = currentCharacter();
                    if (tcharacter < displayMessage[tline].Length && displayMessage[tline][tcharacter] != ' ')
                        chatter.Play(1.0f, (float)game.rand.NextDouble() * 0.1f - 0.05f, 0.0f);
                }
            }

            //figure out screen position (same logic as GameObject base, but we aren't a
            //sprite, so we repeat here)
            Vector2 draw_position = (screen_position + new Vector2(margin_left, margin_top)) - game.camera * camera_weight;
            
            if (parent_object != null)
                draw_position += parent_object.screen_position;

            //figure out our current line and draw just that line (for now)
            int line = currentLine();
            int character = currentCharacter();

            int startingLine = line - (maxLines - 1);
            while (startingLine < line)
            {
                if (startingLine >= 0)
                {
                    game.gameObjectBatch.DrawString(sprite_font, displayMessage[startingLine], draw_position, sprite_color, rotation, rotation_origin, sprite_scale, SpriteEffects.None, z_index / 1000f);
                    draw_position.Y += 25.0f; //TODO: not a constant
                }
                startingLine++;
            }

            game.gameObjectBatch.DrawString(sprite_font, displayMessage[line].Substring(0, Math.Min(character, displayMessage[line].Length)), draw_position, sprite_color, rotation, rotation_origin, sprite_scale, SpriteEffects.None, z_index / 1000f);

            //debug
            //game.gameObjectBatch.DrawString(sprite_font, "Line: " + line, draw_position + new Vector2(0f, 30f), Color.White);
            //game.gameObjectBatch.DrawString(sprite_font, "Character: " + character, draw_position + new Vector2(0f, 40f), Color.White);
        }

        private int currentLine()
        {
            int characters = 0;
            int lineNum = 0;
            while (lineNum < displayMessage.Count() && characters + displayMessage[lineNum].Length < current_character)
            {
                characters += displayMessage[lineNum].Length;
                lineNum++;
            }

            //sanity, because newlines I think?
            if (lineNum >= displayMessage.Count())
                lineNum = displayMessage.Count() - 1;

            return lineNum;
        }

        private int currentCharacter()
        {
            int line = currentLine();
            int character = current_character;
            for (int i = 0; i < displayMessage.Count() && i < line; i++) {
                character -= displayMessage[i].Length;
            }
            return character;
        }

        public bool isFinished()
        {
            return current_character >= displayText.Length;
        }

        public int height = 0;
        public int width = 0;

        public int margin_left = 0;
        public int margin_right = 0;
        public int margin_top = 0;
        public int margin_bottom = 0;

        public int maxLines = 3;
        private SortedList<int, string> displayMessage = new SortedList<int, string>();

        private void textWrap()
        {
            //given an input displayMessage, this modifies it to correct for word wrap
            displayMessage.Clear();

            string[] lines = displayText.Split('\n');
            displayText.Replace("\n", ""); //remove newlines from the original string
            int lineNum = 0;
            foreach (var line in lines)
            {
                string[] words = line.Split(' ');

                string currentLine = "";
                foreach (var word in words)
                {
                    if (currentLine.Length > 0)
                    {
                        if (sprite_font.MeasureString(currentLine + " " + word).X > width - margin_left - margin_right)
                        {
                            displayMessage.Add(lineNum++, currentLine);
                            currentLine = "";
                        }
                        else
                        {
                            currentLine += " ";
                        }
                    }
                    currentLine += word;
                }
                if (currentLine.Length > 0)
                {
                    displayMessage.Add(lineNum++, currentLine);
                }
            }
        }
    }
}
