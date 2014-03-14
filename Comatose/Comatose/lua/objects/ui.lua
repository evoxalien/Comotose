UI= inherits(GameObject)



function UI:init()
	self.table={}
	self.items=0
	self.open=false
end

function UI:AddObject(o)

	o.oldshape=o:GetShape()
	o:shape("none")
	o.z_index=-1		   --remove  from screen by setting behind the map

	o.vx=0
	o.vy=0
	o.vr=0
	--o:rotateTo(math.atan(0)) --this isnt resting the rotation

	self.table[self.items]=o --insert into the table

	self.items=self.items+1  

end

function UI:Display()
	self.open=true	

	local x = 10
	local y = 10

	for key,value in pairs(self.table) do
		self.table[key].x=x
		self.table[key].y=y
		--self.table[key]:rotateTo(math.atan(0)) --this isnt resting the rotation

		x=x+10
	end

	

	for key,value in pairs(self.table) do
		self.table[key].z_index=1
	end

end
function UI:UnDisplay()
	self.open=false

	for key,value in pairs(self.table) do
		self.table[key].z_index=-1
	end

end

function UI:DropItem(x,y)
	if self.items > 0 then
		self.items=self.items-1
		self.table[self.items]:shape( self.table[self.items].oldshape )
		self.table[self.items].z_index=1
		self.table[self.items].x=x
		self.table[self.items].y=y
		self.table[self.items].vx=0
		self.table[self.items].vy=0

		table.remove(self.table,self.items)
	end


end

