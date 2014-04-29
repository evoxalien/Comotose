
Log = inherits(PhysicsObject)

function Log:init()
	self:sprite("recorder-for-table")
	self:body_type("static")
	self.cast_shadow = false
	self.z_index = 0.5
	self.centered = true

	--create an itembubble
	self.title = ItemBubble.create()
	self.title:text("Log")
	self.title.target = self
	self.title.centered = true
end

function Log:use()
	distance = stage.hero:distanceFrom(self.x, self.y)
	if distance <= 15 then
		stage.dialog:portrait("VoiceRecorder")
		stage.dialog:displayText(logdata["demotext1"])
		if self.trigger_event then
			trigger_event(self.trigger_event)
			self.trigger_event = nil -- prevent repeated triggerings
		end
	end
end

registered_objects["Log"] = "recorder-for-table"

--what follows here is actual log data; each is simply a table of strings to display in the text editor

logdata = {}

logdata["demotext1"] = {
	"BOW TO ME!!!\n(demo time, spawn monsters now)"
}