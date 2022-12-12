-- =============================================
-- Author:		ThoughtFocus
-- Create date: 2022-Oct-03
-- Description:	checks for required filed validation and Updates FormState
-- =============================================
--exec [dbo].[checkAndUpdateFormState] 440, 169, 12, 2234
CREATE PROCEDURE [dbo].[checkAndUpdateFormState_old]
	@UserID bigint
	,@FormID bigint
	,@ProgramID bigint
	,@TermCode varchar(10) 
AS
BEGIN

IF EXISTS (SELECT * FROM [Application].[Forms] F  WHERE [ID] = @FormID  AND F.[FormStateID] in (1,2)) --Open, --draft
BEGIN
	IF NOT EXISTS(SELECT *
			FROM [Application].[Forms] F
			 CROSS APPLY OPENJSON(F.[FORM])
			   WITH (
				firstName nvarchar(500)
			  ,lastName nvarchar(500)
			  ,preferredName nvarchar(500)
			  ,otherName nvarchar(500)
			  ,cusulbEmail nvarchar(500)
			  ,altEmail nvarchar(500)
			  ,phoneNumber nvarchar(500)
			  ,csulbCampusId nvarchar(500)
			   ,Semester nvarchar(500)
			   ,Program nvarchar(500)
			  )T
			  WHERE [ID] = @FormID 
			  --AND F.[FormStateID]=1 --Open
			  AND (ISNULL(T.[altEmail],'') = ''
			  OR ISNULL(T.[phoneNumber],'') = ''
			  OR ISNULL(T.[csulbCampusId],'') = ''
			  OR ISNULL(T.[cusulbEmail],'') = ''
			  OR ISNULL(T.[firstName],'') = ''
			  OR ISNULL(T.[lastName],'') = ''))
  BEGIN
	IF NOT EXISTS (SELECT * FROM [Master].[ProgramDocuments] PD JOIN [Application].[Forms] F ON F.[ProgramID] = PD.[ProgramID] LEFT JOIN [Application].[FormAttachments] FA ON FA.[FormID] = F.[ID] WHERE F.[ID]=@FormID AND PD.[IsOptional]=0 AND FA.[ID] IS NULL)
	  BEGIN
		IF EXISTS (SELECT F.[ID], COUNT(R.[ID]) FROM [Application].[Recommendations] R JOIN [Application].[Forms] F ON F.[ID] = R.[FormID] WHERE F.[ID]=@FormID GROUP BY F.[ID] HAVING COUNT(R.[ID])>1)
			BEGIN
				UPDATE [Application].[Forms] SET [FormStateID] = 2 WHERE [ID]=@FormID
			END
			 ELSE
				 BEGIN
					UPDATE [Application].[Forms] SET [FormStateID] = 1 WHERE [ID]=@FormID
				END
	  END 
	   ELSE
		 BEGIN
			UPDATE [Application].[Forms] SET [FormStateID] = 1 WHERE [ID]=@FormID
		END
  END
  ELSE
	 BEGIN
		UPDATE [Application].[Forms] SET [FormStateID] = 1 WHERE [ID]=@FormID
	END
END
END