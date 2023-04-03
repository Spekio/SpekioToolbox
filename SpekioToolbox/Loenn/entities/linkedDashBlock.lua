local fakeTilesHelper = require("helpers.fake_tiles")

local linkedDashBlock = {}

linkedDashBlock.name = "SpekioToolbox/LinkedDashBlock"
linkedDashBlock.depth = 0
linkedDashBlock.placements = {
    name = "LinkedDashBlock",
    data = {
        tiletype = "3",
        blendin = true,
        canDash = true,
        permanent = true,
        width = 8,
        height = 8,
		linkId = "flag"
    }
}

linkedDashBlock.fieldOrder = {"x", "y", "height", "width", "linkId", "tiletype", "blendin", "canDash", "permanent"}
linkedDashBlock.sprite = fakeTilesHelper.getEntitySpriteFunction("tiletype", "blendin")
linkedDashBlock.fieldInformation = fakeTilesHelper.getFieldInformation("tiletype")

return linkedDashBlock