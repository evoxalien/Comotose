hero = Hero.create()
ui=	UI.create()
cursor=Cursor.create()
monster=Monster.create()

monster:Target(hero.ID())

chair = PhysicsObject.create()
chair.x = 40
chair.y = 15
chair:sprite("Chair1")


function ui.everyFrame()

		

end

