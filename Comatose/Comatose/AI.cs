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

        public bool InTriangle(Vector2 p)
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

        public bool SamePoint(Vector2 p)
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
    class NavMesh
    {
        public List<Triangle> triangles;
        private ComatoseGame game;
        private int count;
        
        public NavMesh(ComatoseGame gm)
        {
            game = gm;
            triangles = new List<Triangle>();
            count = 0;
        }

        public void AddTriangle(Vector2 a, Vector2 b, Vector2 c)
        {
            triangles.Add(new Triangle(a,b,c));
            count++;
        }
        //finds triangles that share the same point
        public List<Triangle> FindNeighbors(Vector2 p)
        {
            List<Triangle> neighbors=new List<Triangle>();
            foreach (var triangle in triangles)
            {
                if (triangle.SamePoint(p))
                {
                    neighbors.Add(triangle);
                }
            }
            return neighbors;
        }

        //returns the index to the triangle that contains the point, -1 if otherwise
        public int FindInMesh(Vector2 p)
        { 
            for (int i = 0; i < count ; i++ )
            {
                if (triangles[i].InTriangle(p))
                    return i;
            }
            return -1;
        }
        public void Draw(GameTime gameTime)
        {
            for (int i = 0; i < count*3; i+=3)
            {
                triangles[i].Draw(gameTime, game, Color.FromNonPremultiplied(255, 255, 255, 255));
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
                    triangles[i].Draw(gameTime, game, Color.FromNonPremultiplied(255, 0, 0, 255));
                }
                else
                {
                    triangles[i].Draw(gameTime, game, Color.FromNonPremultiplied(255, 255, 255, 255));
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
            //nice triangle
            mesh = new NavMesh(gm);
            mesh.AddTriangle(new Vector2(0, 0), new Vector2(20, 10), new Vector2(20, 20));
            mesh.AddTriangle(new Vector2(20, 30), new Vector2(30, 10), new Vector2(30, 50));

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
                    else //we need to do astar!
                    {
                        //first get the points to the triangle we are in
                        foreach(var point in mesh.triangles[a].points)
                        {


                        }

                    }

                }
            }


        }

        public float FScore(Vector2 p)
        {
            return Vector2.Distance(p, target.body.Position) + Vector2.Distance(body.Position, p);
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
