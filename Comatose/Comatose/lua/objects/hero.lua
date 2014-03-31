Hero = inherits(PhysicsObject)

function Hero:init()
	self:sprite("Herov3")
	self.framesAlive = 0
	self.speed = 20
	self:shape("circle")
	self.z_index = 0
	self.centered = true
	--self.fixedRotation = true;

	
	self.light = LightSource.create({
		ray_length=50,
		rays_to_cast=2500,
		light_spread_angle=(math.pi / 3)})

	stage.hero = self

	self.camera = HeroCamera.create()
	self.camera.target = self

	stage.dialog = DialogBox.create()
	stage.dialog:position(80, 720 - 100)
	stage.dialog.width = 1280 - 80
	stage.dialog.height = 100
	stage.dialog.character_delay = 3

	--do weird things with the light
	self.light.x = self.x - 1.2
	self.light.y = self.y + 1.5

	self.light:join(self.ID())
end

function Hero:everyFrame()
	Input:setAimCenter(self.x, self.y)

	if not Input:MovementDeadzone() then
		direction = Input:GetMovementDirection()
		self.vx = direction.X * self.speed
		self.vy = direction.Y * self.speed

		--rotationey things
		if Input:AimingDeadzone() then
			self:rotateTo(math.atan2(direction.X, -direction.Y))
		end
	else
		self.vx = 0
		self.vy = 0
		self.vr = 0
	end

	if not Input:AimingDeadzone() then
		aim_direction = Input:GetAimDirection()
		self:rotateTo(math.atan2(aim_direction.X, -aim_direction.Y))
	end

	--self.light.rotation = self.rotation

	--self.light.x = self.x
	--self.light.y = self.y
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

registered_objects["Hero"] = "Herov3"