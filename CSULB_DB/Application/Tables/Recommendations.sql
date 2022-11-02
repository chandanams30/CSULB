CREATE TABLE [Application].[Recommendations] (
    [ID]                      BIGINT           IDENTITY (1, 1) NOT NULL,
    [FormID]                  BIGINT           NOT NULL,
    [RecommenderName]         NVARCHAR (200)   NOT NULL,
    [RecommenderEmail]        NVARCHAR (200)   NOT NULL,
    [CreatedBy]               BIGINT           NOT NULL,
    [CreatedDate]             DATETIME         NOT NULL,
    [RecommenderURL]          NVARCHAR (200)   NOT NULL,
    [RecommenderURLValidTill] DATETIME         NULL,
    [AllowUpload]             BIT              CONSTRAINT [DF_Recomendations_AllowUpload] DEFAULT ((0)) NOT NULL,
    [RecommenderIdentifier]   UNIQUEIDENTIFIER NULL,
    [isMailSent]              BIT              DEFAULT ((0)) NULL
);



