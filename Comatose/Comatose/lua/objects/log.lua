
Log = inherits(PhysicsObject)

function Log:init()
	self:sprite("recorder-for-table")
	self:body_type("static")
	self:shape("none")
	self.cast_shadow = false
	self.z_index = 0.5
	self.centered = true

	--create an itembubble
	self.title = ItemBubble.create()
	self.title:text("Log")
	self.title.target = self
	self.title.centered = true

	self.text = self.text or "demotext1"
end

function Log:use()
	distance = stage.hero:distanceFrom(self.x, self.y)
	if distance <= 15 then
		stage.dialog:portrait("VoiceRecorder")
		stage.dialog:displayText(logdata[self.text])
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

logdata["entrance"] = {
	"Is it this late already? Maintenance has already shut off the lights.",
	"*sigh* I really should head home.\n[Press F to use flashlight]"
}

logdata["elevator"] = {
	"...great. Don't tell me the power's out.",
	"At least I'm not stuck between floors. I wonder why the backup generator hasn't kicked in?",
	"I'd better go check on it."
}

logdata["mechanic1"] = {
	"Holy---!!\n[Press SHIFT to book it.]"
}

logdata["mechanic2"] = {
	"Are they still there?",
	"No time to think; I've got to find somewhere to hide!",
	"[Press E next to a hiding spot]"
}

logdata["mechanic3"] = {
	"That's one of my patients... but I've never seen him this angry.",
	"Calming him down doesn't look like an option right now; I need to find a way past him."
}

