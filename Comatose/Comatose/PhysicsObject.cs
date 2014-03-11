﻿using System;
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
    class PhysicsObject : GameObject
    {
        protected Body body;
        protected Fixture fixture;

        #region Getters / Setters for body properties
        public float x
        {
            get { return body.Position.X; }
            set { body.Position = new Vector2(value, body.Position.Y); body.SetAwake(true); }
        }
        public float y
        {
            get { return body.Position.Y; }
            set { body.Position = new Vector2(body.Position.X, value); body.SetAwake(true); }
        }

        public float vx
        {
            get { return body.GetLinearVelocity().X; }
            set { body.SetLinearVelocity(new Vector2(value, body.GetLinearVelocity().Y)); body.SetAwake(true); }
        }
        public float vy
        {
            get { return body.GetLinearVelocity().Y; }
            set { body.SetLinearVelocity(new Vector2(body.GetLinearVelocity().X, value)); body.SetAwake(true); }
        }

        public float vr
        {
            get { return body.GetAngularVelocity(); }
            set { body.SetAngularVelocity(value); body.SetAwake(true); }
        }

        public bool active
        {
            get { return body.IsActive(); }
            set { body.SetActive(value); }
        }

        public bool fixedRotation
        {
            get { return body.IsFixedRotation(); }
            set { body.SetFixedRotation(value); }
        }

        public bool isSensor
        {
            get { return body.GetFixtureList().IsSensor(); }
            set { body.GetFixtureList().SetSensor(value); }
        }
        #endregion

        public PhysicsObject(ComatoseGame gm)
            : base(gm)
        {
            BodyDef def = new BodyDef();
            def.type = BodyType.Dynamic;
            def.position = new Vector2(0.0f);
            body = game.world.CreateBody(def);

            body.SetUserData(this);
            collision_group = "gameobject";
        }

        #region Collision Groups
        protected string collision_group;
        public void set_group(string gp)
        {
            collision_group = gp;
        }

        public string get_group()
        {
            return collision_group;
        }

        public List<string> collision_targets = new List<string>();

        public void add_target(string target)
        {
            if (!collision_targets.Contains(target))
            {
                collision_targets.Add(target);
                //Console.WriteLine("added collision target: " + target);
                //Console.WriteLine("my collision group: " + collision_group);
            }
        }

        public void remove_target(string target)
        {
            collision_targets.Remove(target);
        }
        #endregion

        #region Physics Properties
        public void body_type(string type)
        {
            switch (type)
            {
                case "static":
                    body.SetType(BodyType.Static);
                    break;
                case "kinematic":
                    body.SetType(BodyType.Kinematic);
                    break;
                case "dynamic":
                    body.SetType(BodyType.Dynamic);
                    break;
                default:
                    Console.WriteLine("ERROR: Bad body type given: " + type);
                    break;
            }
        }

        public void setDensity(float density)
        {
            body.GetFixtureList().SetDensity(density);
            body.ResetMassData();
        }

        public void setFriction(float friction)
        {
            body.GetFixtureList().SetFriction(friction);
            body.ResetMassData();
        }

        public void setRestitution(float restitution)
        {
            body.GetFixtureList().SetRestitution(restitution);
        }
        #endregion

        #region Shape Definitions
        private string current_shape = "box";
        public void shape(string type)
        {
            current_shape = type;
            //delete the existing body
            if (fixture != null)
            {
                body.DestroyFixture(fixture);
            }

            FixtureDef fdef = new FixtureDef();
            fdef.density = 1.0f;
            fdef.friction = 0.3f;

            switch (type)
            {
                case "none":
                    break; //do nothing, leave the body with no fixture
                case "circle":
                    CircleShape circle = new CircleShape();
                    circle._p = new Vector2((float)texture.Width / 20.0f, (float)texture.Height / 20.0f);
                    circle._radius = (float)texture.Width / 20.0f;

                    fdef.shape = circle;

                    fixture = body.CreateFixture(fdef);
                    body.ResetMassData();
                    break;
                case "diamond":
                    //TODO: THIS
                    throw (new NotImplementedException("Diamond Shapes Not Implemented"));
                    break;
                case "box":
                default:
                    PolygonShape box = new PolygonShape();
                    float phys_width = (float)texture.Width / 10.0f;
                    float phys_height = (float)texture.Height / 10.0f;
                    box.SetAsBox(
                        phys_width / 2.0f,
                        phys_height / 2.0f,
                        new Vector2(phys_width / 2.0f, phys_height / 2.0f),
                        0.0f);

                    fdef.shape = box;

                    fixture = body.CreateFixture(fdef);
                    body.ResetMassData();
                    break;
            }
        }
        #endregion

        public override void sprite(string path)
        {
            base.sprite(path);
            shape(current_shape);
        }

        public override void Draw(GameTime gameTime)
        {
            Console.WriteLine("Physics Drew? " + body.GetPosition().X + ", " + body.GetPosition().Y);
            position(body.GetPosition().X * game.physics_scale, body.GetPosition().Y * game.physics_scale);
            rotate(body.GetAngle());

            base.Draw(gameTime);
        }

    }
}
