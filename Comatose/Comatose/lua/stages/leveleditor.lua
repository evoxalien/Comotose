
gameObjects = {}
function registerObject(c, a)
	table.insert(gameObjects, {class=c,art=a})
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

function selector:scroll_up()
	if self.current_index > 1 then
		self.current_index = self.current_index - 1
		self:sprite(gameObjects[self.current_index].art)
	end
end

function selector:scroll_down()
	if self.current_index < #gameObjects then
		self.current_index = self.current_index + 1
		self:sprite(gameObjects[self.current_index].art)
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

function stage.on_click(mx, my)
	if mode == "object" then
		current_level.objects[insert_index] = {
			class=gameObjects[selector.current_index].class,
			defaults={
				x=mx,
				y=my
			}
		}
		placeholders[insert_index] = Placeholder.create()
		placeholders[insert_index].index = insert_index
		placeholders[insert_index]:sprite(gameObjects[selector.current_index].art)
		placeholders[insert_index].x = mx
		placeholders[insert_index].y = my
		placeholders[insert_index]:color(255,255,255,128)

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
	if keys_up.X then
		--attempt to delete the selected object, and any attached joints
		if selected_object then
			current_level.objects[selected_object] = nil
			placeholders[selected_object]:destroy()
			--destroy any joints that reference this object
			for k,v in pairs(current_level.joints) do
				for i = 1, #v do
					if v[i] == selected_object then
						--destroy this joint
						--TODO: make this work
						current_level.joints[k] = nil
					end
				end
			end
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
--camera = WASDcamera.create()

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
		placeholders[k] = Placeholder.create()
		placeholders[k].index = k
		placeholders[k]:sprite(gameObjects[objectByName(v.class)].art)
		placeholders[k].x = v.defaults.x
		placeholders[k].y = v.defaults.y
		placeholders[k]:color(255,255,255,128)
		if v.color then
			placeholders[k]:color(v.color.r,v.color.g,v.color.b,128)
		end
		if k >= insert_index then
			insert_index = k + 1
		end
	end
end