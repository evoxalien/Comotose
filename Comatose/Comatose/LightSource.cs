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
        public int rays_to_cast = 64;
        public float max_fraction = 1;
        
        private Vector2 intersectionNormal = new Vector2(0, 0);

        float angle = 0;

        public bool isIlluminating(int targetID) 
        {
            if (game.game_objects[targetID] is PhysicsObject)
            {
                PhysicsObject target = (PhysicsObject)game.game_objects[targetID];

                if (target.distanceFrom(x, y) <= ray_length && game.hasVectorLineOfSight(body.GetPosition(), target.body.GetPosition())) {
                    //make sure this is inside the "cone" for light sources that are not 360 degrees

                    angle = (float)Math.Atan2(target.y - y, target.x - x) - body.GetAngle() + (float)Math.PI / 2;
                    while (angle < 0) {
                        angle += (float)Math.PI * 2;
                    }
                    while (angle > Math.PI * 2) {
                        angle -= (float)Math.PI * 2;
                    }

                    if (angle < light_spread_angle / 2 || angle > Math.PI * 2 - light_spread_angle / 2) {
                        //Console.WriteLine("success!");
                        return true;
                    }

                }
                else {
                    //Console.WriteLine("distance check failed");
                }
            }

            //Console.WriteLine("angle: " + angle);

            return false;
        }

        public LightSource(ComatoseGame gm) : base(gm)
        {
            //body.SetActive(false);
            buffer = new VertexBuffer(game.GraphicsDevice, typeof(VertexPositionColor), _max_rays * 3, BufferUsage.None);

            light_shader = new BasicEffect(game.GraphicsDevice);
            light_shader.Projection = Matrix.CreateOrthographicOffCenter(0, 128, 72, 0, 1, -1);
            light_shader.VertexColorEnabled = true;

            //add a dummy physics object to the light, to allow it to do things
            body.SetType(BodyType.Dynamic);

            FixtureDef fdef = new FixtureDef();
            fdef.density = 0.1f;
            fdef.friction = 0.0f;

            CircleShape circle = new CircleShape();
            circle._p = new Vector2(0);
            circle._radius = (float)0.1f;
            fdef.shape = circle;

            fixture = body.CreateFixture(fdef);
            body.ResetMassData();

            layer = "light";
        }

        const int _max_rays = 1000;

        VertexBuffer buffer;
        BasicEffect light_shader;

        public override void Draw(GameTime gameTime)
        {
            //position(body.GetPosition().X * game.physics_scale, body.GetPosition().Y * game.physics_scale);

            Vector2 light_origin = new Vector2(x,y);
            float current_rotation = body.GetAngle();
            while (current_rotation < 0)
            {
                current_rotation += (float)Math.PI * 2;
            }
            current_rotation = current_rotation % ((float)Math.PI * 2);

            //thing!
            rotation = current_rotation;

            List<Vector2> testPoints = new List<Vector2>();
            SortedList<float, Vector2> intersectionPoints = new SortedList<float, Vector2>();

            //firstly, cast out 9 rays around the circle/cone; this will define the base shape of this light
            //and ensure that lack of collision targets doesn't lead to rendering holes
            for (int i = -rays_to_cast / 2; i <= rays_to_cast / 2; i++)
            {
                Vector2 target = Vector2.Transform(new Vector2(0, -ray_length), Matrix.CreateRotationZ((light_spread_angle / rays_to_cast) * i + current_rotation)) + light_origin;
                testPoints.Add(target);
            }

            //Gather a list of all test points in the scene
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
                                    //transform this point based on the body transforms
                                    Vector2 target = Vector2.Transform(polygon.GetVertex(curVert), Matrix.CreateRotationZ(b.GetAngle())) + b.GetPosition();

                                    if (Vector2.DistanceSquared(light_origin, target) <= ray_length * ray_length * 2)
                                    {
                                        float ray_angle = (float)Math.Atan2((target - light_origin).Y, (target - light_origin).X) + (float)Math.PI / 2;
                                        float light_min = current_rotation - light_spread_angle / 2;
                                        float light_max = current_rotation + light_spread_angle / 2;

                                        while (ray_angle < light_min) {
                                            ray_angle += (float)Math.PI * 2;
                                        }

                                        if (ray_angle < light_max) {
                                            //Add it to the list for processing
                                            testPoints.Add(target);
                                        }
                                    }
                                }
                            }
                            else if (f.GetShape() is EdgeShape)
                            {
                                //Do the same thing, except for edge shapes

                                //only v1 and v2 will count here
                                EdgeShape line = (EdgeShape)f.GetShape();

                                Vector2 target1 = Vector2.Transform(line._vertex1, Matrix.CreateRotationZ(b.GetAngle())) + b.GetPosition();
                                if (Vector2.DistanceSquared(light_origin, target1) <= ray_length * ray_length * 2)
                                {
                                    float ray_angle = (float)Math.Atan2((target1 - light_origin).Y, (target1 - light_origin).X) + (float)Math.PI / 2;
                                    float light_min = current_rotation - light_spread_angle / 2;
                                    float light_max = current_rotation + light_spread_angle / 2;

                                    while (ray_angle < light_min) {
                                        ray_angle += (float)Math.PI * 2;
                                    }

                                    if (ray_angle < light_max) {
                                        testPoints.Add(target1);
                                    }
                                }

                                Vector2 target2 = Vector2.Transform(line._vertex2, Matrix.CreateRotationZ(b.GetAngle())) + b.GetPosition();
                                if (Vector2.DistanceSquared(light_origin, target2) <= ray_length * ray_length * 2)
                                {
                                    float ray_angle = (float)Math.Atan2((target2 - light_origin).Y, (target2 - light_origin).X) + (float)Math.PI / 2;
                                    float light_min = current_rotation - light_spread_angle / 2;
                                    float light_max = current_rotation + light_spread_angle / 2;

                                    while (ray_angle < light_min) {
                                        ray_angle += (float)Math.PI * 2;
                                    }

                                    if (ray_angle < light_max) {
                                        testPoints.Add(target2);
                                    }
                                }
                            }
                                
                            f = f.GetNext();
                        }
                    }
                b = b.GetNext();
            }

            //For every test point, calculate the closest intersection
            foreach (Vector2 point in testPoints)
            {
                Vector2 target = point;

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

            //now, attempt (poorly) to create a triangle mesh based on our hopefully sorted points
            GraphicsDevice gd = game.GraphicsDevice;

            //VertexPositionColor[] vertex_list = new VertexPositionColor[_max_rays * 3];

            ArrayList vertex_list = new ArrayList();

            Color originColor = sprite_color;
            Color outsideColor = Color.FromNonPremultiplied(sprite_color.R, sprite_color.G, sprite_color.B, 0);
            Color intersectionColor;

            bool first = true;
            foreach (var intersection in intersectionPoints)
            {
                intersectionColor = Color.Lerp(originColor, outsideColor, Math.Min(1.0f, ((intersection.Value - light_origin).Length() / (float)ray_length)));
                if (!first)
                {
                    //finish the last triangle
                    vertex_list.Add(new VertexPositionColor(new Vector3(intersection.Value.X, intersection.Value.Y, 0), intersectionColor));
                }
                //start a new triangle leading with this edge
                vertex_list.Add(new VertexPositionColor(new Vector3(light_origin.X, light_origin.Y, 0), sprite_color));
                vertex_list.Add(new VertexPositionColor(new Vector3(intersection.Value.X, intersection.Value.Y, 0), intersectionColor));

                first = false;
            }

            intersectionColor = Color.Lerp(originColor, outsideColor, Math.Min(1.0f, ((intersectionPoints.First().Value - light_origin).Length() / (float)ray_length)));

            //finish the very last triangle (it loops to the beginning)
            vertex_list.Add(new VertexPositionColor(new Vector3(intersectionPoints.First().Value.X, intersectionPoints.First().Value.Y, 0), intersectionColor));

            buffer.SetData<VertexPositionColor>((VertexPositionColor[])vertex_list.ToArray(typeof(VertexPositionColor)));
            gd.SetVertexBuffer(buffer);

            //make sure the light shader is set up
            light_shader.World = Matrix.CreateTranslation(new Vector3(-game.camera.X / 10, -game.camera.Y / 10, 0));
            light_shader.CurrentTechnique.Passes[0].Apply();

            //draw them primitives!
            gd.DrawPrimitives(PrimitiveType.TriangleList, 0, intersectionPoints.Count);

            
        }

        float min_distance;

        float ReportFixture(Fixture fixture, Vector2 point, Vector2 normal, float fraction)
        {
            if (fraction < min_distance)
            {
                //check shadow logic
                if (((PhysicsObject)fixture.GetBody().GetUserData()).cast_shadow) {
                    min_distance = fraction;
                }
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
