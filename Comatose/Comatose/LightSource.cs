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
        ComatoseGame game;

        public LightSource(ComatoseGame gm) : base(gm)
        {
            body.SetActive(false);
            game = gm;
        }


        public override void Draw(GameTime gameTime)
        {
            position(body.GetPosition().X * game.physics_scale, body.GetPosition().Y * game.physics_scale);
            //rotate(body.GetAngle());

            float currentRayAngle = 0;
            float drawRayAngle = 0;
            for (int i = 1; i <= rays_to_cast && currentRayAngle <= light_spread_angle; i++)
            {
                currentRayAngle += (float)Math.PI * 2 / rays_to_cast;
                drawRayAngle = currentRayAngle - rotation - light_spread_angle / 2 + (float)Math.PI;

                Vector2 p1 = new Vector2(x,y);
                Vector2 p2 = p1 + ray_length * new Vector2((float)Math.Sin((double)drawRayAngle), (float)Math.Cos((double)drawRayAngle));

                Vector2 intersectionNormal = new Vector2( 0 , 0 );

                RayCastInput input;
                input.p1 = p1;
                input.p2 = p2;
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
                                RayCastOutput output;
                                //bool RayCast = f.RayCast(output, input);
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
                                // do something

                                f = f.GetNext();
                            }
                        }
                    b = b.GetNext();
                }

                p2 = p1 + closestFraction * (p2 - p1);

                //game.gDevice.DrawPrimitives( PrimitiveType Triangle

                if (game.input.DevMode)
                    game.drawLine(p1, p2, (Color.White));

            }

            

            //base.Draw(gameTime);
        }

    }
}
