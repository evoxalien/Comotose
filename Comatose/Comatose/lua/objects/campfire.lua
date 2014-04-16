
Campfire = inherits(PhysicsObject)

function Campfire:init()
	self:sprite("logs")
	self:body_type("static")

	self.z_index = 0.5
	self.centered = true
	self.cast_shadow = false

	--copied from hero shamelessly
	self.firelight1 = LightSource.create()
	self.firelight2 = LightSource.create()
	self.firelight1:shape("none")
	self.firelight2:shape("none")
	self.firelight1:color(210, 50, 45, 128)
	self.firelight2:color(230, 170, 20, 200)
	self.firelight1.ray_length = 20
	self.firelight2.ray_length = 25

	self.count = 0

	self:set_group("monster")
	self:add_target("hero")
end

function Campfire:handleCollision()
	stage.hero.on_fire = 60 * 3.0
end

function Campfire:everyFrame()
	
	self.firelight1.x = self.x
	self.firelight1.y = self.y - 1
	self.firelight2.x = self.x
	self.firelight2.y = self.y - 1

	self.firelight1:color(210, 50, 45, 96)
	self.firelight2:color(230, 170, 20, 150)
	if (self.count % 3) == 0 then
		--Effect:CreateExplosion(self.x , self.y, 10, 255, 255, 255)
		--Effect:CreateFire(self.x , self.y, 45)
		self.firelight1:color(210, 50, 45, 128)
	end

	if (self.count % 2) == 0 then
		--Effect:CreateExplosion(self.x , self.y, 10, 255, 255, 255)
		self.firelight2:color(230, 170, 20, 200)
		Effect:CreateFire(self.x , self.y - 1, 20)
	end

	--if the player gets too close, set them on FIRE!!! (super fun time)


	self.count = self.count + 1
end

registered_objects["Campfire"] = {
	art="logs",
	centered=true
}