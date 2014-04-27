
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

for k,v in orderedPairs(registered_objects) do
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

	if currentObject.width then
		self.sprite_width = currentObject.width
	end

	if currentObject.height then
		self.sprite_height = currentObject.height
	end

	if currentObject.centered then
		self.centered = true
		self:origin(self.sprite_width / 2, self.sprite_height / 2)
	else
		self.centered = false
		self:origin(0,0)
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
	Input:setAimCenter(placeholders[index].x, placeholders[index].y)
end

PropertyDisplay = inherits(TextBox)

function PropertyDisplay:init()
	self.width = 1280 / 2
	self.height = 720 / 2
	self.maxLines = 100
	self.z_index = 500
	self.camera_weight = 0
	self:position(0,720/2)

	--shadow copy of self
	self.shadow = TextBox.create()
	self.shadow.width = 1280 / 2
	self.shadow.height = 720 / 2
	self.shadow:position(1, 720/2 + 1)
	self.shadow.maxLines = 100
	self.shadow.z_index = 499
	self.shadow:color(0,0,0,192)
	self.shadow.camera_weight = 0
end

function displayTable(tbl, indent)
	output = ""
	indent = indent or ""
	for k,v in orderedPairs(tbl) do
		if type(v) == "table" then
			output = output .. indent .. "  " .. k .. ": \n"
			output = output .. displayTable(v, indent .. "-")
		else
			output = output .. indent .. "  " .. k .. ": " .. v .. "\n"
		end
	end
	return output
end

function PropertyDisplay:everyFrame()
	if selected_object then
		o = current_level.objects[selected_object]

		display = "Classname: " .. o.class .. "\n"
		if o.color then
			display = display .. "color: " .. o.color.r .. ", " .. o.color.g .. ", " .. o.color.b .. ", " .. o.color.a .. "\n"
		end
		if o.event then
			display = display .. "Spawns On Event: " .. o.event .. "\n"
		end
		if o.rotation then
			display = display .. "Rotation: " .. math.deg(o.rotation) .. "\n"
		end

		--display properties that will be set, if any
		if o.defaults then
			display = display .. "[Properties]\n"
			display = display .. displayTable(o.defaults)
		end
		self:text(display)
		self.shadow:text(display)
	else
		self:text("-- Placing Object: " .. gameObjects[selector.current_index].class .. " --")
		self.shadow:text("-- Placing Object: " .. gameObjects[selector.current_index].class .. " --")
	end
end

property_display = PropertyDisplay.create()

Placeholder = inherits(PhysicsObject)

function Placeholder:init()
	self:body_type("kinematic")
	self.active = true
	self:setLayer("unlit")
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

	if Input:WasKeyReleased("F12") and current_filename then
		save()
		GameEngine:loadLevel(current_filename)
	end

	if Input:WasKeyReleased("F11") and current_filename and current_level.map then
		save()
		GameEngine:editMap(current_level.map)
	end

	if Input:IsKeyHeld("R") and selected_object and (not Input:AimingDeadzone()) then
		aim_direction = Input:GetAimDirection()
		aim_angle = math.atan2(aim_direction.X, -aim_direction.Y)
		print(aim_angle)
		current_level.objects[selected_object].rotation = aim_angle
		placeholders[selected_object]:setRotation(aim_angle)
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

function event(event_name)
	if selected_object then
		current_level.objects[selected_object].event = event_name
	else
		print("No object selected!")
	end
end

function music(filename)
	current_level.music = filename
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
		if v.rotation then
			placeholders[k]:setRotation(v.rotation)
		end

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
levelmap:setLayer("unlit")

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