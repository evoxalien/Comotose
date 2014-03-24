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
            position(body.GetPosition().X * game.physics_scale, body.GetPosition().Y * game.physics_scale);

            //float currentRayAngle = 0;
            //float drawRayAngle = 0;
            /*
            for (int i = 1; i <= rays_to_cast && currentRayAngle <= light_spread_angle; i++)
            {*/
                //currentRayAngle += (float)Math.PI * 2 / rays_to_cast;
                //drawRayAngle = currentRayAngle - rotation - light_spread_angle / 2 + (float)Math.PI;

                Vector2 p1 = new Vector2(x,y);
                //Vector2 p2 = p1 + ray_length * new Vector2((float)Math.Sin((double)drawRayAngle), (float)Math.Cos((double)drawRayAngle));
                Vector2 p2 = new Vector2(0f);

                

                RayCastInput input;
                input.p1 = p1;
                //input.p2 = p2;
                input.maxFraction = max_fraction;

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
                                        //p3 = polygon.GetVertex(curVert);
                                        p2 = polygon.GetVertex(curVert);
                                        //Vector2 p3edge = p2 - p1;
                                        //float p3angle = currentRayAngle - rotation - light_spread_angle / 2 + (float)Math.PI;
                                        //p2 = p1 + ray_length * new Vector2((float)Math.Sin((double)p3angle), (float)Math.Cos((double)p3angle));
                                        input.p2 = p2;
                                        //p2 = p3;

                                        //RayCastOutput output;

                                        //closestFraction = rayCast(input);

                                        RayCastResult RayCastReport;

                                        
                                        //game.world.RayCast(RayCastReport, p1, p2);
                                        

                                        Vector2 intersectPoint = p1 + closestFraction * (p2 - p1);

                                        if (game.input.DevMode)
                                            game.drawLine(p1, intersectPoint, (Color.White));


                                        //bool RayCast = f.RayCast(out output,ref input, 0);
                                        /*

                                        if (!f.RayCast(out output, ref input, 0))
                                        {
                                            f = f.GetNext();
                                            continue;
                                        }
                                        if (output.fraction < closestFraction)
                                        {
                                            closestFraction = output.fraction;
                                            intersectionNormal = output.normal;
                                        }
                                         */
                                        // do something
                                    }

                                }
                                else if (f.GetShape() is EdgeShape)
                                {
                                    //EdgeShape* edge = (EdgeShape)f.GetShape();
                                }





                                
                                f = f.GetNext();
                            }
                        }
                    b = b.GetNext();
                //}

                
                /*
                Vector3[] Triangle = new Vector3[3];

                Triangle[0] = new Vector3 (p1.X, p1.Y, 0);
                Triangle[1] = new Vector3 (p2.X, p2.Y, 0);
                Triangle[2] = new Vector3 (screen_position.X, screen_position.Y, 0);

                VertexBuffer vertexBuffer;

                vertexBuffer = new VertexBuffer(game.gDevice,
                    Triangle.VertexDeclaration,

                    );

                game.gDevice.SetVertexBuffer(vertexBuffer);
                game.gDevice.DrawPrimitives( PrimitiveType.TriangleStrip, 0, 1);
                */
                

            }

            

            //base.Draw(gameTime);
        }

        float rayCast(RayCastInput input)
        {
            float closestFraction = 1;
            Body b = game.world.GetBodyList();
            while (b != null)
            {
                if (b.GetUserData() is PhysicsObject)
                {
                    PhysicsObject testObject = (PhysicsObject)b.GetUserData();

                    if (testObject.cast_shadow)
                    {
                        Fixture f = b.GetFixtureList();
                        while (f != null)
                        {
                            //p3 = polygon.GetVertex(curVert);
                            //p3 = polygon.GetVertex(curVert);
                            //input.p2 = p3;

                            RayCastOutput output;
                            //bool RayCast = f.RayCast(out output,ref input, 0);

                            //adjust our positions based on the fixture's position
                            RayCastInput adjustedInput = input;
                            adjustedInput.p1 -= b.GetPosition();
                            adjustedInput.p2 -= b.GetPosition();

                            

                            if (!f.RayCast(out output, ref adjustedInput, 0))
                            {
                                f = f.GetNext();
                                continue;
                            }
                            if (output.fraction < closestFraction)
                            {
                                closestFraction = output.fraction;
                                intersectionNormal = output.normal;
                            }
                            // do something
                            f = f.GetNext();
                        }
                    }
                }
                b = b.GetNext();
            }
            return closestFraction;
        }

    }
}
