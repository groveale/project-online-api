CREATE TABLE Resources (
    ResourceId VARCHAR(36),
    ResourceBaseCalendar VARCHAR(255),
    ResourceBookingType INT,
    ResourceCanLevel BIT,
    ResourceCode VARCHAR(255),
    ResourceCostCenter VARCHAR(255),
    ResourceCostPerUse DECIMAL(10, 6),
    ResourceCreatedDate DATETIME,
    ResourceEarliestAvailableFrom DATETIME,
    ResourceEmailAddress VARCHAR(255),
    ResourceGroup VARCHAR(255),
    ResourceHyperlink VARCHAR(255),
    ResourceHyperlinkHref VARCHAR(255),
    ResourceInitials VARCHAR(255),
    ResourceIsActive BIT,
    ResourceIsGeneric BIT,
    ResourceIsTeam BIT,
    ResourceLatestAvailableTo DATETIME,
    ResourceMaterialLabel VARCHAR(255),
    ResourceMaxUnits DECIMAL(10, 6),
    ResourceModifiedDate DATETIME,
    ResourceName VARCHAR(255),
    ResourceNTAccount VARCHAR(255),
    ResourceOvertimeRate DECIMAL(10, 6),
    ResourceStandardRate DECIMAL(10, 6),
    ResourceStatusId VARCHAR(36),
    ResourceStatusName VARCHAR(255),
    ResourceTimesheetManageId VARCHAR(36),
    ResourceType INT,
    ResourceWorkgroup VARCHAR(255),
    TypeDescription VARCHAR(255),
    TypeName VARCHAR(255),
    RBS VARCHAR(255),
    CostType VARCHAR(255),
    ResourceDepartments VARCHAR(255),
    SnapshotDate DATETIME
);
