
Log = inherits(PhysicsObject)

function Log:init()
	self:sprite("recorder-for-table")

	--create an itembubble
	self.title = ItemBubble.create()
	self.title:text("--- Log ---")
	self.title:attach(self.ID())
	self.cast_shadow = false
end

function Log:click()
	stage.dialog:portrait("VoiceRecorder")
	stage.dialog:displayText(logdata["demotext1"])
end

registered_objects["Log"] = "recorder-for-table"

--what follows here is actual log data; each is simply a table of strings to display in the text editor

logdata = {}

logdata["demotext1"] = {
	"Hmmm...where am I? It's the map for the game, but it is not complete.",
	"Who releases an incomplete game?\nOH!!! This must be the demo. Which means ....",
	"Hey everybody, welcome to the demo for Comatose!",
	"Today we will be revealing enough to show you that we are doing stuff, but not to much to spoil the game."
}