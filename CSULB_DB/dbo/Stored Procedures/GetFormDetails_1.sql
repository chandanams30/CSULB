-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE GetFormDetails
@FormId int
AS
BEGIN

	SET NOCOUNT ON;
	SELECT * from [Application].[Forms] where  ID=@FormId
END