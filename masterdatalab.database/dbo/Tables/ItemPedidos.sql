CREATE TABLE [dbo].[ItemPedidos] (
    [id]         INT             IDENTITY (1, 1) NOT NULL,
    [item_id]    INT             NOT NULL,
    [pedido_id]  INT             NOT NULL,
    [quantidade] INT             NOT NULL,
    [valor_unit] DECIMAL (10, 2) NULL,
    CONSTRAINT [PK_ItemPedidos] PRIMARY KEY NONCLUSTERED ([id] ASC),
    CONSTRAINT [FK_ItemPedidos_Pedidos] FOREIGN KEY ([pedido_id]) REFERENCES [dbo].[Pedidos] ([id]),
    CONSTRAINT [FK_ItemPedidos_Produtos] FOREIGN KEY ([item_id]) REFERENCES [dbo].[Produtos] ([codigo])
);

