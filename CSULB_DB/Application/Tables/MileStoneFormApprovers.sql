CREATE TABLE [Application].[MileStoneFormApprovers] (
    [ID]               BIGINT         IDENTITY (1, 1) NOT NULL,
    [MilestoneFormID]  BIGINT         NOT NULL,
    [ApproverUserID]   BIGINT         NOT NULL,
    [Sequence]         INT            NOT NULL,
    [isApproved]       BIT            DEFAULT ((0)) NULL,
    [ApproverComments] NVARCHAR (MAX) NULL,
    [ApproveredOn]     DATETIME       NULL
);

