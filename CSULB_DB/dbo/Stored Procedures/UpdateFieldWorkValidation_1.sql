-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE UpdateFieldWorkValidation
@FieldWorkID bigint,
@IsApproved bit,
@ApprovedBy bigint,
@ValidatedDate datetime,
@ValidTill datetime,
@RejectReason nvarchar(255)
AS
BEGIN
	update [FieldWork].[Attachments] set 
	IsApproved=@IsApproved,
	ApprovedBy=@ApprovedBy,
	ValidatedDate=@ValidatedDate,
	ValidTill=@ValidTill,
	RejectedReason=@RejectReason
	where ID=@FieldWorkID
END