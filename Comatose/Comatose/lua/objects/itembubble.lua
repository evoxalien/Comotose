
ItemBubble = inherits(TextBox)

function ItemBubble:init()
	self.width = 200
	self.height = 50
	self.maxLine = 1
	self.character_delay = 0
	self.z_index = 850
end

function ItemBubble:everyFrame()
	if self.target then
		self:position(self.target.x * 10, self.target.y * 10 - 60)
	end
end