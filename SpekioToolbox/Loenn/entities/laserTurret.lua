local drawableSprite = require("structs.drawable_sprite")
local utils = require("utils")
local jautils = require("mods").requireFromPlugin("libraries.jautils")

local laserTurret = {}

laserTurret.name = "SpekioToolbox/LaserTurret"
laserTurret.depth = -13000
laserTurret.justification = {0.5, 0.5}
laserTurret.fieldInformation = {
	laserColor = {
        options = {"Red", "Blue", "Green","Gray","Purple","Yellow"},
        editable = false
	}
}

laserTurret.placements = {
    name = "LaserTurret",
    data = {
		startActivated = true,
		timeOffset = 0,
		cooldown = 2.0,
		silent = false,
		waitForPlayer = true,
		laserColor = "Red",
		toggleFlag = "",
		chargeTimer = "1.4",
		followTimer = "0.9"
    }
}

laserTurret.fieldOrder = {"x", "y", "cooldown", "timeOffset", "chargeTimer", "followTimer", "startActivated", "laserColor", "silent", "toggleFlag","waitForPlayer"}

function laserTurret.sprite(room, entity)
	local sprite
	local laserColor = entity.laserColor or "Red"

	if entity.startActivated then
		sprite = drawableSprite.fromTexture("objects/SpekioToolbox/laserTurrets/laserTurret".. laserColor .."/laserTurret_on", entity)
	else
			sprite = drawableSprite.fromTexture("objects/SpekioToolbox/laserTurrets/laserTurret".. laserColor .."/laserTurret_off", entity)
	end
	
	local sprites = jautils.getBorder(sprite, utils.getColor("000000"))
	table.insert(sprites, sprite)
	return sprites
end

return laserTurret