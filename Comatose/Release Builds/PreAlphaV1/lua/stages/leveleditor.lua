﻿
gameObjects = {}
function registerObject(c, a)
	if type(a) == "string" then
		table.insert(gameObjects, {class=c,art=a})
	end
	if type(a) == "table" then
		a.class = c
		table.insert(gameObjects, a)
	end
end

for k,v in pairs(registered_objects) do
	registerObject(k,v)
end

insert_index = 1
current_level = {
	objects={}
}

--objects to make the editor work

selector = Cursor.create()
selector.current_index = 1
selector:sprite(gameObjects[selector.current_index].art)
selector.z_index = 1

function selector:updateSelector(currentObject)
	self:sprite(currentObject.art)
	if currentObject.centered then
		self.centered = true
		self:origin(currentObject.width / 2, currentObject.height / 2)
	else
		self.centered = false
		self:origin(0,0)
	end

	if currentObject.width then
		self.sprite_width = currentObject.width
	end

	if currentObject.height then
		self.sprite_height = currentObject.height
	end
end

function selector:scroll_up()
	if self.current_index > 1 then
		self.current_index = self.current_index - 1
		self:updateSelector(gameObjects[self.current_index]);
	end
end

function selector:scroll_down()
	if self.current_index < #gameObjects then
		self.current_index = self.current_index + 1
		self:updateSelector(gameObjects[self.current_index]);
	end
end

selected_object = nil
property = nil --global for console editing
placeholders = {}
function select_object(index, type)
	type = type or "object"
	if type == "object" then
		if selected_object then
			placeholders[selected_object]:color(255, 255, 255, 128)
			if current_level.objects[selected_object].color then
				placeholders[selected_object]:color(current_level.objects[selected_object].color.r, current_level.objects[selected_object].color.g, current_level.objects[selected_object].color.b, 128)
			end
		end
		selected_object = index
		property = current_level.objects[selected_object].defaults
		placeholders[index]:color(255, 255, 255, 255)
		if current_level.objects[index].color then
			placeholders[index]:color(current_level.objects[index].color.r, current_level.objects[index].color.g, current_level.objects[index].color.b, 255)
		end
	end
end

Placeholder = inherits(PhysicsObject)

function Placeholder:init()
	self:body_type("static")
	self.active = false
end

function Placeholder:right_click()
	select_object(self.index, self.type)
end

mode = "object"

function stage.click(mx, my)
	if mode == "object" then
		current_level.objects[insert_index] = {
			class=gameObjects[selector.current_index].class,
			defaults={
				x=mx,
				y=my
			}
		}
		currentObject = gameObjects[selector.current_index];
		placeholders[insert_index] = Placeholder.create()
		placeholders[insert_index].index = insert_index
		placeholders[insert_index]:sprite(currentObject.art)
		placeholders[insert_index].x = mx
		placeholders[insert_index].y = my
		placeholders[insert_index]:color(255,255,255,128)
		placeholders[insert_index].z_index = 0.5

		--handle optional parameters
		if currentObject.centered then
			placeholders[insert_index].centered = true
		else
			placeholders[insert_index].centered = false
		end

		if currentObject.width then
			placeholders[insert_index].width = currentObject.width
		end

		if currentObject.height then
			placeholders[insert_index].height = currentObject.height
		end

		insert_index = insert_index + 1
	end
end

function stage.everyFrame()
	--do a thing
	if keys_down.O then 
		mode = "object"
		selector:sprite(gameObjects[selector.current_index].art)
		selector:color(255,255,255,255)
	end
	if Input:WasKeyPressed("X") then
		--attempt to delete the selected object
		if selected_object then
			current_level.objects[selected_object] = nil
			placeholders[selected_object]:destroy()
			
			selected_object = nil
		end
	end
	if keys_up.F12 then
		save()
		loadlevel(current_filename)
	end
end

function color(red, green, blue, alpha)
	if selected_object then
		current_level.objects[selected_object].color = {r=red,g=green,b=blue,a=alpha}
		placeholders[selected_object]:color(red,green,blue,255)
	else
		print("No object selected!")
	end
end

--WASD camera, for moving around the level and stuff
camera = WASDcamera.create()

--save/load functions for the level
current_filename = ""
function save(filename)
	filename = filename or current_filename
	persistence.store("lua/levels/"..filename..".data", current_level)
	if debug then
		persistence.store(debugpath.."lua/levels/"..filename..".data", current_level)
	end
	current_filename = filename
end

function objectByName(classname)
	for k,v in pairs(gameObjects) do
		if v.class == classname then
			return k
		end
	end
end

function load(filename)
	filename = filename or current_filename
	current_level = persistence.load("lua/levels/"..filename..".data")
	if not current_level.joints then current_level.joints = {} end
	current_filename = filename

	--clear out the level state
	for k,v in pairs(placeholders) do
		v:destroy()
	end
	placeholders = {}
	selected_object = nil
	property = nil

	--populate the placeholders for objects
	selected_object = nil
	insert_index = 1
	for k,v in pairs(current_level.objects) do
		currentObject = gameObjects[objectByName(v.class)]
		placeholders[k] = Placeholder.create()
		placeholders[k].index = k
		placeholders[k]:sprite(currentObject.art)
		placeholders[k].x = v.defaults.x
		placeholders[k].y = v.defaults.y
		placeholders[k]:color(255,255,255,128)
		placeholders[k].z_index = 0.5

		--handle optional parameters
		if currentObject.centered then
			placeholders[k].centered = true
		else
			placeholders[k].centered = false
		end

		if currentObject.width then
			placeholders[k].width = currentObject.width
		end

		if currentObject.height then
			placeholders[k].height = currentObject.height
		end

		if v.color then
			placeholders[k]:color(v.color.r,v.color.g,v.color.b,128)
		end
		if k >= insert_index then
			insert_index = k + 1
		end
	end

	--load in the map, if there is one
	if current_level.map then
		map(current_level.map)
	end
end

levelmap = Map.create()
levelmap.z_index = 0

function map(filename)
	--load the file
	savedata = persistence.load("lua/maps/"..filename..".data")

	levelmap:sprite(savedata.image)
	levelmap:resetCollision()

	--load in the collision data
	for k,edge in pairs(savedata.edges) do
		if edge.length > 1 then
			levelmap:beginChain()
			for i = 1, edge.length do
				levelmap:addVertex(edge.verticies[i].x, edge.verticies[i].y)
			end
			levelmap:endChain(edge.looped == true)
		end
	end

	current_level.map = filename
end