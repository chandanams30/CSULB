CREATE TABLE [Application].[Forms] (
    [ID]                BIGINT         IDENTITY (1, 1) NOT NULL,
    [UserID]            BIGINT         NOT NULL,
    [ProgramID]         BIGINT         NOT NULL,
    [TermCode]          VARCHAR (10)   NULL,
    [Form]              NVARCHAR (MAX) NULL,
    [FormStateID]       INT            NOT NULL,
    [ApplicationNumber] NVARCHAR (100) NULL,
    [CreatedDateTime]   DATETIME       NOT NULL,
    [CreatedByUserID]   BIGINT         NOT NULL,
    [ModifiedDateTime]  DATETIME       NULL,
    [ModifiedBy]        BIGINT         NULL,
    [MessageBoard]      NVARCHAR (MAX) NULL
);












GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE TRIGGER [Application].[TR_Form_Update] 
   ON  [Application].[Forms]
   AFTER UPDATE
AS 
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for trigger here
INSERT INTO [Application].[FormsHistory]
           ([FormsID]
           ,[Form]
           ,[FormStateID]
           ,[ModifiedDateTime]
           ,[ModifiedBy]
           ,[CreatedDateTime])
     SELECT [ID]
           ,[Form]
           ,[FormStateID]
           ,[ModifiedDateTime]
           ,[ModifiedBy]
		   ,GETDATE()
		FROM deleted
END