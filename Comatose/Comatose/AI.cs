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

        public Triangle(Vector2 a,Vector2 b, Vector2 c)
        {
            points= new List<Vector2>();
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
            foreach(var point in points)
            {
                if(point==p)
                    return true;
            }
            return false;
        }

        public void Draw(GameTime gameTime,ComatoseGame game,Color c)
        {
                game.drawLine(points[0], points[ 1], c);
                game.drawLine(points[1], points[ 2],c);
                game.drawLine(points[2], points[0], c);
        }
    }

    class Polygon
    {
        public List<Vector2> points;
        public Polygon(List <Vector2> p)
        {
            points = p;
        }

        public bool Inside(Vector2 p)
        {
            int i;
            int j;
            bool r = false;

            for (i = 0, j = points.Count - 1; i < points.Count; j=i++)
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

        public void Draw(GameTime gameTime,ComatoseGame game,Color c)
        {

            for (int i = 0; i < points.Count-1;i++ )
                game.drawLine(points[i], points[i+1], c);

            //last to first
                game.drawLine(points.Last(), points[0], c);
        }


        //returns a list of points connected by edges to the point
        public List<Vector2> Connected(Vector2 p)
        {
            int i;
            List<Vector2> connections=new List<Vector2>();

            for(i =0 ;i<points.Count;i++)
            {
                if (points[i] == p)
                {
                    break;
                }
            }

            //its the first point, loop it to the end
            if(i==0)
            {
                connections.Add(points[i+1]);
                connections.Add(points[points.Count-1]);
            }
            //if its the last point, loop it to the front
            if(i==points.Count-1)
            {
                connections.Add(points[i-1]);
                connections.Add(points[points.Count-1]);
            }

            //its in between points so we can just use its neighbors in the list
            {
                connections.Add(points[i-1]);
                connections.Add(points[i+1]);
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
            List<Polygon> neighbors=new List<Polygon>();
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
            for (int i = 0; i < count ; i++ )
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

    class AI : PhysicsObject
    {
        public enum state { IDLE, SEARCHING, MOVING, ATTACKING }
        NavMesh mesh;
        private PhysicsObject target;


        public AI(ComatoseGame gm)
            : base(gm)
        {
            mesh = new NavMesh(gm);

            List<Vector2> temp=new List<Vector2>();
            temp.Add(new Vector2(0, 0));
            temp.Add(new Vector2(10, 0));
            temp.Add(new Vector2(10, 10));
            temp.Add(new Vector2(20, 10));
            temp.Add(new Vector2(0, 20));
            temp.Add(new Vector2(0, 0));

            mesh.Add(temp);

            temp = new List<Vector2>();

            temp.Add(new Vector2(10, 10));
            temp.Add(new Vector2(20, 10));
            temp.Add(new Vector2(25, 15));
            temp.Add(new Vector2(20, 00));

            mesh.Add(temp);
        }
        public void Target(int objectID) 
        {
            if (objectID == -1)
                target= null;

            if (game.game_objects.ContainsKey(objectID)) 
                target= (PhysicsObject)game.game_objects[objectID];
        }

        public void Astar()
        {
            int a, b;
            //find target in mesh
            b=mesh.FindInMesh(target.body.Position);

            //target is in mesh
            if (b >= 0)
            {
                //find ourselves in the mesh
                a=mesh.FindInMesh(body.Position);

                if(a>=0) //if we are in the mesh, navigate to the target
                {
                    if(a==b) //if we are in the same triangle, line of sight, move straight towards target
                    {


                    }
                    else //astar here
                    { }


                }
            }


        }

        public float FScore(Vector2 p)
        {
            return Vector2.Distance(p, target.body.Position) + Vector2.Distance(body.Position, p);
        }

        //finds the edge we want to walk through to get from poly i, to poly j to 
        //make sure that it is wide enough for this ai to go through
        public float FindEdgeLength(int i,int j)
        {
            Vector2 a=new Vector2();
            Vector2 b = new Vector2();

            return Vector2.Distance(a, b);
        }


        public override void Draw(GameTime gameTime)
        {
            if (game.input.DevMode)
            {
                if (target!= null)
                {
                    mesh.Draw(gameTime, new Vector2(target.x,target.y));
                }
                else
                    mesh.Draw(gameTime);
            }

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
