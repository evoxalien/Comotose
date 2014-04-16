Screamer = inherits(AI)

function Screamer:init()
	self:sprite("Monster_Scream_Test")
	self.centered = true
	self.width = 68
	self.height = 65
	self.frame_delay = 5
	--self.fixedRotation = true
	self:shape("circle")
	self.cast_shadow = false
	self.speed=7.5

	stage.Screamer = self

	self:set_group("Screamer")
	self:add_target("hero")
	self.fade_timer = 0

	self.z_index = 0.5

	--state machine stuff

	self.searchtimer=120
	self.state="wander"
	--self:Target(stage.hero:ID())
end

function Screamer:handleCollision()
	stage.hero.sanity = 0
end

function Screamer:everyFrame()
	if not self:HasTarget() then
		self:setTarget(stage.hero:ID())
	end
	if 
		(GameEngine:hasLineOfSight(stage.hero:ID(), self:ID()) and stage.hero:insideFlashlight(self)) or
		self:distanceFrom(stage.hero.x, stage.hero.y) < 10 then
		stage.hero.sanity = math.max(stage.hero.sanity - 20 / 60, 0)
		stage.hero.sanity_cooldown = 5 * 60
		self.fade_timer = math.min(self.fade_timer + 2, 10)
	else
		--hide
		self.fade_timer = math.max(self.fade_timer - 1, 0)
	end
	self:color(255,255,255,255 * (self.fade_timer / 10))

	self:StateMachine()

	--turn to face our movement direction
	rotate_angle = math.atan2(self.vx, -self.vy)
	--print(rotate_angle)
	self:rotateTo(rotate_angle)
end																   


function Screamer:StateMachine()


	if self.state=="searching" then --goto last known space % of time
		print("searching")

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
		print("attacking")
		self:MoveTowardsTarget(stage.hero.x,stage.hero.y)

		if not GameEngine:hasLineOfSight(stage.hero:ID(), self:ID()) then
			self.last_seen_x=stage.hero.x
			self.last_seen_y=stage.hero.y
			
			self.state="searching"
		end



	elseif self.state=="idle" then --dont do anything,can only enter this from out of the state machine
		print("idle")

	elseif self.state=="wander" then --pick a random location on waypoint and goto
		print("wander")

		--if we see the player while wandering, attack it
		if GameEngine:hasLineOfSight(stage.hero:ID(), self:ID()) then
			self.state="attacking"
		else --we are going to wander the path by pick a random node and going to it

			--choose a random point
			if not self.wandering then
				self.RandomWaypoint()
				self.wandering =true
			end
			self.Wander()
		end

	end
end

--registered_objects["Screamer"] = "ScreamerV_1"
registered_objects["Screamer"] = {
	art="Monster_Scream_Test",
	width=67.5,
	height=64,
	centered=true
}
