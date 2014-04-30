

FloorLamp = inherits(PhysicsObject)

function FloorLamp:init()
	self:sprite("Temp-Lamp")
	self.centered = true
	self:body_type("static")
	self:shape("circle")
	self.cast_shadow = false
	self.z_index = 0.5

	--item bubble for switch tooltip
	self.title = ItemBubble.create()
	self.title:text("Switch")
	self.title.target = self
	self.title.centered = true

	--light source
	self.light = LightSource.create()
	self.light.ray_length = 50
	if self.on == nil then
		self.on = true
	end

	--"switch" noise
	self.SwitchSound=Audio.create()
	self.SwitchSound:audioname("FlashlightOn00")
	self.SwitchSound:attach(self:ID())
end

function FloorLamp:everyFrame()
	if stage.hero then
		self.SwitchSound:attachListener(stage.hero:ID())
	end
	self.light.x = self.x
	self.light.y = self.y

	--update light state thingy based on flag stuff
	if self.on then
		self.light.ray_length = 50
		self.light:color(255,255,255,255)
	else
		self.light.ray_length = 1
		self.light:color(255,255,255,0)
	end
end

function FloorLamp:use()
	distance = stage.hero:distanceFrom(self.x, self.y)
	if distance <= 15 then
		if self.on then
			self.on = false
			self.SwitchSound:Play()
		else
			self.on = true
			self.SwitchSound:Play()
		end
	end
end

registered_objects["FloorLamp"] = {
	art="Temp-Lamp",
	centered=true
}