
Fireball= inherits(AI)

function Fireball:init()
	self:sprite("pixel")

	self.z_index = 0.5
	self.centered = true
	self.cast_shadow = false
	self.width=1
	self.height=1
	self:setDensity(0.01)

	--copied from hero shamelessly
	self.firelight1 = LightSource.create()
	self.firelight1.cast_shadow=false
	self.firelight1:shape("none")
	self.firelight1:color(210, 50, 45, 128)
	self.firelight1.ray_length = 15

	--audio
	self.audio=Audio.create()
	self.audio:audioname("fire00")
	self.audio:attach( self:ID())
	if stage.hero ~= null then
		self.listenerset=true
		self.audio:attachListener( stage.hero:ID())
		self.audio:play()
	end


	self.count = 0

	--how long this fire will burn
	self.burn_time=300
	self.timer=self.burn_time
	self.on=true


	--moving
	self.targetx=0
	self.targety=0
	self.speed=20

	self:set_group("monster")
	self:add_target("hero")
end

function Fireball:handleCollision()
	stage.hero.on_fire = 60 * 3.0
end

function Fireball:everyFrame()

	if self.on	 then
		if self.firelight1~=nil then
			self.firelight1.x = self.x
			self.firelight1.y = self.y
		end

		if (self.count % 4) == 0 then
			Effect:CreateFireBall(self.x , self.y - 1, 55)
		end

		--if the player gets too close, set them on FIRE!!! (super fun time)

		self:AudioMachine()

		self:MoveToPoint(self.targetx,self.targety)

		self.count = self.count + 1
		self.timer=self.timer-1
		if self.timer <0 then
			self.on=false

		end
	end
	
end

function Fireball:Hide()
	self:shape("none")		   --make sure the object doesnt collide with anything
	self.z_index=-1		       --remove  from screen by setting behind the map
	self.audio:stop()			--make sure its not playing any more audio
	self.firelight1.ray_length = 0

	--self.firelight1.destroy()

	--stop the object from moving and straighten it
	self.resetPosition()
end


--resets the fireball
function Fireball:Spawn()
	self:shape("box")
	self.z_index=1
	self.on=true
	self.timer=self.burn_time
	--self.audio:play()

	--copied from hero shamelessly
	--self.firelight1 = LightSource.create()
	--self.firelight1.cast_shadow=false
	--self.firelight1:shape("none")
	--self.firelight1:color(210, 50, 45, 128)
	--self.firelight1.ray_length = 15



	self.firelight1.ray_length = 15
end

function Fireball:AudioMachine()
	if stage.listener~= true then
		self.listenerset=true
		self.audio:attachListener( stage.hero:ID())
		self.audio:Calc3D()
	end



	if self.audio.isPlaying==false then --loop it in lua
		self.audio:play()
	end
		self.audio:Calc3D()
		
end

registered_objects["Fireball"] = {
	art="pixel",
	centered=true
}