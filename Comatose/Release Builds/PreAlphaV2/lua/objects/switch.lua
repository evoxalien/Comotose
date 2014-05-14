

Switch = inherits(PhysicsObject)

function Switch:init()
	self:sprite("ON")
	self:body_type("static")
	
	if self.on == nil then
		self.on = true
	end

	if self.control then
		--item bubble for switch tooltip
		self.title = ItemBubble.create()
		self.title:text(self.control)
		self.title.target = self
		self.title.centered = true
	end
end

function Switch:use()
	distance = stage.hero:distanceFrom(self.x, self.y)
	if distance <= 15 then
		if self.control then
			processEvent("switch", self.control)
		end
	end
end

function Switch:everyFrame()
	if self.on then
		self:sprite("ON")
	else
		self:sprite("OFF")
	end
end

registered_objects["Switch"] = {
	art="ON"
}