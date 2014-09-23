CREATE TABLE `Activists` (
  `ActivistId` int(10) unsigned NOT NULL AUTO_INCREMENT,
  `PersonId` int(10) unsigned NOT NULL,
  `Public` tinyint(1) unsigned NOT NULL,
  `DateTimeCreated` datetime NOT NULL,
  `DateTimeTerminated` datetime NOT NULL,
  `Active` tinyint(1) unsigned NOT NULL,
  `Confirmed` tinyint(1) unsigned NOT NULL,
  PRIMARY KEY (`ActivistId`),
  KEY `Index_PersonId` (`PersonId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='The people who are activists'


#


CREATE TABLE `AutoMailTypes` (
  `AutoMailTypeId` int(10) unsigned NOT NULL AUTO_INCREMENT,
  `Name` varchar(64) NOT NULL,
  PRIMARY KEY (`AutoMailTypeId`),
  KEY `Index_Name` (`Name`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8


#


CREATE TABLE `AutoMails` (
  `AutoMailId` int(10) unsigned NOT NULL AUTO_INCREMENT,
  `AutoMailTypeId` int(10) unsigned NOT NULL,
  `OrganizationId` int(10) unsigned NOT NULL,
  `GeographyId` int(10) unsigned NOT NULL,
  `AuthorPersonId` int(10) unsigned NOT NULL,
  `Title` text NOT NULL,
  `Body` longtext NOT NULL,
  `LastUpdate` datetime DEFAULT NULL,
  PRIMARY KEY (`AutoMailId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8


#


CREATE TABLE `BallotCandidates` (
  `BallotId` int(11) NOT NULL,
  `PersonId` int(11) NOT NULL,
  `SortOrder` int(11) NOT NULL,
  KEY `Ballot` (`BallotId`),
  KEY `Person` (`PersonId`),
  KEY `SortOrder` (`SortOrder`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1


#


CREATE TABLE `Ballots` (
  `BallotId` int(11) NOT NULL AUTO_INCREMENT,
  `ElectionId` int(11) NOT NULL,
  `Name` varchar(128) NOT NULL,
  `OrganizationId` int(11) NOT NULL,
  `GeographyId` int(11) NOT NULL,
  `BallotCount` int(11) NOT NULL DEFAULT '0',
  `DeliveryAddress` text NOT NULL,
  PRIMARY KEY (`BallotId`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1


#


CREATE TABLE `BlogRankingDates` (
  `BlogRankingDateId` int(10) unsigned NOT NULL AUTO_INCREMENT,
  `Date` datetime NOT NULL,
  PRIMARY KEY (`BlogRankingDateId`),
  KEY `Index_Date` (`Date`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8


#


CREATE TABLE `BlogRankings` (
  `BlogRankingDateId` int(10) unsigned NOT NULL,
  `MediaId` int(10) unsigned NOT NULL,
  `Ranking` int(10) unsigned NOT NULL,
  PRIMARY KEY (`BlogRankingDateId`,`MediaId`),
  KEY `Index_Media` (`MediaId`),
  KEY `Index_Date` (`BlogRankingDateId`),
  KEY `Index_Ranking` (`Ranking`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8


#


CREATE TABLE `CandidateDocumentation` (
  `ElectionId` int(11) NOT NULL,
  `OrganizationId` int(11) NOT NULL,
  `PersonId` int(11) NOT NULL,
  `DocumentationReceived` tinyint(1) NOT NULL,
  `DocumentationReceivedDateTime` datetime NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=latin1


#


CREATE TABLE `CashAdvances` (
  `CashAdvanceId` int(11) NOT NULL AUTO_INCREMENT,
  `PersonId` int(11) unsigned NOT NULL,
  `OrganizationId` int(11) unsigned NOT NULL,
  `FinancialAccountId` int(11) unsigned NOT NULL,
  `Description` varchar(128) NOT NULL,
  `Open` bit(1) NOT NULL,
  `Attested` bit(1) NOT NULL,
  `AttestedByPersonId` int(11) unsigned NOT NULL,
  `AttestedDateTime` datetime NOT NULL,
  `CreatedDateTime` datetime NOT NULL,
  `CreatedByPersonId` int(11) unsigned NOT NULL,
  `AmountCents` bigint(20) NOT NULL,
  `PaidOut` bit(1) NOT NULL,
  `PaidOutDateTime` datetime NOT NULL DEFAULT '0001-01-01 00:00:00',
  `ClosedDateTime` datetime NOT NULL DEFAULT '9999-12-31 00:00:00',
  PRIMARY KEY (`CashAdvanceId`),
  KEY `IdxPersonId` (`PersonId`),
  KEY `IdxOrganizationId` (`OrganizationId`),
  KEY `IdxFinancialAccountId` (`FinancialAccountId`),
  KEY `IdxOpen` (`Open`),
  KEY `IdxCreatedDateTime` (`CreatedDateTime`),
  KEY `IdxPaidOutDateTime` (`PaidOutDateTime`),
  KEY `IdxClosedDateTime` (`ClosedDateTime`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8


#


CREATE TABLE `ChurnData` (
  `PersonId` int(10) unsigned NOT NULL,
  `OrganizationId` int(10) unsigned NOT NULL,
  `Churn` tinyint(1) unsigned NOT NULL,
  `DecisionDateTime` datetime NOT NULL,
  `ExpiryDateTime` datetime NOT NULL,
  KEY `Index_Person` (`PersonId`),
  KEY `Index_Organization` (`OrganizationId`),
  KEY `Index_Expiry` (`ExpiryDateTime`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='Data about people churning or not'


#


CREATE TABLE `Cities` (
  `CityId` int(10) unsigned NOT NULL AUTO_INCREMENT,
  `CityName` varchar(256) NOT NULL,
  `CountryId` int(10) unsigned NOT NULL,
  `GeographyId` int(10) unsigned NOT NULL,
  `Comment` varchar(256) NOT NULL,
  PRIMARY KEY (`CityId`),
  KEY `Index_Country` (`CountryId`),
  KEY `Index_Geography` (`GeographyId`),
  KEY `Index_Name` (`CityName`(128)) USING BTREE
) ENGINE=InnoDB DEFAULT CHARSET=utf8


#


CREATE TABLE `CommunicationTurnarounds` (
  `OrganizationId` int(11) NOT NULL,
  `CommunicationTypeId` int(11) NOT NULL,
  `CommunicationId` int(11) NOT NULL,
  `DateTimeOpened` datetime NOT NULL,
  `DateTimeFirstResponse` datetime NOT NULL,
  `PersonIdFirstResponse` int(11) NOT NULL,
  `DateTimeClosed` datetime NOT NULL,
  `PersonIdClosed` int(11) NOT NULL,
  `Open` tinyint(1) NOT NULL,
  `Responded` tinyint(1) NOT NULL,
  PRIMARY KEY (`OrganizationId`,`CommunicationTypeId`,`CommunicationId`),
  KEY `Index_Opened` (`DateTimeOpened`),
  KEY `Index_Organization` (`OrganizationId`),
  KEY `Index_Open` (`Open`),
  KEY `Index_Responded` (`Responded`) USING BTREE
) ENGINE=InnoDB DEFAULT CHARSET=latin1


#


CREATE TABLE `Countries` (
  `CountryId` int(10) unsigned NOT NULL AUTO_INCREMENT,
  `Name` varchar(256) NOT NULL,
  `Code` varchar(16) NOT NULL,
  `Culture` varchar(16) NOT NULL,
  `GeographyId` int(10) unsigned NOT NULL,
  `PostalCodeLength` int(10) unsigned NOT NULL,
  `Collation` varchar(128) NOT NULL,
  PRIMARY KEY (`CountryId`),
  KEY `Index_Code` (`Code`),
  KEY `Index_Geography` (`GeographyId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8


#


CREATE TABLE `Currencies` (
  `CurrencyId` int(10) unsigned NOT NULL AUTO_INCREMENT,
  `Code` varchar(4) NOT NULL,
  `Name` varchar(64) CHARACTER SET utf8 NOT NULL,
  `Sign` varchar(8) CHARACTER SET utf8 NOT NULL,
  PRIMARY KEY (`CurrencyId`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1


#


CREATE TABLE `DbVersion` (
  `DbVersion` int(11) NOT NULL COMMENT 'Contains just one row with the database version, an integer.'
) ENGINE=InnoDB DEFAULT CHARSET=latin1


#


CREATE TABLE `DocumentTypes` (
  `DocumentTypeId` int(10) unsigned NOT NULL AUTO_INCREMENT,
  `Name` varchar(64) NOT NULL,
  PRIMARY KEY (`DocumentTypeId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8


#


CREATE TABLE `Documents` (
  `DocumentId` int(10) unsigned NOT NULL AUTO_INCREMENT,
  `ServerFileName` varchar(128) NOT NULL,
  `ClientFileName` varchar(128) NOT NULL,
  `Description` varchar(128) NOT NULL,
  `DocumentTypeId` int(10) unsigned NOT NULL,
  `ForeignId` int(10) unsigned NOT NULL,
  `FileSize` bigint(20) unsigned NOT NULL,
  `UploadedByPersonId` int(10) unsigned NOT NULL,
  `UploadedDateTime` datetime NOT NULL,
  PRIMARY KEY (`DocumentId`),
  KEY `Index_ForeignId` (`ForeignId`),
  KEY `Index_DocumentTypeId` (`DocumentTypeId`),
  KEY `Index_Uploader` (`UploadedByPersonId`),
  KEY `Index_DateTime` (`UploadedDateTime`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8


#


CREATE TABLE `ExceptionLog` (
  `ExceptionDateTime` datetime NOT NULL,
  `Source` varchar(64) NOT NULL,
  `ExceptionText` text NOT NULL,
  `ExceptionID` int(11) NOT NULL AUTO_INCREMENT,
  PRIMARY KEY (`ExceptionID`),
  KEY `Index_DateTime` (`ExceptionDateTime`),
  KEY `Index_Source` (`Source`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8


#


CREATE TABLE `ExpenseClaims` (
  `ExpenseClaimId` int(10) unsigned NOT NULL AUTO_INCREMENT,
  `ClaimingPersonId` int(10) unsigned NOT NULL,
  `CreatedDateTime` datetime NOT NULL,
  `Open` tinyint(1) NOT NULL,
  `Attested` tinyint(1) NOT NULL DEFAULT '0',
  `Validated` tinyint(1) NOT NULL DEFAULT '0',
  `Claimed` tinyint(1) NOT NULL DEFAULT '0',
  `OrganizationId` int(10) unsigned NOT NULL,
  `GeographyId` int(10) unsigned NOT NULL COMMENT 'Legacy',
  `BudgetId` int(10) unsigned NOT NULL DEFAULT '0',
  `ExpenseDate` datetime NOT NULL,
  `Description` varchar(256) NOT NULL,
  `PreApprovedAmount` double NOT NULL DEFAULT '0',
  `Amount` double NOT NULL,
  `AmountCents` bigint(20) NOT NULL DEFAULT '0',
  `LastExpenseEventTypeId` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'Legacy, ignore',
  `Repaid` tinyint(1) NOT NULL DEFAULT '0',
  `KeepSeparate` tinyint(1) NOT NULL DEFAULT '0',
  `VatCents` bigint(20) NOT NULL DEFAULT '0',
  `ClosedDateTime` datetime NOT NULL DEFAULT '9999-12-31 00:00:00',
  PRIMARY KEY (`ExpenseClaimId`),
  KEY `Index_Open` (`Open`),
  KEY `Index_ClaimingPersonId` (`ClaimingPersonId`),
  KEY `Index_OrganizationId` (`OrganizationId`),
  KEY `Index_CreatedDateTime` (`CreatedDateTime`),
  KEY `Index_Claimed` (`Claimed`),
  KEY `Index_ClosedDateTime` (`ClosedDateTime`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8


#


CREATE TABLE `ExternalActivities` (
  `ExternalActivityId` int(11) NOT NULL AUTO_INCREMENT,
  `OrganizationId` int(11) NOT NULL,
  `GeographyId` int(11) NOT NULL,
  `DateTime` datetime NOT NULL,
  `ExternalActivityTypeId` int(11) NOT NULL,
  `Description` varchar(256) CHARACTER SET utf8 NOT NULL,
  `CreatedByPersonId` int(11) NOT NULL,
  `CreatedDateTime` datetime NOT NULL,
  `DupeOfActivityId` int(11) NOT NULL DEFAULT '0',
  PRIMARY KEY (`ExternalActivityId`),
  KEY `Index_Organization` (`OrganizationId`),
  KEY `Index_Geography` (`GeographyId`),
  KEY `Index_DateTime` (`DateTime`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1


#


CREATE TABLE `ExternalActivityTypes` (
  `ExternalActivityTypeId` int(11) NOT NULL AUTO_INCREMENT,
  `Name` varchar(64) NOT NULL,
  PRIMARY KEY (`ExternalActivityTypeId`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1


#


CREATE TABLE `ExternalCredentials` (
  `ExternalCredentialId` int(10) unsigned NOT NULL AUTO_INCREMENT,
  `ServiceName` varchar(64) NOT NULL,
  `Login` varchar(64) NOT NULL,
  `Password` varchar(256) NOT NULL,
  PRIMARY KEY (`ExternalCredentialId`),
  KEY `Index_ServiceName` (`ServiceName`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8


#


CREATE TABLE `ExternalIdentities` (
  `ExternalIdentityIdentity` int(11) NOT NULL AUTO_INCREMENT,
  `TypeOfAccount` int(11) NOT NULL,
  `ExternalSystem` varchar(50) NOT NULL,
  `UserID` varchar(50) NOT NULL,
  `Password` varchar(50) NOT NULL,
  `AttachedToPerson` int(11) DEFAULT NULL,
  PRIMARY KEY (`ExternalIdentityIdentity`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 ROW_FORMAT=COMPACT


#


CREATE TABLE `ExternalIdentityTypes` (
  `ExternalIdentityTypeId` int(11) NOT NULL AUTO_INCREMENT,
  `ExternalIdentityTypeName` varchar(50) NOT NULL,
  PRIMARY KEY (`ExternalIdentityTypeId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 ROW_FORMAT=COMPACT


#


CREATE TABLE `FailReasons` (
  `FailReasonId` int(11) NOT NULL AUTO_INCREMENT,
  `Name` varchar(2048) DEFAULT NULL,
  PRIMARY KEY (`FailReasonId`),
  KEY `Name` (`Name`(255))
) ENGINE=InnoDB DEFAULT CHARSET=utf8


#


CREATE TABLE `FinancialAccountBudgets` (
  `Year` int(10) unsigned NOT NULL,
  `FinancialAccountId` int(10) unsigned NOT NULL,
  `Amount` double NOT NULL,
  KEY `Index_FinancialAccountId` (`FinancialAccountId`),
  KEY `Index_OrganizationBudgetId` (`Year`) USING BTREE
) ENGINE=InnoDB DEFAULT CHARSET=utf8


#


CREATE TABLE `FinancialAccountBudgetsMonthly` (
  `FinancialAccountId` int(11) NOT NULL,
  `Year` int(11) NOT NULL,
  `Month` int(11) NOT NULL,
  `AmountCents` bigint(20) NOT NULL,
  KEY `Index_Account` (`FinancialAccountId`),
  KEY `Index_Year` (`Year`),
  KEY `Index_Month` (`Month`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1


#


CREATE TABLE `FinancialAccounts` (
  `FinancialAccountId` int(10) unsigned NOT NULL AUTO_INCREMENT,
  `OrganizationId` int(10) unsigned NOT NULL,
  `Name` varchar(64) NOT NULL,
  `AccountType` int(10) unsigned NOT NULL,
  `ParentFinancialAccountId` int(10) unsigned NOT NULL DEFAULT '0',
  `OwnerPersonId` int(10) unsigned NOT NULL DEFAULT '0',
  `Open` tinyint(4) NOT NULL DEFAULT '1',
  `OpenedYear` int(11) NOT NULL DEFAULT '1',
  `ClosedYear` int(11) NOT NULL DEFAULT '9999',
  `Expensable` tinyint(4) NOT NULL DEFAULT '1',
  `Administrative` tinyint(4) NOT NULL DEFAULT '0',
  PRIMARY KEY (`FinancialAccountId`),
  KEY `Index_OrganizationId` (`OrganizationId`),
  KEY `Index_ParentFinancialAccountId` (`ParentFinancialAccountId`),
  KEY `Index_Owner` (`OwnerPersonId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8


#


CREATE TABLE `FinancialDependencyTypes` (
  `FinancialDependencyTypeId` int(10) unsigned NOT NULL AUTO_INCREMENT,
  `Name` varchar(64) NOT NULL,
  PRIMARY KEY (`FinancialDependencyTypeId`),
  KEY `Index_Name` (`Name`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1


#


CREATE TABLE `FinancialTransactionDependencies` (
  `FinancialTransactionId` int(10) unsigned NOT NULL AUTO_INCREMENT,
  `FinancialDependencyTypeId` int(10) unsigned NOT NULL,
  `ForeignId` int(10) unsigned NOT NULL,
  PRIMARY KEY (`FinancialTransactionId`),
  KEY `Index_ForeignKey` (`FinancialDependencyTypeId`,`ForeignId`) USING BTREE
) ENGINE=InnoDB DEFAULT CHARSET=latin1


#


CREATE TABLE `FinancialTransactionRows` (
  `FinancialAccountId` int(10) unsigned NOT NULL,
  `FinancialTransactionId` int(10) unsigned NOT NULL,
  `Amount` double NOT NULL,
  `AmountCents` bigint(20) NOT NULL DEFAULT '0',
  `FinancialTransactionRowId` int(10) unsigned NOT NULL AUTO_INCREMENT,
  `CreatedDateTime` datetime NOT NULL DEFAULT '2009-03-15 00:00:00',
  `CreatedByPersonId` int(10) unsigned NOT NULL DEFAULT '1',
  `Deleted` tinyint(1) NOT NULL DEFAULT '0',
  `DeletedDateTime` datetime NOT NULL DEFAULT '1990-01-01 00:00:00',
  `DeletedByPersonId` int(10) unsigned NOT NULL DEFAULT '0',
  PRIMARY KEY (`FinancialTransactionRowId`),
  KEY `Index_FinancialAccountId` (`FinancialAccountId`),
  KEY `Index_FinancialTransactionId` (`FinancialTransactionId`),
  KEY `Index_Deleted` (`Deleted`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8


#


CREATE TABLE `FinancialTransactionTagSetTypes` (
  `FinancialTransactionTagSetTypeId` int(11) NOT NULL AUTO_INCREMENT,
  `ResourceName` varchar(128) NOT NULL,
  PRIMARY KEY (`FinancialTransactionTagSetTypeId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8


#


CREATE TABLE `FinancialTransactionTagSets` (
  `FinancialTransactionTagSetId` int(11) NOT NULL AUTO_INCREMENT,
  `FinancialTransactionTagSetTypeId` int(11) NOT NULL,
  `OrganizationId` int(11) NOT NULL,
  `DisplayOrder` int(11) NOT NULL,
  `AllowUntagged` tinyint(4) NOT NULL DEFAULT '1',
  `VisibilityLevel` int(11) NOT NULL DEFAULT '1',
  `ProfitLossType` int(11) NOT NULL DEFAULT '3',
  PRIMARY KEY (`FinancialTransactionTagSetId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8


#


CREATE TABLE `FinancialTransactionTagTypes` (
  `FinancialTransactionTagTypeId` int(11) NOT NULL AUTO_INCREMENT,
  `ParentFinancialTransactionTagTypeId` int(11) NOT NULL,
  `FinancialTransactionTagSetId` int(11) NOT NULL,
  `Name` varchar(128) NOT NULL,
  `Active` tinyint(4) NOT NULL DEFAULT '1',
  `OpenedYear` int(11) NOT NULL DEFAULT '1',
  `ClosedYear` int(11) NOT NULL DEFAULT '9999',
  PRIMARY KEY (`FinancialTransactionTagTypeId`),
  KEY `Index_Parent` (`ParentFinancialTransactionTagTypeId`),
  KEY `Index_Set` (`FinancialTransactionTagSetId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8


#


CREATE TABLE `FinancialTransactionTags` (
  `FinancialTransactionTagId` int(11) NOT NULL AUTO_INCREMENT,
  `FinancialTransactionId` int(11) NOT NULL,
  `FinancialTransactionTagTypeId` int(11) NOT NULL,
  PRIMARY KEY (`FinancialTransactionTagId`),
  KEY `Index_FinancialTransaction` (`FinancialTransactionId`),
  KEY `Index_TagType` (`FinancialTransactionTagTypeId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8


#


CREATE TABLE `FinancialTransactions` (
  `FinancialTransactionId` int(10) unsigned NOT NULL AUTO_INCREMENT,
  `OrganizationId` int(10) unsigned NOT NULL,
  `DateTime` datetime NOT NULL,
  `Comment` varchar(128) NOT NULL,
  `ImportHash` varchar(32) NOT NULL DEFAULT '' COMMENT 'Used for identifying auto-imported transactions',
  `HasDocumentation` tinyint(1) NOT NULL DEFAULT '0',
  PRIMARY KEY (`FinancialTransactionId`),
  KEY `Index_OrganizationId` (`OrganizationId`),
  KEY `Index_DateTime` (`DateTime`),
  KEY `Index_ImportHash` (`ImportHash`),
  KEY `Index_HasDocumentation` (`HasDocumentation`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8


#


CREATE TABLE `FinancialValidationTypes` (
  `FinancialValidationTypeId` int(10) unsigned NOT NULL AUTO_INCREMENT,
  `Name` varchar(64) NOT NULL,
  PRIMARY KEY (`FinancialValidationTypeId`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1


#


CREATE TABLE `FinancialValidations` (
  `FinancialValidationId` int(10) unsigned NOT NULL AUTO_INCREMENT,
  `TypeId` int(10) unsigned NOT NULL,
  `FinancialDependencyTypeId` int(10) unsigned NOT NULL,
  `ForeignId` int(10) unsigned NOT NULL,
  `ValidatedDateTime` datetime NOT NULL,
  `PersonId` int(10) unsigned NOT NULL,
  `Amount` double NOT NULL,
  PRIMARY KEY (`FinancialValidationId`),
  KEY `Index_ForeignObject` (`FinancialDependencyTypeId`,`ForeignId`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1


#


CREATE TABLE `FlatData` (
  `DataKey` varchar(128) NOT NULL,
  `DataValue` longtext NOT NULL,
  PRIMARY KEY (`DataKey`),
  KEY `Index_Key` (`DataKey`) USING BTREE
) ENGINE=InnoDB DEFAULT CHARSET=utf8


#


CREATE TABLE `GeoSpatial` (
  `ObjectType` int(11) NOT NULL,
  `ObjectId` int(11) NOT NULL,
  `Lat` double DEFAULT NULL,
  `Lng` double DEFAULT NULL,
  `South` double DEFAULT NULL,
  `West` double DEFAULT NULL,
  `North` double DEFAULT NULL,
  `East` double DEFAULT NULL,
  `PolygonPoints` longtext,
  PRIMARY KEY (`ObjectType`,`ObjectId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 ROW_FORMAT=COMPACT


#


CREATE TABLE `Geographies` (
  `GeographyId` int(10) unsigned NOT NULL AUTO_INCREMENT,
  `ParentGeographyId` int(11) NOT NULL,
  `Name` varchar(128) NOT NULL,
  PRIMARY KEY (`GeographyId`),
  KEY `Index_Parent` (`ParentGeographyId`),
  KEY `Index_Name` (`Name`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8


#


CREATE TABLE `GeographyDesignations` (
  `GeographyId` int(10) unsigned NOT NULL,
  `CountryId` int(10) unsigned NOT NULL,
  `Designation` varchar(16) NOT NULL,
  `GeographyLevelId` int(10) unsigned NOT NULL,
  KEY `Index_Geography` (`GeographyId`),
  KEY `Index_Country` (`CountryId`),
  KEY `Index_Designation` (`Designation`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8


#


CREATE TABLE `GeographyLevels` (
  `GeographyLevelId` int(10) unsigned NOT NULL AUTO_INCREMENT,
  `Name` varchar(32) NOT NULL,
  PRIMARY KEY (`GeographyLevelId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8


#


CREATE TABLE `GeographyUpdateTypes` (
  `GeographyUpdateTypeId` int(11) NOT NULL,
  `Name` varchar(128) NOT NULL,
  PRIMARY KEY (`GeographyUpdateTypeId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8


#


CREATE TABLE `GeographyUpdates` (
  `GeographyUpdateId` int(11) NOT NULL,
  `GeographyUpdateTypeId` int(11) NOT NULL,
  `CreatedDateTime` datetime NOT NULL,
  `EffectiveDateTime` datetime NOT NULL,
  `ChangeDataXml` text NOT NULL,
  `Processed` tinyint(1) NOT NULL,
  PRIMARY KEY (`GeographyUpdateId`),
  KEY `indexProcessed` (`Processed`),
  KEY `indexEffectiveDate` (`EffectiveDateTime`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8


#


CREATE TABLE `InboundInvoices` (
  `InboundInvoiceId` int(10) unsigned NOT NULL AUTO_INCREMENT,
  `CreatedDateTime` datetime NOT NULL,
  `DueDate` datetime NOT NULL,
  `BudgetId` int(10) unsigned NOT NULL,
  `Attested` tinyint(1) NOT NULL,
  `Open` tinyint(1) NOT NULL,
  `PayToAccount` varchar(64) NOT NULL,
  `Ocr` varchar(64) NOT NULL,
  `InvoiceReference` varchar(64) NOT NULL,
  `ClosedDateTime` varchar(64) NOT NULL,
  `Amount` double NOT NULL,
  `AmountCents` bigint(20) NOT NULL DEFAULT '0',
  `CreatedByPersonId` int(10) unsigned NOT NULL,
  `ClosedByPersonId` int(10) unsigned NOT NULL,
  `OrganizationId` int(10) unsigned NOT NULL,
  `Supplier` varchar(64) NOT NULL,
  `VatCents` bigint(20) NOT NULL DEFAULT '0',
  `SupplierId` int(11) NOT NULL DEFAULT '0',
  PRIMARY KEY (`InboundInvoiceId`),
  KEY `Index_Open` (`Open`),
  KEY `Index_OrganizationId` (`OrganizationId`),
  KEY `Index_BudgetId` (`BudgetId`) USING BTREE,
  KEY `Index_CreatedDateTime` (`CreatedDateTime`),
  KEY `Index_ClosedDateTime` (`ClosedDateTime`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1


#


CREATE TABLE `InternalPollCandidates` (
  `InternalPollCandidateId` int(10) unsigned NOT NULL AUTO_INCREMENT,
  `InternalPollId` int(10) unsigned NOT NULL,
  `PersonId` int(10) unsigned NOT NULL,
  `CandidacyStatement` text CHARACTER SET utf8 NOT NULL,
  `SortOrder` varchar(64) NOT NULL DEFAULT '',
  PRIMARY KEY (`InternalPollCandidateId`),
  KEY `Index_InternalPoll` (`InternalPollId`),
  KEY `Index_SortOrder` (`SortOrder`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1


#


CREATE TABLE `InternalPollVoteDetails` (
  `InternalPollVoteId` int(10) unsigned NOT NULL AUTO_INCREMENT,
  `Position` int(10) unsigned NOT NULL,
  `InternalPollCandidateId` int(10) unsigned NOT NULL,
  KEY `Index_InternalPollVoteId` (`InternalPollVoteId`),
  KEY `Index_Candidate` (`InternalPollCandidateId`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1


#


CREATE TABLE `InternalPollVoters` (
  `InternalPollVoterId` int(10) unsigned NOT NULL AUTO_INCREMENT,
  `InternalPollId` int(10) unsigned NOT NULL,
  `PersonId` int(10) unsigned NOT NULL,
  `Open` tinyint(1) NOT NULL,
  `ClosedDateTime` datetime NOT NULL,
  `IPAddress` varchar(48) NOT NULL DEFAULT 'notset',
  PRIMARY KEY (`InternalPollVoterId`),
  KEY `Index_Person` (`PersonId`),
  KEY `Index_PollId` (`InternalPollId`),
  KEY `Index_Open` (`Open`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1


#


CREATE TABLE `InternalPollVotes` (
  `InternalPollVoteId` int(10) unsigned NOT NULL AUTO_INCREMENT,
  `InternalPollId` int(10) unsigned NOT NULL,
  `VerificationCode` varchar(128) NOT NULL,
  `VoteGeographyId` int(10) unsigned NOT NULL DEFAULT '0',
  PRIMARY KEY (`InternalPollVoteId`),
  KEY `Index_Poll` (`InternalPollId`),
  KEY `Index_VerificationCode` (`VerificationCode`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1


#


CREATE TABLE `InternalPolls` (
  `InternalPollId` int(10) unsigned NOT NULL AUTO_INCREMENT,
  `OrganizationId` int(10) unsigned NOT NULL,
  `GeographyId` int(10) unsigned NOT NULL,
  `Name` varchar(128) CHARACTER SET utf8 NOT NULL,
  `RunningOpen` tinyint(1) NOT NULL,
  `VotingOpen` tinyint(1) NOT NULL,
  `MaxVoteLength` int(10) unsigned NOT NULL,
  `CreatedByPersonId` int(10) unsigned NOT NULL,
  `RunningCloses` datetime NOT NULL,
  `VotingCloses` datetime NOT NULL,
  `VotingOpens` datetime NOT NULL,
  `RunningOpens` datetime NOT NULL,
  `ResultsTypeId` int(10) unsigned NOT NULL,
  PRIMARY KEY (`InternalPollId`),
  KEY `Index_Organization` (`OrganizationId`),
  KEY `Index_Open1` (`RunningOpen`) USING BTREE,
  KEY `Index_Open2` (`VotingOpen`) USING BTREE
) ENGINE=InnoDB DEFAULT CHARSET=latin1


#


CREATE TABLE `MailTemplates` (
  `TemplateId` int(11) NOT NULL AUTO_INCREMENT,
  `TemplateName` varchar(50) NOT NULL DEFAULT '',
  `LanguageCode` varchar(10) NOT NULL DEFAULT '',
  `CountryCode` varchar(50) NOT NULL DEFAULT '',
  `OrganizationId` int(11) DEFAULT '0',
  `TemplateBody` longtext,
  PRIMARY KEY (`TemplateId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8


#


CREATE TABLE `Media` (
  `MediaId` int(10) unsigned NOT NULL AUTO_INCREMENT,
  `Name` varchar(128) NOT NULL,
  `IsBlog` tinyint(1) unsigned NOT NULL,
  PRIMARY KEY (`MediaId`),
  KEY `Index_Name` (`Name`) USING BTREE,
  KEY `Index_IsBlog` (`IsBlog`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8


#


CREATE TABLE `MediaCategories` (
  `MediaCategoryId` int(10) unsigned NOT NULL AUTO_INCREMENT,
  `Name` varchar(64) NOT NULL,
  PRIMARY KEY (`MediaCategoryId`),
  KEY `Index_Name` (`Name`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8


#


CREATE TABLE `MediaKeywordEntries` (
  `MediaKeywordEntryId` int(10) unsigned NOT NULL AUTO_INCREMENT,
  `MediaKeywordId` int(10) unsigned NOT NULL,
  `MediaId` int(10) unsigned NOT NULL,
  `MediaEntryDateTime` datetime NOT NULL,
  `MediaEntryUrl` varchar(512) CHARACTER SET latin1 NOT NULL,
  `MediaEntryTitle` varchar(256) NOT NULL,
  PRIMARY KEY (`MediaKeywordEntryId`),
  KEY `Index_Keyword` (`MediaKeywordId`),
  KEY `Index_Media` (`MediaId`),
  KEY `Index_DateTime` (`MediaEntryDateTime`),
  KEY `Index_Url` (`MediaEntryUrl`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8


#


CREATE TABLE `MediaKeywords` (
  `MediaKeywordId` int(10) unsigned NOT NULL,
  `MediaKeyword` varchar(64) NOT NULL,
  `SearchBlogs` tinyint(1) unsigned NOT NULL,
  `SearchOldMedia` tinyint(1) unsigned NOT NULL,
  PRIMARY KEY (`MediaKeywordId`),
  KEY `Index_Keyword` (`MediaKeyword`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8


#


CREATE TABLE `Meetings` (
  `MeetingId` int(11) NOT NULL AUTO_INCREMENT,
  `Name` varchar(128) NOT NULL,
  `MotionSubmissionEnds` datetime NOT NULL,
  `AmendmentSubmissionEnds` datetime NOT NULL,
  `AmendmentVotingStarts` datetime NOT NULL DEFAULT '1900-01-01 00:00:00',
  `AmendmentVotingEnds` datetime NOT NULL DEFAULT '1900-01-01 00:00:00',
  `MotionVotingStarts` datetime NOT NULL,
  `MotionVotingEnds` datetime NOT NULL,
  `OrganizationId` int(11) NOT NULL,
  PRIMARY KEY (`MeetingId`),
  KEY `Organization` (`OrganizationId`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1 COMMENT='Meetings with motions and amendments'


#


CREATE TABLE `MembershipPayments` (
  `MembershipId` int(10) unsigned NOT NULL,
  `MembershipPaymentStatusId` int(10) unsigned NOT NULL,
  `StatusDateTime` datetime NOT NULL,
  KEY `Index_Membership` (`MembershipId`),
  KEY `Index_DateTime` (`StatusDateTime`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8


#


CREATE TABLE `Memberships` (
  `MembershipId` int(10) unsigned NOT NULL AUTO_INCREMENT,
  `PersonId` int(10) unsigned NOT NULL,
  `OrganizationId` int(10) unsigned NOT NULL,
  `MemberSince` datetime NOT NULL,
  `Active` tinyint(1) unsigned NOT NULL,
  `Expires` datetime NOT NULL,
  `DateTimeTerminated` datetime NOT NULL,
  `TerminatedAsInvalid` tinyint(1) unsigned NOT NULL,
  PRIMARY KEY (`MembershipId`),
  KEY `Index_Person` (`PersonId`),
  KEY `Index_Organization` (`OrganizationId`),
  KEY `Index_Active` (`Active`),
  KEY `Index_Expires` (`Expires`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8


#


CREATE TABLE `MotionAmendments` (
  `MotionAmendmentId` int(11) NOT NULL AUTO_INCREMENT,
  `MotionId` int(11) NOT NULL,
  `SequenceNumber` int(11) NOT NULL,
  `Title` text NOT NULL,
  `Text` text NOT NULL,
  `DecisionPoint` text NOT NULL,
  `Open` tinyint(1) NOT NULL,
  `Carried` tinyint(1) NOT NULL,
  `SubmittedByPersonId` int(11) NOT NULL,
  `CreatedByPersonId` int(11) NOT NULL,
  `CreatedDateTime` datetime NOT NULL,
  PRIMARY KEY (`MotionAmendmentId`),
  KEY `Motion` (`MotionId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8


#


CREATE TABLE `Motions` (
  `MotionId` int(11) NOT NULL AUTO_INCREMENT,
  `MeetingId` int(11) NOT NULL,
  `Designation` varchar(32) NOT NULL,
  `Title` text NOT NULL,
  `Text` text NOT NULL,
  `DecisionPoints` text NOT NULL,
  `AmendedText` text NOT NULL,
  `AmendedDecisionPoints` text NOT NULL,
  `Open` tinyint(1) NOT NULL,
  `Amended` tinyint(1) NOT NULL,
  `Carried` tinyint(1) NOT NULL,
  `ThreadUrl` varchar(128) NOT NULL,
  `SubmittedByPersonId` int(11) NOT NULL,
  `CreatedByPersonId` int(11) NOT NULL,
  `AmendedByPersonId` int(11) NOT NULL,
  `CreatedDateTime` datetime NOT NULL,
  `AmendedDateTime` datetime NOT NULL,
  `SequenceNumber` int(11) NOT NULL,
  PRIMARY KEY (`MotionId`),
  KEY `IndexMeeting` (`MeetingId`) USING BTREE,
  KEY `IndexSequenceNumber` (`SequenceNumber`),
  KEY `IndexOpen` (`Open`),
  KEY `IndexPassed` (`Carried`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8


#


CREATE TABLE `NewsletterFeeds` (
  `NewsletterFeedId` int(10) unsigned NOT NULL AUTO_INCREMENT,
  `OrganizationId` int(10) unsigned NOT NULL,
  `DefaultSubscribed` tinyint(1) unsigned NOT NULL,
  `Name` varchar(128) NOT NULL,
  `NewsLetterType` int(11) DEFAULT NULL,
  PRIMARY KEY (`NewsletterFeedId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8


#


CREATE TABLE `NewsletterSubscriptions` (
  `PersonId` int(10) unsigned NOT NULL,
  `NewsletterFeedId` int(10) unsigned NOT NULL,
  `Subscribed` tinyint(1) unsigned NOT NULL,
  PRIMARY KEY (`NewsletterFeedId`,`PersonId`),
  KEY `Index_Person` (`PersonId`),
  KEY `Index_NewsletterFeed` (`NewsletterFeedId`),
  KEY `Index_Subscribed` (`Subscribed`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8


#


CREATE TABLE `ObjectOptionalData` (
  `ObjectTypeId` int(10) unsigned NOT NULL,
  `ObjectId` int(10) unsigned NOT NULL,
  `ObjectOptionalDataTypeId` int(10) unsigned NOT NULL,
  `Data` text NOT NULL,
  PRIMARY KEY (`ObjectTypeId`,`ObjectId`,`ObjectOptionalDataTypeId`),
  KEY `Index_DataType` (`ObjectOptionalDataTypeId`),
  KEY `Index_ObjectTypeId` (`ObjectTypeId`),
  KEY `Index_ObjectId` (`ObjectId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8


#


CREATE TABLE `ObjectOptionalDataTypes` (
  `ObjectOptionalDataTypeId` int(10) unsigned NOT NULL AUTO_INCREMENT,
  `Name` varchar(64) NOT NULL,
  PRIMARY KEY (`ObjectOptionalDataTypeId`),
  KEY `Index_Name` (`Name`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1


#


CREATE TABLE `ObjectTypes` (
  `ObjectTypeId` int(10) unsigned NOT NULL AUTO_INCREMENT,
  `Name` varchar(64) NOT NULL,
  PRIMARY KEY (`ObjectTypeId`),
  KEY `Index_Name` (`Name`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1


#


CREATE TABLE `OrganizationCommTemplateTypes` (
  `OrganizationCommTemplateTypeId` int(11) NOT NULL,
  `Name` varchar(128) NOT NULL,
  PRIMARY KEY (`OrganizationCommTemplateTypeId`),
  KEY `indexName` (`Name`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8


#


CREATE TABLE `OrganizationCommTemplates` (
  `OrganizationCommTemplateId` int(11) NOT NULL,
  `OrganizationCommTemplateTypeId` int(11) NOT NULL,
  `OrganizationId` int(11) NOT NULL,
  `TemplateData` text NOT NULL,
  `UpdatedByPersonId` int(11) NOT NULL,
  `UpdatedDateTime` datetime NOT NULL,
  PRIMARY KEY (`OrganizationCommTemplateId`),
  KEY `indexOrganization` (`OrganizationId`),
  KEY `indexCommType` (`OrganizationCommTemplateTypeId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8


#


CREATE TABLE `OrganizationFinancialAccountTypes` (
  `OrganizationFinancialAccountTypeId` int(10) unsigned NOT NULL AUTO_INCREMENT,
  `Name` varchar(64) NOT NULL,
  PRIMARY KEY (`OrganizationFinancialAccountTypeId`),
  KEY `Index_Name` (`Name`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1


#


CREATE TABLE `OrganizationFinancialAccounts` (
  `OrganizationFinancialAccountTypeId` int(10) unsigned NOT NULL,
  `OrganizationId` int(10) unsigned NOT NULL,
  `FinancialAccountId` int(10) NOT NULL,
  PRIMARY KEY (`OrganizationFinancialAccountTypeId`,`OrganizationId`),
  KEY `Index_OrganizationFinancialAccountTypeId` (`OrganizationFinancialAccountTypeId`),
  KEY `Index_OrganizationId` (`OrganizationId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8


#


CREATE TABLE `OrganizationUptakeGeographies` (
  `OrganizationId` int(10) unsigned NOT NULL,
  `GeographyId` int(10) unsigned NOT NULL,
  PRIMARY KEY (`OrganizationId`,`GeographyId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8


#


CREATE TABLE `Organizations` (
  `OrganizationId` int(10) unsigned NOT NULL AUTO_INCREMENT,
  `ParentOrganizationId` int(11) NOT NULL,
  `NameInternational` varchar(64) NOT NULL,
  `Name` varchar(64) NOT NULL,
  `NameShort` varchar(64) NOT NULL,
  `Domain` varchar(64) NOT NULL,
  `MailPrefix` varchar(64) NOT NULL,
  `AnchorGeographyId` int(10) unsigned NOT NULL,
  `AcceptsMembers` tinyint(1) unsigned NOT NULL,
  `AutoAssignNewMembers` tinyint(1) unsigned NOT NULL,
  `DefaultCountryId` int(10) unsigned NOT NULL,
  `Active` tinyint(1) unsigned NOT NULL DEFAULT '1',
  PRIMARY KEY (`OrganizationId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8


#


CREATE TABLE `OutboundCommRecipients` (
  `OutboundCommRecipientId` int(11) NOT NULL AUTO_INCREMENT,
  `OutboundCommId` int(11) NOT NULL,
  `PersonId` int(11) NOT NULL,
  `Open` tinyint(1) NOT NULL DEFAULT '1',
  `Success` tinyint(1) NOT NULL DEFAULT '0',
  `FailReasonId` int(11) NOT NULL DEFAULT '0',
  PRIMARY KEY (`OutboundCommRecipientId`),
  KEY `Ix_Open` (`Open`),
  KEY `Ix_OutboundComms` (`OutboundCommId`),
  KEY `Ix_Success` (`Success`),
  KEY `Ix_Person` (`PersonId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8


#


CREATE TABLE `OutboundComms` (
  `OutboundCommId` int(11) NOT NULL AUTO_INCREMENT,
  `SenderPersonId` int(11) NOT NULL,
  `FromPersonId` int(11) NOT NULL,
  `OrganizationId` varchar(45) NOT NULL,
  `CreatedDateTime` datetime NOT NULL,
  `ResolverClassId` int(11) NOT NULL,
  `RecipientDataXml` text NOT NULL,
  `Resolved` tinyint(1) NOT NULL DEFAULT '0',
  `ResolvedDateTime` datetime NOT NULL DEFAULT '1970-01-01 00:00:00',
  `Priority` int(11) NOT NULL DEFAULT '128',
  `TransmitterClassId` int(11) NOT NULL,
  `PayloadXml` text NOT NULL,
  `Open` tinyint(1) NOT NULL DEFAULT '1',
  `StartTransmitDateTime` datetime NOT NULL DEFAULT '1970-01-01 00:00:00',
  `ClosedDateTime` datetime NOT NULL DEFAULT '1970-01-01 00:00:00',
  `RecipientCount` int(11) NOT NULL DEFAULT '0',
  `RecipientsSuccess` int(11) NOT NULL DEFAULT '0',
  `RecipientsFail` int(11) NOT NULL DEFAULT '0',
  PRIMARY KEY (`OutboundCommId`),
  KEY `Ix_Resolved` (`Resolved`),
  KEY `Ix_Open` (`Open`),
  KEY `Ix_TransmitterClass` (`TransmitterClassId`),
  KEY `Ix_DateTime` (`CreatedDateTime`),
  KEY `Ix_Sender` (`SenderPersonId`),
  KEY `Ix_From` (`FromPersonId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='Table replacing OutboundMails and OutboundPhoneMessages in v'


#


CREATE TABLE `OutboundInvoiceItemDependencies` (
  `OutboundInvoiceItemId` int(11) NOT NULL,
  `FinancialDependencyTypeId` int(11) NOT NULL,
  `ForeignObjectId` int(11) NOT NULL,
  PRIMARY KEY (`OutboundInvoiceItemId`),
  KEY `Index_ForeignId` (`ForeignObjectId`),
  KEY `Index_FinancialDependencyTypeId` (`FinancialDependencyTypeId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8


#


CREATE TABLE `OutboundInvoiceItems` (
  `OutboundInvoiceItemId` int(10) unsigned NOT NULL AUTO_INCREMENT,
  `OutboundInvoiceId` int(10) unsigned NOT NULL,
  `SortOrder` int(10) unsigned NOT NULL,
  `Description` varchar(128) CHARACTER SET utf8 NOT NULL,
  `Amount` double NOT NULL,
  `AmountCents` bigint(20) NOT NULL DEFAULT '0',
  `VatCents` bigint(20) NOT NULL DEFAULT '0',
  PRIMARY KEY (`OutboundInvoiceItemId`),
  KEY `Index_OutboundInvoice` (`OutboundInvoiceId`),
  KEY `Index_SortOrder` (`SortOrder`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1


#


CREATE TABLE `OutboundInvoices` (
  `OutboundInvoiceId` int(10) unsigned NOT NULL AUTO_INCREMENT,
  `CustomerName` varchar(128) NOT NULL,
  `InvoiceAddressPaper` varchar(256) NOT NULL,
  `InvoiceAddressMail` varchar(128) CHARACTER SET latin1 NOT NULL,
  `CurrencyId` int(10) unsigned NOT NULL,
  `BudgetId` int(10) unsigned NOT NULL,
  `OrganizationId` int(10) unsigned NOT NULL,
  `CreatedDateTime` datetime NOT NULL,
  `CreatedByPersonId` int(10) unsigned NOT NULL,
  `DueDate` datetime NOT NULL,
  `ReminderCount` int(10) unsigned NOT NULL,
  `Reference` varchar(64) NOT NULL,
  `Open` tinyint(1) NOT NULL,
  `Domestic` tinyint(1) NOT NULL,
  `Sent` tinyint(1) NOT NULL,
  `SecurityCode` varchar(16) NOT NULL,
  `TheirReference` varchar(64) NOT NULL,
  `ClosedDateTime` datetime NOT NULL,
  `RecipientTypeId` int(11) NOT NULL DEFAULT '0',
  `RecipientForeignId` int(11) NOT NULL DEFAULT '0',
  PRIMARY KEY (`OutboundInvoiceId`),
  KEY `Index_Organization` (`OrganizationId`),
  KEY `Index_DueDate` (`DueDate`),
  KEY `Index_Customer` (`CustomerName`),
  KEY `Index_Reference` (`Reference`),
  KEY `Index_Open` (`Open`),
  KEY `Index_CreatedDateTime` (`CreatedDateTime`),
  KEY `Index_ClosedDateTime` (`ClosedDateTime`),
  KEY `Index_RecipientForeignId` (`RecipientForeignId`,`RecipientTypeId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8


#


CREATE TABLE `OutboundMailRecipients` (
  `OutboundMailRecipientId` int(10) unsigned NOT NULL AUTO_INCREMENT,
  `OutboundMailId` int(10) unsigned NOT NULL,
  `PersonId` int(10) unsigned NOT NULL,
  `AsOfficer` tinyint(1) unsigned NOT NULL DEFAULT '0',
  `PersonType` int(10) unsigned NOT NULL DEFAULT '0',
  PRIMARY KEY (`OutboundMailRecipientId`),
  KEY `Index_OutboundMail` (`OutboundMailId`),
  KEY `Index_Person` (`PersonId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8


#


CREATE TABLE `OutboundMails` (
  `OutboundMailId` int(10) unsigned NOT NULL AUTO_INCREMENT,
  `AuthorType` int(10) unsigned NOT NULL,
  `AuthorPersonId` int(10) unsigned NOT NULL,
  `Title` text NOT NULL,
  `Body` longtext NOT NULL,
  `MailPriority` int(10) unsigned NOT NULL,
  `MailType` int(10) unsigned NOT NULL,
  `OrganizationId` int(10) unsigned NOT NULL,
  `GeographyId` int(10) unsigned NOT NULL,
  `CreatedDateTime` datetime NOT NULL,
  `ReleaseDateTime` datetime NOT NULL,
  `ReadyForPickup` tinyint(1) unsigned NOT NULL DEFAULT '0',
  `Resolved` tinyint(1) unsigned NOT NULL DEFAULT '0',
  `Processed` tinyint(1) unsigned NOT NULL DEFAULT '0',
  `ResolvedDateTime` datetime NOT NULL DEFAULT '1970-01-01 00:00:00',
  `StartProcessDateTime` datetime NOT NULL DEFAULT '1970-01-01 00:00:00',
  `EndProcessDateTime` datetime NOT NULL DEFAULT '1970-01-01 00:00:00',
  `RecipientCount` int(10) unsigned NOT NULL DEFAULT '0',
  `RecipientsSuccess` int(10) unsigned NOT NULL DEFAULT '0',
  `RecipientsFail` int(10) unsigned NOT NULL DEFAULT '0',
  PRIMARY KEY (`OutboundMailId`),
  KEY `Index_ReleaseDateTime` (`ReleaseDateTime`),
  KEY `Index_ReadyForPickup` (`ReadyForPickup`),
  KEY `Index_Resolved` (`Resolved`),
  KEY `Index_MailPriority` (`MailPriority`),
  KEY `Index_MailQueue` (`ReadyForPickup`,`Processed`,`Resolved`),
  KEY `Index_EndProcessDateTime` (`EndProcessDateTime`) USING BTREE
) ENGINE=InnoDB DEFAULT CHARSET=utf8


#


CREATE TABLE `PWEventSources` (
  `EventSourceId` int(10) unsigned NOT NULL AUTO_INCREMENT,
  `Name` varchar(64) NOT NULL,
  PRIMARY KEY (`EventSourceId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8


#


CREATE TABLE `PWEventTypes` (
  `EventTypeId` int(10) unsigned NOT NULL AUTO_INCREMENT,
  `Name` varchar(64) NOT NULL,
  PRIMARY KEY (`EventTypeId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8


#


CREATE TABLE `PWEvents` (
  `EventId` int(10) unsigned NOT NULL AUTO_INCREMENT,
  `CreatedDateTime` datetime NOT NULL,
  `ProcessDateTime` datetime NOT NULL,
  `ProcessedDateTime` datetime NOT NULL DEFAULT '1900-01-01 00:00:00',
  `Open` tinyint(1) unsigned NOT NULL,
  `EventSourceId` int(11) NOT NULL,
  `EventTypeId` int(11) NOT NULL,
  `OrganizationId` int(10) unsigned NOT NULL,
  `GeographyId` int(10) unsigned NOT NULL,
  `ActingPersonId` int(10) unsigned NOT NULL,
  `AffectedPersonId` int(10) unsigned NOT NULL,
  `ParameterInt` int(10) unsigned NOT NULL,
  `ParameterText` longtext NOT NULL,
  PRIMARY KEY (`EventId`),
  KEY `Index_ProcessDateTime` (`ProcessDateTime`),
  KEY `Index_Open` (`Open`) USING BTREE
) ENGINE=InnoDB DEFAULT CHARSET=utf8


#


CREATE TABLE `PWLog` (
  `DateTimeUtc` datetime NOT NULL,
  `ActingPersonId` int(10) unsigned NOT NULL,
  `AffectedItemType` varchar(64) NOT NULL,
  `AffectedItemId` int(10) unsigned NOT NULL,
  `ActionType` varchar(64) NOT NULL,
  `ActionDescription` varchar(512) NOT NULL,
  `ChangedField` varchar(64) NOT NULL,
  `ValueBefore` varchar(512) NOT NULL,
  `ValueAfter` varchar(512) NOT NULL,
  `Comment` text NOT NULL,
  `IpAddress` varchar(45) DEFAULT NULL,
  `DbId` int(11) NOT NULL AUTO_INCREMENT,
  PRIMARY KEY (`DbId`),
  KEY `Index_ActingPersonId` (`ActingPersonId`),
  KEY `Index_DateTimeUtc` (`DateTimeUtc`),
  KEY `Index_comment` (`Comment`(50)),
  KEY `Index_IP` (`IpAddress`),
  KEY `Index_AffectedItemId_Type` (`AffectedItemId`,`AffectedItemType`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8


#


CREATE TABLE `PaperLetters` (
  `PaperLetterId` int(10) unsigned NOT NULL AUTO_INCREMENT,
  `FromName` varchar(128) NOT NULL,
  `ReplyAddress` varchar(256) NOT NULL COMMENT 'Vertical Bar Separation Between Lines',
  `ReceivedDate` datetime NOT NULL,
  `ToPersonId` int(10) unsigned NOT NULL,
  `Personal` tinyint(1) NOT NULL,
  `OrganizationId` int(10) unsigned NOT NULL,
  `UploadedByPersonId` int(10) unsigned NOT NULL,
  `UploadedDateTime` datetime NOT NULL,
  `ToPersonInRoleId` int(10) unsigned NOT NULL DEFAULT '0',
  PRIMARY KEY (`PaperLetterId`),
  KEY `Index_OrganizationId` (`OrganizationId`),
  KEY `Index_ToPersonId` (`ToPersonId`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1


#


CREATE TABLE `ParleyAttendeeOptions` (
  `ParleyAttendeeOptionId` int(11) NOT NULL AUTO_INCREMENT,
  `ParleyAttendeeId` int(11) NOT NULL,
  `ParleyOptionId` int(11) NOT NULL,
  PRIMARY KEY (`ParleyAttendeeOptionId`),
  KEY `Index_ParleyAttendee` (`ParleyAttendeeId`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1


#


CREATE TABLE `ParleyAttendees` (
  `ParleyAttendeeId` int(11) NOT NULL AUTO_INCREMENT,
  `ParleyId` int(11) NOT NULL,
  `PersonId` int(11) NOT NULL,
  `SignupDateTime` datetime NOT NULL,
  `Active` tinyint(1) NOT NULL,
  `CancelDateTime` datetime NOT NULL,
  `Invoiced` tinyint(1) NOT NULL,
  `OutboundInvoiceId` int(11) NOT NULL,
  `IsGuest` tinyint(1) NOT NULL,
  PRIMARY KEY (`ParleyAttendeeId`),
  KEY `Index_Parley` (`ParleyId`),
  KEY `Index_Person` (`PersonId`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1


#


CREATE TABLE `ParleyOptions` (
  `ParleyOptionId` int(11) NOT NULL AUTO_INCREMENT,
  `ParleyId` int(11) NOT NULL,
  `Description` varchar(512) CHARACTER SET utf8 NOT NULL,
  `AmountCents` bigint(20) NOT NULL,
  `Active` tinyint(1) NOT NULL,
  PRIMARY KEY (`ParleyOptionId`),
  KEY `Index_Parley` (`ParleyId`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1


#


CREATE TABLE `Parleys` (
  `ParleyId` int(11) NOT NULL AUTO_INCREMENT,
  `OrganizationId` int(11) NOT NULL,
  `PersonId` int(11) NOT NULL,
  `BudgetId` int(11) NOT NULL,
  `Open` tinyint(1) NOT NULL,
  `Name` varchar(256) CHARACTER SET utf8 NOT NULL,
  `GeographyId` int(11) NOT NULL,
  `Description` text CHARACTER SET utf8 NOT NULL,
  `InformationUrl` varchar(512) CHARACTER SET utf8 NOT NULL,
  `StartDate` datetime NOT NULL,
  `EndDate` datetime NOT NULL,
  `BudgetCents` bigint(20) NOT NULL,
  `GuaranteeCents` bigint(20) NOT NULL,
  `AttendanceFeeCents` bigint(20) NOT NULL,
  `Attested` tinyint(1) NOT NULL,
  `CreatedDateTime` datetime NOT NULL,
  `ClosedDateTime` datetime NOT NULL,
  PRIMARY KEY (`ParleyId`),
  KEY `Index_Organization` (`OrganizationId`),
  KEY `Index_Person` (`PersonId`),
  KEY `Index_Date` (`StartDate`),
  KEY `Index_Attested` (`Attested`),
  KEY `Index_Open` (`Open`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1


#


CREATE TABLE `PaymentGroups` (
  `PaymentGroupId` int(11) NOT NULL AUTO_INCREMENT,
  `CurrencyId` int(11) NOT NULL,
  `Amount` double NOT NULL,
  `AmountCents` bigint(20) NOT NULL DEFAULT '0',
  `DateTime` datetime NOT NULL,
  `Tag` varchar(32) CHARACTER SET utf8 NOT NULL,
  `CreatedDateTime` datetime NOT NULL,
  `CreatedByPersonId` int(11) NOT NULL,
  `Open` tinyint(1) NOT NULL,
  `OrganizationId` int(11) NOT NULL,
  PRIMARY KEY (`PaymentGroupId`),
  KEY `Index_Tag` (`Tag`),
  KEY `Index_Open` (`Open`),
  KEY `Index_DateTime` (`DateTime`),
  KEY `Index_Organization` (`OrganizationId`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1


#


CREATE TABLE `PaymentInformation` (
  `PaymentInformationId` int(11) NOT NULL AUTO_INCREMENT,
  `PaymentId` int(11) NOT NULL,
  `PaymentInformationTypeId` int(11) NOT NULL,
  `Data` varchar(512) CHARACTER SET utf8 NOT NULL,
  PRIMARY KEY (`PaymentInformationId`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1


#


CREATE TABLE `PaymentInformationTypes` (
  `PaymentInformationTypeId` int(11) NOT NULL AUTO_INCREMENT,
  `Name` varchar(32) NOT NULL,
  PRIMARY KEY (`PaymentInformationTypeId`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1


#


CREATE TABLE `Payments` (
  `PaymentId` int(11) NOT NULL AUTO_INCREMENT,
  `PaymentGroupId` int(11) NOT NULL,
  `Amount` double NOT NULL,
  `AmountCents` bigint(20) NOT NULL DEFAULT '0',
  `Reference` varchar(32) NOT NULL,
  `FromAccount` varchar(32) NOT NULL,
  `PaymentKey` varchar(32) CHARACTER SET latin2 NOT NULL,
  `HasImage` tinyint(1) NOT NULL,
  `OutboundInvoiceId` int(11) NOT NULL,
  PRIMARY KEY (`PaymentId`),
  KEY `Index_PaymentGroupId` (`PaymentGroupId`),
  KEY `Index_Key` (`PaymentKey`),
  KEY `Index_OutboundInvoiceId` (`OutboundInvoiceId`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1


#


CREATE TABLE `PayoutDependencies` (
  `PayoutId` int(10) unsigned NOT NULL,
  `FinancialDependencyTypeId` int(10) unsigned NOT NULL,
  `ForeignId` int(10) unsigned NOT NULL,
  KEY `Index_Payout` (`PayoutId`),
  KEY `Index_Foreign` (`FinancialDependencyTypeId`,`ForeignId`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1


#


CREATE TABLE `Payouts` (
  `PayoutId` int(10) unsigned NOT NULL AUTO_INCREMENT,
  `OrganizationId` int(10) unsigned NOT NULL,
  `Bank` varchar(128) NOT NULL,
  `Account` varchar(128) NOT NULL,
  `Reference` varchar(256) NOT NULL,
  `Amount` double NOT NULL,
  `AmountCents` bigint(20) NOT NULL DEFAULT '0',
  `ExpectedTransactionDate` datetime NOT NULL,
  `Open` tinyint(1) NOT NULL,
  `CreatedDateTime` datetime NOT NULL,
  `CreatedByPersonId` int(10) unsigned NOT NULL,
  PRIMARY KEY (`PayoutId`),
  KEY `Index_Organization` (`OrganizationId`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1


#


CREATE TABLE `Payroll` (
  `PayrollItemId` int(10) unsigned NOT NULL AUTO_INCREMENT,
  `PersonId` int(10) unsigned NOT NULL,
  `OrganizationId` int(10) unsigned NOT NULL,
  `EmployedDate` datetime NOT NULL,
  `ReportsToPersonId` int(10) unsigned NOT NULL,
  `CountryId` int(10) unsigned NOT NULL,
  `BaseSalary` double NOT NULL,
  `BaseSalaryCents` bigint(20) NOT NULL DEFAULT '0',
  `Open` tinyint(1) NOT NULL,
  `TerminatedDate` datetime NOT NULL,
  `SubtractiveTaxLevelId` int(10) unsigned NOT NULL,
  `AdditiveTaxLevel` double NOT NULL,
  `BudgetId` int(10) unsigned NOT NULL,
  PRIMARY KEY (`PayrollItemId`),
  KEY `Index_Organization` (`OrganizationId`),
  KEY `Index_Person` (`PersonId`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1 COMMENT='People employed'


#


CREATE TABLE `PayrollAdjustments` (
  `PayrollAdjustmentId` int(11) NOT NULL AUTO_INCREMENT,
  `PayrollItemId` int(11) NOT NULL,
  `PayrollAdjustmentTypeId` int(11) NOT NULL,
  `Amount` double NOT NULL,
  `AmountCents` bigint(20) NOT NULL DEFAULT '0',
  `Open` tinyint(1) NOT NULL,
  `SalaryId` int(11) NOT NULL,
  `Description` text CHARACTER SET utf8 NOT NULL,
  PRIMARY KEY (`PayrollAdjustmentId`),
  KEY `Index_Payroll` (`PayrollItemId`),
  KEY `Index_Open` (`Open`),
  KEY `Index_Salary` (`SalaryId`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1


#


CREATE TABLE `People` (
  `PersonId` int(10) unsigned NOT NULL AUTO_INCREMENT,
  `Name` varchar(128) NOT NULL,
  `PasswordHash` varchar(128) NOT NULL,
  `Email` varchar(128) NOT NULL,
  `Street` varchar(128) NOT NULL,
  `PostalCode` varchar(32) NOT NULL,
  `City` varchar(128) NOT NULL,
  `CountryId` int(10) unsigned NOT NULL,
  `PhoneNumber` varchar(48) NOT NULL,
  `GeographyId` int(10) unsigned NOT NULL,
  `Birthdate` datetime NOT NULL,
  `GenderId` int(10) unsigned NOT NULL,
  `AnonymizeAfterDate` datetime NOT NULL DEFAULT '1970-01-01 00:00:00',
  `FacebookId` varchar(64) NOT NULL DEFAULT '',
  `TwitterId` varchar(64) NOT NULL DEFAULT '',
  `GPlusId` varchar(64) NOT NULL DEFAULT '',
  `DisplayImage` varchar(256) NOT NULL DEFAULT '',
  PRIMARY KEY (`PersonId`),
  KEY `Index_Geography` (`GeographyId`),
  KEY `Index_AnonymizeAfter` (`AnonymizeAfterDate`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8


#


CREATE TABLE `PeopleRoles` (
  `PersonRoleId` int(10) unsigned NOT NULL AUTO_INCREMENT,
  `PersonId` int(10) unsigned NOT NULL,
  `PersonRoleTypeId` int(10) unsigned NOT NULL,
  `OrganizationId` int(10) unsigned NOT NULL,
  `GeographyId` int(10) unsigned NOT NULL,
  PRIMARY KEY (`PersonRoleId`),
  KEY `Index_Person` (`PersonId`),
  KEY `Index_Organization` (`OrganizationId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8


#


CREATE TABLE `PermissionSpecifications` (
  `RoleType` varchar(64) NOT NULL,
  `PermissionType` varchar(64) NOT NULL,
  PRIMARY KEY (`RoleType`,`PermissionType`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8


#


CREATE TABLE `PermissionTypes` (
  `PermissionName` varchar(64) NOT NULL,
  PRIMARY KEY (`PermissionName`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8


#


CREATE TABLE `PersonRoleTypes` (
  `PersonRoleTypeId` int(10) unsigned NOT NULL AUTO_INCREMENT,
  `Name` varchar(64) NOT NULL,
  PRIMARY KEY (`PersonRoleTypeId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8


#


CREATE TABLE `PostalCodes` (
  `PostalCodeId` int(11) NOT NULL AUTO_INCREMENT,
  `PostalCode` varchar(32) NOT NULL,
  `CountryId` int(10) unsigned NOT NULL,
  `CityId` int(10) unsigned NOT NULL,
  `Lat` double DEFAULT NULL,
  `Long` double DEFAULT NULL,
  PRIMARY KEY (`PostalCodeId`),
  KEY `Index_PostalCode` (`PostalCode`),
  KEY `Index_CountryId` (`CountryId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8


#


CREATE TABLE `Refunds` (
  `RefundId` int(11) NOT NULL AUTO_INCREMENT,
  `PaymentId` int(11) NOT NULL,
  `Open` tinyint(1) NOT NULL,
  `AmountCents` bigint(20) NOT NULL,
  `CreatedDateTime` datetime NOT NULL,
  `ClosedDateTime` datetime NOT NULL,
  `CreatedByPersonId` int(11) NOT NULL,
  PRIMARY KEY (`RefundId`),
  KEY `Index_Payment` (`PaymentId`),
  KEY `Index_Open` (`Open`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1


#


CREATE TABLE `Reporters` (
  `ReporterId` int(10) unsigned NOT NULL AUTO_INCREMENT,
  `Name` varchar(128) NOT NULL,
  `Email` varchar(128) CHARACTER SET latin1 NOT NULL,
  PRIMARY KEY (`ReporterId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8


#


CREATE TABLE `ReportersMediaCategories` (
  `ReporterId` int(10) unsigned NOT NULL,
  `MediaCategoryId` int(10) unsigned NOT NULL,
  PRIMARY KEY (`ReporterId`,`MediaCategoryId`),
  KEY `Index_Reporter` (`ReporterId`),
  KEY `Index_MediaCategory` (`MediaCategoryId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8


#


CREATE TABLE `ResolverClasses` (
  `ResolverClassId` int(11) NOT NULL AUTO_INCREMENT,
  `Name` varchar(512) NOT NULL,
  PRIMARY KEY (`ResolverClassId`),
  KEY `Name` (`Name`(255))
) ENGINE=InnoDB DEFAULT CHARSET=utf8


#


CREATE TABLE `Salaries` (
  `SalaryId` int(10) unsigned NOT NULL AUTO_INCREMENT,
  `PayrollItemId` int(10) unsigned NOT NULL,
  `PayoutDate` datetime NOT NULL,
  `BaseSalary` double NOT NULL,
  `BaseSalaryCents` bigint(20) NOT NULL DEFAULT '0',
  `NetSalary` double NOT NULL,
  `NetSalaryCents` bigint(20) NOT NULL DEFAULT '0',
  `SubtractiveTax` double NOT NULL,
  `SubtractiveTaxCents` bigint(20) NOT NULL DEFAULT '0',
  `AdditiveTax` double NOT NULL,
  `AdditiveTaxCents` bigint(20) NOT NULL DEFAULT '0',
  `Attested` tinyint(1) NOT NULL,
  `TaxPaid` tinyint(1) NOT NULL,
  `NetPaid` tinyint(1) NOT NULL,
  `Open` tinyint(1) NOT NULL,
  PRIMARY KEY (`SalaryId`),
  KEY `Index_Date` (`PayoutDate`),
  KEY `Index_Organization` (`PayrollItemId`) USING BTREE,
  KEY `Index_PayrollItem` (`PayrollItemId`),
  KEY `Index_Open` (`Open`) USING BTREE
) ENGINE=InnoDB DEFAULT CHARSET=latin1 COMMENT='Monthly payouts'


#


CREATE TABLE `SalaryTaxLevels` (
  `CountryId` int(11) NOT NULL DEFAULT '0',
  `TaxLevelId` int(11) NOT NULL DEFAULT '0',
  `BracketLow` int(10) unsigned NOT NULL COMMENT 'Low limit of bracket',
  `Tax` int(11) NOT NULL COMMENT 'Negative is percent',
  PRIMARY KEY (`TaxLevelId`,`BracketLow`,`CountryId`),
  KEY `Index_Bracket` (`BracketLow`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1 COMMENT='Modeled after Sweden; may need adaptation'


#


CREATE TABLE `SwarmopsLog` (
  `SwarmopsLogEntryId` int(11) NOT NULL AUTO_INCREMENT,
  `PersonId` int(11) DEFAULT NULL,
  `DateTime` datetime DEFAULT NULL,
  `EntryTypeId` int(11) DEFAULT NULL,
  `EntryXml` text,
  PRIMARY KEY (`SwarmopsLogEntryId`),
  KEY `Index_Person` (`PersonId`),
  KEY `Index_DateTime` (`DateTime`),
  KEY `Index_EventType` (`EntryTypeId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8


#


CREATE TABLE `SwarmopsLogAffectedObjectTypes` (
  `SwarmopsLogAffectedObjectTypeId` int(11) NOT NULL AUTO_INCREMENT,
  `Name` varchar(128) DEFAULT NULL,
  PRIMARY KEY (`SwarmopsLogAffectedObjectTypeId`),
  KEY `Index_Name` (`Name`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8


#


CREATE TABLE `SwarmopsLogAffectedObjects` (
  `SwarmopsLogEntryId` int(11) NOT NULL,
  `AffectedObjectTypeId` int(11) NOT NULL,
  `AffectedObjectId` int(11) NOT NULL,
  KEY `Index_ObjectType` (`AffectedObjectTypeId`),
  KEY `Index_ObjectId` (`AffectedObjectId`),
  KEY `Index_LogEntryId` (`SwarmopsLogEntryId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8


#


CREATE TABLE `SwarmopsLogEntryTypes` (
  `SwarmopsLogEntryTypeId` int(11) NOT NULL AUTO_INCREMENT,
  `Name` varchar(128) DEFAULT NULL,
  PRIMARY KEY (`SwarmopsLogEntryTypeId`),
  KEY `Index_Name` (`Name`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8


#


CREATE TABLE `TemporaryIdentities` (
  `TemporaryId` int(10) unsigned NOT NULL AUTO_INCREMENT,
  `CreatedDateTime` datetime NOT NULL,
  PRIMARY KEY (`TemporaryId`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1


#


CREATE TABLE `TransmitterClasses` (
  `TransmitterClassId` int(11) NOT NULL AUTO_INCREMENT,
  `Name` varchar(512) NOT NULL,
  PRIMARY KEY (`TransmitterClassId`),
  KEY `Name` (`Name`(255))
) ENGINE=InnoDB DEFAULT CHARSET=utf8


#


CREATE TABLE `VolunteerRoles` (
  `VolunteerRoleId` int(10) unsigned NOT NULL AUTO_INCREMENT,
  `VolunteerId` int(10) unsigned NOT NULL,
  `OrganizationId` int(10) unsigned NOT NULL,
  `GeographyId` int(10) unsigned NOT NULL,
  `RoleTypeId` int(10) unsigned NOT NULL,
  `Open` bit(1) NOT NULL,
  `Assigned` bit(1) NOT NULL,
  PRIMARY KEY (`VolunteerRoleId`),
  KEY `Index_VolunteerId` (`VolunteerId`),
  KEY `Index_Open` (`Open`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8


#


CREATE TABLE `Volunteers` (
  `VolunteerId` int(10) unsigned NOT NULL AUTO_INCREMENT,
  `PersonId` int(10) unsigned NOT NULL,
  `OwnerPersonId` int(10) unsigned NOT NULL,
  `OpenedDateTime` datetime NOT NULL,
  `Open` bit(1) NOT NULL,
  `ClosedDateTime` datetime NOT NULL,
  `ClosingComments` text NOT NULL,
  PRIMARY KEY (`VolunteerId`),
  KEY `Index_OwnerPersonId` (`OwnerPersonId`),
  KEY `Index_OpenedDateTime` (`OpenedDateTime`),
  KEY `Index_Open` (`Open`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8


#


CREATE PROCEDURE `AddParleyAttendeeOption`(
  parleyAttendeeId INTEGER,
  parleyOptionId INTEGER
)
BEGIN

  INSERT INTO ParleyAttendeeOptions (ParleyAttendeeId,ParleyOptionId)
    VALUES (parleyAttendeeId,parleyOptionId);

  SELECT LAST_INSERT_ID() AS Identity;

END


#


CREATE PROCEDURE `ClearBallotCandidates`(
  IN ballotId INTEGER
)
BEGIN

  DELETE FROM BallotCandidates WHERE BallotCandidates.BallotId=ballotId;  

END


#


CREATE PROCEDURE `ClearFinancialTransactionDependency`(
  financialTransactionId INTEGER
)
BEGIN

  DELETE FROM FinancialTransactionDependencies
    WHERE FinancialTransactionDependencies.FinancialTransacionId=financialTransactionId;

END


#


CREATE PROCEDURE `ClearInternalPollVote`(
  IN internalPollVoteId INTEGER
)
BEGIN

  DELETE FROM InternalPollVoteDetails WHERE InternalPollVoteDetails.InternalPollVoteId=internalPollVoteId;

END


#


CREATE PROCEDURE `ClearPayoutDependencies`(
  IN payoutId INTEGER
)
BEGIN

  DELETE FROM PayoutDependencies
    WHERE PayoutDependencies.PayoutId=payoutId;

END


#


CREATE PROCEDURE `CloseEvent`(
  IN eventId INTEGER
)
BEGIN

  UPDATE PWEvents SET PWEvents.Open=0,ProcessedDateTime=NOW()
    WHERE PWEvents.EventId=eventId;

END


#


CREATE PROCEDURE `CloseInternalPollVoter`(
  IN internalPollId INTEGER,
  IN personId INTEGER,
  IN closedDateTime DATETIME,
  IN ipAddress VARCHAR(48)
)
BEGIN

  UPDATE InternalPollVoters
    SET InternalPollVoters.Open=false,InternalPollVoters.ClosedDateTime=closedDateTime,InternalPollVoters.IPAddress=ipAddress
    WHERE InternalPollVoters.InternalPollId=internalPollId AND InternalPollVoters.PersonId=personId;

END


#


CREATE PROCEDURE `ClosePayrollAdjustment`(
  IN payrollAdjustmentId INTEGER,
  IN salaryId INTEGER
)
BEGIN

  UPDATE PayrollAdjustments SET open=0,PayrollAdjustments.SalaryId=salaryId
    WHERE PayrollAdjustments.PayrollAdjustmentId=payrollAdjustmentId;
  
END


#


CREATE PROCEDURE `CloseVolunteer`(
  volunteerId INTEGER,
  closedDateTime DATETIME,
  closingComments TEXT
)
BEGIN

  UPDATE Volunteers 
    SET Volunteers.Open=false,Volunteers.ClosedDateTime=closedDateTime,Volunteers.ClosingComments=closingComments 
    WHERE Volunteers.VolunteerId=volunteerId;

END


#


CREATE PROCEDURE `CloseVolunteerRole`(
  volunteerRoleId INTEGER,
  assigned BIT
)
BEGIN

  UPDATE VolunteerRoles 
    SET VolunteerRoles.Open=false,VolunteerRoles.Assigned=assigned 
    WHERE VolunteerRoles.VolunteerRoleId=volunteerRoleId;

END


#


CREATE PROCEDURE `CreateActivist`(
  IN personId INTEGER,
  IN dateTimeCreated DATETIME,
  IN public TINYINT(1),
  IN confirmed TINYINT(1)
)
BEGIN

  INSERT INTO Activists (PersonId, DateTimeCreated, DateTimeTerminated, Active, Public, Confirmed)
    VALUES (personId, dateTimeCreated, dateTimeCreated, 1, public, confirmed);

  SELECT LAST_INSERT_ID() AS Identity;

END


#


CREATE PROCEDURE `CreateBallot`(
  IN electionId INTEGER,
  IN name VARCHAR(128),
  IN organizationId INTEGER,
  IN geographyId INTEGER,
  IN ballotCount INTEGER,
  IN deliveryAddress TEXT
)
BEGIN

  INSERT INTO Ballots (ElectionId,Name,OrganizationId,GeographyId,BallotCount,DeliveryAddress)
    VALUES (electionId,name,organizationId,geographyId,ballotCount,deliveryAddress);

  SELECT LAST_INSERT_ID() AS Identity;

END


#


CREATE PROCEDURE `CreateBallotCandidate`(
  IN ballotId INTEGER,
  IN personId INTEGER
)
BEGIN
  
  DECLARE sequenceNumber INTEGER;

  SELECT COUNT(*)+1 FROM BallotCandidates WHERE BallotCandidates.BallotId=ballotId INTO sequenceNumber;  

  INSERT INTO BallotCandidates (BallotId,PersonId,SortOrder)
    VALUES (ballotId,personId,sequenceNumber);

END


#


CREATE PROCEDURE `CreateBlogRankingDate`(
  IN date DATETIME
)
BEGIN

  INSERT INTO BlogRankingDates (Date)
    VALUES (date);

  SELECT LAST_INSERT_ID() AS Identity;

END


#


CREATE PROCEDURE `CreateBlogRankingEntry`(
  IN blogRankingDateId INTEGER,
  IN mediaName VARCHAR(128),
  IN ranking INTEGER
)
BEGIN

  DECLARE mediaId INTEGER;

  IF ((SELECT COUNT(*) FROM Media WHERE Media.Name=mediaName) = 0)
  THEN
    INSERT INTO Media (Name, IsBlog) VALUES (mediaName,0);
    SELECT LAST_INSERT_ID() INTO mediaId;
  ELSE
    SELECT Media.MediaId INTO mediaId FROM Media WHERE Media.Name=mediaName;
  END IF;

  INSERT INTO BlogRankings (BlogRankingDateId,MediaId,Ranking)
    VALUES (blogRankingDateId,mediaId,ranking);

END


#


CREATE PROCEDURE `CreateCashAdvance`(
  personId INT,
  createdDateTime DATETIME,
  createdByPersonId INT,
  organizationId INT,
  amountCents BIGINT,
  financialAccountId INT,
  description VARCHAR(128)
)
BEGIN

  INSERT INTO CashAdvances(
      PersonId,CreatedByPersonId,CreatedDateTime,OrganizationId,FinancialAccountId,
      AmountCents,Description,Open,Attested,AttestedByPersonId,
      AttestedDateTime,PaidOut)
    VALUES (
        personId,createdByPersonId,createdDateTime,organizationid,financialAccountId,
        amountCents,description,1,0,0,'1970-01-01',0);

  SELECT LAST_INSERT_ID() AS Identity;

END


#


CREATE PROCEDURE `CreateCity`(
  IN cityName VARCHAR(256),
  IN countryId INTEGER,
  IN geographyId INTEGER,
  IN comment VARCHAR(256)
)
BEGIN

  INSERT INTO Cities (CityName,CountryId,GeographyId,Comment)
    VALUES (cityName,countryId,geographyId,comment);

  SELECT LAST_INSERT_ID() AS Identity;

END


#


CREATE PROCEDURE `CreateCommunicationTurnaround`(
  IN organizationId INTEGER,
  IN communicationTypeId INTEGER,
  IN communicationId INTEGER,
  IN dateTimeOpened DATETIME
)
BEGIN

  INSERT INTO CommunicationTurnarounds (OrganizationId,CommunicationTypeId,CommunicationId,DateTimeOpened,DateTimeFirstResponse,PersonIdFirstResponse,DateTimeClosed,PersonIdClosed,Open,Responded)
    VALUES (organizationId,communicationTypeId,communicationId,dateTimeOpened,'1970-01-01',0,'1970-01-01',0,1,0); 

END


#


CREATE PROCEDURE `CreateCountry`(
  IN name VARCHAR(256),
  IN code VARCHAR(16),
  IN culture VARCHAR(16),
  IN geographyId INTEGER,
  IN postalCodeLength INTEGER,
  IN collation VARCHAR(128)
)
BEGIN

  INSERT INTO Countries (Name,Code,Culture,GeographyId,PostalCodeLength,Collation)
    VALUES (name,code,culture,geographyId,postalCodeLength,collation);

  SELECT LAST_INSERT_ID() AS Identity;

END


#


CREATE PROCEDURE `CreateCurrency`(
  IN name VARCHAR(64),
  IN code VARCHAR(16),
  IN sign VARCHAR(8)
)
BEGIN

  INSERT INTO Currencies (Name,Code,Sign)
    VALUES (name,code,sign);

  SELECT LAST_INSERT_ID() AS Identity;

END


#


CREATE PROCEDURE `CreateDeposit`(
  organizationId INTEGER,
  dateTime DATETIME,
  payer VARCHAR(256),
  payerContact VARCHAR(128),
  payerIdentity VARCHAR(128)
)
BEGIN

  INSERT INTO Deposits (OrganizationId,DateTime,Payer,PayerContact,PayerIdentity)
    VALUES (organizationId,dateTime,payer,payerContact,payerIdentity);

  SELECT LAST_INSERT_ID() AS Identity;

END


#


CREATE PROCEDURE `CreateDepositDependency`(
  IN depositId INTEGER,
  IN financialDependencyType VARCHAR(64),
  IN foreignId INTEGER
)
BEGIN

  DECLARE financialDependencyTypeId INTEGER;

  SELECT 0 INTO financialDependencyTypeId;

  IF ((SELECT COUNT(*) FROM FinancialDependencyTypes WHERE FinancialDependencyTypes.Name=financialDependencyType) = 0)
  THEN
    INSERT INTO FinancialDependencyTypes (Name)
      VALUES (financialDependencyType);

    SELECT LAST_INSERT_ID() INTO financialDependencyTypeId;

  ELSE

    SELECT FinancialDependencyTypes.FinancialDependencyTypeId INTO FinancialDependencyTypeId FROM FinancialDependencyTypes
        WHERE FinancialDependencyTypes.Name=financialDependencyType;

  END IF;


  INSERT INTO DepositDependencies (DepositId,FinancialDependencyTypeId,ForeignId)
    VALUES (depositId,financialDependencyTypeId,foreignId);

END


#


CREATE PROCEDURE `CreateDocument`(
  IN serverFileName VARCHAR(128),
  IN clientFileName VARCHAR(128),
  IN description VARCHAR(128),
  IN docTypeString VARCHAR(64),
  IN foreignId INTEGER,
  IN fileSize BIGINT,
  IN uploadedByPersonId INTEGER,
  IN uploadedDateTime DATETIME
)
BEGIN

  DECLARE documentTypeId INTEGER;

  IF ((SELECT COUNT(*) FROM DocumentTypes WHERE DocumentTypes.Name=docTypeString) = 0)
  THEN
    INSERT INTO DocumentTypes (Name) VALUES (docTypeString);
    SELECT LAST_INSERT_ID() INTO documentTypeId;
  ELSE
    SELECT DocumentTypes.DocumentTypeId INTO documentTypeId FROM DocumentTypes WHERE DocumentTypes.Name = docTypeString;
  END IF;

  INSERT INTO Documents (ServerFileName,ClientFileName,Description,DocumentTypeId,ForeignId,FileSize,UploadedByPersonId,UploadedDateTime)
    VALUES (serverFileName,clientFileName,description,documentTypeId,foreignId,fileSize,uploadedByPersonId,uploadedDateTime);

  SELECT LAST_INSERT_ID() AS Identity;

END


#


CREATE PROCEDURE `CreateExceptionLogEntry`(
  IN exceptionDateTime DATETIME,
  IN source VARCHAR(64),
  IN exceptionText TEXT
)
BEGIN

  INSERT INTO ExceptionLog (ExceptionDateTime,Source,ExceptionText)
    VALUES (exceptionDateTime,source,exceptionText);

END


#


CREATE PROCEDURE `CreateExpenseClaim`(
  IN claimingPersonId INTEGER,
  IN createdDateTime DATETIME,
  IN organizationId INTEGER,
  IN budgetId INTEGER,
  IN expenseDate DATETIME,
  IN description VARCHAR(256),
  IN amount DOUBLE
)
BEGIN

  INSERT INTO ExpenseClaims (ClaimingPersonId,CreatedDateTime,Open,Attested,Validated,
    Claimed,OrganizationId,GeographyId,BudgetId,ExpenseDate,Description,
    PreApprovedAmount,Amount,AmountCents)

    VALUES (claimingPersonId, createdDateTime,1,0,0,1,organizationId,0,budgetId,
      expenseDate,description,0.0,amount,amount*100.0);

  SELECT LAST_INSERT_ID() AS Identity;

END


#


CREATE PROCEDURE `CreateExpenseClaimPrecise`(
  IN claimingPersonId INTEGER,
  IN createdDateTime DATETIME,
  IN organizationId INTEGER,
  IN budgetId INTEGER,
  IN expenseDate DATETIME,
  IN description VARCHAR(256),
  IN amountCents BIGINT
)
BEGIN

  INSERT INTO ExpenseClaims (ClaimingPersonId,CreatedDateTime,Open,Attested,Validated,
    Claimed,OrganizationId,GeographyId,BudgetId,ExpenseDate,Description,
    PreApprovedAmount,Amount,AmountCents)

    VALUES (claimingPersonId, createdDateTime,1,0,0,1,organizationId,0,budgetId,
      expenseDate,description,0.0,amountCents/100.0,amountCents);

  SELECT LAST_INSERT_ID() AS Identity;

END


#


CREATE PROCEDURE `CreateExternalActivity`(
  IN organizationId INTEGER,
  IN geographyId INTEGER,
  IN dateTime DATETIME,
  IN externalActivityType VARCHAR(64),
  IN description VARCHAR(256),
  IN createdByPersonId INTEGER,
  IN createdDateTime DATETIME
)
BEGIN

  DECLARE externalActivityTypeId INTEGER;

  IF ((SELECT COUNT(*) FROM ExternalActivityTypes WHERE ExternalActivityTypes.Name=externalActivityType) = 0)
  THEN
    INSERT INTO ExternalActivityTypes (Name) VALUES (externalActivityType);
    SELECT LAST_INSERT_ID() INTO externalActivityTypeId;
  ELSE
    SELECT ExternalActivityTypes.ExternalActivityTypeId 
      INTO externalActivityTypeId 
      FROM ExternalActivityTypes 
      WHERE ExternalActivityTypes.Name = externalActivityType;
  END IF;

  INSERT INTO ExternalActivities (OrganizationId,GeographyId,DateTime,ExternalActivityTypeId,Description,CreatedByPersonId,CreatedDateTime)
    VALUES (organizationId,geographyId,dateTime,externalActivityTypeId,description,createdByPersonId,createdDateTime);

  SELECT LAST_INSERT_ID() AS Identity;

END


#


CREATE PROCEDURE `CreateFinancialAccount`(
IN pOrganizationId INT UNSIGNED,
IN pName VARCHAR(64),
IN pAccountType INT UNSIGNED,
IN pParentFinancialAccountId INT UNSIGNED
)
BEGIN

  INSERT INTO `FinancialAccounts` (`OrganizationId`, `Name`, `AccountType`, `ParentFinancialAccountId`) 
    VALUES (pOrganizationId, pName, pAccountType, pParentFinancialAccountId);

  SELECT LAST_INSERT_ID() AS Identity;

END


#


CREATE PROCEDURE `CreateFinancialTransaction`(
  IN organizationId INTEGER,
  IN dateTime DATETIME,
  IN comment VARCHAR(128)
)
BEGIN

  INSERT INTO FinancialTransactions (OrganizationId, DateTime, Comment, ImportHash)
     VALUES (organizationId, dateTime, comment, '');

  SELECT LAST_INSERT_ID() AS Identity;

END


#


CREATE PROCEDURE `CreateFinancialTransactionRow`(
  IN financialTransactionId INTEGER,
  IN financialAccountId INTEGER,
  IN amount DOUBLE,
  IN dateTime DATETIME,
  IN personId INTEGER
)
BEGIN

  INSERT INTO FinancialTransactionRows (FinancialAccountId, FinancialTransactionId, Amount, AmountCents, CreatedDateTime, CreatedByPersonId)
    VALUES (financialAccountId, financialTransactionid, amount, amount*100, dateTime, personId);

END


#


CREATE PROCEDURE `CreateFinancialTransactionRowPrecise`(
  IN financialTransactionId INTEGER,
  IN financialAccountId INTEGER,
  IN amountCents BIGINT,
  IN dateTime DATETIME,
  IN personId INTEGER
)
BEGIN

  INSERT INTO FinancialTransactionRows (FinancialAccountId, FinancialTransactionId, Amount, AmountCents, CreatedDateTime, CreatedByPersonId)
    VALUES (financialAccountId, financialTransactionid, amountCents/100.0, amountCents, dateTime, personId);

END


#


CREATE PROCEDURE `CreateFinancialTransactionStub`(
  IN organizationId INTEGER,
  IN financialAccountId INTEGER,
  IN amountCents BIGINT,
  IN dateTime DATETIME,
  IN comment VARCHAR(128),
  IN importHash VARCHAR(32),
  IN personId INTEGER
)
BEGIN
  
  DECLARE financialTransactionId INTEGER;

  SELECT 0 INTO financialTransactionId;

  IF ((SELECT COUNT(*) FROM FinancialTransactions WHERE FinancialTransactions.ImportHash=importHash) = 0)
  THEN
    INSERT INTO FinancialTransactions (OrganizationId, DateTime, Comment, ImportHash)
      VALUES (organizationId, dateTime, comment, importHash);

    SELECT LAST_INSERT_ID() INTO financialTransactionId;

    INSERT INTO FinancialTransactionRows (FinancialAccountId, FinancialTransactionId, Amount, AmountCents, CreatedDateTime, CreatedByPersonId)
      VALUES (financialAccountId, financialTransactionId, amountCents/100.0, amountCents, DateTime, personId);

  END IF;

  SELECT financialTransactionId AS Identity;

END


#


CREATE PROCEDURE `CreateFinancialTransactionTag`(
    IN financialTransactionId INT,
    IN financialTransactionTagTypeId INT
)
BEGIN
    INSERT INTO FinancialTransactionTags(FinancialTransactionId,FinancialTransactionTagTypeId)
    VALUES (financialTransactionId,financialTransactionTagTypeId);

    SELECT LAST_INSERT_ID() as Identity;
END


#


CREATE PROCEDURE `CreateFinancialValidation`(
  IN validationType VARCHAR(64),
  IN dependencyType VARCHAR(64),
  IN foreignId INTEGER,
  IN validatedDateTime DATETIME,
  IN personId INTEGER,
  IN amount DOUBLE
)
BEGIN

  DECLARE dependencyTypeId INTEGER;
  DECLARE validationTypeId INTEGER;

  IF ((SELECT COUNT(*) FROM FinancialDependencyTypes WHERE FinancialDependencyTypes.Name=dependencyType) = 0)
  THEN
    INSERT INTO FinancialDependencyTypes (Name) VALUES (dependencyType);
    SELECT LAST_INSERT_ID() INTO dependencyTypeId;
  ELSE
    SELECT FinancialDependencyTypes.FinancialDependencyTypeId INTO dependencyTypeId FROM FinancialDependencyTypes WHERE FinancialDependencyTypes.Name = dependencyType;
  END IF;


  IF ((SELECT COUNT(*) FROM FinancialValidationTypes WHERE FinancialValidationTypes.Name=validationType) = 0)
  THEN
    INSERT INTO FinancialValidationTypes (Name) VALUES (validationType);
    SELECT LAST_INSERT_ID() INTO validationTypeId;
  ELSE
    SELECT FinancialValidationTypes.FinancialValidationTypeId INTO validationTypeId FROM FinancialValidationTypes WHERE FinancialValidationTypes.Name = validationType;
  END IF;

  
  INSERT INTO FinancialValidations (TypeId,FinancialDependencyTypeId,ForeignId,ValidatedDateTime,PersonId,Amount)
    VALUES (validationTypeId,dependencyTypeId,foreignId,validatedDateTime,personId,amount);

  
  SELECT LAST_INSERT_ID() AS Identity;

END


#


CREATE PROCEDURE `CreateGeography`(
  IN name VARCHAR(128),
  IN parentGeographyId INTEGER
)
BEGIN

  INSERT INTO Geographies (Name,ParentGeographyId)
    VALUES (name,parentGeographyId);

  SELECT LAST_INSERT_ID() AS Identity;

END


#


CREATE PROCEDURE `CreateGeographyUpdate`(
   geographyUpdateType VARCHAR(128),
   changeDataXml TEXT,
   createdDateTime DATETIME,
   effectiveDateTime DATETIME
)
BEGIN

  DECLARE geographyUpdateTypeId INTEGER;

  IF ((SELECT COUNT(*) FROM GeographyUpdateTypes WHERE GeographyUpdateTypes.Name=geographyUpdateType) = 0)
  THEN
    INSERT INTO GeographyUpdateType (Name) VALUES (geographyUpdateType);
    SELECT LAST_INSERT_ID() INTO geographyUpdateTypeId;
  ELSE
    SELECT GeographyUpdateType.GeographyUpdateTypeId INTO geographyUpdateTypeId FROM GeographyUpdateTypes
        WHERE GeographyUpdateType.Name=geographyUpdateType;
  END IF;

  INSERT INTO GeographyUpdates (GeographyUpdateTypeId,CreatedDateTime,EffectiveDateTime,ChangeDataXml,Processed)
     VALUES (geographyUpdateTypeId,createdDateTime,effectiveDateTime,changeDataXml,0);

  SELECT LAST_INSERT_ID() As Identity;

END


#


CREATE PROCEDURE `CreateInboundInvoice`(
  organizationId INTEGER,
  createdDateTime DATETIME,
  dueDate DATETIME,
  budgetId INTEGER,
  supplier VARCHAR(64),
  payToAccount VARCHAR(64),
  ocr VARCHAR(64),
  invoiceReference VARCHAR(64),
  amount DOUBLE,
  createdByPersonId INTEGER
)
BEGIN
	
  INSERT INTO InboundInvoices
    (OrganizationId, CreatedDateTime, DueDate, BudgetId, Supplier, Attested, Open, PayToAccount, Ocr,
     InvoiceReference, ClosedDateTime, Amount, AmountCents, CreatedByPersonId, ClosedByPersonId)
    VALUES
      (organizationId, createdDateTime, dueDate, budgetId, supplier, 0, 1, payToAccount, ocr,
       invoiceReference, createdDateTime, amount, amount*100, createdByPersonId, 0);

  SELECT LAST_INSERT_ID() AS Identity;

END


#


CREATE PROCEDURE `CreateInboundInvoicePrecise`(
  organizationId INTEGER,
  createdDateTime DATETIME,
  dueDate DATETIME,
  budgetId INTEGER,
  supplier VARCHAR(64),
  payToAccount VARCHAR(64),
  ocr VARCHAR(64),
  invoiceReference VARCHAR(64),
  amountCents BIGINT,
  createdByPersonId INTEGER
)
BEGIN
	
  INSERT INTO InboundInvoices
    (OrganizationId, CreatedDateTime, DueDate, BudgetId, Supplier, Attested, Open, PayToAccount, Ocr,
     InvoiceReference, ClosedDateTime, Amount, AmountCents, CreatedByPersonId, ClosedByPersonId)
    VALUES
      (organizationId, createdDateTime, dueDate, budgetId, supplier, 0, 1, payToAccount, ocr,
       invoiceReference, createdDateTime, amountCents/100.0, amountCents, createdByPersonId, 0);

  SELECT LAST_INSERT_ID() AS Identity;

END


#


CREATE PROCEDURE `CreateInternalPoll`(
  IN organizationId INTEGER,
  IN geographyId INTEGER,
  IN name VARCHAR(128),
  IN maxVoteLength INTEGER,
  IN resultsTypeId INTEGER,
  IN createdByPersonId INTEGER,
  IN runningOpens DATETIME,
  IN runningCloses DATETIME,
  IN votingOpens DATETIME,
  IN votingCloses DATETIME
)
BEGIN

  INSERT INTO InternalPolls 
   (OrganizationId, GeographyId, Name, MaxVoteLength, ResultsTypeId, 
    CreatedByPersonId, RunningOpen, VotingOpen, RunningOpens, RunningCloses,
    VotingOpens,VotingCloses)
    VALUES
     (organizationId, geographyId, name, maxVoteLength, resultsTypeId,
      createdByPersonId, 0, 0, runningOpens, runningCloses,
      votingOpens,votingCloses);

  SELECT LAST_INSERT_ID() AS Identity;

END


#


CREATE PROCEDURE `CreateInternalPollCandidate`(

  IN internalPollId INTEGER,
  IN personId INTEGER,
  IN candidacyStatement TEXT

)
BEGIN

  INSERT INTO InternalPollCandidates (InternalPollId, PersonId, CandidacyStatement)
    VALUES (internalPollId, personId, candidacyStatement);

  SELECT LAST_INSERT_ID() AS Identity;

END


#


CREATE PROCEDURE `CreateInternalPollVote`(
  IN internalPollId INTEGER,
  IN voteGeographyId INTEGER,
  IN verificationCode VARCHAR(128)
)
BEGIN

  INSERT INTO InternalPollVotes (InternalPollId,VoteGeographyId,VerificationCode)
    VALUES (internalPollId,voteGeographyId,verificationCode);

  SELECT LAST_INSERT_ID() AS Identity;

END


#


CREATE PROCEDURE `CreateInternalPollVoteDetail`(
  IN internalPollVoteId INTEGER,
  IN position INTEGER,
  IN internalPollCandidateId INTEGER
)
BEGIN

  INSERT INTO InternalPollVoteDetails (InternalPollVoteId,Position,InternalPollCandidateId)
    VALUES (internalPollVoteId,position,internalPollCandidateId);

END


#


CREATE PROCEDURE `CreateInternalPollVoter`(
  IN internalPollId INTEGER,
  IN personId INTEGER
)
BEGIN

  INSERT INTO InternalPollVoters (InternalPollId, PersonId, Open, ClosedDateTime, IPAddress)
    VALUES (internalPollId, personId, true, '1970-01-01', 'unused');

END


#


CREATE PROCEDURE `CreateMediaKeywordEntry`(
  IN mediaKeyword VARCHAR(64),
  IN mediaEntryUrl VARCHAR(512),
  IN mediaEntryDateTime DATETIME,
  IN mediaEntryTitle VARCHAR(256),
  IN mediaName VARCHAR(128),
  IN isBlog TINYINT(1)
)
BEGIN

  DECLARE mediaId INTEGER;
  DECLARE mediaKeywordId INTEGER;

  SELECT MediaKeywords.MediaKeywordId INTO mediaKeywordId from MediaKeywords where MediaKeywords.MediaKeyword=mediaKeyword;
  
  IF ((SELECT COUNT(*) FROM Media WHERE Media.Name=mediaName AND Media.IsBlog=isBlog ) = 0)
  THEN
    INSERT INTO Media (Name, IsBlog) VALUES (mediaName,isBlog);
    SELECT LAST_INSERT_ID() INTO mediaId;
  ELSE
    SELECT Media.MediaId INTO mediaId FROM Media 
        WHERE Media.Name=mediaName AND Media.IsBlog=isBlog 
        ORDER BY Media.MediaId LIMIT 1; 
  END IF;

  IF ((SELECT COUNT(*) FROM MediaKeywordEntries 
        WHERE MediaKeywordEntries.MediaEntryUrl=mediaEntryUrl 
          AND MediaKeywordEntries.MediaKeywordId=mediaKeywordId) = 0)
  THEN
    INSERT INTO MediaKeywordEntries (MediaId,MediaKeywordId,MediaEntryUrl,MediaEntryTitle,MediaEntryDateTime)
    VALUES (mediaId, mediaKeywordId, mediaEntryUrl, mediaEntryTitle, mediaEntryDateTime);

  SELECT LAST_INSERT_ID() AS Identity;
  ELSE
    SELECT 0 AS Identity;
  END IF;


END


#


CREATE PROCEDURE `CreateMembership`(

  IN personId INTEGER,
  IN organizationId INTEGER,
  IN expires DATETIME
)
BEGIN

  

  INSERT INTO Memberships (PersonId,OrganizationId,MemberSince,Active,Expires,DateTimeTerminated,TerminatedAsInvalid)
    VALUES (personId,organizationId,NOW(),1,expires,'1800-01-01',0);

  SELECT LAST_INSERT_ID() AS Identity;

END


#


CREATE PROCEDURE `CreateMotion`(
  IN meetingId INTEGER,
  IN submittingPersonId INTEGER,
  IN creatingPersonId INTEGER,
  IN createdDateTime DATETIME,
  IN title TEXT,
  IN text TEXT,
  IN decisionPoints TEXT
)
BEGIN

  DECLARE sequenceNumber INTEGER;

  SELECT COUNT(*)+1 FROM Motions WHERE Motions.MeetingId=meetingId INTO sequenceNumber;

  INSERT INTO Motions (MeetingId,SequenceNumber,Designation,Title,Text,DecisionPoints,
    AmendedText,AmendedDecisionPoints,SubmittedByPersonId,CreatedByPersonId,CreatedDateTime,
    Amended,AmendedByPersonId,AmendedDateTime,ThreadUrl,Open,Carried)

  VALUES (meetingId,sequenceNumber,'',title,text,decisionPoints,
    '','',submittedByPersonId,createdByPersonId,createdDateTime,
    0, createdByPersonId,createdDateTime,'',1,0);

  SELECT LAST_INSERT_ID() AS Identity;

END


#


CREATE PROCEDURE `CreateMotionAmendment`(
  IN motionId INTEGER,
  IN submittingPersonId INTEGER,
  IN createdByPersonId INTEGER,
  IN createdDateTime DATETIME,
  IN title TEXT,
  IN text TEXT,
  IN decisionPoint TEXT
)
BEGIN
  
  DECLARE sequenceNumber INTEGER;

  SELECT COUNT(*)+1 FROM MotionAmendments WHERE MotionAmendments.MotionId=motionId INTO sequenceNumber;

  INSERT INTO MotionAmendments (MotionId,SequenceNumber,Title,Text,
      DecisionPoint,SubmittedByPersonId,CreatedByPersonId,
      CreatedDateTime,Open,Carried)
    VALUES (motionId,sequenceNumber,title,text,
      decisionPoint,submittingPersonId,createdByPersonId,
      createdDateTime,1,0);

  SELECT LAST_INSERT_ID() AS Identity;

END


#


CREATE PROCEDURE `CreateOrganization`(
          IN pParentOrganizationId INT,
          IN pNameInternational VARCHAR(64),
          IN pName VARCHAR(64),
          IN pNameShort VARCHAR(64),
          IN pDomain VARCHAR(64),
          IN pMailPrefix VARCHAR(64),
          IN pAnchorGeographyId INT,
          IN pAcceptsMembers BOOLEAN,
          IN pAutoAssignNewMembers BOOLEAN,
          IN pDefaultCountryId INT
      )
BEGIN
        INSERT INTO `Organizations`
        (`ParentOrganizationId`,`NameInternational`,`Name`,`NameShort`,`Domain`,`MailPrefix`,
            `AnchorGeographyId`,`AcceptsMembers`,`AutoAssignNewMembers`,`DefaultCountryId`) 
        VALUES (pParentOrganizationId,pNameInternational,pName,pNameShort,pDomain,pMailPrefix,
            pAnchorGeographyId,pAcceptsMembers,pAutoAssignNewMembers,pDefaultCountryId);

        SELECT LAST_INSERT_ID() AS Identity;
END


#


CREATE PROCEDURE `CreateOrganizationCommTemplate`(
   organizationId INT,
   organizationCommTemplateType VARCHAR(128),
   updatedDateTime DATETIME,
   updatedByPersonId INT,
   templateData TEXT
)
BEGIN
  
  DECLARE organizationCommTemplateTypeId INTEGER;

  IF ((SELECT COUNT(*) FROM OrganizationCommTemplateTypes WHERE OrganizationCommTemplateTypes.Name=organizationCommTemplateType) = 0)
  THEN
    INSERT INTO OrganizationCommTemplateTypes (Name) VALUES (organizationCommTemplateType);
    SELECT LAST_INSERT_ID() INTO organizationCommTemplateTypeId;
  ELSE
    SELECT OrganizationCommTemplateTypes.OrganizationCommTemplateTypeId INTO organizationCommTemplateTypeId FROM OrganizationCommTemplateTypes
        WHERE OrganizationCommTemplateType.Name=organizationCommTemplateType;
  END IF;

  INSERT INTO OrganizationCommTemplates (OrganizationId,OrganizationCommTemplateTypeId,TemplateData,UpdatedByPersonId,UpdatedDateTime)
     VALUES (organizationId,organizationCommTemplateTypeId,templateData,updatedByPersonId,updatedDateTime);

  SELECT LAST_INSERT_ID() As Identity;

END


#


CREATE PROCEDURE `CreateOutboundComm`(
  IN senderPersonId INT,
  IN fromPersonId INT,
  IN organizationId INT,
  IN resolverClass VARCHAR(512),
  IN recipientDataXml TEXT,
  IN transmitterClass VARCHAR(512),
  IN payloadXml TEXT,
  IN priority INT,
  IN createdDateTime DATETIME
)
BEGIN

  DECLARE resolverClassId INT;
  DECLARE transmitterClassId INT;

  IF (CHAR_LENGTH(resolverClass) < 2)
  THEN
    SET resolverClassId = 0;
  ELSEIF ((SELECT COUNT(*) FROM ResolverClasses WHERE ResolverClasses.Name=resolverClass) = 0)
  THEN
    INSERT INTO ResolverClasses(Name) VALUES (resolverClass);
    SELECT LAST_INSERT_ID() INTO resolverClassId;
  ELSE
    SELECT ResolverClasses.ResolverClassId INTO resolverClassId FROM ResolverClasses WHERE ResolverClasses.Name = resolverClass;
  END IF;

  IF ((SELECT COUNT(*) FROM TransmitterClasses WHERE TransmitterClasses.Name=transmitterClass) = 0)
  THEN
    INSERT INTO TransmitterClasses(Name) VALUES (transmitterClass);
    SELECT LAST_INSERT_ID() INTO transmitterClassId;
  ELSE
    SELECT TransmitterClasses.TransmitterClassId INTO transmitterClassId FROM TransmitterClasses WHERE TransmitterClasses.Name = transmitterClass;
  END IF;

  INSERT INTO OutboundComms (SenderPersonId,FromPersonId,OrganizationId,ResolverClassId,RecipientDataXml,TransmitterClassId,PayloadXml,Priority,CreatedDateTime)
    VALUES (senderPersonId,fromPersonId,organizationId,resolverClassId,recipientDataXml,transmitterClassId,payloadXml,priority,createdDateTime);

  SELECT LAST_INSERT_ID() AS Identity;

END


#


CREATE PROCEDURE `CreateOutboundCommRecipient`(
  IN outboundCommId INT,
  IN personId INT
)
BEGIN

  INSERT INTO OutboundCommRecipients (OutboundCommId, PersonId)
    VALUES (outboundCommId, personId);

  SELECT LAST_INSERT_ID() AS Identity;

END


#


CREATE PROCEDURE `CreateOutboundInvoice`(
  organizationId INTEGER,
  createdDateTime DATETIME,
  createdByPersonId INTEGER,
  dueDate DATETIME,
  budgetId INTEGER,
  customerName VARCHAR(128),
  invoiceAddressPaper VARCHAR(256),
  invoiceAddressMail VARCHAR(128),
  currencyId INTEGER,
  reference VARCHAR(64),
  domestic TINYINT(1),
  securityCode VARCHAR(16),
  theirReference VARCHAR(64)
)
BEGIN

  INSERT INTO OutboundInvoices
    (CustomerName,InvoiceAddressPaper,InvoiceAddressMail,CurrencyId,
     BudgetId,OrganizationId,CreatedDateTime,CreatedByPersonId,
     DueDate,ReminderCount,Reference,Open,Domestic,Sent,SecurityCode,TheirReference)
  VALUES
    (customerName,invoiceAddressPaper,invoiceAddressMail,currencyId,
     budgetId,organizationId,createdDateTime,createdByPersonId,
     dueDate,0,reference,1,domestic,0,securityCode,theirReference);

  SELECT LAST_INSERT_ID() AS Identity;

END


#


CREATE PROCEDURE `CreateOutboundInvoiceItem`(
  outboundInvoiceId INTEGER,
  description VARCHAR(128),
  amount DOUBLE,
  sortOrder INTEGER
)
BEGIN

  INSERT INTO OutboundInvoiceItems (OutboundInvoiceId,Description,Amount,AmountCents,SortOrder)
  VALUES (outboundInvoiceId,description,amount,amount*100,sortOrder);

  SELECT LAST_INSERT_ID() AS Identity;

END


#


CREATE PROCEDURE `CreateOutboundInvoiceItemPrecise`(
  outboundInvoiceId INTEGER,
  description VARCHAR(128),
  amountCents BIGINT,
  sortOrder INTEGER
)
BEGIN

  INSERT INTO OutboundInvoiceItems (OutboundInvoiceId,Description,Amount,AmountCents,SortOrder)
  VALUES (outboundInvoiceId,description,amountCents/100.0,amountCents,sortOrder);

  SELECT LAST_INSERT_ID() AS Identity;

END


#


CREATE PROCEDURE `CreateOutboundMail`(
  IN authorType INTEGER,
  IN authorPersonId INTEGER,
	IN title TEXT,
	IN body LONGTEXT,
	IN mailPriority INTEGER,
  IN mailType INTEGER,
  IN geographyId INTEGER,
  IN organizationId INTEGER,
  IN createdDateTime DATETIME,
  IN releaseDateTime DATETIME
)
BEGIN

  INSERT INTO OutboundMails (AuthorType, AuthorPersonId, Title, Body, MailPriority,
    MailType, GeographyId, OrganizationId, CreatedDateTime, ReleaseDateTime)
	VALUES (authorType, authorPersonId, title, body, mailPriority,
    mailType, geographyId, organizationId, createdDateTime, releaseDateTime);


  SELECT LAST_INSERT_ID() AS Identity;

END


#


CREATE PROCEDURE `CreateOutboundMailRecipient`(
  IN outboundMailId INTEGER,
  IN personId INTEGER,
  IN asOfficer TINYINT(1),
  IN personType INTEGER
)
BEGIN

  INSERT INTO OutboundMailRecipients (OutboundMailId,PersonId,AsOfficer,PersonType)
    VALUES (outboundMailId,personId,asOfficer,personType);

  SELECT LAST_INSERT_ID() AS Identity;

END


#


CREATE PROCEDURE `CreatePaperLetter`(
  organizationId INTEGER,
  fromName VARCHAR(128),
  replyAddress VARCHAR(256),
  receivedDate DATETIME,
  toPersonId INTEGER,
  toPersonInRoleId INTEGER,
  personal BOOLEAN,
  uploadedByPersonId INTEGER,
  uploadedDateTime DATETIME
)
BEGIN

  INSERT INTO PaperLetters (OrganizationId, FromName, ReplyAddress, ReceivedDate, ToPersonId, ToPersonInRoleId, Personal,
      UploadedByPersonId, UploadedDateTime)
    VALUES (organizationId, fromName, replyAddress, receivedDate, toPersonId, toPersonInRoleId, personal,
        uploadedByPersonId, uploadedDateTime);

  SELECT LAST_INSERT_ID() AS Identity;

END


#


CREATE PROCEDURE `CreateParley`(
  organizationId INTEGER,
  personId INTEGER,
  budgetId INTEGER,
  name VARCHAR(256),
  geographyId INTEGER,
  description TEXT,
  informationUrl VARCHAR(256),
  startDate DATETIME,
  endDate DATETIME,
  budgetCents BIGINT,
  guaranteeCents BIGINT,
  attendanceFeeCents BIGINT,
  createdDateTime DATETIME
)
BEGIN

  INSERT INTO Parleys (OrganizationId, PersonId, BudgetId, Name,
    GeographyId, Description, InformationUrl, StartDate,
    EndDate, BudgetCents, GuaranteeCents, AttendanceFeeCents,
    CreatedDateTime, ClosedDateTime, Attested, Open)

    VALUES (organizationId, personId, budgetId, name,
      geographyId, description, informationUrl, startDate,
      endDate, budgetCents, guaranteeCents, attendanceFeeCents,
      createdDateTime, createdDateTime, 0, 1);

  SELECT LAST_INSERT_ID() AS Identity;

END


#


CREATE PROCEDURE `CreateParleyAttendee`(
  parleyId INTEGER,
  personId INTEGER,
  signupDateTime DATETIME,
  isGuest TINYINT(1)
)
BEGIN

  INSERT INTO ParleyAttendees (ParleyId,PersonId,SignupDateTime,
    Active,CancelDateTime,Invoiced,OutboundInvoiceId,
    IsGuest)

    VALUES (parleyId,personId,signupDateTime,
      isGuest,signupDateTime,0,0,isGuest);

  SELECT LAST_INSERT_ID() AS Identity;

END


#


CREATE PROCEDURE `CreateParleyOption`(
  parleyId INTEGER,
  description VARCHAR(512),
  amountCents BIGINT
)
BEGIN

  INSERT INTO ParleyOptions (ParleyId,Description,AmountCents,Active)
    VALUES (parleyId,description,amountCents,1);

  SELECT LAST_INSERT_ID() AS Identity;

END


#


CREATE PROCEDURE `CreatePayment`(
  paymentGroupId INTEGER,
  amount DOUBLE,
  reference VARCHAR(32),
  fromAccount VARCHAR(32),
  paymentKey VARCHAR(32),
  hasImage TINYINT(1),
  outboundInvoiceId INTEGER
)
BEGIN

  INSERT INTO Payments (PaymentGroupId,Amount,AmountCents,Reference,
    FromAccount,PaymentKey,HasImage,OutboundInvoiceId)

    VALUES (paymentGroupId,amount,amount*100,reference,fromAccount,
      paymentKey,hasImage,outboundInvoiceId);

  SELECT LAST_INSERT_ID() AS Identity;


END


#


CREATE PROCEDURE `CreatePaymentGroup`(
  organizationId INTEGER,
  dateTime DATETIME,
  currencyId INTEGER,
  createdDateTime DATETIME,
  createdByPersonId INTEGER
)
BEGIN

  INSERT INTO PaymentGroups (OrganizationId, CurrencyId,Amount,AmountCents,DateTime,
    Tag,CreatedDateTime,CreatedByPersonId,Open)

    VALUES (organizationId,currencyId,0.0,0,dateTime,
      '',createdDateTime,createdByPersonId,0);

  SELECT LAST_INSERT_ID() AS Identity;


END


#


CREATE PROCEDURE `CreatePaymentInformation`(
  IN paymentId INTEGER,
  IN dataTypeString VARCHAR(32),
  IN data VARCHAR(512)
)
BEGIN

  DECLARE paymentInformationTypeId INTEGER;

  IF ((SELECT COUNT(*) FROM PaymentInformationTypes WHERE PaymentInformationTypes.Name=dataTypeString) = 0)
  THEN
    INSERT INTO PaymentInformationTypes (Name) VALUES (dataTypeString);
    SELECT LAST_INSERT_ID() INTO paymentInformationTypeId;
  ELSE
    SELECT PaymentInformationTypes.PaymentInformationTypeId INTO paymentInformationTypeId FROM PaymentInformationTypes WHERE PaymentInformationTypes.Name = dataTypeString;
  END IF;

  INSERT INTO PaymentInformation (PaymentId,PaymentInformationTypeId,Data)
    VALUES (paymentId,paymentInformationTypeId,data);

  SELECT LAST_INSERT_ID() AS Identity;

END


#


CREATE PROCEDURE `CreatePaymentPrecise`(
  paymentGroupId INTEGER,
  amountCents BIGINT,
  reference VARCHAR(32),
  fromAccount VARCHAR(32),
  paymentKey VARCHAR(32),
  hasImage TINYINT(1),
  outboundInvoiceId INTEGER
)
BEGIN

  INSERT INTO Payments (PaymentGroupId,Amount,AmountCents,Reference,
    FromAccount,PaymentKey,HasImage,OutboundInvoiceId)

    VALUES (paymentGroupId,amountCents/100.0,amountCents,reference,fromAccount,
      paymentKey,hasImage,outboundInvoiceId);

  SELECT LAST_INSERT_ID() AS Identity;


END


#


CREATE PROCEDURE `CreatePayout`(
  organizationId INTEGER,
  bank VARCHAR(128),
  account VARCHAR(128),
  reference VARCHAR(256),
  amount DOUBLE,
  expectedTransactionDate DATETIME,
  createdDateTime DATETIME,
  createdByPersonId INTEGER
)
BEGIN

  INSERT INTO Payouts (OrganizationId, Bank, Account, Reference, Amount,
    AmountCents, ExpectedTransactionDate, Open, CreatedDateTime,
    CreatedByPersonId)

    VALUES (organizationId, bank, account, reference, amount, amount*100,
      expectedTransactionDate, true, createdDateTime, createdByPersonId);

  SELECT LAST_INSERT_ID() AS Identity;

END


#


CREATE PROCEDURE `CreatePayoutDependency`(
  IN payoutId INTEGER,
  IN financialDependencyType VARCHAR(64),
  IN foreignId INTEGER
)
BEGIN

  DECLARE financialDependencyTypeId INTEGER;

  SELECT 0 INTO financialDependencyTypeId;
  

  IF ((SELECT COUNT(*) FROM FinancialDependencyTypes WHERE FinancialDependencyTypes.Name=financialDependencyType) = 0)
  THEN
    INSERT INTO FinancialDependencyTypes (Name)
      VALUES (financialDependencyType);

    SELECT LAST_INSERT_ID() INTO financialDependencyTypeId;

  ELSE

    SELECT FinancialDependencyTypes.FinancialDependencyTypeId INTO FinancialDependencyTypeId FROM FinancialDependencyTypes
        WHERE FinancialDependencyTypes.Name=financialDependencyType;

  END IF;


  INSERT INTO PayoutDependencies (PayoutId,FinancialDependencyTypeId,ForeignId)
  VALUES (payoutId,financialDependencyTypeId,foreignId);

END


#


CREATE PROCEDURE `CreatePayoutPrecise`(
  organizationId INTEGER,
  bank VARCHAR(128),
  account VARCHAR(128),
  reference VARCHAR(256),
  amountCents BIGINT,
  expectedTransactionDate DATETIME,
  createdDateTime DATETIME,
  createdByPersonId INTEGER
)
BEGIN

  INSERT INTO Payouts (OrganizationId, Bank, Account, Reference, Amount,
    AmountCents, ExpectedTransactionDate, Open, CreatedDateTime,
    CreatedByPersonId)
  VALUES (organizationId, bank, account, reference, amountCents/100.0, amountCents,
    expectedTransactionDate, true, createdDateTime, createdByPersonId);

  SELECT LAST_INSERT_ID() AS Identity;


END


#


CREATE PROCEDURE `CreatePayrollAdjustment`(
  IN payrollItemId INTEGER,
  IN payrollAdjustmentTypeId INTEGER,
  IN amount DOUBLE,
  IN description VARCHAR(128)
)
BEGIN

  INSERT INTO PayrollAdjustments (PayrollItemId,PayrollAdjustmentTypeId,Amount,AmountCents,Description,Open,SalaryId)
  VALUES (payrollItemId,payrollAdjustmentTypeId,amount,amount*100,description,1,0);

  SELECT LAST_INSERT_ID() AS Identity;

END


#


CREATE PROCEDURE `CreatePayrollAdjustmentPrecise`(
  IN payrollItemId INTEGER,
  IN payrollAdjustmentTypeId INTEGER,
  IN amountCents BIGINT,
  IN description VARCHAR(128)
)
BEGIN

  INSERT INTO PayrollAdjustments (PayrollItemId,PayrollAdjustmentTypeId,Amount,AmountCents,Description,Open,SalaryId)
  VALUES (payrollItemId,payrollAdjustmentTypeId,amountCents/100.0,amountCents,description,1,0);

  SELECT LAST_INSERT_ID() AS Identity;

END


#


CREATE PROCEDURE `CreatePerson`(
  IN name VARCHAR(128),
  IN email VARCHAR(128),
  IN passwordHash VARCHAR(128),
  IN phoneNumber VARCHAR(48),
  IN street VARCHAR(128),
  IN postalCode VARCHAR(32),
  IN city VARCHAR(128),
  IN countryId INTEGER,
  IN birthdate DATETIME,
  IN genderId INTEGER
)
BEGIN

INSERT INTO People (Name,Email,PasswordHash,PhoneNumber,Street,PostalCode,City,CountryId,Birthdate,GenderId,GeographyId)
  VALUES (name,email,passwordHash,phoneNumber,street,postalCode,city,countryId,birthdate,genderId,0);

SELECT LAST_INSERT_ID() AS Identity;

END


#


CREATE PROCEDURE `CreatePersonRole`(
  IN personId INTEGER,
  IN personRoleType VARCHAR(64),
  IN organizationId INTEGER,
  IN geographyId INTEGER
)
BEGIN

  DECLARE personRoleTypeId INTEGER;

  SELECT 0 INTO personRoleTypeId;

  IF ((SELECT COUNT(*) FROM PersonRoleTypes WHERE PersonRoleTypes.Name=personRoleType) = 0)
  THEN
    INSERT INTO PersonRoleTypes (Name)
      VALUES (personRoleType);

    SELECT LAST_INSERT_ID() INTO personRoleTypeId;

  ELSE

    SELECT PersonRoleTypes.PersonRoleTypeId INTO personRoleTypeId FROM PersonRoleTypes
        WHERE PersonRoleTypes.Name=personRoleType;

  END IF;
  

  INSERT INTO PeopleRoles (personId, personRoleTypeId, organizationId, geographyId)
    VALUES (personId, personRoleTypeId, organizationId, geographyId);

  SELECT LAST_INSERT_ID() AS Identity;

END


#


CREATE PROCEDURE `CreatePostalCode`(
  IN postalCode VARCHAR(32),
  IN cityId  INTEGER,
  IN countryId INTEGER
)
BEGIN

  INSERT INTO PostalCodes (PostalCode,CityId,CountryId)
    VALUES (postalCode,cityId,countryId);

  SELECT LAST_INSERT_ID() AS Identity;

END


#


CREATE PROCEDURE `CreatePWEvent`(
  IN eventSource VARCHAR(128),
  IN eventType VARCHAR(128),
	IN processDateTime DATETIME,
	IN actingPersonId INTEGER,
	IN affectedPersonId INTEGER,
	IN organizationId INTEGER,
	IN geographyId INTEGER,
	IN parameterInt INTEGER,
	IN parameterText LONGTEXT
)
BEGIN

  DECLARE eventTypeId INTEGER;
  DECLARE eventSourceId INTEGER;

  SELECT 0 INTO eventTypeId;
  SELECT 0 INTO eventSourceId;

  IF ((SELECT COUNT(*) FROM PWEventTypes WHERE PWEventTypes.Name=eventType) = 0)
  THEN
    INSERT INTO PWEventTypes (Name)
      VALUES (eventType);

    SELECT LAST_INSERT_ID() INTO eventTypeId;

  ELSE

    SELECT PWEventTypes.EventTypeId INTO eventTypeId FROM PWEventTypes
        WHERE PWEventTypes.Name=eventType;

  END IF;


  IF ((SELECT COUNT(*) FROM PWEventSources WHERE PWEventSources.Name=eventSource) = 0)
  THEN
    INSERT INTO PWEventSources (Name)
      VALUES (eventSource);

    SELECT LAST_INSERT_ID() INTO eventSourceId;

  ELSE

    SELECT PWEventSources.EventSourceId INTO eventSourceId FROM PWEventSources
        WHERE PWEventSources.Name=eventSource;

  END IF;


  INSERT INTO PWEvents (CreatedDateTime, Open, ProcessDateTime, EventSourceId,
    EventTypeId, OrganizationId, GeographyId, ActingPersonId, AffectedPersonId,
    ParameterInt, ParameterText)

    VALUES (NOW(), 1, processDateTime, eventSourceId,
      eventTypeId, organizationId, geographyId, actingPersonId, affectedPersonId,
      parameterInt, parameterText);

  SELECT LAST_INSERT_ID() AS Identity;

END


#


CREATE PROCEDURE `CreatePWLogEntry`(
  IN dateTimeUtc DATETIME,
  IN actingPersonId INTEGER,
  IN affectedItemType VARCHAR(64),
  IN affectedItemId INTEGER,
  IN actionType VARCHAR(64),
  IN actionDescription VARCHAR(512),
  IN changedField VARCHAR(64),
  IN valueBefore VARCHAR(512),
  IN valueAfter VARCHAR(512),
  IN comment TEXT
)
BEGIN

  INSERT INTO PWLog (DateTimeUtc, ActingPersonId, AffectedItemType, AffectedItemId, ActionType, ActionDescription, ChangedField, ValueBefore, ValueAfter, Comment)
    VALUES (dateTimeUtc, actingPersonId, affectedItemType, affectedItemId, actionType, actionDescription, changedField, valueBefore, valueAfter, comment);

END


#


CREATE PROCEDURE `CreatePWLogEntry2`(
 IN dateTimeUtc DATETIME,
 IN actingPersonId INTEGER,
 IN affectedItemType VARCHAR(64),
 IN affectedItemId INTEGER,
 IN actionType VARCHAR(64),
 IN actionDescription VARCHAR(512),
 IN changedField VARCHAR(64),
 IN valueBefore VARCHAR(512),
 IN valueAfter VARCHAR(512),
 IN comment TEXT,
 IN ipAddress VARCHAR(45)
)
BEGIN

  INSERT INTO PWLog
    (DateTimeUtc, ActingPersonId, AffectedItemType, AffectedItemId,
    ActionType, ActionDescription, ChangedField, ValueBefore, ValueAfter,
    Comment, IpAddress)

    VALUES (dateTimeUtc, actingPersonId, affectedItemType, affectedItemId,
      actionType, actionDescription, changedField, valueBefore, valueAfter,
      comment, ipAddress);

END


#


CREATE PROCEDURE `CreateReporter`(
  IN name VARCHAR(128),
  IN email VARCHAR(128) 
)
BEGIN

  INSERT INTO Reporters (Name,Email)
    VALUES (name,email);

  SELECT LAST_INSERT_ID() AS Identity;

END


#


CREATE PROCEDURE `CreateReporterMediaCategory`(
  IN reporterId INTEGER,
  IN mediaCategoryName VARCHAR(128)
)
BEGIN

  DECLARE mediaCategoryId INTEGER;

  IF ((SELECT COUNT(*) FROM MediaCategories WHERE MediaCategories.Name=mediaCategoryName) = 0)
  THEN
    INSERT INTO MediaCategories (Name) VALUES (mediaCategoryName);
    SELECT LAST_INSERT_ID() INTO mediaCategoryId;
  ELSE
    SELECT MediaCategories.MediaCategoryId INTO mediaCategoryId FROM MediaCategories WHERE MediaCategories.Name=mediaCategoryName;
  END IF;

  INSERT INTO ReportersMediaCategories (ReporterId,MediaCategoryId)
  VALUES (reporterId,mediaCategoryId);

END


#


CREATE PROCEDURE `CreateSalary`(
  payrollItemId INTEGER,
  payoutDate DATETIME,
  baseSalary DOUBLE,
  netSalary DOUBLE,
  subtractiveTax DOUBLE,
  additiveTax DOUBLE
)
BEGIN

  INSERT INTO Salaries
     (PayrollItemId,PayoutDate,BaseSalary,BaseSalaryCents,NetSalary,
      NetSalaryCents, SubtractiveTax, SubtractiveTaxCents,
      AdditiveTax,AdditiveTaxCents,Attested,NetPaid,TaxPaid,Open)

    VALUES
      (payrollItemId,payoutDate,baseSalary,baseSalary*100,netSalary,
       netSalary*100,subtractiveTax,subtractiveTax*100,
       additiveTax,additiveTax*100,false,false,false,true);

  SELECT LAST_INSERT_ID() AS Identity;

END


#


CREATE PROCEDURE `CreateSalaryPrecise`(
  payrollItemId INTEGER,
  payoutDate DATETIME,
  baseSalaryCents BIGINT,
  netSalaryCents BIGINT,
  subtractiveTaxCents BIGINT,
  additiveTaxCents BIGINT
)
BEGIN

  INSERT INTO Salaries
    (PayrollItemId,PayoutDate,BaseSalary,BaseSalaryCents,NetSalary,
     NetSalaryCents,SubtractiveTax,SubtractiveTaxCents,
     AdditiveTax,AdditiveTaxCents,Attested,NetPaid,TaxPaid,Open)

    VALUES (payrollItemId,payoutDate,baseSalaryCents/100.0,baseSalaryCents,netSalaryCents/100.0,
      netSalaryCents,subtractiveTaxCents/100.0,subtractiveTaxCents,
      additiveTaxCents/100.0,additiveTaxCents,false,false,false,true);

  SELECT LAST_INSERT_ID() AS Identity;

END


#


CREATE PROCEDURE `CreateSwarmopsLogEntry`(
  IN entryType VARCHAR(128),
  IN entryXml TEXT,
  IN personId INT,
  IN dateTime DATETIME
)
BEGIN

  DECLARE entryTypeId INT;

  IF ((SELECT COUNT(*) FROM SwarmopsLogEntryTypes WHERE SwarmopsLogEntryTypes.Name=entryType) = 0)
  THEN
    INSERT INTO SwarmopsLogEntryTypes (Name) VALUES (entryType);
    SELECT LAST_INSERT_ID() INTO entryTypeId;
  ELSE
    SELECT SwarmopsLogEntryTypes.SwarmopsLogEntryTypeId INTO entryTypeId FROM SwarmopsLogEntryTypes WHERE SwarmopsLogEntryTypes.Name = entryType;
  END IF;

  INSERT INTO SwarmopsLog (DateTime,PersonId,EntryTypeId,EntryXml)
    VALUES (dateTime,personId,entryTypeId,entryXml);

  SELECT LAST_INSERT_ID() AS Identity;

END


#


CREATE PROCEDURE `CreateSwarmopsLogEntryAffectedObject`(
  swarmopsLogEntryId INT,
  affectedObjectType VARCHAR(128),
  affectedObjectId INT
)
BEGIN

  DECLARE affectedObjectTypeId INT;

  IF ((SELECT COUNT(*) FROM SwarmopsLogAffectedObjectTypes WHERE SwarmopsLogAffectedObjectTypes.Name=affectedObjectType) = 0)
  THEN
    INSERT INTO SwarmopsLogAffectedObjectTypes (Name) VALUES (affectedObjectType);
    SELECT LAST_INSERT_ID() INTO affectedObjectTypeId;
  ELSE
    SELECT SwarmopsLogAffectedObjectTypes.SwarmopsLogAffectedObjectTypeId INTO affectedObjectTypeId FROM SwarmopsLogAffectedObjectTypes WHERE SwarmopsLogAffectedObjectTypes.Name = affectedObjectType;
  END IF;

  INSERT INTO SwarmopsLogAffectedObjects (SwarmopsLogEntryId,AffectedObjectTypeId,AffectedObjectId)
    VALUES (swarmopsLogEntryId, affectedObjectTypeId, affectedObjectId);

END


#


CREATE PROCEDURE `CreateTaxLevel`(
  taxLevelId INTEGER,
  countryId INTEGER,
  bracketLow INTEGER,
  tax INTEGER
)
BEGIN

  INSERT INTO SalaryTaxLevels (TaxLevelId,CountryId,BracketLow,Tax)
    VALUES (taxLevelId,countryId,bracketLow,tax);

END


#


CREATE PROCEDURE `CreateTemporaryIdentity`()
BEGIN

  DECLARE temporaryIdentity INTEGER;

  INSERT INTO TemporaryIdentities (CreatedDateTime) VALUES (NOW());

  SELECT LAST_INSERT_ID() AS Identity;

END


#


CREATE PROCEDURE `CreateUptakeGeography`(
          IN p_OrganizationId INT,
          IN p_GeographyId INT
      )
BEGIN

    INSERT INTO `OrganizationUptakeGeographies` (`OrganizationId`, `GeographyId`) VALUES (p_OrganizationId, p_GeographyId);

END


#


CREATE PROCEDURE `CreateVolunteer`(
  IN personId INTEGER,
  IN ownerPersonId INTEGER,
  IN openedDateTime DATETIME
)
BEGIN

  INSERT INTO Volunteers (PersonId, OwnerPersonId, OpenedDateTime, Open, ClosedDateTime, ClosingComments)
     VALUES (personId, ownerPersonId, openedDateTime, true, '1970-01-01', '');

  SELECT LAST_INSERT_ID() AS Identity;

END


#


CREATE PROCEDURE `CreateVolunteerRole`(
  IN volunteerId INTEGER,
  IN organizationId INTEGER,
  IN geographyId INTEGER,
  IN roleTypeId INTEGER
)
BEGIN

  INSERT INTO VolunteerRoles (VolunteerId, OrganizationId, GeographyId, RoleTypeId, Open, Assigned)
     VALUES (volunteerId, organizationId, geographyId, roleTypeId, true, false);

  SELECT LAST_INSERT_ID() AS Identity;

END


#


CREATE PROCEDURE `DeleteFinancialTransactionTag`(
    IN financialTransactionTagId INT
)
BEGIN
    DELETE FROM FinancialTransactionTags WHERE FinancialTransactionTags.FinancialTransactionTagId=financialTransactionTagId;
END


#


CREATE PROCEDURE `DeleteFinancialTransactionTags`(
    IN financialTransactionId INT
)
BEGIN
    DELETE FROM FinancialTransactionTags WHERE FinancialTransactionTags.FinancialTransactionId=financialTransactionId;
END


#


CREATE PROCEDURE `DeleteOutboundMailRecipient`(
  IN outboundMailRecipientId INTEGER
)
BEGIN

  DELETE FROM OutboundMailRecipients 
    WHERE OutboundMailRecipients.OutboundMailRecipientId=outboundMailRecipientId;

END


#


CREATE PROCEDURE `DeletePersonNewsletterSubscriptions`(
  IN personId INTEGER
)
BEGIN

  DELETE FROM NewsletterSubscriptions WHERE NewsletterSubscriptions.PersonId=personId;

END


#


CREATE PROCEDURE `DeletePersonRole`(
  IN personRoleId INTEGER
)
BEGIN

  DELETE FROM PeopleRoles WHERE PeopleRoles.personRoleId=personRoleId;

END


#


CREATE PROCEDURE `DeleteReporter`(
  IN reporterId INTEGER
)
BEGIN

  DELETE FROM ReportersMediaCategories WHERE ReportersMediaCategories.ReporterId=reporterId;

  DELETE FROM Reporters WHERE Reporters.ReporterId=reporterId;

END


#


CREATE PROCEDURE `DeleteReporterMediaCategory`(
  IN pReporterId        INTEGER,
  IN pMediaCategoryName VARCHAR(128)
)
BEGIN

    DECLARE mediaCategoryId   INTEGER;

    IF ((SELECT
           COUNT(*)
         FROM
           MediaCategories
         WHERE
           MediaCategories.Name = mediaCategoryName) > 0)
    THEN
      SELECT
        MediaCategories.MediaCategoryId
      INTO
        mediaCategoryId
      FROM
        MediaCategories
      WHERE
        MediaCategories.Name = mediaCategoryName;

      DELETE FROM
        ReportersMediaCategories
      WHERE
        ReporterId = reporterId AND
        MediaCategoryId = mediaCategoryId;
 END IF;
END


#


CREATE PROCEDURE `DeleteTaxLevels`(
  countryId INTEGER
)
BEGIN

  DELETE FROM SalaryTaxLevels WHERE SalaryTaxLevels.CountryId=countryId;

END


#


CREATE PROCEDURE `DeleteUptakeGeography`(
  IN p_OrganizationId INT,
  IN p_GeographyId INT
)
BEGIN
    DELETE FROM OrganizationUptakeGeographies WHERE (`OrganizationId`=p_OrganizationId AND `GeographyId`=p_GeographyId );
    END


#


CREATE PROCEDURE `ImportMembership`(
  IN personId INTEGER,
  IN organizationId INTEGER,
  IN expires DATETIME,
  IN membersince DATETIME
)
BEGIN
  INSERT INTO Memberships (PersonId,OrganizationId,MemberSince,Active,Expires,DateTimeTerminated,TerminatedAsInvalid)
    VALUES (personId,organizationId,membersince,1,expires,'1800-01-01',0);

  SELECT LAST_INSERT_ID() AS Identity;

END


#


CREATE PROCEDURE `IncrementOutboundMailFailures`(
  IN outboundMailId INTEGER
)
BEGIN

  UPDATE OutboundMails SET OutboundMails.RecipientsFail = OutboundMails.RecipientsFail+1 WHERE OutboundMails.OutboundMailId=outboundMailId;

END


#


CREATE PROCEDURE `IncrementOutboundMailSuccesses`(
  IN outboundMailId INTEGER
)
BEGIN

  UPDATE OutboundMails SET OutboundMails.RecipientsSuccess = OutboundMails.RecipientsSuccess + 1 WHERE OutboundMails.OutboundMailId=outboundMailId;

END


#


CREATE PROCEDURE `LogChurnData`(
  IN p_personId INTEGER,
  IN p_organizationId INTEGER,
  IN p_churn TINYINT(1),
  IN p_decisionDateTime DATETIME,
  IN p_expiryDateTime DATETIME
)
BEGIN

  IF not exists(SELECT   *
                  FROM  `ChurnData`
                  WHERE ( `PersonId`  = p_personId
                        AND`OrganizationId`  = p_organizationId
                        AND`ExpiryDateTime`  = p_expiryDateTime))
  THEN
             INSERT INTO `ChurnData` (`PersonId`, `OrganizationId`, `Churn`, `DecisionDateTime`, `ExpiryDateTime`)
	              VALUES (p_personId, p_organizationId, p_churn, p_decisionDateTime, p_expiryDateTime);
  ELSE

             UPDATE  `ChurnData`
             SET `Churn` = p_churn,
                 `DecisionDateTime` = p_decisionDateTime
	           WHERE ( `PersonId`  = p_personId
               AND`OrganizationId`  = p_organizationId
               AND`ExpiryDateTime`  = p_expiryDateTime);
  END IF;

END


#


CREATE PROCEDURE `MovePayoutDependencies`(
  IN fromPayoutId INTEGER,
  IN toPayoutId INTEGER
)
BEGIN

  UPDATE PayoutDependencies SET PayoutDependencies.PayoutId = toPayoutId
    WHERE PayoutDependencies.PayoutId=fromPayoutId;

END


#


CREATE PROCEDURE `SetAutoMail`(
  IN autoMailType VARCHAR(64),
  IN organizationId INTEGER,
  IN geographyId INTEGER,
  IN authorPersonId INTEGER,
  IN title TEXT,
  IN body LONGTEXT
)
BEGIN

  DECLARE autoMailTypeId INTEGER;
  DECLARE autoMailId INTEGER;
  
  IF ((SELECT COUNT(*) FROM AutoMailTypes WHERE AutoMailTypes.Name=autoMailType) = 0)
  THEN
    INSERT INTO AutoMailTypes (Name) VALUES (autoMailType);
    SELECT LAST_INSERT_ID() INTO autoMailTypeId;
  ELSE
    SELECT AutoMailTypes.AutoMailTypeId INTO autoMailTypeId FROM AutoMailTypes WHERE AutoMailTypes.Name=autoMailType;
  END IF;

  
  IF ((SELECT COUNT(*) FROM AutoMails WHERE AutoMails.AutoMailTypeId=autoMailTypeId AND AutoMails.OrganizationId=organizationId AND AutoMails.GeographyId=geographyId) = 0)
  THEN
    INSERT INTO AutoMails (AutoMailTypeId,OrganizationId,GeographyId,AuthorPersonId,Title,Body,LastUpdate)
      VALUES (autoMailTypeId,organizationId,geographyId,authorPersonId,title,body,NOW());
    SELECT LAST_INSERT_ID() INTO autoMailId;
  ELSE
    UPDATE AutoMails SET AutoMails.AuthorPersonId=authorPersonId,AutoMails.Title=title,AutoMails.Body=body,AutoMails.LastUpdate=NOW() WHERE
      AutoMails.AutoMailTypeId=autoMailTypeId AND AutoMails.OrganizationId=organizationId AND AutoMails.GeographyId=geographyId;
    SELECT 0 INTO autoMailId;  
  END IF;

  SELECT autoMailId AS Identity;

END


#


CREATE PROCEDURE `SetBallotCount`(
  IN ballotId INTEGER,
  IN ballotCount INTEGER
)
BEGIN

  UPDATE Ballots SET Ballots.BallotCount = ballotCount
    WHERE Ballots.BallotId=ballotId;

END


#


CREATE PROCEDURE `SetBallotDeliveryAddress`(
  IN ballotId INTEGER,
  IN deliveryAddress TEXT
)
BEGIN

  UPDATE Ballots SET Ballots.DeliveryAddress = deliveryAddress
    WHERE Ballots.BallotId=ballotId;

END


#


CREATE PROCEDURE `SetCandidateDocumentationReceived`(
  IN electionId INTEGER,
  IN organizationId INTEGER,
  IN personId INTEGER,
  IN dateTimeReceived DATETIME
)
BEGIN

   INSERT INTO CandidateDocumentation (ElectionId,OrganizationId,PersonId,DocumentationReceived,DocumentationReceivedDateTime)
   VALUES (electionId,organizationId,personId,1,dateTimeReceived);  

END


#


CREATE PROCEDURE `SetCashAdvanceAttested`(
  IN cashAdvanceId INTEGER,
  IN attested BOOLEAN,
  IN attestedByPersonId INTEGER,
  IN attestedDateTime DATETIME
)
BEGIN

  UPDATE CashAdvances SET CashAdvances.Attested=attested, CashAdvances.AttestedByPersonId=attestedByPersonId,CashAdvances.AttestedDateTime=attestedDateTime
    WHERE CashAdvances.CashAdvanceId=cashAdvanceId;

END


#


CREATE PROCEDURE `SetCashAdvanceOpen`(
  IN cashAdvanceId INTEGER,
  IN open BOOLEAN
)
BEGIN

  UPDATE CashAdvances SET CashAdvances.Open=open
    WHERE CashAdvances.CashAdvanceId=cashAdvanceId;

END


#


CREATE PROCEDURE `SetCashAdvancePaidOut`(
  IN cashAdvanceId INTEGER,
  IN paidOut BOOLEAN
)
BEGIN

  UPDATE CashAdvances SET CashAdvances.PaidOut=paidOut
    WHERE CashAdvances.CashAdvanceId=cashAdvanceId;

END


#


CREATE PROCEDURE `SetCommunicationTurnaroundClosed`(
  IN organizationId INTEGER,
  IN communicationTypeId INTEGER,
  IN communicationId INTEGER,
  IN dateTimeClosed DATETIME,
  IN personIdClosed INTEGER
)
BEGIN

  UPDATE CommunicationTurnarounds 
    SET Open=0,
        DateTimeClosed=dateTimeClosed,
        PersonIdClosed=personIdClosed
    WHERE CommunicationTurnarounds.OrganizationId=organizationId AND CommunicationTurnarounds.CommunicationTypeId=communicationTypeId AND CommunicationTurnarounds.CommunicationId=communicationId; 

END


#


CREATE PROCEDURE `SetCommunicationTurnaroundResponded`(
  IN organizationId INTEGER,
  IN communicationTypeId INTEGER,
  IN communicationId INTEGER,
  IN dateTimeResponded DATETIME,
  IN personIdResponded INTEGER
)
BEGIN

  UPDATE CommunicationTurnarounds 
    SET Responded=1,
        DateTimeFirstResponse=dateTimeResponded,
        PersonIdFirstResponse=personIdResponded
    WHERE CommunicationTurnarounds.OrganizationId=organizationId AND CommunicationTurnarounds.CommunicationTypeId=communicationTypeId AND CommunicationTurnarounds.CommunicationId=communicationId; 

END


#


CREATE PROCEDURE `SetCountryGeographyId`(
  IN countryId INTEGER,
  IN geographyId INTEGER
)
BEGIN

  UPDATE Countries SET Countries.GeographyId=geographyId
    WHERE Countries.CountryId=countryId;

END


#


CREATE PROCEDURE `SetDocumentDescription`(
  IN documentId INTEGER,
  IN newDescription VARCHAR(128)
)
BEGIN

  UPDATE Documents
    SET Documents.Description=newDescription
    WHERE Documents.DocumentId=documentId;

END


#


CREATE PROCEDURE `SetDocumentForeignObject`(

  IN documentId INTEGER,
  IN newDocumentTypeString VARCHAR(64),
  IN newForeignId INTEGER

)
BEGIN

  DECLARE newDocumentTypeId INTEGER;

  IF ((SELECT COUNT(*) FROM DocumentTypes WHERE DocumentTypes.Name=newDocumentTypeString) = 0)
  THEN
    INSERT INTO DocumentTypes (Name) VALUES (newDocumentTypeString);
    SELECT LAST_INSERT_ID() INTO newDocumentTypeId;
  ELSE
    SELECT DocumentTypes.DocumentTypeId INTO newDocumentTypeId FROM DocumentTypes WHERE DocumentTypes.Name = newDocumentTypeString;
  END IF;

  UPDATE Documents
    SET Documents.DocumentTypeId=newDocumentTypeId,Documents.ForeignId=newForeignId
    WHERE Documents.DocumentId=documentId;

END


#


CREATE PROCEDURE `SetDocumentServerFileName`(
  IN documentId INTEGER,
  IN newServerFileName VARCHAR(128)
)
BEGIN

  UPDATE Documents SET Documents.ServerFileName=newServerFileName WHERE Documents.DocumentId=documentId;

END


#


CREATE PROCEDURE `SetExpenseClaimAmount`(
  IN expenseClaimId INTEGER,
  IN amount DOUBLE
)
BEGIN

 UPDATE ExpenseClaims SET ExpenseClaims.Amount=amount,ExpenseClaims.AmountCents=amount*100.0
    WHERE ExpenseClaims.ExpenseClaimId=expenseClaimId;

END


#


CREATE PROCEDURE `SetExpenseClaimAmountPrecise`(
  IN expenseClaimId INTEGER,
  IN amountCents BIGINT
)
BEGIN

 UPDATE ExpenseClaims SET ExpenseClaims.Amount=amountCents/100.0,ExpenseClaims.AmountCents=amountCents
    WHERE ExpenseClaims.ExpenseClaimId=expenseClaimId;

END


#


CREATE PROCEDURE `SetExpenseClaimAttested`(
  IN expenseClaimId INTEGER,
  IN attested BOOLEAN
)
BEGIN

  UPDATE ExpenseClaims SET ExpenseClaims.Attested=attested
    WHERE ExpenseClaims.ExpenseClaimId=expenseClaimId;

END


#


CREATE PROCEDURE `SetExpenseClaimBudget`(
  IN expenseClaimId INTEGER,
  IN budgetId INTEGER
)
BEGIN

  UPDATE ExpenseClaims SET ExpenseClaims.BudgetId=budgetId
    WHERE ExpenseClaims.ExpenseClaimId=expenseClaimId;

END


#


CREATE PROCEDURE `SetExpenseClaimBudgetYear`(
  IN expenseClaimId INTEGER,
  IN budgetYear INTEGER
)
BEGIN

  UPDATE ExpenseClaims SET ExpenseClaims.BudgetYear=budgetYear
    WHERE ExpenseClaims.ExpenseClaimId=expenseClaimId;

END


#


CREATE PROCEDURE `SetExpenseClaimClaimed`(
  IN expenseClaimId INTEGER,
  IN claimed TINYINT(1)
)
BEGIN

  UPDATE ExpenseClaims SET ExpenseClaims.Claimed=claimed
    WHERE ExpenseClaims.ExpenseClaimId=expenseClaimId;

END


#


CREATE PROCEDURE `SetExpenseClaimDate`(
  IN expenseClaimId INTEGER,
  IN expenseDate DATETIME
)
BEGIN

  UPDATE ExpenseClaims SET ExpenseClaims.ExpenseDate=expenseDate
    WHERE ExpenseClaims.ExpenseClaimId=expenseClaimId;

END


#


CREATE PROCEDURE `SetExpenseClaimDescription`(
  IN expenseClaimId INTEGER,
  IN description VARCHAR(256)
)
BEGIN

  UPDATE ExpenseClaims SET ExpenseClaims.Description=description
    WHERE ExpenseClaims.ExpenseClaimId=expenseClaimId;

END


#


CREATE PROCEDURE `SetExpenseClaimKeepSeparate`(
  IN expenseClaimId INTEGER,
  IN keepSeparate BOOLEAN
)
BEGIN

  UPDATE ExpenseClaims SET ExpenseClaims.KeepSeparate=keepSeparate
    WHERE ExpenseClaims.ExpenseClaimId=expenseClaimId;

END


#


CREATE PROCEDURE `SetExpenseClaimOpen`(
  IN expenseClaimId INTEGER,
  IN open BOOLEAN
)
BEGIN

  UPDATE ExpenseClaims SET ExpenseClaims.Open=open
    WHERE ExpenseClaims.ExpenseClaimId=expenseClaimId;

END


#


CREATE PROCEDURE `SetExpenseClaimRepaid`(
  IN expenseClaimId INTEGER,
  IN repaid BOOLEAN
)
BEGIN

  UPDATE ExpenseClaims SET ExpenseClaims.Repaid=repaid
    WHERE ExpenseClaims.ExpenseClaimId=expenseClaimId;

END


#


CREATE PROCEDURE `SetExpenseClaimValidated`(
  IN expenseClaimId INTEGER,
  IN validated BOOLEAN
)
BEGIN

  UPDATE ExpenseClaims SET ExpenseClaims.Validated=validated
    WHERE ExpenseClaims.ExpenseClaimId=expenseClaimId;

END


#


CREATE PROCEDURE `SetExternalIdentity`(
	IN pExternalIdentityIdentity INT ,
	IN pTypeOfAccount VARCHAR(50),
	IN pExternalSystem VARCHAR(50),
	IN pUserID VARCHAR(50),
	IN pPassword VARCHAR(50),
	IN pAttachedToPerson INT
	)
BEGIN

  DECLARE externalIdentityTypeId INTEGER;
  DECLARE newID INTEGER;

  IF ((SELECT COUNT(*) FROM ExternalIdentityTypes  WHERE ExternalIdentityTypes.ExternalIdentityTypeName=pTypeOfAccount) = 0) THEN
    INSERT INTO ExternalIdentityTypes (ExternalIdentityTypeName) VALUES (pTypeOfAccount);
    SELECT LAST_INSERT_ID() INTO externalIdentityTypeId;
  ELSE
    SELECT ExternalIdentityTypes.externalIdentityTypeId INTO externalIdentityTypeId
    FROM ExternalIdentityTypes WHERE ExternalIdentityTypes.ExternalIdentityTypeName = pTypeOfAccount;
  END IF;

  IF pExternalIdentityIdentity < 1 then
          INSERT INTO ExternalIdentities
                  (  TypeOfAccount, ExternalSystem, UserID, `Password`, AttachedToPerson)
          VALUES
                  ( externalIdentityTypeId, pExternalSystem, pUserID, pPassword, pAttachedToPerson);
          set newID= LAST_INSERT_ID();
  ELSE
          UPDATE ExternalIdentities
                  SET     TypeOfAccount =    externalIdentityTypeId,
                          ExternalSystem =   pExternalSystem,
                          UserID =	   pUserID,
                          `Password` =       pPassword,
                          AttachedToPerson = pAttachedToPerson
          WHERE ExternalIdentityIdentity=pExternalIdentityIdentity;
          set newid=pExternalIdentityIdentity;
  END if;

  SELECT ExternalIdentityIdentity, ExternalIdentityTypeName, ExternalSystem, UserID, `Password`, AttachedToPerson
        FROM ExternalIdentities inner join ExternalIdentityTypes on TypeOfAccount=externalIdentityTypeId WHERE  ExternalIdentityIdentity=newID;
END


#


CREATE PROCEDURE `SetFinancialAccountBudget`(
  IN financialAccountId INTEGER,
  IN year INTEGER,
  IN amount DOUBLE
)
BEGIN

  IF ((SELECT COUNT(*) FROM FinancialAccountBudgets WHERE FinancialAccountBudgets.FinancialAccountId=financialAccountId AND FinancialAccountBudgets.Year = year) = 0)
  THEN
    INSERT INTO FinancialAccountBudgets (Year,FinancialAccountId,Amount)
      VALUES (year, financialAccountId, amount);

  ELSE

    UPDATE FinancialAccountBudgets SET FinancialAccountBudgets.Amount = amount
      WHERE FinancialAccountBudgets.FinancialAccountId=financialAccountId AND FinancialAccountBudgets.Year=year;

  END IF;


END


#


CREATE PROCEDURE `SetFinancialAccountBudgetMonthly`(
  IN financialAccountId INTEGER,
  IN year INTEGER,
  IN month INTEGER,
  IN amountCents BIGINT
)
BEGIN

  IF ((SELECT COUNT(*) FROM FinancialAccountBudgetsMonthly WHERE FinancialAccountBudgetsMonthly.FinancialAccountId=financialAccountId AND FinancialAccountBudgetsMonthly.Year = year AND FinancialAccountBudgetsMonthly.Month = month) = 0)
  THEN
    INSERT INTO FinancialAccountBudgetsMonthly (FinancialAccountId,Year,Month,AmountCents)
      VALUES (financialAccountId,year,month,amountCents);

  ELSE

    UPDATE FinancialAccountBudgetsMonthly SET FinancialAccountBudgetsMonthly.AmountCents=amountCents
      WHERE FinancialAccountBudgetsMonthly.FinancialAccountId=financialAccountId AND FinancialAccountBudgetsMonthly.Year=year AND FinancialAccountBudgetsMonthly.Month=month;

  END IF;

END


#


CREATE PROCEDURE `SetFinancialAccountClosedYear`(
  IN financialAccountId INTEGER,
  IN closedYear INTEGER
)
BEGIN

  UPDATE FinancialAccounts SET FinancialAccounts.ClosedYear = closedYear
      WHERE FinancialAccounts.FinancialAccountId=financialAccountId;

END


#


CREATE PROCEDURE `SetFinancialAccountOpen`(
  IN financialAccountId INTEGER,
  IN open TINYINT
)
BEGIN

  UPDATE FinancialAccounts SET FinancialAccounts.Open = open
      WHERE FinancialAccounts.FinancialAccountId=financialAccountId;

END


#


CREATE PROCEDURE `SetFinancialAccountOpenedYear`(
  IN financialAccountId INTEGER,
  IN openedYear INTEGER
)
BEGIN

  UPDATE FinancialAccounts SET FinancialAccounts.OpenedYear = openedYear
      WHERE FinancialAccounts.FinancialAccountId=financialAccountId;

END


#


CREATE PROCEDURE `SetFinancialAccountName`(
  IN financialAccountId INTEGER,
  IN name VARCHAR(128)
)
BEGIN

  UPDATE FinancialAccounts SET FinancialAccounts.Name = name
      WHERE FinancialAccounts.FinancialAccountId=financialAccountId;

END


#


CREATE PROCEDURE `SetFinancialAccountOwner`(
  IN financialAccountId INTEGER,
  IN ownerPersonId INTEGER
)
BEGIN

  UPDATE FinancialAccounts SET FinancialAccounts.OwnerPersonId = ownerPersonId
      WHERE FinancialAccounts.FinancialAccountId=financialAccountId;

END


#


CREATE PROCEDURE `SetFinancialAccountParent`(
  IN financialAccountId INTEGER,
  IN parentFinancialAccountId INTEGER
)
BEGIN

  UPDATE FinancialAccounts SET FinancialAccounts.ParentFinancialAccountId = parentFinancialAccountId
      WHERE FinancialAccounts.FinancialAccountId=financialAccountId;

END


#


CREATE PROCEDURE `SetFinancialTransactionDependency`(
  IN financialTransactionId INTEGER,
  IN financialDependencyType VARCHAR(64),
  IN foreignId INTEGER
)
BEGIN

  DECLARE financialDependencyTypeId INTEGER;

  SELECT 0 INTO financialDependencyTypeId;

  IF ((SELECT COUNT(*) FROM FinancialDependencyTypes WHERE FinancialDependencyTypes.Name=financialDependencyType) = 0)
  THEN
    INSERT INTO FinancialDependencyTypes (Name)
      VALUES (financialDependencyType);

    SELECT LAST_INSERT_ID() INTO financialDependencyTypeId;

  ELSE

    SELECT FinancialDependencyTypes.FinancialDependencyTypeId INTO FinancialDependencyTypeId FROM FinancialDependencyTypes
        WHERE FinancialDependencyTypes.Name=financialDependencyType;

  END IF;

  
  IF ((SELECT COUNT(*) FROM FinancialTransactionDependencies WHERE FinancialTransactionDependencies.FinancialTransactionId=financialTransactionId) = 0)
  THEN

    INSERT INTO FinancialTransactionDependencies (FinancialTransactionId,FinancialDependencyTypeId,ForeignId)
    VALUES (financialTransactionId,financialDependencyTypeId,foreignId);

  ELSE

    UPDATE FinancialTransactionDependencies SET
      FinancialTransactionDependencies.ForeignId=foreignId,
      FinancialTransactionDependencies.FinancialDependencyTypeId=financialDependencyTypeId
      WHERE FinancialTransactionDependencies.FinancialTransactionId=financialTransactionId;

  END IF;

END


#


CREATE PROCEDURE `SetFinancialTransactionDescription`(
  IN financialTransactionId INTEGER,
  IN description VARCHAR(128)
)
BEGIN

  UPDATE FinancialTransactions SET FinancialTransactions.Comment=description
    WHERE FinancialTransactions.FinancialTransactionId = financialTransactionId;

END


#


CREATE PROCEDURE `SetGeographyUpdateProcessed`(
	geographyUpdateId int
)
BEGIN

UPDATE GeograpyUpdates
   SET GeographyUpdates.Processed=1
   WHERE GeographyUpdates.GeographyUpdateId=geographyUpdateId;

END


#


CREATE PROCEDURE `SetInboundInvoiceAmount`(
  IN inboundInvoiceId INTEGER,
  IN amount DOUBLE
)
BEGIN

  UPDATE InboundInvoices SET InboundInvoices.Amount=amount,InboundInvoices.AmountCents=amount*100
    WHERE InboundInvoices.InboundInvoiceId=inboundInvoiceId;

END


#


CREATE PROCEDURE `SetInboundInvoiceAmountPrecise`(
  IN inboundInvoiceId INTEGER,
  IN amountCents BIGINT
)
BEGIN

  UPDATE InboundInvoices SET InboundInvoices.Amount=amountCents/100.0,InboundInvoices.AmountCents=amountCents
    WHERE InboundInvoices.InboundInvoiceId=inboundInvoiceId;

END


#


CREATE PROCEDURE `SetInboundInvoiceAttested`(
  IN inboundInvoiceId INTEGER,
  IN attested BOOLEAN
)
BEGIN

  UPDATE InboundInvoices SET InboundInvoices.Attested=attested
    WHERE InboundInvoices.InboundInvoiceId=inboundInvoiceId;

END


#


CREATE PROCEDURE `SetInboundInvoiceBudget`(
  IN inboundInvoiceId INTEGER,
  IN budgetId INTEGER
)
BEGIN

  UPDATE InboundInvoices SET InboundInvoices.BudgetId=budgetId
    WHERE InboundInvoices.InboundInvoiceId=inboundInvoiceId;

END


#


CREATE PROCEDURE `SetInboundInvoiceDueDate`(
  IN inboundInvoiceId INTEGER,
  IN dueDate DATETIME
)
BEGIN

  UPDATE InboundInvoices SET InboundInvoices.DueDate=dueDate
    WHERE InboundInvoices.InboundInvoiceId=inboundInvoiceId;

END


#


CREATE PROCEDURE `SetInboundInvoiceOpen`(
  IN inboundInvoiceId INTEGER,
  IN open BOOLEAN
)
BEGIN

  UPDATE InboundInvoices SET InboundInvoices.Open=open
    WHERE InboundInvoices.InboundInvoiceId=inboundInvoiceId;

END


#


CREATE PROCEDURE `SetInternalPollCandidateSortOrder`(
  IN internalPollCandidateId INTEGER,
  IN sortOrder VARCHAR(64)
)
BEGIN

  UPDATE InternalPollCandidates SET InternalPollCandidates.SortOrder=sortOrder
    WHERE InternalPollCandidates.InternalPollCandidateId=internalPollCandidateId;

END


#


CREATE PROCEDURE `SetInternalPollCandidateStatement`(
  IN internalPollCandidateId INTEGER,
  IN candidacyStatement TEXT
)
BEGIN

  UPDATE InternalPollCandidates SET InternalPollCandidates.CandidacyStatement=candidacyStatement
    WHERE InternalPollCandidates.InternalPollCandidateId=internalPollCandidateId;

END


#


CREATE PROCEDURE `SetInternalPollRunningOpen`(
  IN internalPollId INTEGER,
  IN open BOOLEAN
)
BEGIN

  UPDATE InternalPolls SET InternalPolls.RunningOpen=open
    WHERE InternalPolls.InternalPollId=internalPollId;

END


#


CREATE PROCEDURE `SetInternalPollVotingOpen`(
  IN internalPollId INTEGER,
  IN open BOOLEAN
)
BEGIN

  UPDATE InternalPolls SET InternalPolls.VotingOpen=open
    WHERE InternalPolls.InternalPollId=internalPollId;

END


#


CREATE PROCEDURE `SetKeyValue`(
  IN dataKey VARCHAR(128),
  IN dataValue LONGTEXT
)
BEGIN

  IF ((SELECT COUNT(*) FROM FlatData WHERE FlatData.DataKey = dataKey) = 0)
  THEN
    INSERT INTO FlatData (DataKey,DataValue) VALUES (dataKey,dataValue);
  ELSE
    UPDATE FlatData SET FlatData.DataValue=dataValue WHERE FlatData.DataKey=dataKey;
  END IF;

END


#


CREATE PROCEDURE `SetMailTemplate`(
  IN p_templateId INT,
  IN p_templateName VARCHAR(50),
  IN p_languageCode VARCHAR(2),
  IN p_countryCode VARCHAR(2),
  IN p_organizationId INT,
  IN p_templateBody TEXT CHARACTER SET utf8
)
BEGIN

        DECLARE l_mailTemplateId INT;

        IF EXISTS(SELECT   *
                FROM  `MailTemplates`
                WHERE ( `TemplateId`  = p_templateId )) THEN
          BEGIN
              UPDATE `MailTemplates`
              SET `templateName`=p_templateName,`languageCode`=p_languageCode,`countryCode`=p_countryCode,
                `organizationId`=p_organizationId,`templateBody`=p_templateBody
              WHERE ( `templateId`  = p_templateId );
              SET l_mailTemplateId = p_templateId;
          END;
        ELSE 
            BEGIN
                INSERT INTO `MailTemplates`
                (`templateName`,`languageCode`,`countryCode`,`organizationId`,
                    `templateBody`) 
                VALUES (p_templateName,p_languageCode,p_countryCode,p_organizationId,
                    p_templateBody);
                SET l_mailTemplateId = LAST_INSERT_ID();
            END;
        END IF;
        SELECT l_mailTemplateId;
    END


#


CREATE PROCEDURE `SetMembershipExpires`(
  IN membershipId INTEGER,
  IN expires DATETIME
)
BEGIN

  UPDATE Memberships 
    SET Memberships.Expires=expires
    WHERE Memberships.MembershipId=membershipId;

  UPDATE Memberships 
    SET Memberships.Active=1,
      Memberships.DateTimeTerminated=19000101,
      Memberships.TerminatedAsInvalid=0
    WHERE Memberships.MembershipId=membershipId ;
    

END


#


CREATE PROCEDURE `SetMembershipPaymentStatus`(
  IN membershipId INTEGER,
  IN paymentStatusId INTEGER,
  IN statusDateTime DATETIME
)
BEGIN

  INSERT INTO MembershipPayments (MembershipId,MembershipPaymentStatusId,StatusDateTime)
    VALUES (membershipId,paymentStatusId,statusDateTime);

END


#


CREATE PROCEDURE `SetObjectOptionalData`(
  IN objectType VARCHAR(64),
  IN objectId INTEGER,
  IN objectOptionalDataType VARCHAR(64),
  IN data TEXT
)
BEGIN

  DECLARE objectTypeId INTEGER;
  DECLARE objectOptionalDataTypeId INTEGER;

  SELECT 0 INTO objectTypeId;
  SELECT 0 INTO objectOptionalDataTypeId;

  IF ((SELECT COUNT(*) FROM ObjectTypes WHERE ObjectTypes.Name=objectType) = 0)
  THEN
    INSERT INTO ObjectTypes (Name)
      VALUES (objectType);

    SELECT LAST_INSERT_ID() INTO objectTypeId;

  ELSE

    SELECT ObjectTypes.ObjectTypeId INTO objectTypeId FROM ObjectTypes
        WHERE ObjectTypes.Name=objectType;

  END IF;


  IF ((SELECT COUNT(*) FROM ObjectOptionalDataTypes WHERE ObjectOptionalDataTypes.Name=objectOptionalDataType) = 0)
  THEN
    INSERT INTO ObjectOptionalDataTypes (Name)
      VALUES (objectOptionalDataType);

    SELECT LAST_INSERT_ID() INTO objectOptionalDataTypeId;

  ELSE

    SELECT ObjectOptionalDataTypes.ObjectOptionalDataTypeId INTO objectOptionalDataTypeId FROM ObjectOptionalDataTypes
        WHERE ObjectOptionalDataTypes.Name=objectOptionalDataType;

  END IF;
  

  IF data IS NULL
  THEN
    DELETE FROM ObjectOptionalData
      WHERE ObjectOptionalData.ObjectId=objectId
        AND ObjectOptionalData.ObjectTypeId=objectTypeId
        AND ObjectOptionalData.ObjectOptionalDataTypeId=objectOptionalDataTypeId;
  ELSE
    IF ((SELECT COUNT(*) FROM ObjectOptionalData
        WHERE ObjectOptionalData.ObjectTypeId = objectTypeId
        AND ObjectOptionalData.ObjectOptionalDataTypeId=objectOptionalDataTypeId
        AND ObjectOptionalData.ObjectId=objectId) = 0)
    THEN

      INSERT INTO ObjectOptionalData (ObjectTypeId,ObjectId,ObjectOptionalDataTypeId,Data)
        VALUES (objectTypeId,objectId,objectOptionalDataTypeId,data);

    ELSE

      UPDATE ObjectOptionalData SET
        ObjectOptionalData.Data=data
        WHERE ObjectOptionalData.ObjectId=objectId
        AND ObjectOptionalData.ObjectTypeId=objectTypeId
        AND ObjectOptionalData.ObjectOptionalDataTypeId=objectOptionalDataTypeId;

    END IF;
  END IF;

END


#


CREATE PROCEDURE `SetOrganizationFinancialAccount`(
  IN organizationId INTEGER,
  IN organizationFinancialAccountTypeName VARCHAR(64),
  IN financialAccountId INTEGER
)
BEGIN

  DECLARE organizationFinancialAccountTypeId INTEGER;

  IF ((SELECT COUNT(*) FROM OrganizationFinancialAccountTypes WHERE OrganizationFinancialAccountTypes.Name=organizationFinancialAccountTypeName) = 0)
  THEN
    INSERT INTO OrganizationFinancialAccountTypes(Name) VALUES(organizationFinancialAccountTypeName);
    SELECT LAST_INSERT_ID() INTO organizationFinancialAccountTypeId;
  ELSE
    SELECT OrganizationFinancialAccountTypes.OrganizationFinancialAccountTypeId 
      INTO organizationFinancialAccountTypeId 
      FROM OrganizationFinancialAccountTypes
      WHERE OrganizationFinancialAccountTypes.Name=organizationFinancialAccountTypeName;
  END IF;


  IF ((SELECT COUNT(*) FROM OrganizationFinancialAccounts 
    WHERE OrganizationFinancialAccounts.OrganizationId=organizationId 
          AND OrganizationFinancialAccounts.OrganizationFinancialAccountTypeId=
              organizationFinancialAccountTypeId) = 0)
  THEN
    INSERT INTO OrganizationFinancialAccounts (OrganizationId,OrganizationFinancialAccountTypeId,FinancialAccountId)
      VALUES (organizationId, organizationFinancialAccountTypeId, financialAccountId);

  ELSE

    UPDATE OrganizationFinancialAccounts 
      SET OrganizationFinancialAccounts.FinancialAccountId=financialAccountId
      WHERE OrganizationFinancialAccounts.OrganizationId=organizationId
        AND OrganizationFinancialAccounts.OrganizationFinancialAccountTypeId=organizationFinancialAccountTypeId;

  END IF;

END


#


CREATE PROCEDURE `SetOutboundCommClosed`(
  IN outboundCommId INTEGER,
  IN dateTime DATETIME
)
BEGIN

  DECLARE recipientSuccessCount INT;
  DECLARE recipientFailCount INT;

  SELECT (SELECT COUNT(*) FROM OutboundCommRecipients WHERE OutboundCommRecipients.OutboundCommId=outboundCommId AND OutboundCommRecipients.Success=1) INTO recipientSuccessCount;
  SELECT (SELECT COUNT(*) FROM OutboundCommRecipients WHERE OutboundCommRecipients.OutboundCommId=outboundCommId AND OutboundCommRecipients.Success=0) INTO recipientFailCount;

  UPDATE OutboundComms SET OutboundComms.Open=0, OutboundComms.RecipientsFail=recipientFailCount, OutboundComms.RecipientsSuccess=recipientSuccessCount, OutboundComms.ClosedDateTime=dateTime WHERE OutboundComms.OutboundCommId=outboundCommId;

END


#


CREATE PROCEDURE `SetOutboundCommRecipientClosed`(
  IN outboundCommRecipientId INTEGER,
  IN dateTime DATETIME
)
BEGIN

  UPDATE OutboundCommRecipients SET OutboundCommRecipients.Open=0, OutboundCommRecipients.Success=1 WHERE OutboundCommRecipients.OutboundCommRecipientId=outboundCommRecipientId;

END


#


CREATE PROCEDURE `SetOutboundCommRecipientFailed`(
  IN outboundCommRecipientId INTEGER,
  IN dateTime DATETIME,
  IN failReason TEXT
)
BEGIN

  DECLARE failReasonId INT;


  IF ((SELECT COUNT(*) FROM FailReasons WHERE FailReasons.Name=failReason) = 0)
  THEN
    INSERT INTO FailReasons(Name) VALUES (failReason);
    SELECT LAST_INSERT_ID() INTO failReasonId;
  ELSE
    SELECT FailReasons.FailReasonId INTO failReasonId FROM FailReasons WHERE FailReasons.Name = failReason;
  END IF;

  UPDATE OutboundCommRecipients SET OutboundCommRecipients.Open=0, OutboundCommRecipients.Success=0, OutboundCommRecipients.FailReasonId=failReasonId WHERE OutboundCommRecipients.OutboundCommRecipientId=outboundCommRecipientId;

END


#


CREATE PROCEDURE `SetOutboundCommResolved`(
  IN outboundCommId INTEGER,
  IN dateTime DATETIME
)
BEGIN

  DECLARE recipientCount INT;

  SELECT (SELECT COUNT(*) FROM OutboundCommRecipients WHERE OutboundCommRecipients.OutboundCommId=outboundCommId) INTO recipientCount;

  UPDATE OutboundComms SET OutboundComms.Resolved=1, OutboundComms.RecipientCount=recipientCount, OutboundComms.ResolvedDateTime=dateTime WHERE OutboundComms.OutboundCommId=outboundCommId;

END


#


CREATE PROCEDURE `SetOutboundCommTransmissionStart`(
  IN outboundCommId INTEGER,
  IN dateTime DATETIME
)
BEGIN

  UPDATE OutboundComms SET OutboundComms.StartTransmitDateTime=dateTime WHERE OutboundComms.OutboundCommId=outboundCommId;

END


#


CREATE PROCEDURE `SetOutboundInvoiceBudget`(
  IN outboundInvoiceId INTEGER,
  IN budgetId INTEGER
)
BEGIN

  UPDATE OutboundInvoices SET OutboundInvoices.BudgetId=budgetId
    WHERE OutboundInvoices.OutboundInvoiceId=outboundInvoiceId;

END


#


CREATE PROCEDURE `SetOutboundInvoiceOpen`(
  IN outboundInvoiceId INTEGER,
  IN open BOOLEAN
)
BEGIN

  UPDATE OutboundInvoices SET OutboundInvoices.Open=open
    WHERE OutboundInvoices.OutboundInvoiceId=outboundInvoiceId;

END


#


CREATE PROCEDURE `SetOutboundInvoiceReference`(
  IN outboundInvoiceId INTEGER,
  IN reference VARCHAR(128)
)
BEGIN

  UPDATE OutboundInvoices SET OutboundInvoices.Reference=reference
    WHERE OutboundInvoices.OutboundInvoiceId=outboundInvoiceId;

END


#


CREATE PROCEDURE `SetOutboundInvoiceSent`(
  IN outboundInvoiceId INTEGER,
  IN sent BOOLEAN
)
BEGIN

  UPDATE OutboundInvoices SET OutboundInvoices.Sent=sent
    WHERE OutboundInvoices.OutboundInvoiceId=outboundInvoiceId;

END


#


CREATE PROCEDURE `SetOutboundMailProcessed`(
  IN outboundMailId INTEGER,
  IN dateTime DATETIME
)
BEGIN

  UPDATE OutboundMails SET OutboundMails.Processed=1, OutboundMails.EndProcessDateTime=dateTime WHERE OutboundMails.OutboundMailId=outboundMailId;

END


#


CREATE PROCEDURE `SetOutboundMailReadyForPickup`(
  IN outboundMailId INTEGER
)
BEGIN

  UPDATE OutboundMails SET OutboundMails.ReadyForPickup=1 WHERE OutboundMails.OutboundMailId=outboundMailId;

END


#


CREATE PROCEDURE `SetOutboundMailRecipientCount`(
  IN outboundMailId INTEGER,
  IN recipientCount INTEGER
)
BEGIN

  UPDATE OutboundMails SET OutboundMails.RecipientCount=recipientCount WHERE OutboundMails.OutboundMailId=outboundMailId;

END


#


CREATE PROCEDURE `SetOutboundMailResolved`(
  IN outboundMailId INTEGER,
  IN dateTime DATETIME
)
BEGIN

  UPDATE OutboundMails SET OutboundMails.Resolved=1, OutboundMails.ResolvedDateTime=dateTime WHERE OutboundMails.OutboundMailId=outboundMailId;

END


#


CREATE PROCEDURE `SetOutboundMailStartProcess`(
  IN outboundMailId INTEGER,
  IN dateTime DATETIME
)
BEGIN

  UPDATE OutboundMails SET OutboundMails.StartProcessDateTime=dateTime WHERE OutboundMails.OutboundMailId=outboundMailId;

END


#


CREATE PROCEDURE `SetParleyAttendeeActive`(
  IN parleyAttendeeId INTEGER,
  IN active TINYINT(1),
  IN cancelDateTime DATETIME
)
BEGIN

  UPDATE ParleyAttendees 
    SET ParleyAttendees.Active=active,
        ParleyAttendees.CancelDateTime=cancelDateTime
    WHERE ParleyAttendees.ParleyAttendeeId=parleyAttendeeId;

END


#


CREATE PROCEDURE `SetParleyAttendeeInvoiced`(
  IN parleyAttendeeId INTEGER,
  IN invoiced TINYINT(1),
  IN outboundInvoiceId INTEGER
)
BEGIN

 UPDATE ParleyAttendees 
    SET ParleyAttendees.Invoiced=invoiced,
        ParleyAttendees.OutboundInvoiceId=outboundInvoiceId
    WHERE ParleyAttendees.ParleyAttendeeId=parleyAttendeeId;

END


#


CREATE PROCEDURE `SetParleyAttested`(
  IN parleyId INTEGER,
  IN attested BOOLEAN
)
BEGIN

  UPDATE Parleys SET Parleys.Attested=attested
    WHERE Parleys.ParleyId=parleyId;

END


#


CREATE PROCEDURE `SetParleyBudget`(
  IN parleyId INTEGER,
  IN budgetId INTEGER
)
BEGIN

  UPDATE Parleys SET Parleys.BudgetId=budgetId
    WHERE Parleys.ParleyId=parleyId;

END


#


CREATE PROCEDURE `SetParleyOpen`(
  IN parleyId INTEGER,
  IN open TINYINT(1),
  IN closedDateTime DATETIME
)
BEGIN

  UPDATE Parleys 
    SET Parleys.Open=open,
        Parleys.ClosedDateTime=closedDateTime
    WHERE Parleys.ParleyId=parleyId;

END


#


CREATE PROCEDURE `SetParleyOptionActive`(
  IN parleyOptionId INTEGER,
  IN active TINYINT(1)
)
BEGIN

  UPDATE ParleyOptions 
    SET ParleyOptions.Active=active
    WHERE ParleyOptions.ParleyOptionId=parleyOptionId;

END


#


CREATE PROCEDURE `SetPaymentGroupAmount`(
  IN paymentGroupId INTEGER,
  IN amount DOUBLE
)
BEGIN

  UPDATE PaymentGroups SET PaymentGroups.Amount=amount,PaymentGroups.AmountCents=amount*100
    WHERE PaymentGroups.PaymentGroupId=paymentGroupId;

END


#


CREATE PROCEDURE `SetPaymentGroupAmountPrecise`(
  IN paymentGroupId INTEGER,
  IN amountCents BIGINT
)
BEGIN

  UPDATE PaymentGroups SET PaymentGroups.Amount=amountCents/100.0,PaymentGroups.AmountCents=amountCents
    WHERE PaymentGroups.PaymentGroupId=paymentGroupId;

END


#


CREATE PROCEDURE `SetPaymentGroupOpen`(
  IN paymentGroupId INTEGER,
  IN open TINYINT(1)
)
BEGIN

  UPDATE PaymentGroups SET PaymentGroups.Open=open
    WHERE PaymentGroups.PaymentGroupId=paymentGroupId;

END


#


CREATE PROCEDURE `SetPaymentGroupTag`(
  IN paymentGroupId INTEGER,
  IN tag VARCHAR(32)
)
BEGIN

  UPDATE PaymentGroups SET PaymentGroups.Tag = tag
    WHERE PaymentGroups.PaymentGroupId=paymentGroupId;

END


#


CREATE PROCEDURE `SetPayoutAmount`(
  IN payoutId INTEGER,
  IN amount DOUBLE
)
BEGIN

  UPDATE Payouts SET Payouts.Amount=amount, Payouts.AmountCents=amount*100
    WHERE Payouts.PayoutId=payoutId;

END


#


CREATE PROCEDURE `SetPayoutAmountPrecise`(
  IN payoutId INTEGER,
  IN amountCents BIGINT
)
BEGIN

  UPDATE Payouts SET Payouts.Amount=amountCents/100.0, Payouts.AmountCents=amountCents
    WHERE Payouts.PayoutId=payoutId;

END


#


CREATE PROCEDURE `SetPayoutOpen`(
  IN payoutId INTEGER,
  IN open BOOLEAN
)
BEGIN

  UPDATE Payouts SET Payouts.Open=open
    WHERE Payouts.PayoutId=payoutId;

END


#


CREATE PROCEDURE `SetPayoutReference`(
  IN payoutId INTEGER,
  IN reference VARCHAR(256)
)
BEGIN

  UPDATE Payouts SET Payouts.Reference=reference
    WHERE Payouts.PayoutId=payoutId;

END


#


CREATE PROCEDURE `SetPersonBirthdate`(
  IN personId INTEGER,
  IN birthdate DATETIME
)
BEGIN

  UPDATE People SET People.Birthdate=birthdate
    WHERE People.PersonId=personId;

END


#


CREATE PROCEDURE `SetPersonCity`(
  IN personId INTEGER,
  IN city VARCHAR(128)
)
BEGIN

  UPDATE People SET People.City=city
    WHERE People.PersonId=personId;

END


#


CREATE PROCEDURE `SetPersonCountry`(
  IN personId INTEGER,
  IN countryId INTEGER
)
BEGIN

  UPDATE People SET People.CountryId=countryId
    WHERE People.PersonId=personId;

END


#


CREATE PROCEDURE `SetPersonEmail`(
  IN personId INTEGER,
  IN email VARCHAR(128)
)
BEGIN

  UPDATE People SET People.Email=email
    WHERE People.PersonId=personId;

END


#


CREATE PROCEDURE `SetPersonGender`(
  IN personId INTEGER,
  IN genderId INTEGER
)
BEGIN

  UPDATE People SET People.GenderId=genderId
    WHERE People.PersonId=personId;

END


#


CREATE PROCEDURE `SetPersonGeography`(
  IN personId INTEGER,
  IN geographyId INTEGER
)
BEGIN

  UPDATE People SET People.GeographyId=geographyId
    WHERE People.PersonId=personId;

END


#


CREATE PROCEDURE `SetPersonName`(
  IN personId INTEGER,
  IN name VARCHAR(128)
)
BEGIN

  UPDATE People SET People.Name=name
    WHERE People.PersonId=personId;

END


#


CREATE PROCEDURE `SetPersonNewsletterSubscription`(
IN personId INTEGER, 
IN newsletterFeedId INTEGER,
IN subscribed TINYINT(1)                                   
  )
BEGIN
    DECLARE isDefault TINYINT(1);

    SELECT DefaultSubscribed
    FROM   PirateWeb.NewsletterFeeds
    WHERE  NewsletterFeeds.NewsletterFeedId = newsletterFeedId
    INTO   isDefault;

    IF (isDefault = subscribed)
    THEN
      DELETE FROM NewsletterSubscriptions
      WHERE       NewsletterSubscriptions.PersonId = personId AND NewsletterSubscriptions.NewsletterFeedId = newsletterFeedId;
    ELSE
      IF ((SELECT COUNT(*)
           FROM   NewsletterSubscriptions
           WHERE  NewsletterSubscriptions.PersonId = personId AND NewsletterSubscriptions.NewsletterFeedId = newsletterFeedId) = 0)
      THEN
        INSERT INTO NewsletterSubscriptions(PersonId, NewsletterFeedId, Subscribed)
        VALUES      (personId, newsletterFeedId, subscribed);
      ELSE
        UPDATE NewsletterSubscriptions
        SET    NewsletterSubscriptions.Subscribed = subscribed
        WHERE  NewsletterSubscriptions.PersonId = personId AND NewsletterSubscriptions.NewsletterFeedId = newsletterFeedId;
      END IF;
    END IF;
  END


#


CREATE PROCEDURE `SetPersonPasswordHash`(
  IN personId INTEGER,
  IN passwordHash VARCHAR(128)
)
BEGIN

  UPDATE People SET People.PasswordHash=passwordHash
    WHERE People.PersonId=personId;

END


#


CREATE PROCEDURE `SetPersonPhoneNumber`(
  IN personId INTEGER,
  IN phoneNumber VARCHAR(48)
)
BEGIN

  UPDATE People SET People.PhoneNumber=phoneNumber
    WHERE People.PersonId=personId;

END


#


CREATE PROCEDURE `SetPersonPostalCode`(
  IN personId INTEGER,
  IN postalCode VARCHAR(32)
)
BEGIN

  UPDATE People SET People.PostalCode=postalCode
    WHERE People.PersonId=personId;

END


#


CREATE PROCEDURE `SetPersonStreet`(
  IN personId INTEGER,
  IN street VARCHAR(128)
)
BEGIN

  UPDATE People SET People.Street=street
    WHERE People.PersonId=personId;

END


#


CREATE PROCEDURE `SetSalaryAttested`(
  IN salaryId INTEGER,
  IN attested BOOLEAN
)
BEGIN

  UPDATE Salaries SET Salaries.Attested=attested
    WHERE Salaries.SalaryId=salaryId;

END


#


CREATE PROCEDURE `SetSalaryNetPaid`(
  IN salaryId INTEGER,
  IN netPaid BOOLEAN
)
BEGIN

  UPDATE Salaries SET Salaries.NetPaid=netPaid
    WHERE Salaries.SalaryId=salaryId;

  UPDATE Salaries SET Salaries.Open = not (Salaries.NetPaid AND Salaries.TaxPaid)
    WHERE Salaries.SalaryId=salaryId;

END


#


CREATE PROCEDURE `SetSalaryNetSalary`(
  IN salaryId INTEGER,
  IN netSalary DOUBLE
)
BEGIN
  
  UPDATE Salaries SET Salaries.NetSalary=netSalary,Salaries.NetSalaryCents=netSalary*100
    WHERE Salaries.SalaryId=salaryId;

END


#


CREATE PROCEDURE `SetSalaryNetSalaryPrecise`(
  IN salaryId INTEGER,
  IN netSalaryCents BIGINT
)
BEGIN
  
  UPDATE Salaries SET Salaries.NetSalary=netSalaryCents/100.0,Salaries.NetSalaryCents=netSalaryCents
    WHERE Salaries.SalaryId=salaryId;

END


#


CREATE PROCEDURE `SetSalaryTaxPaid`(
  IN salaryId INTEGER,
  IN taxPaid BOOLEAN
)
BEGIN

  UPDATE Salaries SET Salaries.TaxPaid=taxPaid
    WHERE Salaries.SalaryId=salaryId;

  UPDATE Salaries SET Salaries.Open = not (Salaries.NetPaid AND Salaries.TaxPaid)
    WHERE Salaries.SalaryId=salaryId;

END


#


CREATE PROCEDURE `SetVolunteerOwnerPersonId`(
  volunteerId INTEGER,
  ownerPersonId INTEGER
)
BEGIN

  UPDATE Volunteers 
    SET Volunteers.OwnerPersonId=ownerPersonId 
    WHERE Volunteers.VolunteerId=volunteerId;

END


#


CREATE PROCEDURE `StoreOnePermission`(
          IN p_RoleType VARCHAR(64),
          IN p_PermissionType VARCHAR(64),
          IN p_allow INT
      )
BEGIN
         IF not exists(SELECT   *
                  FROM  `PermissionTypes`
                  WHERE ( `PermissionName`  = p_PermissionType )) THEN
          INSERT INTO `PermissionTypes`
          (`PermissionName`)
          VALUES (p_PermissionType);
        END IF;
        IF ( p_allow  = 1 ) THEN
          BEGIN
              IF not exists(SELECT   *
                        FROM  `PermissionSpecifications`
                        WHERE ( `RoleType`  = p_RoleType ) and ( `PermissionType`  = p_PermissionType )) THEN
                INSERT INTO `PermissionSpecifications`
                (`RoleType`,`PermissionType`) 
                VALUES (p_RoleType,p_PermissionType);
              END IF;
          END;
        ELSE
            IF ( p_allow  = 0 ) THEN
              BEGIN
                  DELETE FROM `PermissionSpecifications`
                  WHERE ( `RoleType`  = p_RoleType ) and ( `PermissionType`  = p_PermissionType );
              END;
            END IF;
        END IF;
    END


#


CREATE PROCEDURE `TerminateActivist`(
  IN personId INTEGER
)
BEGIN

  UPDATE Activists SET Active=0,DateTimeTerminated=NOW() WHERE Active=1 AND Activists.PersonId=personId;

END


#


CREATE PROCEDURE `TerminateMembership`(

  IN membershipId INTEGER,
  IN terminatedAsInvalid TINYINT(1)
)
BEGIN

  UPDATE Memberships 
    SET Memberships.Active=0,Memberships.TerminatedAsInvalid=terminatedAsInvalid,DateTimeTerminated=NOW()
    WHERE Memberships.MembershipId=membershipId;

END


#


CREATE PROCEDURE `UpdateOrganization`(
          IN p_ParentOrganizationId INT,
          IN p_NameIntl VARCHAR(64),
          IN p_Name VARCHAR(64),
          IN p_NameShort VARCHAR(64),
          IN p_Domain VARCHAR(64),
          IN p_MailPrefix VARCHAR(64),
          IN p_AnchorGeographyId INT,
          IN p_AcceptsMembers BOOLEAN,
          IN p_AutoAssignNewMembers BOOLEAN,
          IN p_DefaultCountryId INT,
          IN p_OrganizationId INT
      )
BEGIN

  UPDATE `Organizations`
    SET
       `ParentOrganizationId`=p_ParentOrganizationId,
       `NameInternational`=p_NameIntl,
       `Name`=p_Name,
       `NameShort`=p_NameShort,
       `Domain`=p_Domain,
       `MailPrefix`=p_MailPrefix,
       `AnchorGeographyId`=p_AnchorGeographyId,
       `AcceptsMembers`=p_AcceptsMembers,
       `AutoAssignNewMembers`=p_AutoAssignNewMembers,
       `DefaultCountryId`=p_DefaultCountryId
    WHERE ( `OrganizationId`  = p_OrganizationId );

END


#


CREATE PROCEDURE `UpdateOrganizationCommTemplate`(
   organizationCommTemplateId INT,
   templateData TEXT,
   updatedByPersonId INT,
   updatedDateTime DATETIME
)
BEGIN

   UPDATE OrganizationCommTemplates
      SET OrganizationCommTemplates.TemplateData=templateData,
          OrganizationCommTemplates.UpdatedByPersonId=updatedByPersonId,
          OrganizationCommTemplates.UpdatedDateTime=updatedDateTime
      WHERE OrganizationCommTemplates.OrganizationCommTemplateId=organizationCommTemplateId;
    
END

