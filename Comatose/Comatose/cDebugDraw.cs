using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Box2D.XNA;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Comatose
{
    class cDebugDraw : DebugDraw
    {
        Texture2D pixel;
        ComatoseGame game;
        public cDebugDraw(ComatoseGame gm)
        {
            game = gm;
            pixel = game.Content.Load<Texture2D>("art/pixel");
        }

        private void drawLine(Vector2 start, Vector2 end, Color color)
        {
            start = game.screenCoordinates(start);
            end = game.screenCoordinates(end);
            Vector2 edge = end - start;
            float angle = (float)Math.Atan2(edge.Y, edge.X);

            game.debugBatch.Draw(pixel,
                new Rectangle((int)(start.X), (int)(start.Y), (int)(edge.Length()), 2),
                null,
                color,
                angle,
                new Vector2(0),
                SpriteEffects.None,
                0);
        }

        public override void DrawPolygon(ref FixedArray8<Vector2> vertices, int count, Color color)
        {
            drawLine(vertices[0], vertices[count - 1], color);
            for (int i = 0; i < count - 1; i++)
            {
                drawLine(vertices[i], vertices[i + 1], color);
            }
        }

        public override void DrawSolidPolygon(ref FixedArray8<Vector2> vertices, int count, Color color)
        {
            //throw new NotImplementedException();
            //for now, render this as an empty shape, not filled
            DrawPolygon(ref vertices, count, color);
        }

        public override void DrawCircle(Vector2 center, float radius, Color color)
        {
            //throw new NotImplementedException();
            int steps = 16;

            for (int i = 0; i < steps; i += 1)
            {
                Vector2 p1 = center + radius * new Vector2((float)Math.Cos((2 * Math.PI) * i / steps), (float)Math.Sin((2 * Math.PI) * i / steps));
                Vector2 p2 = center + radius * new Vector2((float)Math.Cos((2 * Math.PI) * ((i + 1) % steps) / steps), (float)Math.Sin((2 * Math.PI) * ((i + 1) % steps) / steps));
                drawLine(p1, p2, color);
            }
        }

        public override void DrawSolidCircle(Vector2 center, float radius, Vector2 axis, Color color)
        {
            //throw new NotImplementedException();
            DrawCircle(center, radius, color);
            drawLine(center, center + axis * radius, color);
        }

        public override void DrawSegment(Vector2 p1, Vector2 p2, Color color)
        {
            drawLine(p1, p2, color);
        }

        public override void DrawTransform(ref Transform xf)
        {
            //throw new NotImplementedException();
        }

        public void DrawBackground()
        {
            game.debugBatch.Draw(pixel, new Rectangle(0, 0, 1000, 1000), Color.FromNonPremultiplied(0, 0, 0, 192));
        }
    }
}
