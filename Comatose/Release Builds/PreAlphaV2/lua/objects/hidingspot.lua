

HidingSpot = inherits(PhysicsObject)

function HidingSpot:init()
	self.centered = true
	self:sprite("HI_locker")
	self.width = 90
	self.height = 70
	self:body_type("static")
	self.z_index = 0.5
	self.cast_shadow = false
	self.animation = 1

	--create an itembubble
	self.title = ItemBubble.create()
	self.title:text("Hide")
	self.title.target = self
	self.title.centered = true

	self.oldx = 0
	self.oldy = 0

	--create sounds for opening and closing
	self.OpenSound=Audio.create()
	self.OpenSound:audioname("CabinetOpen00")
	self.OpenSound:attach(self:ID())

	--create sounds for opening and closing
	self.CloseSound=Audio.create()
	self.CloseSound:audioname("CabinetClose00")
	self.CloseSound:attach(self:ID())
end

function HidingSpot:everyFrame()
	if stage.hero then
		self.CloseSound:attachListener(stage.hero:ID())
		self.OpenSound:attachListener(stage.hero:ID())
	end
end

function HidingSpot:use()
	distance = stage.hero:distanceFrom(self.x, self.y)
	if distance <= 15 then
		if stage.hero.hiding then
			stage.hero.x = self.oldx
			stage.hero.y = self.oldy
			stage.hero.flashlight.x = self.oldx
			stage.hero.flashlight.y = self.oldy

			stage.hero.hiding = false
			stage.hero.flashlight.on = true
			stage.hero:body_type("dynamic")

			self.animation = 1
			self.title:text("Hide")
			self.OpenSound:Play()
		else
			self.oldx = stage.hero.x
			self.oldy = stage.hero.y
			stage.hero.x = self.x
			stage.hero.y = self.y
			stage.hero.flashlight.x = self.x
			stage.hero.flashlight.y = self.y
	
			stage.hero.hiding = true
			stage.hero.flashlight.on = false
			stage.hero:body_type("static")
			stage.hero:setRotation(self.rotation + math.pi / 2)

			self.animation = 0
			self.title:text("Leave")
			self.CloseSound:Play()
		end
	end
end

registered_objects["HidingSpot"] = {
	art="HI_locker",
	width=90,
	height=70,
	centered=true
}