local drawableSprite = require("structs.drawable_sprite")
local utils = require("utils")

local toggleTouchSwitch = {}

toggleTouchSwitch.name = "SpekioToolbox/ToggleTouchSwitch"
toggleTouchSwitch.depth = 2000
toggleTouchSwitch.placements = {
    {
        name = "ToggleTouchSwitch",
    }
}

local containerTexture = "objects/touchswitch/container"
local iconTexture = "objects/touchswitch/icon00"

function toggleTouchSwitch.sprite(room, entity)
    local containerSprite = drawableSprite.fromTexture(containerTexture, entity)
    local iconSprite = drawableSprite.fromTexture(iconTexture, entity)

    return {containerSprite, iconSprite}
end

return toggleTouchSwitch