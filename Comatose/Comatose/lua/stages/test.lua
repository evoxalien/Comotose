
Hero = inherits(PhysicsObject)

function Hero:init()
	self.x = 5
	self.y = 5
	self:sprite("Hero")
	self.framesAlive = 0
	self:origin(64,64)
	self.speed = 15
end

function Hero:everyFrame()
	if Input:WasKeyPressed("Space") then
		self:color(255, 0, 0, 128)
	end

	if not Input:MovementDeadzone() then
		direction = Input:GetMovementDirection()
		self.vx = direction.X * self.speed
		self.vy = direction.Y * self.speed
	else
		self.vx = 0
		self.vy = 0
	end
end

hero = Hero.create()

Chair = inherits(PhysicsObject)

function Chair:init()
	self:body_type("static")
	self:sprite("Chair1")
	self.x = 20
	self.y = 20
end

chair = Chair.create()