CREATE TABLE [dbo].[Pedidos] (
    [id]           INT           IDENTITY (1, 1) NOT NULL,
    [id_cliente]   INT           NULL,
    [status]       INT           NULL,
    [data_emissao] DATETIME2 (0) CONSTRAINT [DF_Pedidos_data_emissao] DEFAULT (sysdatetime()) NOT NULL,
    [valor_total]  AS            ([dbo].[fn_Pedido_ValorTotal]([id])),
    [qtd_produtos] AS            ([dbo].[fn_Pedido_QtdProdutos]([id])),
    [qtd_unidades] AS            ([dbo].[fn_Pedido_QtdUnidades]([id])),
    CONSTRAINT [PK_Pedidos] PRIMARY KEY NONCLUSTERED ([id] ASC),
    CONSTRAINT [FK_Pedidos_Cliente] FOREIGN KEY ([id_cliente]) REFERENCES [dbo].[Cliente] ([id]),
    CONSTRAINT [FK_Pedidos_Situacao] FOREIGN KEY ([status]) REFERENCES [dbo].[situacao] ([id])
);

