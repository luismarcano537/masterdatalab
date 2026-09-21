CREATE TABLE [dbo].[tabela_precos] (
    [id]            INT             IDENTITY (1, 1) NOT NULL,
    [cod_cliente]   INT             NOT NULL,
    [cod_produto]   INT             NOT NULL,
    [valor_unit]    DECIMAL (10, 2) NOT NULL,
    [data_inclusao] DATETIME2 (7)   NOT NULL,
    CONSTRAINT [PK_tabela_precos] PRIMARY KEY NONCLUSTERED ([id] ASC),
    CONSTRAINT [FK_tabela_precos_Cliente] FOREIGN KEY ([cod_cliente]) REFERENCES [dbo].[Cliente] ([id]),
    CONSTRAINT [FK_tabela_precos_Produtos] FOREIGN KEY ([cod_produto]) REFERENCES [dbo].[Produtos] ([codigo])
);

