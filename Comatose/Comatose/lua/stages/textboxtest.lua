

textbox = TextBox.create()
textbox.character_delay = 4
textbox:position(50,50)
textbox.width = 500
textbox.height = 50
textbox.maxLine = 3
textbox.z_index = 500
textbox:color(128,0,192,255)
textbox:font("buxton")
textbox:text("First Line\nThis line is really long and should be clipped by the engine if it is sane.\nThird Line\nFourth Line")

function textbox:everyFrame()
	if Input:WasKeyPressed("Enter") or Input:WasButtonPressed("A") then
		textbox:text("Advance the text! Do it!")
	end
end

dialogbox = DialogBox.create()

dialogbox:position(100, 300)
dialogbox.width = 500
dialogbox.height = 200
dialogbox.maxLine = 4

dialogbox:displayText({
	"This is a section of dialogue. It can be advanced by pressing the A button.",
	"This is the second text area thing.",
	"Hello World!"
})