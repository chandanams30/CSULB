CREATE TYPE [User].[UDT_UserPrograms] AS TABLE (
    [UserID]    BIGINT         NOT NULL,
    [FirstName] NVARCHAR (255) NOT NULL,
    [LastName]  NVARCHAR (255) NOT NULL,
    [CSULBID]   VARCHAR (50)   NULL,
    [Email]     NVARCHAR (255) NULL);

