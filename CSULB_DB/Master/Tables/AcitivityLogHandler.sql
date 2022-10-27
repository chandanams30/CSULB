CREATE TABLE [Master].[AcitivityLogHandler] (
    [ID]               BIGINT       IDENTITY (1, 1) NOT NULL,
    [RoleID]           BIGINT       NOT NULL,
    [showClose]        BIT          DEFAULT ((0)) NOT NULL,
    [showEdit]         BIT          DEFAULT ((0)) NOT NULL,
    [showSubmit]       BIT          DEFAULT ((0)) NOT NULL,
    [showRejectHours]  BIT          DEFAULT ((0)) NOT NULL,
    [showApproveHours] BIT          DEFAULT ((0)) NOT NULL,
    [Status]           VARCHAR (50) NULL
);

