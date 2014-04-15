function static_init(self)
	--self:body_type("static")
	self:sprite(self.art)
	self.centered = true
	if self.body_shape then
		self:shape(self.body_shape)
	end
	self.z_index = 0.5
	self:color(32,32,32,255)
	self.cast_shadow = false
end

function static_update(self)
	self.vx = self.vx / 2
	self.vy = self.vy / 2
	self.vr = self.vr / 2
end

Chair = inherits(PhysicsObject)
Chair.art = "Chair1"
Chair.init = static_init
Chair.everyFrame = static_update

Sofa = inherits(PhysicsObject)
Sofa.art = "Sofa"
Sofa.init = static_init
Sofa.everyFrame = static_update

Table = inherits(PhysicsObject)
Table.art = "Table_Round1"
Table.init = static_init
Table.body_shape = "circle"
Table.everyFrame = static_update

Recliner = inherits(PhysicsObject)
Recliner.art = "Chair_Recliner1"
Recliner.init = static_init
Recliner.everyFrame = static_update

Chair2 = inherits(PhysicsObject)
Chair2.art = "DY_Chair01"
Chair2.init = static_init
Chair2.everyFrame = static_update

registered_objects["Chair"] = {
	art="Chair1",
	centered=true
}

registered_objects["Chair2"] = {
	art="DY_Chair01",
	centered=true
}

registered_objects["Sofa"] = {
	art="Sofa",
	centered=true
}
registered_objects["Table"] = {
	art="Table_Round1",
	centered=true
}
registered_objects["Recliner"] = {
	art="Chair_Recliner1",
	centered=true
}