CREATE TABLE Tasks (
    ProjectId VARCHAR(36),
    TaskId VARCHAR(36),
    ParentTaskId VARCHAR(36),
    ParentTaskName VARCHAR(255),
    ProjectName VARCHAR(255),
    TaskActualCost DECIMAL(10, 6),
    TaskActualDuration DECIMAL(10, 6),
    TaskActualFinishDate DATETIME,
    TaskActualFixedCost DECIMAL(10, 6),
    TaskActualOvertimeCost DECIMAL(10, 6),
    TaskActualOvertimeWork DECIMAL(10, 6),
    TaskActualRegularCost DECIMAL(10, 6),
    TaskActualRegularWork DECIMAL(10, 6),
    TaskActualStartDate DATETIME,
    TaskActualWork DECIMAL(10, 6),
    TaskACWP DECIMAL(10, 6),
    TaskBCWP DECIMAL(10, 6),
    TaskBCWS DECIMAL(10, 6),
    TaskBudgetCost DECIMAL(10, 6),
    TaskBudgetWork DECIMAL(10, 6),
    TaskClientUniqueId INT,
    TaskCost DECIMAL(10, 6),
    TaskCostVariance DECIMAL(10, 6),
    TaskCPI DECIMAL(10, 6),
    TaskCreatedDate DATETIME,
    TaskCreatedRevisionCounter INT,
    TaskCV DECIMAL(10, 6),
    TaskCVP DECIMAL(16, 13),
    TaskDeadline DATETIME,
    TaskDeliverableFinishDate DATETIME,
    TaskDeliverableStartDate DATETIME,
    TaskDuration DECIMAL(10, 6),
    TaskDurationIsEstimated BIT,
    TaskDurationString VARCHAR(255),
    TaskDurationVariance DECIMAL(10, 6),
    TaskEAC DECIMAL(10, 6),
    TaskEarlyFinish DATETIME,
    TaskEarlyStart DATETIME,
    TaskFinishDate DATETIME,
    TaskFinishDateString VARCHAR(255),
    TaskFinishVariance DECIMAL(10, 6),
    TaskFixedCost DECIMAL(10, 6),
    TaskFixedCostAssignmentId VARCHAR(36),
    TaskFreeSlack DECIMAL(10, 6),
    TaskHyperLinkAddress VARCHAR(255),
    TaskHyperLinkFriendlyName VARCHAR(255),
    TaskHyperLinkSubAddress VARCHAR(255),
    TaskIgnoresResourceCalendar BIT,
    TaskIndex INT,
    TaskIsActive BIT,
    TaskIsCritical BIT,
    TaskIsEffortDriven BIT,
    TaskIsExternal BIT,
    TaskIsManuallyScheduled BIT,
    TaskIsMarked BIT,
    TaskIsMilestone BIT,
    TaskIsOverallocated BIT,
    TaskIsProjectSummary BIT,
    TaskIsRecurring BIT,
    TaskIsSummary BIT,
    TaskLateFinish DATETIME,
    TaskLateStart DATETIME,
    TaskLevelingDelay DECIMAL(10, 6),
    TaskModifiedDate DATETIME,
    TaskModifiedRevisionCounter INT,
    TaskName VARCHAR(255),
    TaskOutlineLevel INT,
    TaskOutlineNumber VARCHAR(255),
    TaskOvertimeCost DECIMAL(10, 6),
    TaskOvertimeWork DECIMAL(10, 6),
    TaskPercentCompleted INT,
    TaskPercentWorkCompleted INT,
    TaskPhysicalPercentCompleted INT,
    TaskPriority INT,
    TaskRegularCost DECIMAL(10, 6),
    TaskRegularWork DECIMAL(10, 6),
    TaskRemainingCost DECIMAL(10, 6),
    TaskRemainingDuration DECIMAL(10, 6),
    TaskRemainingOvertimeCost DECIMAL(10, 6),
    TaskRemainingOvertimeWork DECIMAL(10, 6),
    TaskRemainingRegularCost DECIMAL(10, 6),
    TaskRemainingRegularWork DECIMAL(10, 6),
    TaskRemainingWork DECIMAL(10, 6),
    TaskResourcePlanWork DECIMAL(10, 6),
    TaskSPI DECIMAL(10, 6),
    TaskStartDate DATETIME,
    TaskStartDateString VARCHAR(255),
    TaskStartVariance DECIMAL(10, 6),
    TaskStatusManagerUID VARCHAR(36),
    TaskSV DECIMAL(10, 6),
    TaskSVP DECIMAL(16, 13),
    TaskTCPI DECIMAL(10, 6),
    TaskTotalSlack DECIMAL(10, 6),
    TaskVAC DECIMAL(10, 6),
    TaskWBS VARCHAR(255),
    TaskWork DECIMAL(10, 6),
    TaskWorkVariance DECIMAL(10, 6),
    FlagStatus VARCHAR(255),
    Health VARCHAR(255),
    SnapshotDate DATETIME
);
