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
    class Triangle
    {
        public List<Vector2> points;

        public Triangle(Vector2 a, Vector2 b, Vector2 c)
        {
            points = new List<Vector2>();
            points.Add(a);
            points.Add(b);
            points.Add(c);
        }

        public bool Inside(Vector2 p)
        {
            Vector2 v0, v1, v2;
            float d0, d1, d2, d3, d4;
            float u, v;
            float den;



            v0 = points[2] - points[0];
            v1 = points[1] - points[0];
            v2 = p - points[0];

            d0 = Vector2.Dot(v0, v0);
            d1 = Vector2.Dot(v0, v1);
            d2 = Vector2.Dot(v0, v2);
            d3 = Vector2.Dot(v1, v1);
            d4 = Vector2.Dot(v1, v2);


            den = 1 / (d0 * d3 - d1 * d1);
            u = (d3 * d2 - d1 * d4) * den;
            v = (d0 * d4 - d1 * d2) * den;

            if (u >= 0 && v >= 0 && u + v < 1)
                return true;

            return false;

        }

        public bool SharePoint(Vector2 p)
        {
            foreach (var point in points)
            {
                if (point == p)
                    return true;
            }
            return false;
        }

        public void Draw(GameTime gameTime, ComatoseGame game, Color c)
        {
            game.drawLine(points[0], points[1], c);
            game.drawLine(points[1], points[2], c);
            game.drawLine(points[2], points[0], c);
        }
    }

    class Polygon
    {
        public List<Vector2> points;
        public Polygon(List<Vector2> p)
        {
            points = p;
        }

        public bool Inside(Vector2 p)
        {
            int i;
            int j;
            bool r = false;

            for (i = 0, j = points.Count - 1; i < points.Count; j = i++)
            {
                if (
                    ((points[i].Y > p.Y) != (points[j].Y > p.Y)) &&
                    (p.X < (points[j].X - points[i].X) *
                    (p.Y - points[i].Y) / (points[j].Y - points[i].Y) + points[i].X))
                {
                    r = !r;
                }
            }
            return r;
        }

        public bool SharePoint(Vector2 p)
        {
            foreach (var point in points)
                if (p == point)
                    return true;

            return false;
        }

        public void Draw(GameTime gameTime, ComatoseGame game, Color c)
        {

            for (int i = 0; i < points.Count - 1; i++)
                game.drawLine(points[i], points[i + 1], c);

            //last to first
            game.drawLine(points.Last(), points[0], c);
        }


        //returns a list of points connected by edges to the point
        public List<Vector2> Connected(Vector2 p)
        {
            int i;
            List<Vector2> connections = new List<Vector2>();

            for (i = 0; i < points.Count; i++)
            {
                if (points[i] == p)
                {
                    break;
                }
            }

            //its the first point, loop it to the end
            if (i == 0)
            {
                connections.Add(points[i + 1]);
                connections.Add(points[points.Count - 1]);
            }
            //if its the last point, loop it to the front
            if (i == points.Count - 1)
            {
                connections.Add(points[i - 1]);
                connections.Add(points[points.Count - 1]);
            }

            //its in between points so we can just use its neighbors in the list
            {
                connections.Add(points[i - 1]);
                connections.Add(points[i + 1]);
            }

            return connections;
        }
    }

    class NavMesh
    {
        public List<Polygon> polygons;
        private ComatoseGame game;
        private int count;

        public NavMesh(ComatoseGame gm)
        {
            game = gm;
            polygons = new List<Polygon>();
            count = 0;
        }

        public void Add(List<Vector2> p)
        {
            polygons.Add(new Polygon(p));
            count++;
        }
        //finds polygons that share the same point
        public List<Polygon> FindNeighbors(Vector2 p)
        {
            List<Polygon> neighbors = new List<Polygon>();
            foreach (var poly in polygons)
            {
                if (poly.SharePoint(p))
                {
                    neighbors.Add(poly);
                }
            }
            return neighbors;
        }

        //returns the index to the triangle that contains the point, -1 if otherwise
        public int FindInMesh(Vector2 p)
        {
            for (int i = 0; i < count; i++)
            {
                if (polygons[i].Inside(p))
                    return i;
            }
            return -1;
        }
        public void Draw(GameTime gameTime)
        {
            for (int i = 0; i < count; i++)
            {
                polygons[i].Draw(gameTime, game, Color.FromNonPremultiplied(255, 255, 255, 255));
            }
        }
        public void Draw(GameTime gameTime, Vector2 p)
        {
            int found = FindInMesh(p);
            Console.WriteLine(found);
            Console.WriteLine(p);

            for (int i = 0; i < count; i++)
            {
                if (i == found)
                {
                    polygons[i].Draw(gameTime, game, Color.FromNonPremultiplied(255, 0, 0, 255));
                }
                else
                {
                    polygons[i].Draw(gameTime, game, Color.FromNonPremultiplied(255, 255, 255, 255));
                }
            }
        }
    }

    /*class Waypoint
    {
        //public Vector2 point;
        //public List<Waypoint> edges=new List<Waypoint>();
        //public float fscore;

        public Waypoint(Vector2 p)
        {
            point = p;
        }

        public void AddEdgeNode(Waypoint e)
        {
            //add this waypoint to our connected edges
            edges.Add(e);

            //since this is now connected, add this point to this other point
            e.edges.Add(this);
        }

        public void FScore(Vector2 start, Vector2 end)
        {
            fscore=Vector2.Distance(point, end) + Vector2.Distance(start, point);
        }

        public void Draw(GameTime gameTime, ComatoseGame game, Color c)
        {
            game.drawLine(point, point, c);
            foreach (var edge in edges)
            {
                game.drawLine(point, edge.point, c);
            }
        }
    }*/

    public class AI : PhysicsObject
    {
        public enum state { IDLE, SEARCHING, MOVING, ATTACKING }
        private PhysicsObject target;
        public float testything = 42f;
        public List<Waypoint> path=new List<Waypoint>();

        public AI(ComatoseGame gm)
            : base(gm)
        {
            //Console.WriteLine("Created an AI thing");
        }

        public void Target(int objectID)
        {
            if (objectID == -1)
                target = null;

            if (game.game_objects.ContainsKey(objectID))
                target = (PhysicsObject)game.game_objects[objectID];
        }

        public void Astar()
        {
            //only do astar if we dont have line of sight
            if (!game.hasLineOfSight(ID(), target.ID()))
            {
                if (game.waypoints.Count != 0)
                {

                    //each store an id of a waypoint
                    List<int> open = new List<int>();
                    List<int> closed = new List<int>();
                    int current = 0;
                    bool pathfound = false;

                    // add the starting point to the list
                    // starting point is the monster, find the closest 
                    List<int> lineofsightnodes = new List<int>();

                    foreach (var w in game.waypoints)
                    {
                        //get this point ready for astar
                        w.Value.Reset();
                        if (game.hasVectorLineOfSight(body.Position, w.Value.point))
                        {
                            //add it to the list of nodes within sight
                            lineofsightnodes.Add(w.Value.ID());
                        }
                    }
                    //we are not lost!
                    if (lineofsightnodes.Count != 0)
                    {
                        #region start at closest node

                        //find the closest node to start us off
                        float lowest = Vector2.Distance(body.Position, game.waypoints[lineofsightnodes[0]].point);
                        current = lineofsightnodes[0]; //id of the first in the list

                        foreach (var i in lineofsightnodes)
                        {
                            float temp;
                            temp = Vector2.Distance(body.Position, game.waypoints[i].point);
                            if (temp < lowest)
                            {
                                lowest = temp;
                                current = i;
                            }
                        }

                        //calculate the starting points fscore
                        game.waypoints[current].FScore(body.Position, target.body.Position);

                        //add the current ID to the open list
                        open.Add(current);

                        #endregion

                        #region part that loops

                        while (!pathfound && open.Count != 0)
                        {
                            int best = 0;
                            //error here
                            game.waypoints[open.First()].FScore(body.Position, target.body.Position);

                            //find lowest f cost on the open list
                            for (int i = 0; i < open.Count; i++)
                            {
                                game.waypoints[open[i]].FScore(body.Position, target.body.Position);
                                if (game.waypoints[open[best]].fscore >= game.waypoints[open[i]].fscore)
                                {
                                    best = i; //store the index of the best  so far
                                    current = open[i]; //get the id of the best
                                }
                            }

                            //move current to closed
                            open.Remove(current);
                            closed.Add(current);

                            //check if the current node can see the target, if so we have the path!
                            if (game.hasVectorLineOfSight(game.waypoints[current].point, target.body.Position))
                            {
                                pathfound = true;
                            }
                            else
                            {

                                //for all edges of current
                                foreach (var edge in game.waypoints[current].edges)
                                {

                                    //if on closed ignore
                                    if (!closed.Contains(edge.ID()))
                                    {
                                        if (!open.Contains(edge.ID()))
                                        {
                                            game.waypoints[edge.ID()].parent_id = current;
                                            game.waypoints[edge.ID()].FScore(body.Position, target.body.Position);

                                            open.Add(edge.ID());
                                        }
                                        //already on the list, check to see if this new path is better
                                        else
                                        {
                                            if (game.waypoints[edge.ID()].gscore >
                                            game.waypoints[edge.ID()].GScore(edge.ID(), body.Position))
                                            {
                                                game.waypoints[edge.ID()].parent_id = current;
                                            }
                                        }
                                    }


                                }

                            }
                        }






                        #endregion



                    }


                    //this will create the path
                    if (pathfound)
                    {
                        while (current != -1)
                        {
                            path.Add(game.waypoints[current]);

                            current = game.waypoints[current].parent_id;
                        }
                    }
                }

            }
        }


        public  void MoveTowardsTarget()
        {
            if (target == null)
            {
                Console.WriteLine("target is null?!");
            }
            else
            {
                //move straight to the target
                if (game.hasLineOfSight(ID(), target.ID()))
                {
                }
                //move along path
                else
                {
                    Console.WriteLine("lost line of sight");
                    //need to do astar and move along the path
                }
            }
        }

        public override void Draw(GameTime gameTime)
        {
            rotate(body.GetAngle());

            if (_centered)
            {
                position(body.GetPosition().X * game.physics_scale, body.GetPosition().Y * game.physics_scale);
                rotation_origin = new Vector2(texture.Width / 2, texture.Height / 2);
            }
            else
            {
                position(body.GetPosition().X * game.physics_scale, body.GetPosition().Y * game.physics_scale);
                rotation_origin = new Vector2(0);
            }

            base.Draw(gameTime);

    }
    }
}