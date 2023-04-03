local drawableSprite = require("structs.drawable_sprite")
local utils = require("utils")
local jautils = require("mods").requireFromPlugin("libraries.jautils")

local bulletGenerator = {}

bulletGenerator.name = "SpekioToolbox/BulletGenerator"
bulletGenerator.depth = -13000
bulletGenerator.justification = {0.5, 0.5}

bulletGenerator.fieldInformation = {
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
					"Fireball Purple"
					},
        editable = false
	},
	trajectory = {
        options = {"Straight", "Wavy", "Curved", "Homing"},
        editable = false
	},
	shotSequence = {
        options = {"Simultaneous", "Sequential", "Up and Down", "Random"},
        editable = false
	},
	cannonStyle = {
        options = {"Retro", "Ice", "Fire", "Wood"},
        editable = false
	}
	
}

bulletGenerator.placements = {
    name = "Bullet Generator",
    data = {
        attachDirection = "None",
		startActivated = true,
		timeOffset = 0,
		cooldown = "2",
		silent = false,
		shotCollision = true,
		cannonStyle = "Retro",
		shotStyle = "Badeline Red",
		toggleFlag = "",
		shotSpeed = 1,
		trajectory = "Wavy",
		moveTargetWithCannon = false,
		shotSequence = "Simultaneous",
		showSprite = true,
		angleOffset = 0,
		disableShotParticles = false,
		curveStrength = 0,
		timeToLive = -1,
		harmlessTimer = 0,
		depth = 2000;
    }
}

bulletGenerator.fieldOrder = {"x", "y", "cooldown", "timeOffset", "shotStyle", 
"shotSpeed", "toggleFlag", "attachDirection", "trajectory", "shotSequence", "cannonStyle", "angleOffset", "curveStrength", "timeToLive", "harmlessTimer", "depth", "moveTargetWithCannon","silent", "startActivated", "shotCollision", "showSprite", "disableShotParticles"}

bulletGenerator.nodeLimits = {0, -1}
bulletGenerator.nodeVisibility = "selected"
bulletGenerator.nodeLineRenderType = "fan"

function bulletGenerator.nodeSprite(room, entity, node, nodeIndex, viewport)
	local nx = node.x
	local ny = node.y
	local index = string.reverse(tostring(nodeIndex))
	local mainSprite = drawableSprite.fromTexture("editor/SpekioToolbox/target")
	mainSprite:addPosition(nx,ny)
	local sprites = {mainSprite}
	
	for i = 1, index:len() do
		local sprite = drawableSprite.fromTexture("editor/SpekioToolbox/number0"..index:sub(i,i))
		sprite:addPosition(nx+10+i*-4,ny+4)
		table.insert(sprites, sprite)
	end

	return sprites
end


function bulletGenerator.sprite(room, entity)
	local sprite
	local cannonStyle = string.lower(entity.cannonStyle) or "gray"
	if entity.startActivated then
		sprite = drawableSprite.fromTexture("objects/SpekioToolbox/bulletGenerators/"..cannonStyle.."/bulletGenerator_on", entity)
	else
		sprite = drawableSprite.fromTexture("objects/SpekioToolbox/bulletGenerators/"..cannonStyle.."/bulletGenerator_off", entity)
	end
	
	return sprite
end

return bulletGenerator