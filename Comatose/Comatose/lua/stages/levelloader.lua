--note: global "level_name" is passed in by the game engine

--setup some things
stage.triggered = {}

current_filename = ""

current_map = Map.create()
current_map.z_index = 0
--current_map:color(32,32,32,255)

--collision for non-shadow objects here
transparent_map = Map.create()
transparent_map.cast_shadow = false


--cursor, for selecting stuff
cursor = Cursor.create()
cursor:color(0, 255, 255, 255)

--make this a global
current_level = {}

function load_map(filename)
	--load the file
	savedata = persistence.load("lua/maps/"..filename..".data")
	current_map:sprite(savedata.image)

	--load in the collision data
	for k,edge in pairs(savedata.edges) do
		if edge.length > 1 then
			activeMap = current_map
			if edge.transparent then
				activeMap = transparent_map
			end
			activeMap:beginChain()
			for i = 1, edge.length do
				activeMap:addVertex(edge.verticies[i].x, edge.verticies[i].y)
			end
			activeMap:endChain(edge.looped == true)
		end
	end

	current_level.map = filename
end

function trigger_event(event_name)
	load_objects(current_level.objects, event_name)
end

function load_objects(objects, event_name)
	loaded_objects = {}
	for k,v in pairs(objects) do
		--sanity
		if _G[v.class] and event_name == v.event then
			print("Attempting to instanciate a " .. v.class)
			loaded_objects[k] = _G[v.class].create(v.defaults)
			if v.color then
				loaded_objects[k]:color(v.color.r, v.color.g, v.color.b, v.color.a)
			end
			if v.rotation then
				loaded_objects[k]:setRotation(v.rotation)
			end
		else
			print("Error loading object -- bad classname: " .. v.class);
		end
	end
end

function load(name)
	print("Called load level")
	current_filename = name
	current_level = persistence.load("lua/levels/"..name..".data")

	if current_level.map then
		print("attempting map load...")
		load_map(current_level.map)
	end

	print("starting an object load...")
	--load up all the objects
	load_objects(current_level.objects)
	print("Loaded all objects")
end

function stage.everyFrame()
	--TODO: on F12, load this level up in the editor

	if Input:WasKeyReleased("R") or Input:WasButtonReleased("Back") then
		--restart this level
		GameEngine:loadLevel(current_filename)
	end

	if Input:WasKeyReleased("F12") then
		GameEngine:editLevel(current_filename)
	end

	if Input:WasKeyReleased("F11") then
		GameEngine:editMap(current_level.map)
	end
end