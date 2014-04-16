

Door = inherits(PhysicsObject)

function Door:init()
	self.hinge = PhysicsObject.create()
	self.hinge.centered = true
	self.hinge.y = self.y - 3
	self.hinge.x = self.x
	self.hinge:sprite("testyhinge")
	self.hinge:body_type("static")

	--spawn a door, and spawn a hinge
	self:sprite("testydoor")
	self.centered = true
	self.hinge:join(self:ID(), "revolute")
end

registered_objects["Door"] = {
	art="testydoor",
	centered=true
}
