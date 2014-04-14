print("Lua Started...")
debug = true
debugpath = "../../../"

--Object Inheretance; handles derived classes in a cleanish manner
function inherits(parent_class)
	local new_class = {}
	local base = parent_class

	if base then
		setmetatable(new_class, {__index=base})
	end
	
	function new_class.create(original)
		local o = original or {}
		local new_mt = {__index=new_class}
		setmetatable(o, new_mt)
		if (base and base.create) then
			--call the parent class's constructor
			o = base.create(o)
		end
		
		return o
	end

	return new_class
end

GameObject = {}

function process_defaults(o, keep)
	if not type(o) == "table" then
		return
	end
	
	--important: do this for parents
	if (getmetatable(o) and getmetatable(o).__index) then
		process_defaults(getmetatable(o).__index, true)
	end
	
	for k, v in pairs(o) do
		if object_to_bind[k] and type(object_to_bind[k]) ~= "string" then
			object_to_bind[k] = v
			if not keep then
				o[k] = nil
			end
		end
	end
end

function process_metatables(o)
	--attempt fun things!
	local mt = getmetatable(o) or {}
	mt.__newindex = function(t, k, v)
		if k ~= "object" and t.object and type(t.object[k]) ~= "string" then
			t.object[k] = v
		else
			--print("New index: ", k)
			rawset(t, k, v)
		end
	end

	--weird things now
	o.parentclass = mt.__index
	mt.__index = function(t, k)
		local object = rawget(t, "object")
		if type(object[k]) ~= "string" then
			if type(object[k]) == "userdata" then
				return function(self, ...)
					return object[k](object, ...)
				end
			else
				return object[k]
			end
		end
		local parentclass = rawget(t, "parentclass")
		if type(parentclass) == "table" then
			if k == "released_art" then
				print("RELEASED PARENT")
				print(parentclass[k])
				print(parentclass)
			end
			return parentclass[k]
		end
		return rawget(t, k)
	end
	setmetatable(o,mt)
end

function GameObject.create(original, classname)
	classname = classname or "GameObject"
	--print("Spawning an object with parent: ", getmetatable(original).__index)

	local spawn_id = GameEngine:spawn(classname) --returns the assigned object ID, which we need to keep track of

	local o = original or {}
	--print("Spawned object ID:" .. spawn_id)

	--set defaults; any keys that exist in the C# class will be set from their values in the original
	--object
	process_defaults(o)

	--print("Successfully processed defaults")

	o.object = object_to_bind
	o.body = body_to_bind
	
	process_metatables(o)

	objects[spawn_id] = o

	if o.init then
		o:init()
	end

	return o
end

function GameObject:destroy()
	self.dead = true
end

PhysicsObject = {destroy=GameObject.destroy,create=function(original) return GameObject.create(original, "PhysicsObject") end}
LightSource = {destroy=GameObject.destroy,create=function(original) return GameObject.create(original, "LightSource") end}
Map = {destroy=GameObject.destroy,create=function(original) return GameObject.create(original, "Map") end}
TextBox = {destroy=GameObject.destroy,create=function(original) return GameObject.create(original, "TextBox") end}
AI = {destroy=GameObject.destroy,create=function(original) return GameObject.create(original, "AI") end}
Waypoint = {destroy=GameObject.destroy,create=function(original) return GameObject.create(original, "Waypoint") end}

function destroyObjects()
	for k,v in pairs(objects) do
		if v.dead then
			objects[v.ID()] = nil --remove this object from the update table
			--v:destroyObject() --gameengine call
			GameEngine:destroy(v.ID())
		end
	end
end

--table for GameEngine stuff
--GameEngine = {}

--global collection of all game objects
--TODO: Do we really need tilemaps and objects to be separated?
objects = {}

processEvent = function(event)
	for k, v in pairs(objects) do
		if objects[k][event] then
			objects[k][event](v)
		end
	end

	if stage[event] then
		stage[event](stage)
	end

	--debug code
	if event == "update" then
		if keys_up.F9 then
			--reload the current level
			loadstage(current_stage)
		end
		if keys_up.F3 then
			GameEngine.toggleDebug()
		end
	end
end

--Collision handling
function processCollision(aID, bID)
	--make sure a collision handler and the objects themselves actually exist
	if objects[aID] and objects[aID].handleCollision and objects[bID] then
		objects[aID]:handleCollision(objects[bID])
	end
end

--These are filled by the game engine
prev_keys_held = {}
keys_held = {}

prev_gamepad_held = {}
gamepad_held = {}

gamepad_left = {}
gamepad_right = {}

function vector_length(vector)
	return math.sqrt(vector.x * vector.x + vector.y * vector.y)
end

function vector_normal(vector)
	local result = {}
	local length = vector_length(vector)
	result.x = vector.x / length
	result.y = vector.y / length
	return result
end

gamepad_left.length = vector_length
gamepad_right.length = vector_length
gamepad_left.normal = vector_normal
gamepad_right.normal = vector_normal

mouse = {}

--These here let us do *sane things* with key polling
_key_down = function(t, k)
	return keys_held[k] and not prev_keys_held[k]
end

_gamepad_down = function(t, k)
	return gamepad_held[k] and not prev_gamepad_held[k]
end

_key_up = function(t, k)
	return prev_keys_held[k] and not keys_held[k]
end

_gamepad_up = function(t, k)
	return prev_gamepad_held[k] and not gamepad_held[k]
end

keys_down = setmetatable({}, {__index=_key_down})
gamepad_down = setmetatable({}, {__index=_gamepad_down})
keys_up = setmetatable({}, {__index=_key_up})
gamepad_up = setmetatable({}, {__index=_gamepad_up})

--this is filled by the game engine, and is used in debug mode for restarts
current_stage = ""

--bring in some happy things from lua tables
dofile("lua/savetable.lua")

--this is used by the level editor
registered_objects = {}

--global stuff goes here  (mostly update related functions)
stage = {}


debugprint = function(...)
	local output = ""
	local arg = {...}
	for k,v in pairs(arg) do
		output = output .. tostring(v)
	end
	GameEngine:consoleWriteLn(output)
end

--print = debugprint

function loadstage(filename)
	GameEngine:loadStage(filename)
end

function loadlevel(filename)
	GameEngine:loadLevel(filename)
end