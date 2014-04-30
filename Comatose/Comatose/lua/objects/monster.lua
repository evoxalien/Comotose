
Monster = inherits(AI)

function Monster:init()
	self:sprite("MonsterV_1")

	--audio
	self.audio=Audio.create()
	self.audio:audioname("CrawlerMove02")
	self.audio:attach( self:ID())
	if stage.hero ~= null then
		self.listenerset = true
		self.audio:attachListener( stage.hero:ID() )
	end
	--/audio

	self.centered = true
	self.width = 64
	self.height = 64
	self.frame_delay = 5
	self:shape("circle")
	self.cast_shadow = false
	self.speed = 7.5

	self.count = 0

	stage.monster = self

	self:set_group("monster")
	self:add_target("hero")
	self.fade_timer = 0

	self.z_index = 0.5



	--state machine stuff
	self.searchtimer=700
	self.wandertimer=1000
	self.state="wander"

	--self:Target(stage.hero:ID())
end

function Monster:handleCollision()
	--stage.hero.sanity = 0
end

function Monster:everyFrame()
	if not self:HasTarget() then
		self:setTarget(stage.hero:ID())
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
	rotate_angle = math.atan2(self.vx, -self.vy) + math.pi
	self:rotateTo(rotate_angle)
	number = math.random(30, 50)
	
	if( self.count % number) == 0 then
		self:AudioMachine()
	end
	self:StateMachine()
	self.count = self.count + 1
end

function Monster:AudioMachine()
	if stage.listener~= true then
		self.listenerset=true
		self.audio:attachListener( stage.hero:ID())
	end

	if self.audio.isPlaying==false then --loop it in lua
		self.audio:play()
	end
		
	self.audio:Calc3D()
end


function Monster:StateMachine()


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
		self:MoveTowardsTarget(stage.hero.x,stage.hero.y)

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



--registered_objects["Monster"] = "MonsterV_1"
registered_objects["Monster"] = {
	art="MonsterV_1",
	width=64,
	height=64,
	centered=true
}
