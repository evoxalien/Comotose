Flashlight = inherits(LightSource)

function Flashlight:init()
	self.light_spread_angle = math.pi / 3
	self.ray_length = 50

	self.on = true
	self.charge = 100
end

function Flashlight:addCharge(battery_level)
	self.charge = math.min(self.charge + charge_level, 100)
end

function Flashlight:everyFrame()
	--listen for the flashlight button and toggle ourselves
	if Input:WasButtonPressed("B") or Input:WasKeyPressed("F") then
		self.on = not self.on
		--TODO: play "flashlight click" sound here
	end

	--deplete charge over time
	if self.on then
		self.charge = math.max(self.charge - 0.01, 0)
		if self.charge == 0 then
			self.on = false
			--TODO: play "flashlight deplete" sound here
		end
	end

	--update display based on current status
	if self.on then
		if self.charge > 10 then
			--normal, full power beam
			self.ray_length = 50
			self:color(255,255,255,255)
		else 
			--slowly depleting beam of low battery and misery
			self.ray_length = 25 + 25 * self.charge / 10
			local fadeColor = math.min((255 * self.charge / 10) + math.random(20), 255) --random to make it flicker
			--I dunno, this should make it "yellow out" as the battery dies maybe?
			self:color(127 + fadeColor/2, 127 + fadeColor/2, fadeColor, fadeColor)
		end
	else
		self.ray_length = 1
		self:color(0,0,0,0)
	end
end