local drawableSprite = require("structs.drawable_sprite")
local utils = require("utils")
local jautils = require("mods").requireFromPlugin("libraries.jautils")

local aimingCannon = {}

aimingCannon.name = "SpekioToolbox/AimingCannon"
aimingCannon.depth = -13000
aimingCannon.justification = {0.5, 0.5}
aimingCannon.fieldInformation = {
    attachDirection = {
        options = {"None", "Left", "Right", "Up", "Down"},
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
	cannonStyle = {
        options = {"City", "Moon", "Moon2"},
        editable = false
	},
	trajectory = {
        options = {"Straight", "Wavy", "Curved", "Homing"},
        editable = false
	}
}

aimingCannon.placements = {
    name = "Aiming Cannon",
    data = {
        attachDirection = "None",
		startActivated = true,
		timeOffset = 0,
		cooldown = "2",
		silent = false,
		shotCollision = true,
		cannonStyle = "City",
		shotStyle = "Badeline Red",
		toggleFlag = "",
		shotSpeed = 1,
		trajectory = "Wavy",
		moveTargetWithCannon = false,
		disableShotParticles = false,
		curveStrength = 0,
		timeToLive = -1,
		harmlessTimer = 0;
    }
}

aimingCannon.fieldOrder = {"x", "y", "cooldown", "timeOffset", "cannonStyle", "shotStyle", "shotSpeed", "toggleFlag", "trajectory", "curveStrength", "timeToLive", "harmlessTimer", "attachDirection","startActivated", "silent", "shotCollision","moveTargetWithCannon","disableShotParticles"}

aimingCannon.nodeLimits = {0, 1}
aimingCannon.nodeVisibility = "selected"
aimingCannon.nodeLineRenderType = "line"
aimingCannon.nodeTexture = "editor/SpekioToolbox/target"

function aimingCannon.sprite(room, entity)
	local sprite
	local cannonStyle = string.lower(entity.cannonStyle) or "gray"
	
	if entity.startActivated then
		sprite = drawableSprite.fromTexture("objects/SpekioToolbox/aimingCannons/".. cannonStyle .."/aimingcannon_on", entity)
	else
		sprite = drawableSprite.fromTexture("objects/SpekioToolbox/aimingCannons/".. cannonStyle .."/aimingcannon_off", entity)
	end
	
	sprite2 =  drawableSprite.fromTexture("objects/SpekioToolbox/aimingCannons/".. cannonStyle .."/gunbarrel_a", entity)
	local sprites = jautils.getBorder(sprite, utils.getColor("000000"))
	table.insert(sprites, sprite)
	table.insert(sprites, sprite2)
	return sprites 
end

return aimingCannon