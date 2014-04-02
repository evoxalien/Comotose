
Monster = inherits(PhysicsObject)

function Monster:init()
	self:sprite("MonsterV_1")
	self.centered = true
	self.width = 64
	self.height = 64
	self.frame_delay = 5
	self.fixedRotation = true
	self.cast_shadow = false

	stage.monster = self

	self:set_group("monster")
	self:add_target("hero")
	self.fade_timer = 0
end

function Monster:handleCollision()
	stage.hero.sanity = 0
end

function Monster:everyFrame()
	if GameEngine:hasLineOfSight(stage.hero:ID(), self:ID()) and stage.hero:insideFlashlight(self) then
		stage.hero.sanity = math.max(stage.hero.sanity - 3 / 60, 0)
		stage.hero.sanity_cooldown = 5 * 60
		self.fade_timer = math.min(self.fade_timer + 2, 10)
	else
		--hide
		self.fade_timer = math.max(self.fade_timer - 1, 0)
	end
	self:color(255,255,255,255 * (self.fade_timer / 10))
end

registered_objects["Monster"] = "MonsterV_1"