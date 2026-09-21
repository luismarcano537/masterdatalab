CREATE TABLE [dbo].[Produtos] (
    [id]            INT           IDENTITY (1, 1) NOT NULL,
    [codigo]        INT           NOT NULL,
    [nome]          VARCHAR (120) NOT NULL,
    [descricao]     VARCHAR (180) NOT NULL,
    [data_cadastro] DATE          NOT NULL,
    CONSTRAINT [PK_Produtos] PRIMARY KEY NONCLUSTERED ([id] ASC)
);


GO
CREATE UNIQUE NONCLUSTERED INDEX [UX_Produtos_codigo]
    ON [dbo].[Produtos]([codigo] ASC);

