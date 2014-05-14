
ItemBubble = inherits(TextBox)

function ItemBubble:init()
	self.width = 200
	self.height = 50
	self.maxLine = 1
	--self.character_delay = 0
	self.z_index = 850
end

function ItemBubble:everyFrame()
	if self.target then
		self:position(self.target.x * 10, self.target.y * 10 - 60)

		--if we're too far away from the hero, hide ourselves
		distance = stage.hero:distanceFrom(self.target.x, self.target.y)
		if distance > 15 then
			self:color(0,0,0,0)
		else
			self:color(255,255,255,255)
		end
	end
 
end