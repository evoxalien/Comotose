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
    class LightSource : PhysicsObject
    {
        public int ray_length = 50;
        public float light_spread_angle = (float)Math.PI * 2;
        //public int rays_to_cast = 640;
        public float max_fraction = 1;
        
        private Vector2 intersectionNormal = new Vector2(0, 0);

        public LightSource(ComatoseGame gm) : base(gm)
        {
            body.SetActive(false);
            buffer = new VertexBuffer(game.GraphicsDevice, typeof(VertexPositionColor), _max_rays * 3, BufferUsage.None);

            light_shader = new BasicEffect(game.GraphicsDevice);
            light_shader.Projection = Matrix.CreateOrthographicOffCenter(0, 128, 72, 0, 1, -1);
            light_shader.VertexColorEnabled = true;
        }

        const int _max_rays = 1000;

        VertexBuffer buffer;
        BasicEffect light_shader;

        public override void Draw(GameTime gameTime)
        {
            //position(body.GetPosition().X * game.physics_scale, body.GetPosition().Y * game.physics_scale);

            Vector2 light_origin = new Vector2(x,y);

            SortedList<float, Vector2> intersectionPoints = new SortedList<float, Vector2>();

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
                                    Vector2 target = polygon.GetVertex(curVert);

                                    //transform this point based on the body transforms
                                    target = Vector2.Transform(target, Matrix.CreateRotationZ(b.GetAngle()));
                                    target += b.GetPosition();

                                    //normalize this point, then multiply it by the length; this will give us a
                                    //line originating from the light source, and cast "toward" this corner point
                                    //that is exactly the distance of the light's normal max range
                                    target -= light_origin;
                                    target.Normalize();
                                    target = target * ray_length;

                                    //cast two more rays at slight angle offsets, to deal with corner edge cases
                                    //(only 2, so everything after this is doubled, because loops are silly for only 2 elements)
                                    Vector2 target_neg = Vector2.Transform(target, Matrix.CreateRotationZ(-0.0001f));
                                    Vector2 target_pos = Vector2.Transform(target, Matrix.CreateRotationZ(0.0001f));

                                    target_neg += light_origin;
                                    target_pos += light_origin;

                                    //perform the ray cast, and figure out what to do about the result
                                    float closestFractionNeg = Math.Min(rayCast(light_origin, target_neg), 1f);
                                    float closestFractionPos = Math.Min(rayCast(light_origin, target_pos), 1f);

                                    Vector2 intersectPointPos = light_origin + closestFractionPos * (target_pos - light_origin);
                                    Vector2 intersectPointNeg = light_origin + closestFractionNeg * (target_neg - light_origin);

                                    if (game.input.DevMode)
                                    {
                                        game.drawLine(light_origin, intersectPointNeg, (Color.White));
                                        game.drawLine(light_origin, intersectPointPos, (Color.White));
                                    }

                                    intersectionPoints[(float)Math.Atan2((float)(intersectPointNeg - light_origin).Y, (float)(intersectPointNeg - light_origin).X)] = intersectPointNeg;
                                    intersectionPoints[(float)Math.Atan2((float)(intersectPointPos - light_origin).Y, (float)(intersectPointPos - light_origin).X)] = intersectPointPos;
                                }
                            }
                            else if (f.GetShape() is EdgeShape)
                            {
                                //Do the same thing, except for edge shapes
                                //TODO: Not fail at this
                            }
                                
                            f = f.GetNext();
                        }
                    }
                b = b.GetNext();
            }

            //now, attempt (poorly) to create a triangle mesh based on our hopefully sorted points
            GraphicsDevice gd = game.GraphicsDevice;

            //VertexPositionColor[] vertex_list = new VertexPositionColor[_max_rays * 3];

            ArrayList vertex_list = new ArrayList();

            bool first = true;
            foreach (var intersection in intersectionPoints)
            {
                if (!first)
                {
                    //finish the last triangle
                    vertex_list.Add(new VertexPositionColor(new Vector3(intersection.Value.X, intersection.Value.Y, 0), Color.White));
                }
                //start a new triangle leading with this edge
                vertex_list.Add(new VertexPositionColor(new Vector3(light_origin.X, light_origin.Y, 0), Color.White));
                vertex_list.Add(new VertexPositionColor(new Vector3(intersection.Value.X, intersection.Value.Y, 0), Color.White));

                first = false;
            }

            //finish the very last triangle (it loops to the beginning)
            vertex_list.Add(new VertexPositionColor(new Vector3(intersectionPoints.First().Value.X, intersectionPoints.First().Value.Y, 0), Color.White));

            buffer.SetData<VertexPositionColor>((VertexPositionColor[])vertex_list.ToArray(typeof(VertexPositionColor)));
            gd.SetVertexBuffer(buffer);

            //make sure the light shader is set up
            light_shader.CurrentTechnique.Passes[0].Apply();

            //draw them primitives!
            gd.DrawPrimitives(PrimitiveType.TriangleList, 0, intersectionPoints.Count);

            
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
