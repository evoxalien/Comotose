
WASDcamera = inherits(GameObject)

function WASDcamera:init()
	self.x = 0
	self.y = 0
end

function WASDcamera:everyFrame()
	direction = Input:GetMovementDirection()

	self.x = self.x + direction.X * 10
	self.y = self.y + direction.Y * 10

	GameEngine:setCamera(self.x, self.y)
end