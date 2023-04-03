module SpekioToolboxLinkedDashBlock

using ..Ahorn, Maple

@mapdef Entity "SpekioToolbox/LinkedDashBlock" LinkedDashBlock(x::Int, y::Int,
   width::Int=Maple.defaultBlockWidth, height::Int=Maple.defaultBlockHeight, tiletype::String="3",
   blendin::Bool=true, canDash::Bool=true,
   permanent::Bool=true, linkId::String="flag")

Ahorn.editingOrder(entity::LinkedDashBlock) = String["x", "y", "height", "width", "linkId", "tiletype", "blendin", "canDash", "permanent"]

const placements = Ahorn.PlacementDict(
    "Linked Dash Block (Spekio's Toolbox)" => Ahorn.EntityPlacement(
        LinkedDashBlock,
        "rectangle",
	    Dict{String, Any}(),
        Ahorn.tileEntityFinalizer
    ),
)

Ahorn.editingOptions(entity::LinkedDashBlock) = Dict{String, Any}(
    "tiletype" => Ahorn.tiletypeEditingOptions()
)

Ahorn.minimumSize(entity::LinkedDashBlock) = 8, 8
Ahorn.resizable(entity::LinkedDashBlock) = true, true

Ahorn.selection(entity::LinkedDashBlock) = Ahorn.getEntityRectangle(entity)

Ahorn.renderAbs(ctx::Ahorn.Cairo.CairoContext, entity::LinkedDashBlock, room::Maple.Room) = Ahorn.drawTileEntity(ctx, room, entity)

end
