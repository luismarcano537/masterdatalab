CREATE TABLE [dbo].[Cliente] (
    [id]           INT           IDENTITY (1, 1) NOT NULL,
    [nome]         VARCHAR (180) NOT NULL,
    [documento]    NVARCHAR (14) NOT NULL,
    [razao_social] VARCHAR (120) NULL,
    CONSTRAINT [PK_Cliente] PRIMARY KEY NONCLUSTERED ([id] ASC)
);

