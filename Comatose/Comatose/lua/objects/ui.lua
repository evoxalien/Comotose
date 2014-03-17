UI= inherits(GameObject)

function UI:init()
	self.table={}
	self.items=0
	self.open=false
	self.selected=0
end

function UI:AddObject(o)

	o.oldshape=o:GetShape()
	o:shape("none")		   --make sure the object doesnt collide with anything
	o.z_index=-1		   --remove  from screen by setting behind the map

	--stop the object from moving and straighten it
	o.resetPosition()

	self.table[self.items]=o --insert into the table

	self.items=self.items+1  
	self.selected=self.items-1

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
		self.table[key].z_index=1
		if key==self.selected then
			self.table[key]:color(254,254,254,254)
		else
			self.table[key]:color(254,254,254,128)
		end
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
		self.table[self.selected]:shape( self.table[self.selected].oldshape )
		self.table[self.selected].z_index=1
		self.table[self.selected].x=x
		self.table[self.selected].y=y
		self.table[self.selected].vx=0
		self.table[self.selected].vy=0

		table.remove(self.table,self.selected)
		self:SelectLeft()
	end


end
function UI:SelectLeft()

	self:UnDisplay()
	if self.selected > 0 and self.items >0	 then --just move down
		self.selected = self.selected -1
	elseif self.selected ==0 then --loop around
		self.selected=self.items-1
	end
	self:Display()

	print(self.selected)
end

function UI:SelectRight()
	self:UnDisplay()

	if self.selected < self.items-1 and self.items-1 >0	 then --just move down
		self.selected = self.selected +1
	elseif self.selected ==self.items-1 then --loop around
		self.selected=0
	end

	self:Display()

	print(self.selected)
end

