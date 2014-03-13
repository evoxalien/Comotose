
hero = Hero.create()
--hero.cast_shadow = false

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

chair = PhysicsObject.create()
chair.x = 50
chair.y = 12
chair:sprite("Chair1")

chair = PhysicsObject.create()
chair.x = 50
chair.y = 15
chair:sprite("Chair1")

chair = PhysicsObject.create()
chair.x = 50
chair.y = 10
chair:sprite("Chair1")


chair = PhysicsObject.create()
chair.x = 54
chair.y = 10
chair:sprite("Chair1")

chair = PhysicsObject.create()
chair.x = 50
chair.y = 10
chair:sprite("Chair1")

chair = PhysicsObject.create()
chair.x = 50
chair.y = 10
chair:sprite("Chair1")

function light2:everyFrame()
	self.x = hero.x
	self.y = hero.y
	self:rotate(hero.rotation)
	
	print(hero.rotation)
	print(self.rotation)
end