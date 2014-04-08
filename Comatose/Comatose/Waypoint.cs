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
using Box2D.XNA;
using System.Collections;

namespace Comatose {
    public class Waypoint : PhysicsObject {
        protected List<Waypoint> edges = new List<Waypoint>();
        public float fscore;

        public void FScore(Vector2 start, Vector2 end) {
            fscore = Vector2.Distance(body.GetPosition(), end) + Vector2.Distance(start, body.GetPosition());
        }

        public void AddEdgeNode(Waypoint e) {
            //add this waypoint to our connected edges
            edges.Add(e);

            //since this is now connected, add this point to this other point
            e.edges.Add(this);
        }

        public Waypoint(ComatoseGame gm)
            : base(gm) {



        }

        public override void Draw(Microsoft.Xna.Framework.GameTime gameTime) {
            //debug lines
            if (game.input.DevMode) {
                game.drawLine(body.GetPosition(), body.GetPosition(), sprite_color);
                foreach (var edge in edges) {
                    game.drawLine(body.GetPosition(), edge.body.GetPosition(), Color.Lerp(sprite_color, Color.White, 0.5f));
                }
            }
        }
    }
}
