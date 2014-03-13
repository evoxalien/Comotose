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

namespace Comatose {
    public class ComatoseGame : Microsoft.Xna.Framework.Game 
    {
        public GraphicsDeviceManager graphics;
        public GraphicsDevice gDevice;
        public SpriteBatch spriteBatch;
        public SpriteBatch debugBatch;
        public Dictionary<int, GameObject> game_objects = new Dictionary<int,GameObject>();
        public Input input;

        public Texture2D pixel;

        public float physics_scale = 10f;
        public float gravity = 0f;

        public Lua vm;
        GameConsole console;
        public World world;

        public Vector2 camera = new Vector2(0);

        public Vector2 screenCoordinates(Vector2 physics_coordinates)
        {
            return new Vector2((physics_coordinates.X * physics_scale) - camera.X, (physics_coordinates.Y * physics_scale) - camera.Y);
        }

        #region Lua
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

        protected void InitLua()
        {
            //clear out the current gamestate
            vm = new Lua();
            world = new World(new Vector2(0f, gravity), true);
            world.DebugDraw = new cDebugDraw(this);
            world.DebugDraw.Flags = DebugDrawFlags.Shape;

            //bind in the game object; this means that any public functions
            //or variables will  be accessible by lua, assuming lua understands how
            //to call / manipulate them
            vm["GameEngine"] = this;
            vm["Input"] = input;
            vm.DoFile("lua/main.lua");

            loadAllObjects();
        }

        public void loadAllObjects()
        {
            List<string> files = new List<string>(Directory.EnumerateFiles("lua/objects"));
            foreach (var file in files)
            {
                vm.DoFile(file);
            }
        }

        public void loadStage(string filename)
        {
            //delete all existing objects
            game_objects.Clear();
            camera = new Vector2(0);

            //Initialize lua
            InitLua();

            vm.DoFile("lua/stages/" + filename + ".lua");
        }

        string levelToLoad = "";
        public void loadLevel(string filename)
        {
            levelToLoad = filename;
        }

        protected void realLoadLevel() {
            loadStage("levelloader");
            vm.DoString("load(\"" + levelToLoad + "\")");
            levelToLoad = "";
        }

        public int spawn(string classname)
        {
            GameObject new_object;

            switch (classname)
            {
                case "GameObject":
                    new_object = new GameObject(this);
                    break;
                case "PhysicsObject":
                    new_object = (GameObject)new PhysicsObject(this);
                    break;
                case "LightSource":
                    new_object = (GameObject)new LightSource(this);
                    break;
                case "Map":
                    new_object = (GameObject)new Map(this);
                    break; 
                default:
                    throw (new NotImplementedException("Spawn Class Not Found! -_-"));
            }

            game_objects[new_object.ID()] = new_object;
            return new_object.ID();
        }

        public void consoleWriteLn(string message)
        {
            console.WriteLine(message);
        }

        public void setCamera(int x, int y)
        {
            camera = new Vector2(x, y);
        }
        #endregion

        public ComatoseGame() 
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            input = new Input(this);
        }

        #region Initialization

        protected override void Initialize() 
        {
            

            graphics.PreferredBackBufferWidth = 1280;
            graphics.PreferredBackBufferHeight = 720;
            graphics.ApplyChanges();

            base.Initialize();
        }

        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        protected override void LoadContent() 
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            debugBatch = new SpriteBatch(GraphicsDevice);
            pixel = Content.Load<Texture2D>("art/pixel");

            gameObjectBatch = new SpriteBatch(GraphicsDevice);

            console = new GameConsole(this, spriteBatch);
            console.AddCommand(new LuaCommand(this));
            console.Options.OpenOnWrite = false;

            //load the test level
            //loadStage("leveleditor");
            loadStage("mapeditor");

        }
        #endregion

        public void drawLine(Vector2 start, Vector2 end, Color startColor, Color endColor)
        {
            //divide this line into segments and fake a gradient. Because lazy, and debugging.
            int segments = 32;
            for (int i = 0; i < segments - 1; i++)
            {
                float a = (float)(segments - i) / (float)segments;
                float b = (float)i / (float)segments;
                
                float a1 = (float)(segments - (i + 1)) / (float)segments;
                float b1 = (float)(i + 1) / (float)segments;

                //blend the colors
                Color finalColor = new Color();
                finalColor.R = (byte)(startColor.R * a + endColor.R * b);
                finalColor.G = (byte)(startColor.G * a + endColor.G * b);
                finalColor.B = (byte)(startColor.B * a + endColor.B * b);
                finalColor.A = (byte)(startColor.A * a + endColor.A * b);

                drawLine(start * a + end * b, start * a1 + end * b1, finalColor);
            }
        }

        public void drawLine(Vector2 start, Vector2 end, Color color)
        {
            start = screenCoordinates(start);
            end = screenCoordinates(end);
            Vector2 edge = end - start;
            float angle = (float)Math.Atan2(edge.Y, edge.X);

            debugBatch.Draw(pixel,
                new Rectangle((int)(start.X), (int)(start.Y), (int)(edge.Length()), 2),
                null,
                color,
                angle,
                new Vector2(0),
                SpriteEffects.None,
                .5f);
        }

        #region Game Loop
        protected override void Update(GameTime gameTime) 
        {
            if (levelToLoad != "")
            {
                realLoadLevel();
            }

            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            input.Update();

            //process world stuffs
            world.Step(1.0f / 60.0f, 6, 2);
            //listener.HandleEvents(); //process collisions engineside as needed
            vm.DoString("destroyObjects()"); //cleanup any objects that need to die
            vm.DoString("processEvent(\"everyFrame\")");
            
            base.Update(gameTime);
        }

        public SpriteBatch gameObjectBatch;

        protected override void Draw(GameTime gameTime) 
        {
            debugBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend);
            GraphicsDevice.Clear(Color.White);

            // TODO: Add your drawing code here
            gameObjectBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend);
            foreach (var o in game_objects) {
                o.Value.Draw(gameTime);
            }
            gameObjectBatch.End();

            //now draw the debug stuff, if needed
            if (input.DevMode)
            {

                ((cDebugDraw)world.DebugDraw).DrawBackground();
                world.DrawDebugData();

            }
            debugBatch.End();

            base.Draw(gameTime);
        }
        #endregion


    }
}
