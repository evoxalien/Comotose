Screamer = inherits(AI)

function Screamer:init()
	self:sprite("MonsterV2-move")
	self.centered = true
	self.width = 64
	self.height = 64
	self.frame_delay = 5
	--self.fixedRotation = true
	self:shape("circle")
	self.cast_shadow = false
	self.speed=10

	stage.Screamer = self

	self:set_group("Screamer")
	self:add_target("hero")
	self.fade_timer = 0

	self.z_index = 0.5

	--self:Target(stage.hero:ID())
end

function Screamer:handleCollision()
	stage.hero.sanity = 0
end

function Screamer:everyFrame()
	if not self:HasTarget() then
		self:Target(stage.hero:ID())
	end
	if 
		(GameEngine:hasLineOfSight(stage.hero:ID(), self:ID()) and stage.hero:insideFlashlight(self)) or
		self:distanceFrom(stage.hero.x, stage.hero.y) < 10 then
		stage.hero.sanity = math.max(stage.hero.sanity - 20 / 60, 0)
		stage.hero.sanity_cooldown = 5 * 60
		self.fade_timer = math.min(self.fade_timer + 2, 10)
	else
		--hide
		self.fade_timer = math.max(self.fade_timer - 1, 0)
	end
	self:color(255,255,255,255 * (self.fade_timer / 10))

	self:MoveTowardsTarget()

	--turn to face our movement direction
	rotate_angle = math.atan2(self.vx, -self.vy) + math.pi
	--print(rotate_angle)
	self:rotateTo(rotate_angle)
end																   

--registered_objects["Screamer"] = "ScreamerV_1"
registered_objects["Screamer"] = {
	art="MonsterV2-move",
	width=64,
	height=64,
	centered=true
}
