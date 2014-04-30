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
    /*
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

    class Waypoint
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
        //public enum state { IDLE, SEARCHING, MOVING, ATTACKING }
        public PhysicsObject target;
        public List<Waypoint> path = new List<Waypoint>();
        private int current_path_node = -1;
        public float speed = 0;
        private Vector2 random_point;

        public AI(ComatoseGame gm)
            : base(gm)
        {
        }

        public void setTarget(int objectID)
        {
            if (objectID == -1)
                target = null;

            if (game.game_objects.ContainsKey(objectID))
                target = (PhysicsObject)game.game_objects[objectID];
        }

        public bool HasTarget()
        {
            return !(target == null);
        }

        public void NewAstar(Vector2 t)
        {
            List<Waypoint> open = new List<Waypoint>();
            List<Waypoint> closed = new List<Waypoint>();
            List<Waypoint> lineofsightnodes = new List<Waypoint>();
            path = new List<Waypoint>();
            Waypoint current = new Waypoint(game);
            bool pathfound = false;

            //make sure thigns are empty!
            open.Clear();
            closed.Clear();
            path.Clear();


            foreach (var wp in game.waypoints)
            {
                //get this point ready for astar
                wp.Value.Reset();
                if (game.hasVectorLineOfSight(body.Position, wp.Value.point))
                {
                    //add it to the list of nodes within sight
                    lineofsightnodes.Add(wp.Value);
                }
            }

            //find the closest node to start us off
            if (lineofsightnodes.Count != 0)
            {
                float lowest = Vector2.Distance(body.Position, lineofsightnodes.First().point);
                current = lineofsightnodes.First();

                foreach (var wp in lineofsightnodes)
                {
                    float temp;
                    temp = Vector2.Distance(body.Position, wp.point);
                    if (temp < lowest)
                    {
                        lowest = temp;
                        current = wp;
                    }
                }
                //calculate the starting points fscore
                current.parent = null;
                current.GScore();
                current.HScore(target.body.Position);
                current.fscore = current.gscore + current.hscore;



                //add the current to the open list
                open.Add(current);
            }

            while (open.Count > 0 && !pathfound)
            {
                Waypoint q;//= new Waypoint(game);
                q = open.First();

                q.GScore();
                q.HScore(t);
                q.fscore = q.gscore + q.hscore;
                //find node with least f value on open list
                foreach (var wp in open)
                {
                    wp.GScore();
                    wp.HScore(target.body.Position);
                    wp.fscore = wp.gscore + wp.hscore;

                    if (q.fscore > wp.fscore)
                        q = wp;
                }

                //remove q from the list
                open.Remove(q);




                //if it is the goal node create the path!
                if (game.hasVectorLineOfSight(t, q.point))
                {
                    //we have the path!
                    pathfound = true;
                    current = q;


                    while (current != null)
                    {
                        if (!path.Contains(current))
                            path.Add(current);
                        //Console.WriteLine(current.point);
                        current = current.parent;
                    }
                    path.Reverse(); //fix it!
                }
                //we dont have the path and we need to search the edges
                else
                {
                    //for every child node
                    foreach (var e in q.edges)
                    {
                        //avoid grabbing the parent
                        if (e != q.parent)
                        {

                            if (open.Contains(e))
                            {
                                //check if the one on the open list is lower...
                                float oldg = e.gscore;
                                Waypoint oldparent = e.parent;
                                e.parent = q;

                                //old is better, reset
                                if (oldg < e.GScore())
                                {
                                    e.parent = oldparent;
                                    e.gscore = oldg;
                                    continue;
                                }

                            }
                            if (closed.Contains(e))
                            {
                                //check if the one on the closed list is lower...
                                float oldg = e.gscore;
                                Waypoint oldparent = e.parent;
                                e.parent = q;

                                //old is better, reset
                                if (oldg < e.GScore())
                                {
                                    e.parent = oldparent;
                                    e.gscore = oldg;
                                    continue;
                                }
                            }


                            //in this case the newer version is better!
                            open.Remove(e);
                            closed.Remove(e);

                            e.parent = q;
                            e.GScore();
                            e.HScore(t);
                            e.fscore = e.gscore + e.hscore;

                            open.Add(e);
                        }
                        else
                        {
                            //Console.WriteLine("Got a parent node");
                        }
                    }
                    closed.Add(q);
                }
            }
            //reset the current path node
            current_path_node = -1;
        }

        public void MoveTowardsTarget(Vector2 t)
        {
            //move straight to the target
            if (game.hasVectorLineOfSight(t, body.Position))
            {
                Vector2 distance = body.Position - t;
                distance.Normalize();
                this.vx = -distance.X * speed;
                this.vy = -distance.Y * speed;
            }
            //move along path
            else
            {
                if (path.Count != 0)
                {
                    if (!game.hasVectorLineOfSight(t, path.Last().point))
                    {
                        //Console.WriteLine("path is >0 and the last point in the path lost sight");
                        //need to do astar and move along the path
                        NewAstar(t);
                    }
                    //we have a good path! follow it!
                    else
                    {
                        //start fresh at the first point
                        if (current_path_node == -1)
                        {
                            current_path_node = 0;
                        }
                        //check if we can see the next point, if so we should move to that one now
                        if (current_path_node < path.Count - 1)
                        {
                            if (game.hasVectorLineOfSight(body.Position, path[current_path_node + 1].point))
                            {
                                current_path_node += 1;
                            }
                        }
                        Vector2 distance = body.Position - path[current_path_node].point;
                        distance.Normalize();
                        this.vx = -distance.X * speed;
                        this.vy = -distance.Y * speed;
                        //Console.WriteLine("THING");
                    }
                }
                else
                {
                    //Console.WriteLine("no path!");
                    NewAstar(t);
                }
            }
        }

        public void MoveTowardsTarget(float x,float y)
        {
            Vector2 t = new Vector2(x, y);
            //move straight to the target
            if (game.hasVectorLineOfSight(t, body.Position)) 
            {
                Vector2 distance = body.Position - t;
                distance.Normalize();
                this.vx = -distance.X * speed;
                this.vy = -distance.Y * speed;
            }
            //move along path
            else
            {
                if (path.Count != 0)
                {
                    if (!game.hasVectorLineOfSight(t, path.Last().point))
                    {
                        //Console.WriteLine("path is >0 and the last point in the path lost sight");
                        //need to do astar and move along the path
                        NewAstar(t);
                    }
                    //we have a good path! follow it!
                    else
                    {
                        //start fresh at the first point
                        if (current_path_node == -1)
                        {
                            current_path_node = 0;
                        }
                        //check if we can see the next point, if so we should move to that one now
                        if (current_path_node < path.Count - 1)
                        {
                            if (game.hasVectorLineOfSight(body.Position, path[current_path_node + 1].point))
                            {
                                current_path_node += 1;
                            }
                        }
                        Vector2 distance = body.Position - path[current_path_node].point;
                        distance.Normalize();
                        this.vx = -distance.X * speed;
                        this.vy = -distance.Y * speed;
                        //Console.WriteLine("THING");
                    }
                }
                else
                {
                    //Console.WriteLine("no path!");
                    NewAstar(t);
                }
            }
        }


        public void MoveToPoint(float x, float y)
        {
            Vector2 t = new Vector2(x, y);

            Vector2 distance = body.Position - t;
            distance.Normalize();
            this.vx = -distance.X * speed;
            this.vy = -distance.Y * speed;
        }

        public void RandomWaypoint()
        {
            //choose a random waypoint
            if (game.waypoints.Count > 0)
            {
                Random r = new Random();
                random_point = game.waypoints.ElementAt(r.Next(0, game.waypoints.Count)).Value.point;
            }
        }

        public void Wander()
        {
            Vector2 t = random_point;
            
            //move straight to the target
            if (game.hasVectorLineOfSight(t, body.Position) && Vector2.Distance(body.Position,t) >=5)
            {
                Vector2 distance = body.Position - t;
                distance.Normalize();
                this.vx = -distance.X * speed;
                this.vy = -distance.Y * speed;
            }
            else if (Vector2.Distance(body.Position, t) < 5)
            {
                RandomWaypoint();
            }
            //move along path
            else
            {
                if (path.Count != 0)
                {
                    if (!game.hasVectorLineOfSight(t, path.Last().point))
                    {
                        NewAstar(t);
                    }
                                        else
                    {
                        if (current_path_node == -1)
                        {
                            current_path_node = 0;
                        }
                        if (current_path_node < path.Count - 1)
                        {
                            if (game.hasVectorLineOfSight(body.Position, path[current_path_node + 1].point))
                            {
                                current_path_node += 1;
                            }
                        }
                        Vector2 distance = body.Position - path[current_path_node].point;
                        distance.Normalize();
                        this.vx = -distance.X * speed;
                        this.vy = -distance.Y * speed;
                    }
                }
                else
                {
                    NewAstar(t);
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


            //debug lines
            if (game.input.DevMode)
            {
                for (int i = 0; i < path.Count - 1; i++)
                {
                    game.drawLine(path[i].point, path[i + 1].point, Color.Yellow);
                }
                if (current_path_node != -1)
                {
                    game.drawLine(body.GetPosition(), path[current_path_node].point, Color.Green);
                }
                if (path.Count > 0)
                {
                    game.drawLine(target.body.GetPosition(), path.Last().point, Color.Cyan);
                }

            }



            base.Draw(gameTime);

        }

    }
}