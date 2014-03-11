Hero = inherits(PhysicsObject)

function Hero:init()
	self.x = 5
	self.y = 5
	self:sprite("Hero")
	self.framesAlive = 0
	self.speed = 15
	self:shape("circle")
	self.z_index = 0
	--self.fixedRotation = true;
end

function Hero:everyFrame()
	Input:setAimCenter(self.x, self.y)
	if Input:WasKeyPressed("Space") then
		self:color(255, 0, 0, 128)
	end

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
end

registered_objects["Hero"] = "Hero"