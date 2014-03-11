using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using NLua;
using XNAGameConsole;

namespace Comatose {
    public class ComatoseGame : Microsoft.Xna.Framework.Game 
    {
        GraphicsDeviceManager graphics;
        public SpriteBatch spriteBatch;
        Dictionary<int, GameObject> game_objects = new Dictionary<int,GameObject>();
        Input input = new Input();

        public Lua vm;
        GameConsole console;

        private class LuaCommand : IConsoleCommand
        {
            public string Name
            {
                get { return "lua"; }
            }

            public string Description
            {
                get { return "lua <command>: runs the command in the lua VM"; }
            }

            private ComatoseGame game;
            public LuaCommand(ComatoseGame gm)
            {
                this.game = gm;
            }

            public string Execute(string[] arguments)
            {
                string commandString = arguments[0];
                for (int i = 1; i < arguments.Length; i++)
                {
                    commandString += " " + arguments[i];
                }

                try
                {
                    game.vm.DoString(commandString);
                    return "";
                }
                catch (NLua.Exceptions.LuaException e)
                {
                    return "ERROR: " + e.Message;
                }
            }
        }

        public ComatoseGame() 
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected void InitLua()
        {
            vm = new Lua();

            //bind in the game object; this means that any public functions
            //or variables will  be accessible by lua, assuming lua understands how
            //to call / manipulate them
            vm["GameEngine"] = this;
            vm["Input"] = input;
            vm.DoFile("lua/main.lua");
        }

        protected override void Initialize() 
        {
            base.Initialize();
        }

        public void consoleWriteLn(string message)
        {
            console.WriteLine(message);
        }

        protected override void LoadContent() 
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);


            // TODO: use this.Content to load your game content here
            console = new GameConsole(this, spriteBatch);
            console.AddCommand(new LuaCommand(this));
            console.Options.OpenOnWrite = false;

            
            //load the test level
            loadStage("test");

        }

        public void loadStage(string filename) 
        {
            //Initialize lua
            InitLua();

            vm.DoFile("lua/stages/" + filename + ".lua");
        }

        protected override void UnloadContent() 
        {
            // TODO: Unload any non ContentManager content here
        }

        protected override void Update(GameTime gameTime) 
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            // TODO: Add your update logic here
            vm.DoString("processEvent(\"everyFrame\")");

            input.Update();
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime) 
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            base.Draw(gameTime);
        }

        public int spawn()
        {
            GameObject new_object = new GameObject(this);
            game_objects[new_object.ID()] = new_object;
            Components.Add(new_object);

            return new_object.ID();
        }
    }
}
