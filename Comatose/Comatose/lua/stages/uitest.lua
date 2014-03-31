hero = Hero.create()
--hero.cast_shadow = false
ui=	UI.create()
cursor=Cursor.create()


lightbar= Bar.create()
lightbar:set(100,100,10,680,300,5) 
lightbar:setColor(254,254,0,254)
lightbar:Center()
lightbar:setCurrent(50)

sanitybar= Bar.create()
sanitybar:set(100,100,10,700,300,5) 
sanitybar:setColor(128,128,128,254)
sanitybar:Center()





chair = PhysicsObject.create()
chair.x = 40
chair.y = 15
chair:sprite("Chair1")

--chair2 = PhysicsObject.create()
--chair2.x = 50
--chair2.y = 15
--chair2:sprite("Chair1")

--chair3 = PhysicsObject.create()
--chair3.x = 60
--chair3.y = 15
--chair3:sprite("Chair1")




function chair.click(mx,my)
	ui:AddObject(chair)
	--chair.z_index=-1
end


--function chair2.click(mx,my)
--	ui:AddObject(chair2)
	--chair.z_index=-1
--end
--function chair3.click(mx,my)
--	ui:AddObject(chair3)
	--chair.z_index=-1
--end




function ui.everyFrame()

	if Input:WasKeyPressed("tab")  and not ui.open then
		ui:Display()

	elseif Input:WasKeyPressed("tab") and ui.open then
		ui:UnDisplay()
	end

	if Input:WasKeyPressed("e") then
		ui:UnDisplay()
		ui:DropItem(hero.x,hero.y)
	end

	if ui.open then
		if Input:WasButtonReleased("DPadLeft") then
			print("selecting to left")
			ui:SelectLeft()

		elseif Input:WasButtonReleased("DPadRight") then
			print("selecting to right")
			ui:SelectRight()
		end
	end



		

end






--test out some textbox stuff

textbox = TextBox.create()
--textbox.character_delay = 4
textbox:position(50,50)
textbox.width = 300
textbox.height = 50
textbox.maxLinex = 3
textbox.z_index = 500
textbox:color(128,0,192,255)
textbox:attach(hero.ID())
textbox:font("buxton")
textbox:text("First Line\nThis line is really long and should be clipped by the engine if it is sane.\nThird Line\nFourth Line")
