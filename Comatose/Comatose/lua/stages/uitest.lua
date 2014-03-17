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




function chair.click(mx,my)
	ui:AddObject(chair)
	--chair.z_index=-1
end


function chair2.click(mx,my)
	ui:AddObject(chair2)
	--chair.z_index=-1
end



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
--textbox.character_delay = 0
textbox:position(50,50)
textbox.z_index = 500
textbox:attach(hero.ID())
textbox:text("Hello World! Hello World! Hello World! Hello World! Hello World!")

