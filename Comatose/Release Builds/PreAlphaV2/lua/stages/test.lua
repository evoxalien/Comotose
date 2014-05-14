
hero = Hero.create({x=20,y=20})

map = Map.create()
map:sprite("../maps/awfulmap")
map.z_index = 0

map:beginChain()
map:addVertex(10,10)
map:addVertex(10,20)
map:addVertex(20,20)
map:addVertex(20,10)
map:endChain(true)

