module SpekioToolboxProjectileBlockField

using ..Ahorn, Maple

@mapdef Entity "SpekioToolbox/ProjectileBlockField" ProjectileBlockField(x::Int, y::Int, activeFlag::String="", instantRemoval::Bool=false, blockAngleStart::Number=0.0, blockAngleEnd::Number=0.0, directionalBlocking::Bool=false)

Ahorn.editingOrder(entity::ProjectileBlockField) = String["x", "y", "height", "width", "blockAngleStart","blockAngleEnd","activeFlag", "instantRemoval", "directionalBlocking"]


const placements = Ahorn.PlacementDict(
	"Projectile Block Field (Spekio's Toolbox)" => Ahorn.EntityPlacement(
        ProjectileBlockField,
        "rectangle"
    ),
)

Ahorn.minimumSize(entity::ProjectileBlockField) = 8, 8
Ahorn.resizable(entity::ProjectileBlockField) = true, true

Ahorn.selection(entity::ProjectileBlockField) = Ahorn.getEntityRectangle(entity)

function Ahorn.render(ctx::Ahorn.Cairo.CairoContext, entity::ProjectileBlockField, room::Maple.Room)
    width = Int(get(entity.data, "width", 32))
    height = Int(get(entity.data, "height", 32))

    Ahorn.drawRectangle(ctx, 0, 0, width, height, (1.0, 1.0, 0.4, 0.2), (1.0, 1.0, 0.4, 1.0))
end

end