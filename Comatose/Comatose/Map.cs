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
            body.SetType(BodyType.Static);
        }

        public bool debugdraw = false;

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            if (debugdraw)
            {
                Fixture fixture = body.GetFixtureList();
                while (fixture != null)
                {
                    EdgeShape shape = (EdgeShape) fixture.GetShape();
                    game.drawLine(shape._vertex1, shape._vertex2, Color.Blue, Color.LightBlue);
                    fixture = fixture.GetNext();
                }
            }
        }

        public override void sprite(string path)
        {
            base.sprite("../maps/"+path);
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
            //add a *buncha* fixtures
            for (int i = 0; i < vertexChain.Count - 1; i++)
            {
                EdgeShape shape = new EdgeShape();
                shape.Set(vertexChain[i], vertexChain[i + 1]);
                
                //handle ghost verticies
                if (i > 0)
                {
                    shape._hasVertex0 = true;
                    shape._vertex0 = vertexChain[i - 1];
                }
                else if (looped)
                {
                    shape._hasVertex0 = true;
                    shape._vertex0 = vertexChain[vertexChain.Count - 1];
                }

                if (i + 2 < vertexChain.Count)
                {
                    shape._hasVertex3 = true;
                    shape._vertex3 = vertexChain[i + 2];
                }
                else if (looped)
                {
                    shape._hasVertex3 = true;
                    shape._vertex3 = vertexChain[0];
                }

                FixtureDef def = new FixtureDef();
                def.shape = shape;
                def.friction = 0.0f;
                def.density = 0.0f;

                body.CreateFixture(def);
            }

            //add an extra edge if this is looped
            if (looped)
            {
                EdgeShape shape = new EdgeShape();
                shape.Set(vertexChain[0], vertexChain[vertexChain.Count - 1]);
                shape._hasVertex0 = true;
                shape._vertex0 = vertexChain[1];
                shape._hasVertex3 = true;
                shape._vertex3 = vertexChain[vertexChain.Count - 2];

                FixtureDef def = new FixtureDef();
                def.shape = shape;
                def.friction = 0.0f;
                def.density = 0.0f;

                body.CreateFixture(def);
            }

            vertexChain.Clear();
        }

        public void resetCollision()
        {
            game.world.DestroyBody(body);

            BodyDef def = new BodyDef();
            def.type = BodyType.Static;
            def.position = new Vector2(0.0f);
            body = game.world.CreateBody(def);
        }
    }
}
