Hero = inherits(PhysicsObject)

function Hero:init()
	self:sprite("Herov3")
	self.framesAlive = 0
	self.speed = 20
	self:shape("circle")
	self.z_index = 1
	self.centered = true
	self.count = 0
	self:set_group("hero")

	self.on_fire = 0
	
	--add ourselves to the stage as a global object
	stage.hero = self
	
	--set up flashlight stuff
	self.flashlight = Flashlight.create()
	self.flashlight.cast_shadow = false
	--offset the light so it's in the hero's "hand"
	self.flashlight.x = self.x - 1.0
	self.flashlight.y = self.y + 1.7

	self.flashlight:join(self.ID())

	--setup the hero camera
	self.camera = HeroCamera.create()
	self.camera.target = self

	--the hero only exists once, so he will control the stage dialog and UI
	stage.dialog = DialogBox.create()
	stage.dialog:position(0, 720 - 100)
	stage.dialog.width = 1280 / 2
	stage.dialog.height = 100
	stage.dialog.character_delay = 1

	--setup the sanity and flashlight bars
	self.sanity = 100
	self.sanity_cooldown = 0

	self.sanity_bar = Bar.create()
	self.sanity_bar:set(100,100,1280/2,720-100,1280/2,50)
	self.sanity_bar:color(128,128,128,255)
	
	self.flashlight_bar = Bar.create()
	self.flashlight_bar:set(300,300,1280/2,720-50,1280/2,50)
	self.flashlight_bar:color(255,255,64,255)

	--add a light source centered on the player's location that represents the
	--player's inate short-distance field of view
	self.near_sight = GameObject.create()
	self.near_sight:sprite("player_vision")
	self.near_sight:origin(75,75)
	self.near_sight:color(255, 255, 255, 48)
	self.near_sight.z_index = 0.5


	self.firelight1 = LightSource.create()
	self.firelight2 = LightSource.create()
	self.firelight1:shape("none")
	self.firelight2:shape("none")
	self.firelight1:color(210, 50, 45, 0)
	self.firelight2:color(230, 170, 20, 0)
	self.firelight1.ray_length = 2
	self.firelight2.ray_length = 2

end

function Hero:everyFrame()
	--root the aiming function, so that subsequent calls will have the correct angle
	Input:setAimCenter(self.x, self.y)
	self.firelight1.x = self.x
	self.firelight1.y = self.y
	self.firelight2.x = self.x
	self.firelight2.y = self.y
	--FUN TIMES
	
	if self.on_fire > 0 then
		self.firelight1.ray_length = 20
		self.firelight2.ray_length = 25
		self.firelight1:color(210, 50, 45, 0)
		self.firelight2:color(230, 170, 20, 0)
		if (self.count % 3) == 0 then
			--Effect:CreateExplosion(self.x , self.y, 10, 255, 255, 255)
			--Effect:CreateFire(self.x , self.y, 45)
			self.firelight1:color(210, 50, 45, 128)
		end

		if (self.count % 2) == 0 then
			--Effect:CreateExplosion(self.x , self.y, 10, 255, 255, 255)
			self.firelight2:color(230, 170, 20, 200)
			Effect:CreateFire(self.x , self.y, 45)

		end
		
		self.on_fire = self.on_fire - 1
		self.sanity = self.sanity - 0.4
	else
		self.firelight1.ray_length = 2
		self.firelight2.ray_length = 2
		self.firelight1:color(210, 50, 45, 0)
		self.firelight2:color(230, 170, 20, 0)
	end


	if Input:WasKeyPressed("K") then
		self.on_fire = 60 * 3.0
	end


	
	--Particle:CreateExplosion(self.x , self.y, 10, 255, 255, 255)
	--Particle:CreateFire(self.x , self.y, 55)
	-- Keep it reasonable

	if not Input:MovementDeadzone() then
		direction = Input:GetMovementDirection()
		--self.vx = direction.X * self.speed
		--self.vy = direction.Y * self.speed
		self:impulse(direction.X * self.speed * 40, direction.Y * self.speed * 40)

		self.vx = self.vx / 2
		self.vy = self.vy / 2

		--if we're not using the right-stick, then rotate to face the direction we're walking
		if Input:AimingDeadzone() then
			self:rotateTo(math.atan2(direction.X, -direction.Y))
		end

		self:sprite("Hero_Move_Quick")
		self.width = 71
		self.height = 79
		self.frame_delay = 6
		self:shape("circle")
	else
		--kill off any momemtum we might have
		self.vx = 0
		self.vy = 0
		self.vr = 0

		self:sprite("Herov3")
		self.current_frame = 0
	end

	--if we're using the right stick, aim that way (regardless of movement direction)
	if not Input:AimingDeadzone() then
		aim_direction = Input:GetAimDirection()
		self:rotateTo(math.atan2(aim_direction.X, -aim_direction.Y))
	end

	--update my sanity based on the flashlight and darkness status
	--TODO: Handle "lit rooms" where the flashlight can be off safely
	if not self.flashlight.on then
		self.sanity = math.max(self.sanity - (1 / 60), 0)
		self.sanity_cooldown = 5 * 60 --10 second cooldown
	end

	--process sanity cooldowns, and restore sanity when I am safe
	self.sanity_cooldown = math.max(self.sanity_cooldown - 1, 0)
	if self.sanity_cooldown == 0 then
		self.sanity = math.min(self.sanity + (10 / 60), 100)
	end

	--update our near_sight light
	self.near_sight:position(self.x * 10, self.y * 10)

	--update the UI
	self.flashlight_bar:setCurrent(self.flashlight.charge)
	self.sanity_bar:setCurrent(self.sanity)
	
	Input:setAimCenter(self.x * 10, self.y * 10)
	self.count = self.count + 1
end

function Hero:insideFlashlight(entity)
	--get the angle between this object and the flashlight's center
	angle = math.atan2(entity.y - self.flashlight.y, entity.x - self.flashlight.x)

	--see if that angle is within the flashlight's cone
	angle = angle - self.flashlight.rotation + math.pi / 2
	while angle < 0 do
		angle = angle + math.pi * 2
	end
	while angle > math.pi * 2 do
		angle = angle - math.pi * 2
	end

	if angle < self.flashlight.light_spread_angle / 2 or angle > math.pi * 2 - self.flashlight.light_spread_angle / 2 then
		--distance check now
		if math.abs(entity.x - self.flashlight.x) ^ 2 + math.abs(entity.y - self.flashlight.y) ^ 2 < self.flashlight.ray_length ^ 2 then
			return true
		else
			return false
		end
	end
	return false
end

HeroCamera = inherits(GameObject)

function HeroCamera:init()
	self.margin = 15 --game world units
	self.x = 0
	self.y = 0
	self.stage_width = 1280.0 / 10.0
	self.stage_height = 720.0 / 10.0

	self.positions = {}
end

function HeroCamera:everyFrame()
	if self.target then
		local hero_x = self.target.x
		local hero_y = self.target.y

		--adjust the hero's coordinates for the flashlight
		local flashlight_x = math.cos(self.target.rotation - math.pi / 2) * 25 + hero_x
		local flashlight_y = math.sin(self.target.rotation - math.pi / 2) * 25 + hero_y

		--print("flashlight: " .. flashlight_x .. ", " .. flashlight_y)

		local min_x = math.min(hero_x, flashlight_x) - self.margin
		local min_y = math.min(hero_y, flashlight_y) - self.margin

		local max_x = math.max(hero_x, flashlight_x) - self.stage_width + self.margin
		local max_y = math.max(hero_y, flashlight_y) - self.stage_height + self.margin

		--print("min: " .. min_x .. ", " .. min_y)
		--print("max: " .. max_x .. ", " .. max_y)

		self.x = math.max(math.min(self.x, min_x), max_x)
		self.y = math.max(math.min(self.y, min_y), max_y)

		--print("self: " .. self.x .. ", " .. self.y)

		table.insert(self.positions, {x=self.x, y=self.y})
		if #self.positions > 32 then
			table.remove(self.positions, 1)
		end

		--get the average of all points in the camera list
		local target_x = 0
		local target_y = 0
		for k,v in ipairs(self.positions) do
			target_x = target_x + v.x
			target_y = target_y + v.y
		end
		target_x = target_x / #self.positions
		target_y = target_y / #self.positions

		GameEngine:setCamera(target_x * 10, target_y * 10)
	end
end

registered_objects["Hero"] = {
	art="Herov3",
	centered=true
}
