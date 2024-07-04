CREATE TYPE [User].[UDT_UserRoles] AS TABLE (
    [RoleID]                BIGINT        NULL,
    [RoleName]              VARCHAR (100) NULL,
    [ProgramID]             BIGINT        NULL,
    [ProgramName]           VARCHAR (100) NULL,
    [CommunityDistrictID]   BIGINT        NULL,
    [CommunityDistrictName] VARCHAR (100) NULL);

