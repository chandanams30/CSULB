-- ================================================
-- Template generated from Template Explorer using:
-- Create Procedure (New Menu).SQL

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
--exec [dbo].[GetProgramList]
CREATE PROCEDURE [dbo].[GetProgramList]
--@ApplicationTypeId int,
--@SemesterId int,
--@StateId int
AS
BEGIN
		select P.[Name] as 'ProgramName', AP.[ID], T.TermCode, T.[Name] as 'TermName' from [Application].[ApplicationPrograms] AP
		JOIN [Master].[Programs]P ON P.[ID] = AP.[ProgramID]
		CROSS JOIN [Master].[Term] T
		WHERE T.ApplicationStartDate > '2022-01-01 00:00:00.000' AND T.ApplicationEndDate<'2022-2-23 00:00:00.000' --T.TermCode =2224
		ORDER BY T.TermCode
	----SET NOCOUNT ON;
	--select f.ProgramID,p.Name,count(f.ID) [ApplicationCount],count(case when f.State=@StateId then 1 end) [OfferedCount] from [Application].[Forms] as f 
	--join [Master].[Programs] p on p.ID=f.ProgramID
	--where p.ApplicationTypesID=@ApplicationTypeId and f.SemesterID=@SemesterId
	--group by f.ProgramID,p.Name
END