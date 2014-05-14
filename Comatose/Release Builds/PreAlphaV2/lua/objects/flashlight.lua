Flashlight = inherits(LightSource)

function Flashlight:init()
	self.light_spread_angle = math.pi / 3
	self.ray_length = 50

	self.on = true
	self.charge = 300

	--audio things
	self.sounds = {}
	self.sounds.on = Audio.create()
	self.sounds.on:audioname("FlashlightOn00")
	--self.sounds.on:attach(self:ID())
	--self.sounds.on:attachListener(self:ID())

	self.sounds.off = Audio.create()
	self.sounds.off:audioname("FlashlightOff00")
	--self.sounds.off:attach(self:ID())
	--self.sounds.off:attachListener(self:ID())
end

function Flashlight:addCharge(battery_level)
	self.charge = math.min(self.charge + charge_level, 100)
end

function Flashlight:everyFrame()
	--listen for the flashlight button and toggle ourselves
	if Input:WasButtonPressed("B") or Input:WasKeyPressed("F") then
		if self.on then
			self.on = false
			self.sounds.off:play()
		else
			self.on = true
			self.sounds.on:play()
		end
		--TODO: play "flashlight click" sound here
	end

	--deplete charge over time
	if self.on then
		self.charge = math.max(self.charge - (1 / 60), 0)
		if self.charge == 0 then
			self.on = false
			--TODO: play "flashlight deplete" sound here
		end
	end

	--update display based on current status
	if self.on then
		self.ray_length = 50
		self:color(255,255,204,255)
		
		--special case animations
		--at 75% flicker for 0.5 seconds
		if self.charge > 224.5 and self.charge < 225 then
			local flickerColor = math.random(225,255)
			self:color(flickerColor, flickerColor, flickerColor * 204 / 255, flickerColor)
		end

		--at 50%, flicker for 1.5 seconds at 80% brightness
		if self.charge > 148.5 and self.charge < 150 then
			local flickerColor = math.random(184,224)
			self:color(flickerColor, flickerColor, flickerColor * 204 / 255, flickerColor)
		end

		--at 25%, several things happen
		--first, we dim to 60% brightness while also flickering for 1.5 seconds
		if self.charge > 73.5 and self.charge < 75 then
			local flickerColor = math.min(math.random(30) + (((self.charge - 73.5) / 1.5) * 0.4 + 0.6) * 255, 255)
			self:color(flickerColor, flickerColor, flickerColor * 204 / 255, flickerColor)
		end

		--then, we flicker some more (intensely) for 0.5 seconds
		if self.charge > 73 and self.charge < 73.5 then
			local flickerColor = math.random(255 * 0.55, 255 * 0.65)
			self:color(flickerColor, flickerColor, flickerColor * 204 / 255, flickerColor)
		end

		--finally, we flicker much less intensely and return to full brightness over 1 second
		if self.charge > 72 and self.charge < 73 then
			local flickerColor = math.min(math.random(10) + (0.4 - (((self.charge - 72) / 1) * 0.4) + 0.6) * 255, 255)
			self:color(flickerColor, flickerColor, flickerColor * 204 / 255, flickerColor)
		end

		--at 10%, the flashlight fades down to 40%
		if self.charge > 28 and self.charge < 30 then
			local flickerColor = math.min(math.random(30) + (((self.charge - 28) / 2.0) * 0.6 + 0.4) * 255, 255)
			self:color(255, 234, 204, flickerColor)
		end
		if self.charge > 2 and self.charge < 28 then
			self:color(255, 234, 204, 255 * 0.4)
		end
		if self.charge < 2 then
			--slowly depleting beam of low battery and misery
			self.ray_length = 50 * self.charge / 2
			local fadeColor = math.min((255 * self.charge / 2) + math.random(30), 255) --random to make it flicker
			--I dunno, this should make it "yellow out" as the battery dies maybe?
			self:color(fadeColor, fadeColor * 234 / 255, fadeColor * 204 / 255, fadeColor * 0.4)
		end
	else
		self.ray_length = 1
		self:color(0,0,0,0)
	end
end