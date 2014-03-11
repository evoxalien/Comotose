--note: global "level_name" is passed in by the game engine

--setup some things
stage.triggered = {}

current_filename = ""

current_map = {}

function load(name)
	print("Called load level")
	current_filename = name
	current_level = persistence.load("lua/levels/"..name..".data")

	loaded_objects = {}

	print("starting an object load...")
	--load up all the objects
	for k,v in pairs(current_level.objects) do
		--sanity
		if _G[v.class] then
			print("Attempting to instanciate a " .. v.class)
			loaded_objects[k] = _G[v.class].create(v.defaults)
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