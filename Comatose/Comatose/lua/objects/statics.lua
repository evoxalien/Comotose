function static_init(self)
	self:body_type("static")
	self:sprite(self.art)
	if self.body_shape then
		self:shape(self.body_shape)
	end
	self.z_index = 0.5
end

Chair = inherits(PhysicsObject)
Chair.art = "Chair1"
Chair.init = static_init

Sofa = inherits(PhysicsObject)
Sofa.art = "Sofa"
Sofa.init = static_init

Table = inherits(PhysicsObject)
Table.art = "Table_Round1"
Table.init = static_init
Table.body_shape = "circle"

Recliner = inherits(PhysicsObject)
Recliner.art = "Chair_Recliner1"
Recliner.init = static_init

registered_objects["Chair"] = {
	art="Chair1",
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