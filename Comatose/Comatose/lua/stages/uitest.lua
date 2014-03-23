hero = Hero.create()
--hero.cast_shadow = false
ui=	UI.create()
cursor=Cursor.create()
monster=Monster.create()


lightbar= Bar.create()
lightbar:set(100,100,10,680,300,5) 
lightbar:setColor(254,254,0,254)
lightbar:Center()
lightbar:setCurrent(50)

sanitybar= Bar.create()
sanitybar:set(100,100,10,700,300,5) 
sanitybar:setColor(128,128,128,254)
sanitybar:Center()



light = LightSource.create()
light.x = 25
light.y = 10
--light:sprite("Chair1")
light1 = LightSource.create()
light1.x = 65
light1.y = 62

light2 = LightSource.create()
light2.x = 65
light2.y = 31
light2.light_spread_angle = math.pi / 4
light2.rotation = hero.rotation

chair = PhysicsObject.create()
chair.x = 50
chair.y = 15
chair:sprite("Chair1")

chair2 = PhysicsObject.create()
chair2.x = 50
chair2.y = 15
chair2:sprite("Chair1")

chair3 = PhysicsObject.create()
chair3.x = 50
chair3.y = 15
chair3:sprite("Chair1")

chair4 = PhysicsObject.create()
chair4.x = 50
chair4.y = 15
chair4:sprite("Chair1")




function chair.click(mx,my)
	--if chair.in_inv== false or chair.in_inv ==nil then
	if chair.in_inv==nil or chair.in_inv==false then --has not been added yet
		ui:AddObject(chair,"chair 1")

	else
		print(chair.in_inv)
		ui:SelectID(chair.ID())
	end


end


function chair2.click(mx,my)
	if chair2.in_inv== false or chair2.in_inv==nil then
		ui:AddObject(chair2,"chair 2")
	else
		ui:SelectID(chair2.ID())
	end
end


function chair3.click(mx,my)
	if chair3.in_inv== false or chair3.in_inv==nil then
		ui:AddObject(chair3,"chair 3")
	else
		ui:SelectID(chair3.ID())
	end
end

function chair4.click(mx,my)
	if chair4.in_inv== false or chair4.in_inv==nil then
		ui:AddObject(chair4,"chair 4")
	else
		ui:SelectID(chair4.ID())
	end
end
function ui.everyFrame()

	--open and close ui
	if Input:WasKeyPressed("tab")  and not ui.open then
		ui:Display()

	elseif Input:WasButtonReleased("Y") and not ui.open then
		ui:Display()

	elseif Input:WasKeyPressed("tab") and ui.open then
		ui:UnDisplay()

	elseif Input:WasButtonReleased("Y") and ui.open then
		ui:UnDisplay()
	end

	if ui.open then
		--we can drop stuff
		if Input:WasKeyPressed("e") then
			ui:Display()
			ui:DropItem(hero.x,hero.y)
		elseif Input:WasButtonReleased("A") then
			ui:Display()
			ui:DropItem(hero.x,hero.y)
		end

		--we can select what we want
		if Input:WasButtonReleased("DPadLeft") then
			ui:SelectLeft()
		elseif Input:WasButtonReleased("DPadRight") then
			ui:SelectRight()
		elseif Input:WasKeyPressed("left") then
			ui:SelectLeft()
		elseif Input:WasKeyPressed("right") then
			ui:SelectRight()
		end
	end



		

end




function light2:everyFrame()
	self.x = hero.x
	self.y = hero.y
	self:rotate(hero.rotation)
	
	--print(hero.rotation)
	--print(self.rotation)
end

--test out some textbox stuff

textbox = TextBox.create()
--textbox.character_delay = 4
textbox:position(50,50)
textbox.width = 300
textbox.height = 50
textbox.maxLine = 3
textbox.z_index = 500
textbox:color(128,0,192,255)
textbox:attach(hero.ID())
textbox:font("buxton")
textbox:text("First Line\nThis line is really long and should be clipped by the engine if it is sane.\nThird Line\nFourth Line")
