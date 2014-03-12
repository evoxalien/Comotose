
WASDcamera = inherits(GameObject)

function WASDcamera:init()
	self.x = 0
	self.y = 0
end

function WASDcamera:everyFrame()
	direction = Input:GetMovementDirection()

	self.x = self.x + direction.X
	self.y = self.y + direction.Y

	GameEngine:setCamera(self.x, self.y)
end