CREATE TABLE [dbo].[situacao] (
    [descricao] VARCHAR (255) NOT NULL,
    [icone]     INT           NOT NULL,
    [cor]       VARCHAR (10)  NULL,
    [id]        INT           DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_situacao] PRIMARY KEY CLUSTERED ([id] ASC)
);

