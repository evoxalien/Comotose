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
    public class ComatoseGame : Microsoft.Xna.Framework.Game
    {
        public GraphicsDeviceManager graphics;
        public GraphicsDevice gDevice;
        public SpriteBatch spriteBatch;
        public SpriteBatch debugBatch;
        public Dictionary<int, GameObject> game_objects = new Dictionary<int, GameObject>();
        public Dictionary<int, Waypoint> waypoints = new Dictionary<int, Waypoint>();
        public Input input;

        public Effects Effects;
        public ParticleManager ParticleManager;

        public Random rand = new Random();

        public Texture2D pixel;

        public float physics_scale = 10f;
        public float gravity = 0f;

        public Lua vm;
        public GameConsole console;
        public World world;

        public Vector2 camera = new Vector2(0);

        Song current_song;

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

        ContactListener listener;

        protected void InitLua()
        {
            //clear out the current gamestate
            vm = new Lua();
            world = new World(new Vector2(0f, gravity), true);
            world.DebugDraw = new cDebugDraw(this);
            world.DebugDraw.Flags = DebugDrawFlags.Shape;

            listener = new ContactListener(this, vm);
            world.ContactListener = listener;

            //bind in the game object; this means that any public functions
            //or variables will  be accessible by lua, assuming lua understands how
            //to call / manipulate them
            vm["GameEngine"] = this;
            vm["Input"] = input;
            vm["Particle"] = ParticleManager;
            vm["Effect"] = Effects;
            vm.DoFile("lua/main.lua");

            loadAllObjects();
        }

        public void playMusic(string filename)
        {
            current_song = Content.Load<Song>("music/" + filename);
            MediaPlayer.Volume = 0.5f;
            MediaPlayer.Play(current_song);
        }

        public void playSound(string filename)
        {
            Content.Load<SoundEffect>("sounds/" + filename).Play();
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
            MediaPlayer.Stop();
            //delete all existing objects
            game_objects.Clear();
            waypoints.Clear();
            camera = new Vector2(0);

            //Initialize lua
            InitLua();

            vm.DoFile("lua/stages/" + filename + ".lua");
        }

        string levelToLoad = "";
        public void loadLevel(string filename)
        {
            MediaPlayer.Stop();
            levelToLoad = filename;
        }

        string levelToEdit = "";
        public void editLevel(string filename)
        {
            levelToEdit = filename;
        }

        string mapToEdit = "";
        public void editMap(string filename)
        {
            mapToEdit = filename;
        }

        protected void realLoadLevel()
        {
            loadStage("levelloader");
            vm.DoString("load(\"" + levelToLoad + "\")");
            levelToLoad = "";
        }

        protected void realEditLevel()
        {
            loadStage("leveleditor");
            vm.DoString("load(\"" + levelToEdit + "\")");
            levelToEdit = "";
        }

        protected void realEditMap()
        {
            loadStage("mapeditor");
            vm.DoString("load(\"" + mapToEdit + "\")");
            mapToEdit = "";
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
                case "TextBox":
                    new_object = (GameObject)new TextBox(this);
                    break;
                case "Audio":
                    new_object = (GameObject)new Audio(this);
                    break;
                case "AI":
                    new_object = (GameObject)new AI(this);
                    break;
                case "Waypoint":
                    new_object = (GameObject)new Waypoint(this);
                    waypoints[new_object.ID()] = (Waypoint)new_object;
                    break;
                default:
                    throw (new NotImplementedException("Spawn Class Not Found! -_-"));
            }

            game_objects[new_object.ID()] = new_object;
            return new_object.ID();
        }

        public void destroy(int objectID)
        {
            //simply pull it from game_objects, and the GC will delete it
            if (game_objects[objectID] is PhysicsObject)
            {
                world.DestroyBody(((PhysicsObject)game_objects[objectID]).body);
            }
            game_objects.Remove(objectID);
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

        #region Initialization
        public ComatoseGame()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            input = new Input(this);
        }

        protected override void Initialize()
        {
            graphics.PreferredBackBufferWidth = 1280;
            graphics.PreferredBackBufferHeight = 720;
            this.IsFixedTimeStep = false;
            graphics.ApplyChanges();

            //MODIFYING THE FIRST VALUE WILL LIMIT PARTICLE COUNT
            ParticleManager = new ParticleManager(1024 * 24, ParticleState.UpdateParticle, this);
            Effects = new Effects(this);
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
            //loadStage("mapeditor");
            //loadStage("test");
            //loadStage("textboxtest");
            //loadStage("aitest");
            loadLevel("debugroom");


        }
        #endregion

        #region Drawing Utilities
        public Vector2 screenCoordinates(Vector2 physics_coordinates)
        {
            return new Vector2((physics_coordinates.X * physics_scale) - camera.X, (physics_coordinates.Y * physics_scale) - camera.Y);
        }

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

        float last_ray_distance;

        float ReportFixture(Fixture fixture, Vector2 point, Vector2 normal, float fraction)
        {
            if (fraction < last_ray_distance)
            {
                //check shadow logic
                if (((PhysicsObject)fixture.GetBody().GetUserData()).cast_shadow)
                {
                    last_ray_distance = fraction;
                }
            }

            return 1;
        }

        public bool hasLineOfSight(int objectA_id, int objectB_id)
        {

            if (game_objects[objectA_id] is PhysicsObject && game_objects[objectB_id] is PhysicsObject)
            {
                PhysicsObject objectA = (PhysicsObject)game_objects[objectA_id];
                PhysicsObject objectB = (PhysicsObject)game_objects[objectB_id];

                Vector2 start = objectA.body.GetPosition();
                Vector2 end = objectB.body.GetPosition();

                //note: this intentionally uses object centers. If you don't like it, code something better;
                //the correct solution to this is HARD.
                last_ray_distance = 1.0f;
                world.RayCast(ReportFixture, start, end);
                if (last_ray_distance < 1.0)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            return false;
        }

        public bool hasVectorLineOfSight(Vector2 start, Vector2 end)
        {
            last_ray_distance = 1.0f;
            world.RayCast(ReportFixture, start, end);
            if (last_ray_distance < 1.0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        public void drawLine(Vector2 start, Vector2 end, Color color)
        {
            start = screenCoordinates(start);
            end = screenCoordinates(end);
            Vector2 edge = end - start;
            float angle = (float)Math.Atan2(edge.Y, edge.X);

            debugBatch.Draw(pixel,
                new Rectangle((int)(start.X), (int)(start.Y), (int)(edge.Length()), 1),
                null,
                color,
                angle,
                new Vector2(0),
                SpriteEffects.None,
                .5f);
        }

        public bool isIlluminated(int targetID) 
        {
            if (game_objects[targetID] is PhysicsObject) {
                PhysicsObject target = (PhysicsObject)game_objects[targetID];
                foreach (var o in game_objects) {
                    if (o.Value is LightSource) {
                        LightSource light = (LightSource)o.Value;
                        if (light.isIlluminating(targetID)) {
                            return true;
                        }
                    }
                }
            }

            return false;
        }
        #endregion

        #region Game Loop
        protected override void Update(GameTime gameTime)
        {
            if (levelToLoad != "")
            {
                realLoadLevel();
            }

            if (levelToEdit != "")
            {
                realEditLevel();
            }

            if (mapToEdit != "")
            {
                realEditMap();
            }

            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            input.Update();

            //process world stuffs
            world.Step(1.0f / 60.0f, 6, 2);
            listener.HandleEvents(); //process collisions engineside as needed
            vm.DoString("destroyObjects()"); //cleanup any objects that need to die
            vm.DoString("processEvent(\"everyFrame\")");


            ParticleManager.Update();

            base.Update(gameTime);
        }

        public SpriteBatch gameObjectBatch;
        Texture2D lightMap;
        protected override void Draw(GameTime gameTime)
        {

            //section: render to texture for light sources
            RenderTarget2D lightTarget = new RenderTarget2D(GraphicsDevice, 1280, 720);
            GraphicsDevice.SetRenderTarget(lightTarget);
            GraphicsDevice.Clear(Color.FromNonPremultiplied(24,24,24,255)); //default ambient light

            debugBatch.Begin(SpriteSortMode.FrontToBack, BlendState.Additive);
            gameObjectBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);
            //draw lights to this new texture
            foreach (var o in game_objects)
            {
                if (o.Value.layer == "light")
                {
                    o.Value.Draw(gameTime);
                }
            }
            gameObjectBatch.End();

            //switch back to the main device for drawing the scene
            GraphicsDevice.SetRenderTarget(null);
            lightMap = (Texture2D)lightTarget;

            //Draw all the main objects in the scene
            GraphicsDevice.Clear(Color.Black);
            gameObjectBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend);
            foreach (var o in game_objects)
            {
                if (o.Value.layer == "gameobject") //only draw non-lights here
                {
                    o.Value.Draw(gameTime);
                }
            }
            
            gameObjectBatch.End();

            //Set up a multiply state for the light layer
            BlendState multiply = new BlendState();
            multiply.ColorBlendFunction = BlendFunction.Add;
            multiply.ColorSourceBlend = Blend.DestinationColor;
            multiply.AlphaSourceBlend = Blend.DestinationColor;
            multiply.ColorDestinationBlend = Blend.Zero;

            //Draw lights; this will multiply the light layer with everything drawn before, and properly darken unlit areas
            gameObjectBatch.Begin(SpriteSortMode.BackToFront, multiply);
            gameObjectBatch.Draw(lightMap, new Vector2(0), Color.White);
            gameObjectBatch.End();

            //Draw unlit objects in the game world (these objects ignore lighting)
            gameObjectBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend);
            foreach (var o in game_objects)
            {
                if (o.Value.layer == "unlit") //only draw non-lights here
                {
                    o.Value.Draw(gameTime);
                }
            }
            gameObjectBatch.End();

            //Draw all particles using a new batch; this forces particles to be drawn on top of all other sprites,
            //plus they get a different blend mode for fancy effects
            gameObjectBatch.Begin(SpriteSortMode.BackToFront, BlendState.Additive);
            ParticleManager.Draw(gameObjectBatch);
            gameObjectBatch.End();

            //Draw UI
            gameObjectBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend);
            foreach (var o in game_objects)
            {
                if (o.Value.layer == "ui") //only draw non-lights here
                {
                    o.Value.Draw(gameTime);
                }
            }
            gameObjectBatch.End();

            //finally draw the debug stuff, if needed
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
