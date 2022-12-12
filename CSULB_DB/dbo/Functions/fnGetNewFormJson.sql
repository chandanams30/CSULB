-- =============================================
-- Author:		ThoughtFocus
-- Create date: 2022-Nov-17
-- Description:	Return JSOn Form String based on Application Type
-- =============================================
--SELECT [dbo].[fnGetNewFormJson] (338,7,2234);
--SELECT [dbo].[fnGetNewFormJson] (338,1,2234);
--SELECT [dbo].[fnGetNewFormJson] (338,2,2234);
--SELECT [dbo].[fnGetNewFormJson] (338,4,2234);
--SELECT [dbo].[fnGetNewFormJson] (338,6,2234);
--SELECT [dbo].[fnGetNewFormJson] (338,257,2234);
--SELECT [dbo].[fnGetNewFormJson] (338,7,2234);
-- =============================================
CREATE FUNCTION [dbo].[fnGetNewFormJson] 
(
	-- Add the parameters for the function here
	@Userid bigint,
	@ProgramID bigint,
	@TermCode varchar(10)
)
RETURNS NVARCHAR(MAX)
AS
BEGIN
	-- Declare the return variable here
	DECLARE @Result NVARCHAR(MAX)
	DECLARE @newSemester nvarchar(50), @newProgram nvarchar(250),@ApplicationTypeID bigint,	@ProgramFormIdentifier VARCHAR(10)

	SELECT @newSemester = [Name] FROM [Master].[Term] WHERE [TermCode] = @TermCode
	SELECT @newProgram = [Name],@ProgramFormIdentifier=[ProgramFormIdentifier] FROM [Master].[Programs] WHERE [ID] = @ProgramID
	SELECT @ApplicationTypeID = [ApplicationTypeID] FROM [Master].[ProgramApplicationType] pat WHERE [ProgramID] = @ProgramID

	IF (@ApplicationTypeID=1) --1	Initial Teacher Credential Programs
		BEGIN
			DECLARE @newJSON VARCHAR(MAX)

			SELECT @newJSON ='{"credentialPathway": "","credentialObjective": "","firstName": "'+ U.[FirstName] +'","middleName": "","lastName": "'+ U.[LastName] +'","altEmail": "","phoneNumber": "","csulbCampusId": "'+ U.[CSULBID] +'","cusulbEmail": "'+ U.[Email] +'","address": {"streetAddress": "","apartment": "","city": "","state": ""},"Semester": "'+ @newSemester +'","Program": "'+ @newProgram +'"}'  FROM  [User].[Users] U WHERE U.[ID] = @UserID

			IF (@ProgramFormIdentifier ='ESCP') --ESCP
				BEGIN
					SELECT @Result = @newJSON 
				END
			ELSE IF (@ProgramFormIdentifier ='MSCP') --MSCP
				BEGIN
					SELECT @Result = @newJSON
				END
			ELSE IF (@ProgramFormIdentifier = 'SSCP') --SSCP
				BEGIN
					SELECT @Result = @newJSON 
				END
			ELSE IF (@ProgramFormIdentifier ='UDCP') --UDCP
				BEGIN
					SELECT @Result = @newJSON 
				END
			ELSE 
				BEGIN
					SELECT @Result = '' 
				END
		END
	ELSE IF (@ApplicationTypeID=2) --2	Graduate / Advanced Credential Programs GACP
		BEGIN
			SELECT @Result='{"firstName": "'+ U.[FirstName] +'","lastName": "'+ U.[LastName] +'",
							"preferredName": "","otherName": "","altEmail": "","phoneNumber": "","csulbCampusId": "'+ U.[CSULBID] +'", 
							"cusulbEmail": "'+ U.[Email] +'","casId": "","languages": [""],"Semester": "'+ @newSemester +'","Program": "'+ @newProgram +'"}' FROM  [User].[Users] U WHERE U.[ID] = @UserID
		END
	ELSE 
		BEGIN
			SELECT @Result = '' 
		END
	-- Return the result of the function
	RETURN @Result
END