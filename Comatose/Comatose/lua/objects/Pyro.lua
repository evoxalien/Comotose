
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
	self.audio:play()

	--set up flashlight stuff
	--self.flame = Flame.create()
	--self.flame.cast_shadow = false
	--offset the light so it's in the hero's "hand"
	--self.flame.x = self.x - 1.0
	--self.flame.y = self.y + 1.7
	--self.flame:join(self:ID(), "weld")

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

	--hand held fireball!
	self.handheld= Flame.create()
	self.handheld.x = self.x -1.0
	self.handheld.y = self.y +1.9
	self.handheld:join(self:ID(),"weld")



	--state machine stuff
	self.searchtimer=700
	self.wandertimer=1000
	self.state="wander"


	--pyro fireball 
	self.fireballs={}
	self.fireball_count=1
	self.fireball_time=10
	self.fireball_limit=5
	self.fireball_timer=self.fireball_time
	self.fireball_oldest=1

	--self:Target(stage.hero:ID())
end

function Pyro:handleCollision()
	--stage.hero.sanity = 0
end

function Pyro:everyFrame()
	if not self:HasTarget() then
		self:setTarget(stage.hero:ID())
	end
	
	if stage.hero:canSee(self) then
		if self.handheld.on==false then
			self.handheld:Spawn()
		end
		self.fade_timer = math.min(self.fade_timer + 2, 10)
	else
		--hide
		self.fade_timer = math.max(self.fade_timer - 1, 0)
		self.handheld:Hide()
	end
	self:color(255,255,255,255 * (self.fade_timer / 10))


	--turn to face our movement direction
	rotate_angle = math.atan2(self.vx, -self.vy) 
	self:rotateTo(rotate_angle)

	--dampen (no spinspinspin)
	self.vr = self.vr / 2


	--gather used fireballs
	for key,value in pairs(self.fireballs) do
		--the fire is off, lets gather it somewhere nice
		if self.fireballs[key].on == false then
			self.fireballs[key]:Hide()
		end
	end




	--ai logic
	self:StateMachine()

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

			self:MoveTowardsTarget(stage.hero.x,stage.hero.y)
			if self:distanceFrom(stage.hero.x,stage.hero.y) <50 then
				--stop moving
				--self.vy=0
				--self.vx=0


				--if the pyro hasnt spawned enough fireball spawn new ones
				if self.fireball_timer <0  and self.fireball_count <self.fireball_limit then
					self.fireballs[self.fireball_count]=Fireball.create()

					self:ThrowFireball(self.fireball_count)

					self.fireball_timer=self.fireball_time
					self.fireball_count=self.fireball_count+1
				end

				--if we have spawned to the limit, reuse old fireball
				if self.fireball_timer <0  and self.fireball_count == self.fireball_limit then

					if self.fireballs[self.fireball_oldest].on ==false then
						
						self.fireballs[self.fireball_oldest]:Spawn()

						self:ThrowFireball(self.fireball_oldest)

						self.fireball_timer=self.fireball_time
						self.fireball_oldest=self.fireball_oldest+1



						if self.fireball_oldest == self.fireball_count then
							self.fireball_oldest=1
						end 
					end
				end




				self.fireball_timer	=self.fireball_timer-1
			end
end

function Pyro:ThrowFireball(i)
	self.fireballs[i].x=self.x
	self.fireballs[i].y=self.y
	self.fireballs[i].targetx=stage.hero.x
	self.fireballs[i].targety=stage.hero.y
end


function Pyro:AudioMachine()

	if stage.listener~= true then
		self.listenerset=true
		self.audio:attachListener( stage.hero:ID())
	end

	if self.audio.isPlaying==false then --loop it in lua
		self.audio:play()
	end
		
	self.audio:Calc3D()
end

function Pyro:switch(trigger)
	if trigger == "generator"  then --wussy mode engage!


		for key,value in pairs(self.fireballs) do --hide all the fireballs!
			--the fire is off, lets gather it somewhere nice
			self.fireballs[key].on = false 
			self.fireballs[key]:Hide()
		end

		--hide the pyros handheld fire
		self.handheld.on=false
		self.handheld:Hide()

		self:shape("none")		   --make sure the object doesnt collide with anything
		self.z_index=-1		       --remove  from screen by setting behind the map
		self.audio:stop()			--make sure its not playing any more audio

		--stop the object from moving and straighten it
		self.resetPosition()


	end

end


registered_objects["Pyro"] = {
	art="Pyro",
	width=64,
	height=64,
	centered=true
}
