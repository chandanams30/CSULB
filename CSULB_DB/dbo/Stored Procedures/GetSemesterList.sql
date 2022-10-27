

CREATE PROCEDURE [dbo].[GetSemesterList]
AS
BEGIN

	SELECT DISTINCT T.[TermCode]
			,T.[Name] 
	FROM [Master].[Term] T JOIN [Master].[ProgramApplicationDates] PAD ON PAD.[TermCode] = T.[TermCode]
	ORDER BY T.[TermCode]
END