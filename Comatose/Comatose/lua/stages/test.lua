
print("notfail1")

Hero = inherits(GameObject)

function Hero:init()
	self.x = 50
	self.y = 50
	self:sprite("Hero")
	self.framesAlive = 0
end

print("notfail2")

function Hero:everyFrame()
	self.framesAlive = self.framesAlive + 1

	self.vx = math.cos(self.framesAlive / 60) * 2
	self.vy = math.sin(self.framesAlive / 60) * 2

	self.x = self.x + self.vx
	self.y = self.y + self.vy

	self:position(self.x, self.y)
end

print("notfail3")

hero = Hero.create()

print("notfail4")