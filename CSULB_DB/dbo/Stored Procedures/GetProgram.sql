-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[GetProgram]
@ProgramId int
AS
BEGIN

	--SET NOCOUNT ON;
select * from [Master].[Programs] where ID=@ProgramId
END