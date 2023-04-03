module SpekioToolboxBulletGenerator

using ..Ahorn, Maple

@mapdef Entity "SpekioToolbox/BulletGenerator" BulletGenerator(
x::Int, 
y::Int, 
cooldown::String="1.0", 
timeOffset::Number=0.0, 
attachDirection::String="None", 
startActivated::Bool=true,  
shotStyle::String="Badeline Red", 
shotCollision::Bool=true, 
silent::Bool=false, 
shotSpeed::Number=1.0, 
toggleFlag::String="", 
moveTargetWithCannon::Bool=false, 
nodes::Array{Tuple{Integer, Integer}, 1}=Tuple{Integer, Integer}[],
trajectory::String="Wavy",
shotSequence::String="Simultaneous",
cannonStyle::String="Retro",
showSprite::Bool=true,
angleOffset::Number=0.0,
disableShotParticles::Bool=false,
curveStrength::Number=0.0,
timeToLive::Number=-1.0,
harmlessTimer::Number=0.0,
depth::Int=2000)


Ahorn.editingOrder(entity::BulletGenerator) = String["x", "y", "cooldown", "timeOffset", "shotStyle", 
"shotSpeed", "toggleFlag", "attachDirection", "trajectory", "shotSequence", "cannonStyle", "angleOffset", "curveStrength", "timeToLive", "harmlessTimer", "depth", "moveTargetWithCannon","silent", "startActivated", "shotCollision", "showSprite", "disableShotParticles"]

const placements = Ahorn.PlacementDict(
    "Bullet Generator (Spekio's Toolbox)" => Ahorn.EntityPlacement(
        BulletGenerator,
        "point",
	    Dict{String, Any}()
    )
)

Ahorn.editingOptions(entity::BulletGenerator) = Dict{String, Any}(
  "attachDirection" => String["None", "Left", "Right", "Up", "Down"],
  "shotStyle" => String["Badeline Red",
					"Badeline Blue", 
					"Badeline Green",
					"Badeline Yellow",
					"Badeline Gray",
					"Badeline Purple",
					"Simple Red",
					"Simple Blue", 
					"Simple Green",
					"Simple Yellow",
					"Simple Gray",
					"Simple Purple",
					"Ring Red",
					"Ring Blue",
					"Ring Green",
					"Ring Orange",
					"Ring Gray",
					"Ring Pink",
					"Fireball Red",
					"Fireball Blue",
					"Fireball Green",
					"Fireball Yellow",
					"Fireball Gray",
					"Fireball Purple"],
 "trajectory" => String["Straight", "Wavy", "Curved", "Homing"],
 "shotSequence" => String["Simultaneous", "Sequential", "Up and Down", "Random"],
 "cannonStyle" => String["Retro", "Ice", "Fire", "Wood"]
)


Ahorn.nodeLimits(entity::BulletGenerator) = 0, -1

nodesprite = "editor/SpekioToolbox/target"

function Ahorn.selection(entity::BulletGenerator)
    nodes = get(entity.data, "nodes", ())
    x, y = Ahorn.position(entity)

    res = Ahorn.Rectangle[Ahorn.getSpriteRectangle(nodesprite, x, y)]
    
    for node in nodes
        nx, ny = Int.(node)

        push!(res, Ahorn.getSpriteRectangle(nodesprite, nx, ny))
    end

    return res
end


function Ahorn.renderSelectedAbs(ctx::Ahorn.Cairo.CairoContext, entity::BulletGenerator)
    px, py = Ahorn.position(entity)
	
	nodes = get(entity.data, "nodes", ())
	for (i, node) in enumerate(nodes) 
		node = nodes[i]
		nx, ny = Int.(node)
		displayIndex = string(i)
		
		Ahorn.drawSprite(ctx, nodesprite, nx, ny)
	    Ahorn.drawArrow(ctx, px, py, nx, ny, Ahorn.colors.selection_selected_fc, headLength=6)
		Ahorn.drawCenteredText(ctx, displayIndex, nx, ny, 8, 8,scale=1,tint=(0.6, 1.0, 0.3,1.0))
    end
	
	nx, ny = Ahorn.position(entity)
	
end


function Ahorn.renderAbs(ctx::Ahorn.Cairo.CairoContext, entity::BulletGenerator, room::Maple.Room)
    x, y = Ahorn.position(entity)
	c = lowercase(entity.cannonStyle)
	if entity.startActivated
		sprite = "objects/SpekioToolbox/bulletGenerators/"*c*"/bulletGenerator_on"
	else
		sprite = "objects/SpekioToolbox/bulletGenerators/"*c*"/bulletGenerator_off"
	end
    Ahorn.drawSprite(ctx, sprite, x, y)
end

end