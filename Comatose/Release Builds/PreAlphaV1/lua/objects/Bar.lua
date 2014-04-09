Bar = inherits(GameObject)

function Bar:init()
	self.max=0
	self.cur=0
	self.z_index=1
	self.percent=1
	self.width=0
	self.height=0

	self.camera_weight = 0
end

function Bar:set(m,i,x,y,width,height)
	self.max=m
	self.cur=i
	self.percent=self.cur/self.max
	self.width=width
	self.height=height
	self:sprite("pixel")
	self:position(x,y)
	self:scale(self.percent * self.width,height)
end

function Bar:setColor(r,g,b,a)
	self:color(r,g,b,a)
end

function Bar:setCurrent(c)
	self.cur=c
	self.percent=self.cur/self.max
	self:scale(self.percent * self.width,self.height)
end