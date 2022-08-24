-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[GetApplications] 

AS
BEGIN

	SET NOCOUNT ON;
	SELECT * FROM [Master].[ApplicationTypes] WHERE [ID]=4
END