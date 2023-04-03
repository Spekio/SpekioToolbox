local drawableSprite = require("structs.drawable_sprite")
local drawableLine = require("structs.drawable_line")
local drawing = require("utils.drawing")

local Sawblade = {}

Sawblade.name = "SpekioToolbox/Sawblade"
Sawblade.nodeLimits = {0, -1}
Sawblade.nodeLineRenderType = "line"
Sawblade.nodeVisibility = "always"
Sawblade.fieldOrder = {
    "x", "y", "mode", "directory", "radius", "lineColorA", "lineColorB", "amount", "totalTime", "startOffset", "spacingOffset", "stopFlag", "swapFlag", "resumeFlag", "moveFlag", "stopAtEachNode", "drawTrack"
}
Sawblade.fieldInformation = {
    mode = {
        options = {"Restart", "Back And Forth", "Back And Forth - Pause At Each Node", "Back And Forth - All Sawblades", "Back And Forth -  All Sawblades - Pause At Each Node", "Flag To Move"},
        editable = false
    },
    amount = {
        fieldType = "integer",
        minimumValue = 1
    },
    startOffset = {
        minimumValue = 0,
        maximumValue = 1
    },
    spacingOffset = {
        minimumValue = 0,
        maximumValue = 1
    }
}
Sawblade.placements = {
    name = "Sawblade",
    data = {
        directory = "objects/SpekioToolbox/Sawblade",
        radius = 8,
        mode = "Restart",
        totalTime = 2,
        stopFlag = "",
        swapFlag = "",
        resumeFlag = "",
        moveFlag = "",
        drawTrack = true,
        amount = 3,
        startOffset = 0,
        spacingOffset = 0.5,
        lineColorA = "2A251F",
        lineColorB = "C97F35"
    }
}

function tablelength(T)
    local count = 0
    for _ in pairs(T) do count = count + 1 end
    return count
  end

function Sawblade.sprite(room, entity)
    local directory = entity.directory
    local lineColorA = entity.lineColorA or "2A251F"
    local lineColorB = entity.lineColorB or "C97F35"
    if directory == "" then
        directory = "objects/SpekioToolbox/Sawbladee"
    end
    local x, y = entity.x or 0, entity.y or 0
    local nodes = entity.nodes or {}
    local lineSprite = {}
    if tablelength(nodes) > 0 then
        local line = drawing.getSimpleCurve({x, y}, {nodes[1].x, nodes[1].y})
        lineSprite = drawableLine.fromPoints(line, lineColorB, 1):getDrawableSprite()
    end
    local sprite = drawableSprite.fromTexture(directory .. "/saw00", entity)
    table.insert(lineSprite, sprite)

    return lineSprite
end

function Sawblade.nodeSprite(room, entity, node, nodeIndex)
    local directory = entity.directory
    local lineColorA = entity.lineColorA or "2A251F"
    local lineColorB = entity.lineColorB or "C97F35"
    if directory == "" then
        directory = "objects/SpekioToolbox/Sawblade"
    end
    local nodes = entity.nodes or {}
    local lineSprite = {}
    if tablelength(nodes) > nodeIndex and nodeIndex ~= tablelength(nodes) - 1 then
        local line = drawing.getSimpleCurve({nodes[nodeIndex].x, nodes[nodeIndex].y}, {nodes[nodeIndex + 1].x, nodes[nodeIndex + 1].y})
        lineSprite = drawableLine.fromPoints(line, lineColorB, 1):getDrawableSprite()
    end
    local sprite = drawableSprite.fromTexture(directory .. "/node00", entity)
    sprite:addPosition(nodes[nodeIndex].x - entity.x, nodes[nodeIndex].y - entity.y)
    table.insert(lineSprite, sprite)

    return lineSprite
end

function Sawblade.rectangle(room, entity)
    local directory = entity.directory
    if directory == "" then
        directory = "objects/SpekioToolbox/Sawblade"
    end
    local sprite = drawableSprite.fromTexture(directory .. "/saw00", entity)

    return sprite:getRectangle()
end

function Sawblade.nodeRectangle(room, entity, node, nodeIndex)
    local directory = entity.directory
    if directory == "" then
        directory = "objects/SpekioToolbox/Sawblade"
    end
    local nodes = entity.nodes or {}
    local sprite = drawableSprite.fromTexture(directory .. "/node00", entity)
    sprite:addPosition(nodes[nodeIndex].x - entity.x, nodes[nodeIndex].y - entity.y)

    return sprite:getRectangle()
end

return Sawblade