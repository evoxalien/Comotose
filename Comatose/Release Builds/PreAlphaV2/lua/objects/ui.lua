--the current method of removing the shape from the object isnt going to work for mouse input
	--the shape needs to be there for nicks click detection, have to find some other method
	--to make sure it does not collide and stuff

UI= inherits(GameObject)

function UI:init()
	self.table={}
	self.items=1 			--lua tables are 1 based, start at 1...
	self.open=false
	self.selected=1			--lua tables are 1 based, start at 1...
	self.text=TextBox.create()
	self.text.width=400
	self.text.height=50
	self.text.maxline=2
	self.text.z_index=-1
	self.text:color(255,255,255,255)
	self.text:position(10,10) --ontop the the item 
	self.text:font("buxton")
	self.text:text("empty")

end

function UI:AddObject(o,name)
	o.oldshape=o:GetShape()
	o:shape("none")		   --make sure the object doesnt collide with anything
	o.z_index=-1		   --remove  from screen by setting behind the map
	o.in_inv=true
	o.name=name

	--stop the object from moving and straighten it
	o.resetPosition()

	self.table[self.items]=o --insert into the table
	print("adding at index " .. self.items)

	self.selected=self.items
	self.items=self.items+1  

	if self.open ==true then
		self:Display() --refresh the menu
	end

end
function UI:Display()

	--be sure to display text or hide it
	if self.items==1 then
		self.text.z_index=-1
	else
		self.text.z_index=500
	end


	if self.items > 1 then --only let the menu open if there are items in it
		self.open=true	

		local x = 10
		local y = 10

		--create a nice menu
		for key,value in pairs(self.table) do
			self.table[key].x=x
			self.table[key].y=y
			self.table[key].z_index=1

			x=x+10	

			--self.table[self.selected]:shape( self.table[self.selected].oldshape )


			if key==self.selected then

				self.text:attach(self.table[key].ID())

				if self.table[key].name ~=nil then
					self.text:text(self.table[key].name)
				end

				self.table[key]:color(254,254,254,254)


			else

				self.table[key]:color(254,254,254,128)

			end


		end
	end
end
function UI:UnDisplay()
	self.open=false
	self.text.z_index=-1


	for key,value in pairs(self.table) do
		self.table[key].z_index=-1

		--self.table[key].oldshape=self.table[key]:GetShape()
		self.table[key] :shape("none")		   --make sure the object doesnt collide with anything
	end

end
function UI:DropItem(x,y)
	if self.items > 1 then

		--place the item back into the world infront of the player 
		self.table[self.selected]:shape( self.table[self.selected].oldshape )
		self.table[self.selected].z_index=1
		self.table[self.selected].x=x
		self.table[self.selected].y=y
		self.table[self.selected].vx=0
		self.table[self.selected].vy=0
		self.table[self.selected].in_inv=false

		--remove from table
		table.remove(self.table,self.selected)

		self.items=self.items-1

		--select a new object because we just got ride of the selected on
		if self.selected==1 then
			self:SelectFirst()
		elseif self.selected==self.items then
			self:SelectLast()
		end

		--hide text
		if self.items==1 then
			self.text:attach(-1)
			self.text.z_index=-1
		end
	end

	self:Display() --refresh the display

end
function UI:SelectLeft()

	if self.selected > 1 and self.items >1	 then --just move left
		self.selected = self.selected -1
	elseif self.selected ==1 then --loop around
		self.selected=self.items-1
	end

	self:Display() --refresh the display
end
function UI:SelectRight()

	if self.selected < self.items-1 and self.items-1 >1	 then --just move right
		self.selected = self.selected +1
	elseif self.selected ==self.items-1 then --loop around
		self.selected=1
	end
	self:Display() --refresh the display
end
function UI:SelectFirst()
	self.selected=1
	self:Display()
end
function UI:SelectLast()
	self.selected=self.items-1
	self:Display()
end
function UI:SelectID(id)
	
	for key,value in pairs(self.table) do 
		print(self.table[key].ID())
		print(id)
		if id==self.table[key].ID() then
			self.selected=key
			print(self.selected)
		end
	end
end

