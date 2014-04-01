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
    class AI : PhysicsObject
    {
        public enum state { IDLE,SEARCHING,MOVING,ATTACKING}
        private List<Vector2> navmesh=new List<Vector2>();


        //stuff to draw mesh
        /*
        VertexBuffer buffer;
        Color vertexColor = Color.FromNonPremultiplied(255,255,255,0);
        ArrayList vertex_list = new ArrayList();
        BasicEffect nav_shader;
         */


        public AI(ComatoseGame gm)
            : base(gm)
        {


            //nice triangle
            navmesh.Add(new Vector2( 10,10));
            navmesh.Add(new Vector2( 20,10));
            navmesh.Add(new Vector2( 10,20));
            navmesh.Add(new Vector2( 20,20));

        }

        public override void Draw(GameTime gameTime)
        {
            if (game.input.DevMode)
            {
                game.drawLine(navmesh[0], navmesh[1], Color.FromNonPremultiplied(255, 255, 255, 255));
                game.drawLine(navmesh[1], navmesh[2], Color.FromNonPremultiplied(255, 255, 255, 255));
                game.drawLine(navmesh[0], navmesh[2], Color.FromNonPremultiplied(255, 255, 255, 255));
                game.drawLine(navmesh[3], navmesh[0], Color.FromNonPremultiplied(255, 255, 255, 255));
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
