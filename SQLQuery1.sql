INSERT INTO UserWardrobes (AppUserId, ClothingItemId)
SELECT 4, Id FROM ClothingItems
WHERE Id NOT IN (
    SELECT ClothingItemId FROM UserWardrobes WHERE AppUserId = 4
)