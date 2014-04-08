hero = Hero.create()
ui=	UI.create()
cursor=Cursor.create()
monster=Monster.create()

p1 = Waypoint.create({x=0,y=0})
p2 = Waypoint.create({x=10,y=10})
p3 = Waypoint.create({x=25,y=20})

monster:Target(hero.ID())


chair = PhysicsObject.create()
chair.x = 40
chair.y = 15
chair:sprite("Chair1")


function ui.everyFrame()

		

end
