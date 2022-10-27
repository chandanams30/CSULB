CREATE TABLE [dbo].[SchoolPsychologyPartnersList_PartnersAndEmails] (
    [School_District]              NVARCHAR (50) NOT NULL,
    [FirstName]                    NVARCHAR (50) NOT NULL,
    [LastName]                     NVARCHAR (50) NOT NULL,
    [School_Site_Supervisor_Email] NVARCHAR (50) NOT NULL,
    [isUserAdded]                  BIT           NULL,
    [ID]                           BIGINT        IDENTITY (1, 1) NOT NULL
);

