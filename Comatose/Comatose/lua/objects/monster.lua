
Monster = inherits(PhysicsObject)

function Monster:init()
	self:sprite("MonsterV_1")
	self.width = 64
	self.height = 64
	self.frame_delay = 5
	self.fixedRotation = true

	stage.monster = self
end

registered_objects["Monster"] = "MonsterV_1"