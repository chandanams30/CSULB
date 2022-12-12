-- =============================================
-- Author:		ThoughtFocus
-- Create date: 2022-Nov-30
-- Description:	Stores information of email sent from MyCED
-- =============================================

CREATE PROCEDURE [dbo].[InsertEmailHistory]
@UserID bigint
,@Subject nvarchar(200) 
,@Body nvarchar(max) 
,@Email_to varchar(500) 
,@Email_from varchar(500) 
,@email_date datetime 
,@cc nvarchar(500) 
,@bcc nvarchar(500) 
,@notes nvarchar(500) 
,@isEmailSent bit

AS
BEGIN
	INSERT INTO [User].[EmailHistory]
           ([UserID]
		   ,[Subject]
           ,[Body]
           ,[Email_to]
           ,[Email_from]
           ,[email_date]
           ,[cc]
           ,[bcc]
           ,[notes]
           ,[isEmailSent])
     VALUES
           (@UserID
			,@Subject
			,@Body 
			,@Email_to
			,@Email_from
			,@email_date
			,@cc
			,@bcc
			,@notes
			,@isEmailSent)
END