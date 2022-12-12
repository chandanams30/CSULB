-- =============================================
-- Author:		Thoughtfocus
-- Create date: 21-Oct-2022
-- Description:	Returns all Intial Creditial Option Items List as JSON String
-- =============================================
-- EXEC [dbo].[getIntialCreditialOptionItemsList] 1
-- EXEC [dbo].[getIntialCreditialOptionItemsList] 2
-- EXEC [dbo].[getIntialCreditialOptionItemsList] 4
-- EXEC [dbo].[getIntialCreditialOptionItemsList] 6
-- EXEC [dbo].[getIntialCreditialOptionItemsList] 8
-- =============================================
CREATE PROCEDURE [dbo].[getIntialCreditialOptionItemsList]
	@ProgramID bigint
AS
BEGIN
	
	SELECT '{' + STRING_AGG([OptionItemsList], ',') + '}' AS [IntialCreditialOptionItemsList]
	FROM (
		SELECT '"' + [ControlLabel] + '":' + '["' + STRING_AGG(STRING_ESCAPE([Value], 'json'), '","') + '"]' AS [OptionItemsList]
		FROM [Master].[IntialCreditialOptionItemsList]  WHERE ([ProgramID] = 0 OR [ProgramID]=@ProgramID)
		GROUP BY [ControlLabel]
		) T;
   
END