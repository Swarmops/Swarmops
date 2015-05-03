CREATE TABLE `PositionCustomTitles` (
  `PositionId` int(11) unsigned NOT NULL,
  `CustomTitle` varchar(128) NOT NULL,
  PRIMARY KEY (`PositionId`),
  KEY `Index_Title` (`CustomTitle`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8


#


DROP procedure IF EXISTS `SetCustomPositionTitle`



#


CREATE PROCEDURE `SetCustomPositionTitle`(
  IN positionId INT,
  IN customTitle VARCHAR(128)
)
BEGIN

  IF ((SELECT COUNT(*) FROM PositionCustomTitles WHERE PositionCustomTitles.PositionId = positionId) = 0)
  THEN
    INSERT INTO PositionCustomTitles (PositionId,CustomTitle) VALUES (positionId,customTitle);
  ELSE
    UPDATE PositionCustomTitles SET PositionCustomTitles.CustomTitle=customTitle WHERE PositionCustomTitles.PositionId=positionId;
  END IF;

END
