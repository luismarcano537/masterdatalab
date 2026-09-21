CREATE   FUNCTION dbo.fn_Pedido_QtdUnidades (@pedido_id INT)
RETURNS INT AS
BEGIN
  RETURN (SELECT ISNULL(SUM(quantidade), 0)
          FROM dbo.ItemPedidos WHERE pedido_id = @pedido_id);
END
