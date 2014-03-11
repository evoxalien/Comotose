
Hero = inherits(GameObject)

function Hero:init()
	self.x = 50
	self.y = 50
	self:sprite("Hero")
	self.framesAlive = 0
	self:origin(64,64)
	self.speed = 5
end

function Hero:everyFrame()
	if Input:WasKeyPressed("Space") then
		self:color(255, 0, 0, 128)
	end

	direction = Input:GetMovementDirection()
	self.x = self.x + direction.X * self.speed
	self.y = self.y + direction.Y * self.speed

	self:position(self.x, self.y)

	if Input:LeftStickDead() then
		self:rotate(math.atan2(-1 * direction.Y, -1 * direction.X) - (math.pi / 2))
	end
end

hero = Hero.create()