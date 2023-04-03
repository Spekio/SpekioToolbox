module SpekioToolboxToggleTouchSwitch

using ..Ahorn, Maple

@mapdef Entity "SpekioToolbox/ToggleTouchSwitch" ToggleTouchSwitch(x::Integer, y::Integer)
    
const placements = Ahorn.PlacementDict(
    "Toggle Touch Switch (Spekio's Toolbox)" => Ahorn.EntityPlacement(
        ToggleTouchSwitch
    )
)

function Ahorn.selection(entity::ToggleTouchSwitch)
    x, y = Ahorn.position(entity)
    return  Ahorn.Rectangle(x - 7, y - 7, 14, 14)
end

function Ahorn.render(ctx::Ahorn.Cairo.CairoContext, entity::ToggleTouchSwitch, room::Maple.Room)
    Ahorn.drawSprite(ctx, "objects/touchswitch/container.png", 0, 0)
    iconPath = "objects/touchswitch/icon00.png"
    Ahorn.drawSprite(ctx, iconPath, 0, 0)
end

end