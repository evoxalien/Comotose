

--It's pretty! That's... all it does for now.

Sprinkler = inherits(PhysicsObject)

function Sprinkler:init()
	self:shape("none")
	self:body_type("static")		
	self.on = false
end

function Sprinkler:switch(trigger)
	if trigger == self.control then
		self.on = not self.on
	end
end

function Sprinkler:everyFrame()
	if self.on then
		Effect:CreateSprinkler(self.x, self.y, 10)
	end
end

registered_objects["Sprinkler"] = {
	art="sprinkler_icon",
	centered=true
}