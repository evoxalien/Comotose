using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using NLua;
using XNAGameConsole;
using Box2D.XNA;
namespace Comatose
{

    class RayCastResult /*: Box2D.XNA.RayCastCallback */
    {
        float min_distance = 5f;
        Fixture result_fixture;
        Vector2 result_point;
        Vector2 result_normal;

        public RayCastResult()
        {
            result_fixture = null;
        }

        float ReportFixture(Fixture fixture, Vector2 point, Vector2 normal, float fraction)
        {
            result_fixture = fixture;
            result_point = point;
            result_normal = normal;

            return fraction;
        }
    }
}
