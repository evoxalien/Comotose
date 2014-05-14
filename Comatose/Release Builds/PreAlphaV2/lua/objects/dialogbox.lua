DialogBox = inherits(TextBox)

function DialogBox:init()
	self.hidden = true
	self.current_section = 1
	self.character_delay = 2

	self.camera_weight = 0

	self.button = GameObject.create()
	self.button:sprite("a_button")

	self.button.sprite_width = 32
	self.button.sprite_height = 32
	self.button.frame_delay = 30
	self.button.camera_weight = 0
	self.button:setLayer("ui")

	self.portrait_box = GameObject.create()
	self.portrait_box:color(0,0,0,0)
	self.portrait_box.camera_weight = 0
	self.portrait_box:setLayer("ui")

	self.z_index = 900
	self.portrait_box.z_index = 950
	self.button.z_index = 950

	--margins for the textbox
	self.margin_left = 8 + 50 - 32 + 64
	self.margin_right = 50 - 32
	self.margin_top = 8
	self.margin_bottom = 8

	--background sprite / color thing
	self.background = GameObject.create()
	self.background:sprite("pixel")
	self.background:color(0,0,0,128)
	self.background.z_index = 890
	self.background.camera_weight = 0
	self.background:setLayer("ui")
end

function DialogBox:portrait(sprite)
	self.portrait_box:sprite(sprite)
end

function DialogBox:displayText(text)
	if type(text) == "string" then
		self.sections = {}
		self.sections[1] = text
	end

	if type(text) == "table" then
		self.sections = text
	end

	self.hidden = false
	self.current_section = 1
	
	self:text(self.sections[self.current_section])
end

function DialogBox:everyFrame()
	if not self.hidden then
		if self:isFinished() then
			if Input:WasKeyPressed("Enter") or Input:WasButtonPressed("A") then
				if self.current_section >= #self.sections then
					self.hidden = true
				else
					self.current_section = self.current_section + 1
					self:text(self.sections[self.current_section])
				end
			end
		end
	end

	if self.hidden then
		self:color(0,0,0,0)
		self.button:color(0,0,0,0)
		self.portrait_box:color(0,0,0,0)
	else
		position = self:getPosition()

		--position everything correctly, based on our current size and position
		self.button:position(position.X + self.width - 32 - self.margin_right, position.Y + self.height - 32 - self.margin_bottom)
		self.portrait_box:position(position.X + 50 - 32, position.Y + 50 - 32)
		self.background:position(position.X, position.Y)
		self.background:scale(self.width, self.height)

		self:color(255,255,128,255)
		self.portrait_box:color(255,255,255,255)

		if self:isFinished() then
			self.button:color(255,255,255,255)
		else
			self.button:color(0,0,0,0)
		end
	end
end