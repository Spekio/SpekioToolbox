local drawableSprite = require("structs.drawable_sprite")
local utils = require("utils")
local jautils = require("mods").requireFromPlugin("libraries.jautils")

local cannon = {}

cannon.name = "SpekioToolbox/Cannon"
cannon.depth = -9000
cannon.justification = {0.5, 0.5}
cannon.fieldInformation = {
    direction = {
        options = {"Left", "Right", "Up", "Down"},
        editable = false
    },
	cannonStyle = {
        options = {"Gray", "Green", "Purple"},
        editable = false
	},
	shotStyle = {
        options = { "Badeline Red",
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
					"Fireball Purple"},
        editable = false
	},
	trajectory = {
        options = {"Straight", "Wavy", "Curved"},
        editable = false
	}
}

cannon.placements = {
    name = "Cannon",
    data = {
        direction = "Left",
		startActivated = true,
		timeOffset = 0,
		cooldown = "2",
		silent = false,
		shotCollision = true,
		cannonStyle = "Gray",
		shotStyle = "Badeline Red",
		toggleFlag = "",
		shotSpeed = 1,
		attachToSolid = false,
		trajectory = "Wavy",
		disableShotParticles = false,
		curveStrength = 0,
		timeToLive = -1,
		harmlessTimer = 0;
    }
}

cannon.fieldOrder = {"x", "y", "cooldown", "timeOffset", "direction", "cannonStyle", "shotStyle", "shotSpeed", "toggleFlag", "trajectory", "curveStrength", "timeToLive", "harmlessTimer","attachToSolid", "startActivated", "shotCollision", "silent", "disableShotParticles"}

function cannon.sprite(room, entity)
	local sprite
	local direction = entity.direction or "Left"
    direction = string.lower(direction)
	local cannonStyle = entity.cannonStyle or "Gray"
	
	if entity.startActivated then
		sprite = drawableSprite.fromTexture("objects/SpekioToolbox/cannons/cannon".. cannonStyle .."/cannon_" .. direction .. "_on", entity)
	else
		sprite = drawableSprite.fromTexture("objects/SpekioToolbox/cannons/cannon".. cannonStyle .."/cannon_" .. direction .. "_off", entity)
	end
	
	local sprites = jautils.getBorder(sprite, utils.getColor("000000"))
	table.insert(sprites, sprite)
	return sprites 
end

return cannon