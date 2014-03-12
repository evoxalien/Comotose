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
        public LightSource(ComatoseGame gm) : base(gm)
        {

        }

        public override void Draw(GameTime gameTime)
        {
            //position(body.GetPosition().X * game.physics_scale, body.GetPosition().Y * game.physics_scale);
            //rotate(body.GetAngle());

            base.Draw(gameTime);
        }

    }
}
