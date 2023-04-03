module SpekioToolboxAimingCannon

using ..Ahorn, Maple

@mapdef Entity "SpekioToolbox/AimingCannon" AimingCannon(
x::Int, 
y::Int, 
cooldown::String="1.0", 
timeOffset::Number=0.0, 
attachDirection::String="None",
startActivated::Bool=true, 
cannonStyle::String="City", 
shotStyle::String="Badeline Red", 
shotCollision::Bool=true, 
silent::Bool=false, 
shotSpeed::Number=1.0, 
toggleFlag::String="", 
moveTargetWithCannon::Bool=false, 
nodes::Array{Tuple{Integer, Integer}, 1}=Tuple{Integer, Integer}[],
trajectory::String="Wavy", 
disableShotParticles::Bool=false,
curveStrength::Number=0.0,
timeToLive::Number=-1.0,
harmlessTimer::Number=0.0)
		
Ahorn.editingOrder(entity::AimingCannon) = String["x", "y", "cooldown", "timeOffset", "cannonStyle", "shotStyle", 
"shotSpeed", "toggleFlag", "attachDirection", "trajectory", "curveStrength", "timeToLive", "harmlessTimer","moveTargetWithCannon","silent", "startActivated", "shotCollision", "disableShotParticles"]

const placements = Ahorn.PlacementDict(
    "Aiming Cannon (Spekio's Toolbox)" => Ahorn.EntityPlacement(
        AimingCannon,
        "point",
	    Dict{String, Any}()
    )
)

Ahorn.editingOptions(entity::AimingCannon) = Dict{String, Any}(
  "attachDirection" => String["None", "Left", "Right", "Up", "Down"],
  "cannonStyle" => String["City", "Moon", "Moon2"],
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
 "trajectory" => String["Straight", "Wavy", "Curved", "Homing"]
)


Ahorn.nodeLimits(entity::AimingCannon) = 0, 1

nodesprite = "editor/SpekioToolbox/target"

function Ahorn.selection(entity::AimingCannon)
    nodes = get(entity.data, "nodes", ())
    x, y = Ahorn.position(entity)

    res = Ahorn.Rectangle[Ahorn.getSpriteRectangle(nodesprite, x, y)]
    
    for node in nodes
        nx, ny = Int.(node)

        push!(res, Ahorn.getSpriteRectangle(nodesprite, nx, ny))
    end

    return res
end


function Ahorn.renderSelectedAbs(ctx::Ahorn.Cairo.CairoContext, entity::AimingCannon)
    px, py = Ahorn.position(entity)

    for node in get(entity.data, "nodes", ())
        nx, ny = Int.(node)

        Ahorn.drawSprite(ctx, nodesprite, nx, ny)
        Ahorn.drawArrow(ctx, px, py, nx, ny, Ahorn.colors.selection_selected_fc, headLength=6)

        px, py = nx, ny
    end
	nx, ny = Ahorn.position(entity)
	
end


function Ahorn.renderAbs(ctx::Ahorn.Cairo.CairoContext, entity::AimingCannon, room::Maple.Room)
    x, y = Ahorn.position(entity)
	c = lowercase(entity.cannonStyle)
	
	if entity.startActivated
		sprite = "objects/SpekioToolbox/aimingCannons/"*c*"/aimingcannon_on"
	else
		sprite = "objects/SpekioToolbox/aimingCannons/"*c*"/aimingcannon_off"
	end
    Ahorn.drawSprite(ctx, sprite, x, y)
	sprite2= "objects/SpekioToolbox/aimingCannons/"*c*"/gunbarrel_a"
	Ahorn.drawSprite(ctx, sprite2, x, y)
end

end