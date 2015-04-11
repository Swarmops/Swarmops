ALTER TABLE `PositionAssignments` 
ADD COLUMN `Acting` TINYINT NOT NULL DEFAULT '0' AFTER `CreatedByPositionId`;
