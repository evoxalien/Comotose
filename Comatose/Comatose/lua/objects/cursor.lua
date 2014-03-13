Cursor = inherits(GameObject)

function Cursor:init()
	self:sprite("cursor")
	self.z_index = 1
end

function Cursor:everyFrame()
	mouse_position = Input:GetMousePosition()
	self:position(mouse_position.X, mouse_position.Y)
end