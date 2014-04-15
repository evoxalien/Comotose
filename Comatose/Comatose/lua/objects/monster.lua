
Monster = inherits(AI)

function Monster:init()
	self:sprite("MonsterV_1")

	--audio test
	self.audio=Audio.create()
	self.audio:audioname("chatter")
	self.audio:attach( self:ID())
	self.audio:attachListener( stage.hero:ID())
	self.audio.looped=true
	self.audio:play()

	self.centered = true
	self.width = 64
	self.height = 64
	self.frame_delay = 5
	self:shape("circle")
	self.cast_shadow = false
	self.speed = 10

	stage.monster = self

	self:set_group("monster")
	self:add_target("hero")
	self.fade_timer = 0

	self.z_index = 0.5

	--self:Target(stage.hero:ID())
end

function Monster:handleCollision()
	stage.hero.sanity = 0
end

function Monster:everyFrame()
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

	self.audio:Calc3D()

end

--registered_objects["Monster"] = "MonsterV_1"
registered_objects["Monster"] = {
	art="MonsterV_1",
	width=64,
	height=64,
	centered=true
}
