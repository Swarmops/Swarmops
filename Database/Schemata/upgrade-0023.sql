DROP procedure IF EXISTS `SetPersonFacebookId`


#


DROP procedure IF EXISTS `SetPersonTwitterId`


#


DROP procedure IF EXISTS `SetPersonGPlusId`


#


CREATE PROCEDURE `SetPersonFacebookId`(
  IN personId INTEGER,
  IN facebookId VARCHAR(64)
)
BEGIN

  UPDATE People SET People.FacebookId=facebookId
    WHERE People.PersonId=personId;

  SELECT ROW_COUNT() AS RowsUpdated;


END


#


CREATE PROCEDURE `SetPersonTwitterId`(
  IN personId INTEGER,
  IN twitterId VARCHAR(64)
)
BEGIN

  UPDATE People SET People.TwitterId=twitterId
    WHERE People.PersonId=personId;

  SELECT ROW_COUNT() AS RowsUpdated;


END


#


CREATE PROCEDURE `SetPersonGPlusId`(
  IN personId INTEGER,
  IN gPlusId VARCHAR(64)
)
BEGIN

  UPDATE People SET People.GPlusId=gPlusId
    WHERE People.PersonId=personId;

  SELECT ROW_COUNT() AS RowsUpdated;


END
