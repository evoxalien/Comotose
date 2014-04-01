bar = inherits(GameObject)

function bar:init()
	self.max=0
	self.cur=0
	self.z_index=1
	self.percent=1
	self.width=0
	self.height=0
end

function bar:set(m,i,x,y,width,height)
	self.max=m
	self.cur=i
	self.percent=self.cur/self.max
	self.width=width
	self.height=height
	self:sprite("pixel")
	self:position(x,y)
	self:scale(self.percent*self.max,height)
end

function bar:setColor(r,g,b,a)
	self:color(r,g,b,a)
end

function bar:setCurrent(c)
	self.cur=c
	self.percent=self.cur/self.max
	self:scale(self.percent*self.max,self.height)
end