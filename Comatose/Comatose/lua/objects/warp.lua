

LocalWarp = inherits(PhysicsObject)

function LocalWarp:init()
	self:sprite("warp_sparkle")
	self.width = 32
	self.height = 32
	self.frame_delay = 8
	self.centered = true
	self:body_type("static")
	self.cast_shadow = false
	self.z_index = 0.5
	self:setLayer("unlit")

	self:set_group("warp")
	self:add_target("hero")

	self.is_sensor = true
end

function LocalWarp:handleCollision()
	if self.target and stage.hero.warp_cooldown == 0 then
		stage.hero.x = self.target.x
		stage.hero.y = self.target.y

		stage.hero.flashlight.x = self.target.x
		stage.hero.flashlight.y = self.target.y

		--kill momentum
		stage.hero.vx = 0
		stage.hero.vy = 0
		stage.hero.vr = 0

		stage.hero.flashlight.vx = 0
		stage.hero.flashlight.vy = 0
		stage.hero.flashlight.vr = 0

		stage.hero.warp_cooldown = 500

		--desperation?
		stage.hero.active = false
		stage.hero.active_cooldown = 50
	end
end

StageWarp = inherits(PhysicsObject)

function StageWarp:init()
	self:sprite("warp_sparkle")
	self:color(128,128,255,255)
	self.width = 32
	self.height = 32
	self.frame_delay = 8
	self.centered = true
	self.cast_shadow = false
	self:body_type("static")
	self.z_index = 0.5
	self:setLayer("unlit")

	self:set_group("warp")
	self:add_target("hero")

	--item bubble, for display purposes
	self.title = ItemBubble.create()
	self.title:text(self.target)
	self.title.target = self
	self.title.centered = true
end

function StageWarp:handleCollision()
	if self.target then
		GameEngine:playSound("door-enter")
		loadstage(self.target)
	end
end

LevelWarp = inherits(PhysicsObject)

function LevelWarp:init()
	self:sprite("warp_sparkle")
	self:color(128,255,128,255)
	self.width = 32
	self.height = 32
	self.frame_delay = 8
	self.centered = true
	self.cast_shadow = false
	self:body_type("static")
	self.z_index = 0.5
	self:setLayer("unlit")

	self:set_group("warp")
	self:add_target("hero")

	--item bubble, for display purposes
	self.title = ItemBubble.create()
	self.title:text(self.target)
	self.title.target = self
	self.title.centered = true
end

function LevelWarp:handleCollision()
	if self.target then
		GameEngine:playSound("door-enter")
		loadlevel(self.target)
	end
end

registered_objects["LocalWarp"] = {
	art="warp_sparkle",
	width=32,
	height=32,
	centered=true
}

registered_objects["StageWarp"] = {
	art="warp_sparkle",
	width=32,
	height=32,
	centered=true
}

registered_objects["LevelWarp"] = {
	art="warp_sparkle",
	width=32,
	height=32,
	centered=true
}
