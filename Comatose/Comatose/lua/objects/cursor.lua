Cursor = inherits(GameObject)

function Cursor:everyFrame()
	mouse_position = Input:GetMousePosition()
	self:position(mouse_position.X, mouse_position.Y)
end