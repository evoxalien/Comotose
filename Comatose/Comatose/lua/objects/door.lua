

Door = inherits(PhysicsObject)

function Door:init()
	self.hinge = PhysicsObject.create()
	self.hinge.centered = true
	self.hinge.y = self.y - 4
	self.hinge.x = self.x
	self.hinge:sprite("testyhinge")
	self.hinge:body_type("static")
	self.hinge.z_index = 0.5
	self.hinge:shape("circle")

	--spawn a door, and spawn a hinge
	self:sprite("testydoor")
	self.centered = true
	self.hinge:join(self:ID(), "revolute")
	self.z_index = 0.5
end

function Door:everyFrame()
	
end

registered_objects["Door"] = {
	art="testydoor",
	centered=true
}
