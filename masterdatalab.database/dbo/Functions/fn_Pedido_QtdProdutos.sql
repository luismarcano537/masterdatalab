CREATE   FUNCTION dbo.fn_Pedido_QtdProdutos (@pedido_id INT)
RETURNS INT AS
BEGIN
  RETURN (SELECT COUNT(DISTINCT item_id)
          FROM dbo.ItemPedidos WHERE pedido_id = @pedido_id);
END
