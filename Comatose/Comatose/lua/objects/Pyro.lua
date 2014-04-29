
Pyro= inherits(AI)

function Pyro:init()
	self:sprite("Pyro")

	--audio
	self.audio=Audio.create()
	self.audio:audioname("chatter")
	self.audio:attach( self:ID())
	if stage.hero ~= null then
		self.listenerset=true
		self.audio:attachListener( stage.hero:ID())
	end
	self.audio.looped = true
	self.audio:play()

	--position & collision
	self.centered = true
	self.width = 95
	self.height = 80
	self.frame_delay = 0
	self:shape("circle")
	self.cast_shadow = false
	self.speed = 7.5
	stage.monster = self
	self:set_group("pyro")
	self:add_target("hero")
	self.fade_timer = 0
	self.z_index = 0.5



	--state machine stuff
	self.searchtimer=700
	self.wandertimer=1000
	self.state="wander"


	--pyro fireball 
	self.fireballs={}
	self.fireball_count=1
	self.fireball_time=5
	self.fireball_limit=15
	self.fireball_timer=self.fireball_time
	self.fireball_oldest=1

	--self:Target(stage.hero:ID())
	self.firelight1 = LightSource.create()
	self.firelight2 = LightSource.create()
	self.firelight1:shape("none")
	self.firelight2:shape("none")
	self.firelight1:color(210, 50, 45, 0)
	self.firelight2:color(230, 170, 20, 0)
	self.firelight1.ray_length = 2
	self.firelight2.ray_length = 2
	self.on_fire = 0
end

function Pyro:handleCollision()
	--stage.hero.sanity = 0
end

function Pyro:everyFrame()
	if not self:HasTarget() then
		self:setTarget(stage.hero:ID())
	end

	if stage.listener~= true then
		self.listenerset=true
		self.audio:attachListener( stage.hero:ID())
	end
	
	if stage.hero:canSee(self) then
		if stage.hero.flashlight:isIlluminating(self:ID()) then
			stage.hero.sanity = math.max(stage.hero.sanity - 20 / 60, 0)
			stage.hero.sanity_cooldown = 5 * 60
		else
			stage.hero.sanity = math.max(stage.hero.sanity - 5 / 60, 0)
			stage.hero.sanity_cooldown = 5 * 60
		end
		self.fade_timer = math.min(self.fade_timer + 2, 10)
	else
		--hide
		self.fade_timer = math.max(self.fade_timer - 1, 0)
	end
	self:color(255,255,255,255 * (self.fade_timer / 10))


	--turn to face our movement direction
	rotate_angle = math.atan2(self.vx, -self.vy) 
	--print(rotate_angle)
	self:rotateTo(rotate_angle)

	self.audio:Calc3D()

	--print( self.fireball_count)

	--ai logic
	self:StateMachine()

	self.firelight1.x = self.x
	self.firelight1.y = self.y
	self.firelight2.x = self.x
	self.firelight2.y = self.y
	--FUN TIMES
	
	if self.on_fire > 0 then
		self.firelight1.ray_length = 20
		self.firelight2.ray_length = 25
		self.firelight1:color(210, 50, 45, 64)
		self.firelight2:color(230, 170, 20, 100)
		if (self.count % 3) == 0 then
			--Effect:CreateExplosion(self.x , self.y, 10, 255, 255, 255)
			--Effect:CreateFire(self.x , self.y, 45)
			self.firelight1:color(210, 50, 45, 128)
		end

		if (self.count % 2) == 0 then
			--Effect:CreateExplosion(self.x , self.y, 10, 255, 255, 255)
			self.firelight2:color(230, 170, 20, 200)
			Effect:CreateFire(self.x , self.y, 45)

		end
		
		
	else
		self.firelight1.ray_length = 2
		self.firelight2.ray_length = 2
		self.firelight1:color(210, 50, 45, 0)
		self.firelight2:color(230, 170, 20, 0)
	end
end



function Pyro:StateMachine()


	if self.state=="searching" then --goto last known space % of time

		if GameEngine:hasLineOfSight(stage.hero:ID(), self:ID()) then
			self.state="attacking"


		else	--only search for a certain amount of time
			self:MoveTowardsTarget(self.last_seen_x,self.last_seen_y)
			self.searchtimer= self.searchtimer-1

			if self.searchtimer < 0  then
				self.state="wander"
				self.searchtimer=400
			end
		end
																	

	elseif self.state=="attacking" then

		self:AttackStateMachine()

		if not GameEngine:hasLineOfSight(stage.hero:ID(), self:ID()) then
			self.last_seen_x=stage.hero.x
			self.last_seen_y=stage.hero.y
			
			self.state="searching"
		end



	elseif self.state=="idle" then --dont do anything,can only enter this from out of the state machine

	elseif self.state=="wander" then --pick a random location on waypoint and goto

		--if we see the player while wandering, attack it
		if GameEngine:hasLineOfSight(stage.hero:ID(), self:ID()) then
			self.state="attacking"
		else --we are going to wander the path by pick a random node and going to it

			self.wandertimer= self.wandertimer-1
			if self.wandertimer<0 then
				self.RandomWaypoint()
				self.wandertimer=1000
			end


			--choose a random point
			if not self.wandering then
				self.RandomWaypoint()
				self.wandering =true
			end
			self.Wander()
		end

	end
end


function Pyro:AttackStateMachine()

		--throw fireballs


			--keep distance
			--about the distance of the flash light
			if self:distanceFrom(stage.hero.x,stage.hero.y) <50 then
				--stop moving
				self.vy=0
				self.vx=0

				--if the pyro hasnt spawned enough fireball spawn new ones
				if self.fireball_timer <0  and self.fireball_count <self.fireball_limit then
					self.fireballs[self.fireball_count]=Fireball.create()
					self.fireballs[self.fireball_count].x=self.x
					self.fireballs[self.fireball_count].y=self.y

					self.fireball_timer=self.fireball_time
					self.fireball_count=self.fireball_count+1
				end

				--if we have spawned to the limit, reuse old fireball
				if self.fireball_timer <0  and self.fireball_count == self.fireball_limit then

					if self.fireballs[self.fireball_oldest].on ==false then
						
						self.fireballs[self.fireball_oldest]:Spawn()
						self.fireballs[self.fireball_oldest].x=self.x
						self.fireballs[self.fireball_oldest].y=self.y

						self.fireball_timer=self.fireball_time
						self.fireball_oldest=self.fireball_oldest+1



						if self.fireball_oldest == self.fireball_count then
							self.fireball_oldest=1
						end 
					end
				end




				self.fireball_timer	=self.fireball_timer-1

			else
				self:MoveTowardsTarget(stage.hero.x,stage.hero.y)
			end


			--gather used fireballs
			for key,value in pairs(self.fireballs) do
				--the fire is off, lets gather it somewhere nice
				if self.fireballs[key].on == false then
					self.fireballs[key]:Hide()
				end
			end


		--set self on fire and move towards player	
		


end

registered_objects["Pyro"] = {
	art="Pyro",
	width=64,
	height=64,
	centered=true
}
