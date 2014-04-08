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

        private bool dirty = true;

        public Vector2 point {
            get { return body.Position; }
            set { body.Position = value; body.SetAwake(true); updateEdgeList(); }
        }
        public float x {
            get { return body.Position.X; }
            set { body.Position = new Vector2(value, body.Position.Y); body.SetAwake(true); dirty = true; }
        }
        public float y {
            get { return body.Position.Y; }
            set { body.Position = new Vector2(body.Position.X, value); body.SetAwake(true); dirty = true; }
        }

        public void updateEdgeList() {
            edges.Clear();
            foreach (var waypoint in game.waypoints) {
                if (this != waypoint.Value && game.hasVectorLineOfSight(this.point, waypoint.Value.point)) {
                    AddEdgeNode(waypoint.Value);
                }
            }
        }

        public override void Draw(Microsoft.Xna.Framework.GameTime gameTime) {
            if (dirty) {
                updateEdgeList();
                dirty = false;
            }

            //debug lines
            if (game.input.DevMode) {
                

                foreach (Waypoint edge in edges) {
                    game.drawLine(body.GetPosition(), edge.body.GetPosition(), Color.Red);
                }

            }

        }
    }
}
