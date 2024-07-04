CREATE TABLE [Application].[Instructor] (
    [ID]               BIGINT IDENTITY (1, 1) NOT NULL,
    [FormID]           BIGINT NOT NULL,
    [InstructorUserID] BIGINT NOT NULL,
    [isAssigned]       BIT    CONSTRAINT [DF_Instructor_isAssigned] DEFAULT ((1)) NOT NULL
);



