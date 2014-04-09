
ItemBubble = inherits(TextBox)

function ItemBubble:init()
	self:position(-55,-62)
	self.width = 200
	self.height = 50
	self.maxLine = 1
	self.character_delay = 0
	self.z_index = 850
end

function ItemBubble:everyFrame()
	--todo: make this less awful
end