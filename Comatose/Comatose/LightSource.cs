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
    class LightSource : PhysicsObject
    {
        public int ray_length = 50;
        public float light_spread_angle = (float)Math.PI * 2;
        public int rays_to_cast = 640;
        public float max_fraction = 1;
        
        private Vector2 intersectionNormal = new Vector2(0, 0);

        public LightSource(ComatoseGame gm) : base(gm)
        {
            body.SetActive(false);
        }


        public override void Draw(GameTime gameTime)
        {
                //position(body.GetPosition().X * game.physics_scale, body.GetPosition().Y * game.physics_scale);

                Vector2 p1 = new Vector2(x,y);
                Vector2 p2;

                float closestFraction = 1;
                Body b = game.world.GetBodyList();
                while (b != null)
                {
                    if (b.GetUserData() is PhysicsObject)
                        if (((PhysicsObject)b.GetUserData()).cast_shadow)
                        {
                            Fixture f = b.GetFixtureList();
                            while (f != null)
                            {
                                Type shapeType = f.GetType();

                                if (f.GetShape() is PolygonShape)
                                {
                                    PolygonShape polygon = (PolygonShape)f.GetShape();

                                    for (int curVert = 0; curVert < polygon.GetVertexCount(); curVert++)
                                    {
                                        p2 = polygon.GetVertex(curVert);
                                        
                                        //transform this point based on the body transforms
                                        p2 = Vector2.Transform(p2, Matrix.CreateRotationZ(b.GetAngle()));
                                        p2 += b.GetPosition();

                                        //perform the ray cast, and figure out what to do about the result
                                        closestFraction = rayCast(p1, p2);
                                        
                                        if (closestFraction > 1f) {
                                            closestFraction = 1f;
                                        }

                                        Vector2 intersectPoint = p1 + closestFraction * (p2 - p1);


                                        if (game.input.DevMode)
                                            game.drawLine(p1, intersectPoint, (Color.White));

                                    }

                                }
                                else if (f.GetShape() is EdgeShape)
                                {
                                    //Do the same thing, except for edge shapes
                                }
                                
                                f = f.GetNext();
                            }
                        }
                    b = b.GetNext();
            }

            
        }

        float min_distance;

        float ReportFixture(Fixture fixture, Vector2 point, Vector2 normal, float fraction)
        {
            if (fraction < min_distance)
            {
                min_distance = fraction;
            }

            return 1;
        }

        float rayCast(Vector2 start, Vector2 end)
        {
            min_distance = (end - start).Length();
            game.world.RayCast(ReportFixture, start, end);
            return min_distance;
        }

    }
}
