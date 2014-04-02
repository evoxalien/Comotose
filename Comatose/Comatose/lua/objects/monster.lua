
Monster = inherits(PhysicsObject)

function Monster:init()
	self:sprite("MonsterV_1")
	self.width = 64
	self.height = 64
	self.frame_delay = 5
	self.fixedRotation = true
	self.cast_shadow = false

	stage.monster = self

	self:set_group("monster")
	self:add_target("hero")
end

function Monster:handleCollision()
	stage.hero.sanity = 0
end

registered_objects["Monster"] = "MonsterV_1"