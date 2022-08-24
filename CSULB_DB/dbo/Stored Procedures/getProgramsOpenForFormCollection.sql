--exec getProgramsOpenForFormCollection 338
CREATE PROCEDURE getProgramsOpenForFormCollection 
@UserID bigint
AS
BEGIN
select
		F.[ID] FormID,
		P.[Name] ProgramName
		,T.[Name] Term
		,F.[FormStateID] FormState
		,FS.[Name] [Status]
		FROM [Application].[Forms] F
		JOIN [Application].[ApplicationPrograms] AP ON AP.[ID]=F.[ApplicationProgramsID]
		JOIN [Master].[Programs] P ON P.[ID] = AP.[ProgramID]
		JOIN [Master].[Term] T ON T.[TermCode] = F.[TermCode]
		JOIN [Master].[FormState] FS ON FS.ID = F.[FormStateID]
		WHERE F.UserID  = @UserID
END