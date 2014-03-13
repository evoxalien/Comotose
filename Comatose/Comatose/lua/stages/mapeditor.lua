
map = Map.create()
map.debugdraw = true
mapdata = {edges={}}

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

function VertexHandle:click()
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
		else
			--complex case: we're connecting to the start (or end!) of another chain
			if self.previous == nil and selected_vertex.next == nil then
				--my head to your tail
				selected_vertex.next = self
				self.previous = selected_vertex
				mapdata.edges[self.edge] = nil
				self.edge = nil
				clear_selection()
			end
			if self.next == nil and selected_vertex.previous == nil then
				--my tail to your head
				self.next = selected_vertex
				selected_vertex.previous = self
				mapdata.edges[selected_vertex.edge] = nil
				selected_vertex.edge = nil
				clear_selection()
			end

			if self.previous == nil and selected_vertex.previous == nil then
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
			end

			if self.next == nil and selected_vertex.next == nil then
				--my tail to your tail; equally complicated
				selected_head = find_chain_begin(selected_vertex)
				mapdata.edges[selected_head.edge] = nil

				selected_tail = reverse_list(find_chain_begin(selected_vertex))
				selected_head = find_chain_begin(selected_tail)
				self.next = selected_head
				selected_head.previous = self

			end


		end
	end
	handled_by_vertex = true
end

selected_vertex = nil
function select_vertex(vertex)
	clear_selection()
	selected_vertex = vertex
	vertex:color(128,255,255,255) --cyan ish
end

function clear_selection()
	if selected_vertex then
		selected_vertex:color(128,128,128,255)
		selected_vertex = nil
	end
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

function stage.everyFrame()
	if Input:WasKeyPressed("Escape") then
		clear_selection()
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