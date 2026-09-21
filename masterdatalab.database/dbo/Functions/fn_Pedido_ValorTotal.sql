-- 2. Funções de cálculo
CREATE   FUNCTION dbo.fn_Pedido_ValorTotal (@pedido_id INT)
RETURNS DECIMAL(18,2) AS
BEGIN
  RETURN (SELECT ISNULL(SUM(quantidade * valor_unit), 0)
          FROM dbo.ItemPedidos WHERE pedido_id = @pedido_id);
END
