UI= inherits(GameObject)

function UI:init()
	self.table={}
	self.items=0
	self.open=false
end
function UI:AddObject(o)

	o.oldshape=o:GetShape()
	o:shape("none")		   --make sure the object doesnt collide with anything
	o.z_index=-1		   --remove  from screen by setting behind the map

	--stop the object from moving and straighten it
	o.resetPosition()

	self.table[self.items]=o --insert into the table

	self.items=self.items+1  

end
function UI:Display()
	self.open=true	

	local x = 10
	local y = 10

	--create a nice menu
	for key,value in pairs(self.table) do
		self.table[key].x=x
		self.table[key].y=y
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

		--place the item back into the world infront of the player 
		self.table[self.items]:shape( self.table[self.items].oldshape )
		self.table[self.items].z_index=1
		self.table[self.items].x=x
		self.table[self.items].y=y
		self.table[self.items].vx=0
		self.table[self.items].vy=0

		table.remove(self.table,self.items)
	end


end

