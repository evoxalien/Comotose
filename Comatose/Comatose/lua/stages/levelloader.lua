--note: global "level_name" is passed in by the game engine

--setup some things
stage.triggered = {}

current_filename = ""

current_map = Map.create()
current_map.z_index = 0

function load_map(filename)
	--load the file
	savedata = persistence.load("lua/maps/"..filename..".data")
	current_map:sprite(savedata.image)

	--load in the collision data
	for k,edge in pairs(savedata.edges) do
		if edge.length > 1 then
			current_map:beginChain()
			for i = 1, edge.length do
				current_map:addVertex(edge.verticies[i].x, edge.verticies[i].y)
			end
			current_map:endChain(edge.looped == true)
		end
	end

	current_level.map = filename
end

function load(name)
	print("Called load level")
	current_filename = name
	current_level = persistence.load("lua/levels/"..name..".data")

	loaded_objects = {}

	if current_level.map then
		print("attempting map load...")
		load_map(current_level.map)
	end

	print("starting an object load...")
	--load up all the objects
	for k,v in pairs(current_level.objects) do
		--sanity
		if _G[v.class] then
			print("Attempting to instanciate a " .. v.class)
			loaded_objects[k] = _G[v.class].create(v.defaults)
			loaded_objects[k].z_index = 0.5
			if v.color then
				loaded_objects[k]:color(v.color.r, v.color.g, v.color.b, v.color.a)
			end
		else
			print("Error loading object -- bad classname: " .. v.class);
		end
	end
	print("Loaded all objects")
end

function stage.update()
	--TODO: on F12, load this level up in the editor

	if Input:WasKeyReleased("R") or Input:WasButtonReleased("Back") then
		--restart this level
		GameEngine:loadLevel(current_filename)
	end
end