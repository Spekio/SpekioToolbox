module SpekioToolboxCannon

using ..Ahorn, Maple

@mapdef Entity "SpekioToolbox/Cannon" Cannon(
x::Int, y::Int, 
cooldown::String="1.0", 
timeOffset::Number=0.0, 
direction::String="Left", 
startActivated::Bool=true, 
cannonStyle::String="Gray", 
shotStyle::String="Badeline Red", 
shotCollision::Bool=true, 
silent::Bool=false, 
shotSpeed::Number=1.0, 
toggleFlag::String="", 
attachToSolid::Bool=false,
trajectory::String="Wavy", 
disableShotParticles::Bool=false,
curveStrength::Number=0.0,
timeToLive::Number=-1.0,
harmlessTimer::Number=0.0
)

Ahorn.editingOrder(entity::Cannon) = String["x", "y", "cooldown", "timeOffset", "direction",  "shotStyle","cannonStyle",  
 "shotSpeed", "toggleFlag", "trajectory", "curveStrength", "timeToLive", "harmlessTimer", "startActivated","silent","attachToSolid", "shotCollision","disableShotParticles"]

const placements = Ahorn.PlacementDict(
    "Cannon (Spekio's Toolbox)" => Ahorn.EntityPlacement(
        Cannon,
        "point",
	    Dict{String, Any}()
    )
)

Ahorn.editingOptions(entity::Cannon) = Dict{String, Any}(
  "direction" => String["Left", "Right", "Up", "Down"],
  "cannonStyle" => String["Gray", "Green", "Purple"],
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
 "trajectory" => String["Straight", "Wavy", "Curved"]
)

function Ahorn.selection(entity::Cannon)
    x, y = Ahorn.position(entity)
    return  Ahorn.Rectangle(x - 8, y - 8, 16, 16)
end

function Ahorn.render(ctx::Ahorn.Cairo.CairoContext, entity::Cannon, room::Maple.Room)
	d = lowercase(entity.direction)
	cannonStyle = get(entity.data, "cannonStyle", get(entity.data, "cannonColor", "Gray"))
	

	
	spritePath = ""
	
	if entity.startActivated
		spritePath = "objects/SpekioToolbox/cannons/cannon"* cannonStyle *"/cannon_" * d * "_on"
	else
		spritePath = "objects/SpekioToolbox/cannons/cannon"* cannonStyle *"/cannon_" * d * "_off"
	end
		
	Ahorn.drawSprite(ctx, spritePath, 0,0)
end

end