using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using NLua;
using Box2D.XNA;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Comatose
{
    class Map : PhysicsObject
    {
        public Map(ComatoseGame gm) : base(gm) {
            
        }

        public override void sprite(string path)
        {
            base.sprite(path);
            shape("none");
        }

        List<Vector2> vertexChain = new List<Vector2>();

        public void beginChain()
        {
            vertexChain.Clear();
        }

        public void addVertex(float x, float y)
        {
            vertexChain.Add(new Vector2(x, y));
        }

        public void endChain(bool looped)
        {

        }
        
    }
}
