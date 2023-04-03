local projectileBlockField = {}

projectileBlockField.name = "SpekioToolbox/ProjectileBlockField"
projectileBlockField.fillColor = {1.0, 1.0, 0.4, 0.2}
projectileBlockField.borderColor = {1.0, 1.0, 0.4, 1.0}
projectileBlockField.placements = {
    name = "Projectile Block Field",
    data = {
        width = 8,
        height = 8,
		activeFlag = "",
		blockAngleStart = 0,
		blockAngleEnd = 0,
		instantRemoval = false,
		directionalBlocking = false;
    }
}
projectileBlockField.fieldOrder = {"x", "y", "height", "width", "blockAngleStart", "blockAngleEnd", "activeFlag", "instantRemoval", "directionalBlocking"}

return projectileBlockField