
map = Map.create()
map.debugdraw = true
mapdata = {edges={}}

transparent_map = Map.create()
transparent_map.debugdraw = true
transparent_map.cast_shadow = false

--camera for moving around
camera = WASDcamera.create()

--cursor, for selecting stuff
cursor = Cursor.create()
cursor:color(255, 128, 0, 255)

function image(path)
	map:sprite(path)
	mapdata.image = path
end

--placeholder for verticies
VertexHandle = inherits(PhysicsObject)

function VertexHandle:init()
	self:body_type("static")
	self:sprite("vertex_handle")
	self:color(128, 128, 128, 255)
	self:shape("circle")
	self.centered = true
	self.z_index = 0.5

	self.previous = nil
	self.next = nil
	self.edgeID = nil --only valid for the first vertex in a chain, for deletions to be properly handled
end

function find_edge_id(vertex)
	--traverse the chain backwards until you hit the beginning
	while vertex.previous do
		vertex = vertex.previous
	end
	return vertex.edge
end

function find_chain_end(vertex)
	while vertex.next do
		vertex = vertex.next
	end
	return vertex
end

function find_chain_begin(vertex)
	while vertex.previous do
		vertex = vertex.previous
	end
	return vertex
end

function reverse_list(head)
	--reverses a linked list in place, then returns the new *tail*
	node = head
	while node do
		--swap next and previous
		temp = node.next
		node.next = node.previous
		node.previous = temp

		node = node.previous
	end
	return head
end

--Handle what happens when any vertex is clicked, including all the weird cases
--where something else was already clicked before this one.
function VertexHandle:click()
	--bail on double click
	if selected_vertex == self then
		clear_selection()
		handled_by_vertex = true
		return
	end

	if selected_vertex == nil then
		select_vertex(self)
	else
		--test for looped points; if either point is looped, bail
		if self.looped or selected_vertex.looped then
			handled_by_vertex = true
			return
		end
		--if either vertex is invalid (no free edges) bail
		if (self.previous and self.next) or (selected_vertex.previous and selected_vertex.next) then
			handled_by_vertex = true
			return
		end

		--TODO: Handle "edge" cases
		clicked_edge = find_edge_id(self)
		selected_edge = find_edge_id(selected_vertex)
		if clicked_edge == selected_edge then
			--simple case: we are closing a loop
			mapdata.edges[clicked_edge].looped = true
			find_chain_end(self).looped = true
			clear_selection()
			recolor(self)
		else
			--complex case: we're connecting to the start (or end!) of another chain
			if self.previous == nil and selected_vertex.next == nil then
				--my head to your tail
				selected_vertex.next = self
				self.previous = selected_vertex
				mapdata.edges[self.edge] = nil
				self.edge = nil
				clear_selection()
				recolor(self)
			elseif self.next == nil and selected_vertex.previous == nil then
				--my tail to your head
				self.next = selected_vertex
				selected_vertex.previous = self
				mapdata.edges[selected_vertex.edge] = nil
				selected_vertex.edge = nil
				clear_selection()
				recolor(self)
			elseif self.previous == nil and selected_vertex.previous == nil then
				--my head to your head; this one's complicated
				
				--kill the edge pointers; we need to establish a *new* head
				mapdata.edges[self.edge] = nil
				mapdata.edges[selected_vertex.edge] = nil
				self.edge = nil
				selected_vertex.edge = nil

				--reverse the "selected" linked list, and attach its tail to our head
				new_tail = reverse_list(selected_vertex)
				new_tail.next = self
				self.previous = new_tail
				
				--finally, add the new chain beginning to the edge list
				new_head = find_chain_begin(new_tail)
				mapdata.edges[next_edge_id] = new_head
				new_head.edge = next_edge_id
				next_edge_id = next_edge_id + 1
				recolor(self)
			elseif self.next == nil and selected_vertex.next == nil then
				--my tail to your tail; equally complicated
				selected_head = find_chain_begin(selected_vertex)
				mapdata.edges[selected_head.edge] = nil

				selected_tail = reverse_list(find_chain_begin(selected_vertex))
				selected_head = find_chain_begin(selected_tail)
				self.next = selected_head
				selected_head.previous = self
				clear_selection()
				recolor(self)
			end
		end
	end
	handled_by_vertex = true
end

selected_vertex = nil
function select_vertex(vertex)
	clear_selection()
	selected_vertex = vertex
	recolor(vertex)
end

function clear_selection()
	if selected_vertex then
		vertex = selected_vertex
		selected_vertex = nil
		recolor(vertex)
	end
end

function recolor(vertex)
	alpha = 96
	vertex:sprite("vertex_handle")
	if selected_vertex == vertex then
		alpha = 255
		vertex:sprite("vertex_handle_selected")
	end
	-- default grey for orphans
	red = 128
	green = 128
	blue = 128
	if vertex.previous and not vertex.next then
		--tail (darker)
		blue = 0
		if vertex.looped then
			red = 64
			green = 128
		else
			red = 128
			green = 64
		end
	end
	if not vertex.previous and vertex.next then
		--head (darker)
		blue = 0
		if vertex.looped then
			red = 0
			green = 255
		else
			red = 255
			green = 0
		end
	end
	if vertex.next and vertex.previous then
		red = 0
		green = 255
		blue = 255
	end
	vertex:color(red,green,blue,alpha)
end

next_edge_id = 1
function stage.click()
	if not handled_by_vertex then
		if selected_vertex == nil then
			--create a new chain / vertex at this point
			new_vertex = VertexHandle.create({x=mouse.x,y=mouse.y})
			select_vertex(new_vertex)
			--add the new vertex to a new edge chain
			mapdata.edges[next_edge_id] = new_vertex
			new_vertex.edge = next_edge_id
			next_edge_id = next_edge_id + 1
		else
			--figure out if anything is legal here and do something
			if selected_vertex.next == nil and not selected_vertex.looped then
				--create a new vertex, and join it to this one
				new_vertex = VertexHandle.create({x=mouse.x,y=mouse.y})
				new_vertex.previous = selected_vertex
				selected_vertex.next = new_vertex
				select_vertex(new_vertex)
			elseif selected_vertex.previous == nil and not selected_vertex.looped then
				--create a new vertex, this time attaching to the REAR (this can happen sometimes
				new_vertex = VertexHandle.create({x=mouse.x,y=mouse.y})
				new_vertex.next = selected_vertex
				selected_vertex.previous = new_vertex
				--some weirdness: this is likely the beginning of an edge, so re-point that edge to this new vertex
				if selected_vertex.edge then
					mapdata.edges[selected_vertex.edge] = new_vertex
					new_vertex.edge = selected_vertex.edge
					selected_vertex.edge = nil
				end
				select_vertex(new_vertex)
			else
				clear_selection() --error state, clear selection to show this
			end
		end
	end
	handled_by_vertex = false
	process_collision()
end

function toggleShadow(vertex)
	head = find_chain_begin(vertex)
	if head.transparent then
		head.transparent = nil
	else
		head.transparent = true
	end
end

function stage.everyFrame()
	if Input:WasKeyPressed("Escape") then
		clear_selection()
	end

	if Input:WasKeyPressed("X") then
		if selected_vertex then
			delete_vertex(selected_vertex)
			process_collision()
		end
	end

	if Input:WasKeyPressed("C") then
		if selected_vertex then
			
		end
	end
end

function process_collision()
	map:resetCollision()
	print("recalculating collision...")
	for k,chainstart in pairs(mapdata.edges) do
		if chainstart.next then
			--this chain is longer than one element, draw it
			i = 0
			map:beginChain()
			vertex = chainstart
			while vertex do
				map:addVertex(vertex.x, vertex.y)
				vertex = vertex.next
				i = i + 1
			end
			print("made a chain of " .. i .. " elements")
			map:endChain(chainstart.looped == true) --todo: handle loops?
		end
	end
end

function delete_vertex(vertex)
	--several cases here

	--easy case: orphans
	if vertex.previous == nil and vertex.next == nil then
		mapdata.edges[vertex.edge] = nil
		vertex:destroy()
		selected_vertex = nil
	end

	--tail deletion (looped?)
	if vertex.previous and not vertex.next then
		if vertex.looped then
			head = find_chain_begin(vertex)
			head.looped = false
			recolor(head)
		end
		vertex.previous.next = nil
		vertex:destroy()
		selected_vertex = nil
	end

	--head deletion (looped?)
	if not vertex.previous and vertex.next then
		if vertex.looped then
			tail = find_chain_end(vertex)
			tail.looped = false
			recolor(tail)
		end
		mapdata.edges[vertex.edge] = nil
		vertex.next.previous = nil
		mapdata.edges[next_edge_id] = vertex.next
		vertex.next.edge = next_edge_id
		next_edge_id = next_edge_id + 1
		vertex:destroy()
		selected_vertex = nil
	end

	--mid-chain deletion (looped?) (complex...)
	if vertex.previous and vertex.next then
		--figure out if we're looped
		head = find_chain_begin(vertex)
		if head.looped then
			head.looped = false
			tail = find_chain_end(vertex)
			tail.looped = false
			recolor(head)
			recolor(tail)
		end

		--remove ourselves from the previous chain *and* the next chain
		vertex.previous.next = nil

		--create a new chain starting with the node following this one
		mapdata.edges[next_edge_id] = vertex.next
		vertex.next.edge = next_edge_id
		next_edge_id = next_edge_id + 1

		--remove ourselves from the next chain
		vertex.next.previous = nil

		--finally, destroy this node
		vertex:destroy()
		selected_vertex = nil
	end
end

current_filename = ""
function save(filename)
	--construct the thing with the stuff for saving
	savedata = {}
	savedata.image = mapdata.image
	savedata.edges = {}

	for k,v in pairs(mapdata.edges) do
		savedata.edges[k] = {}
		savedata.edges[k].looped = v.looped
		savedata.edges[k].verticies = {}
		i = 1
		current_vertex = v
		while current_vertex do
			savedata.edges[k].verticies[i]= {
				x=current_vertex.x,
				y=current_vertex.y
			}
			current_vertex = current_vertex.next
			i = i + 1
		end
		savedata.edges[k].length = i - 1
	end

	filename = filename or current_filename
	persistence.store("lua/maps/"..filename..".data", savedata)
	if debug then
		persistence.store(debugpath.."lua/maps/"..filename..".data", savedata)
	end
	current_filename = filename
end

function clear()
	if map then
		
		--map:destroy()
		map.dead = true
		map = Map.create()
		map.debugdraw = true
	end

	--destroy all vertex handles
	for k,edge in pairs(mapdata.edges) do
		for i, vertex in pairs(edge) do
			vertex:destroy()
		end
	end
	--delete!
	mapdata.edges = {}
	mapdata.image = nil
	selected_vertex = nil
end

function load(filename)
	filename = filename or current_filename
	savedata = persistence.load("lua/maps/"..filename..".data")
	current_filename = filename

	--now, do some fun things
	clear()

	if savedata.image then
		map:sprite(savedata.image)
		mapdata.image = savedata.image
	end

	--load in all the verticies and re-create the joints and stuff
	next_edge_id = 1
	for k,edge in pairs(savedata.edges) do
		local previous_vertex = nil
		for i = 1, edge.length do
			vertex = VertexHandle.create({x=edge.verticies[i].x,y=edge.verticies[i].y})
			if i == 1 then
				--first vertex in this chain
				mapdata.edges[next_edge_id] = vertex
				vertex.edge = next_edge_id
				next_edge_id = next_edge_id + 1

				if edge.looped then
					vertex.looped = true
				end
				print("head: " .. i)
			else
				previous_vertex.next = vertex
				vertex.previous = previous_vertex

				print("vertex: " .. i)

				if i == edge.length and edge.looped then
					vertex.looped = true
				end
			end
			previous_vertex = vertex
		end
	end

	recolorAll()
	process_collision()
end

function recolorAll()
	for k,vertex in pairs(mapdata.edges) do
		while vertex do
			recolor(vertex)
			vertex = vertex.next
		end
	end
end