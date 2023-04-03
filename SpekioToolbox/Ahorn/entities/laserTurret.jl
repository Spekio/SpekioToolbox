module SpekioToolboxLaserTurret

using ..Ahorn, Maple

@mapdef Entity "SpekioToolbox/LaserTurret" LaserTurret(x::Int, y::Int, cooldown::String="2.0", timeOffset::Number=0.0, chargeTimer::Number=1.4, followTimer::Number=0.9, startActivated::Bool=true, laserColor::String="Red", silent::Bool=false, toggleFlag::String="", waitForPlayer::Bool=true)

Ahorn.editingOrder(entity::LaserTurret) = String["x", "y", "cooldown", "timeOffset", "chargeTimer", "followTimer",  "laserColor",  "toggleFlag","startActivated","waitForPlayer","silent"]

const placements = Ahorn.PlacementDict(
    "Laser Turret (Spekio's Toolbox)" => Ahorn.EntityPlacement(
        LaserTurret,
        "point",
	    Dict{String, Any}()
    )
)

Ahorn.editingOptions(entity::LaserTurret) = Dict{String, Any}(
  "laserColor" => String["Red", "Blue", "Green","Gray","Purple","Yellow"]
)

function Ahorn.selection(entity::LaserTurret)
    x, y = Ahorn.position(entity)
    return  Ahorn.Rectangle(x - 8, y - 8, 16, 16)
end

function Ahorn.render(ctx::Ahorn.Cairo.CairoContext, entity::LaserTurret, room::Maple.Room)
	c = entity.laserColor
	spritePath = ""
	
	if entity.startActivated
		spritePath = "objects/SpekioToolbox/laserTurrets/laserTurret"* c *"/laserTurret_on"
	else
		spritePath = "objects/SpekioToolbox/laserTurrets/laserTurret"* c *"/laserTurret_off"
	end
		
	Ahorn.drawSprite(ctx, spritePath, 0,0)
end

end