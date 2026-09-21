CREATE   PROCEDURE [dbo].[PedidosGet]
@orderby VARCHAR(MAX), 
@id Int, 
@regporpag INT, 
@pag INT, 
@qtdtotal INT OUTPUT 
AS 
BEGIN 
	DECLARE @sqlColumn   NVARCHAR(MAX)
	DECLARE @sqlTable    NVARCHAR(MAX)
	DECLARE @sqlWhere    NVARCHAR(MAX)
	DECLARE @sqlOrderBy  NVARCHAR(MAX)
	DECLARE @sqlOffset   NVARCHAR(MAX)
	DECLARE @query       NVARCHAR(MAX)
	DECLARE @count       INT

	--COLUMNS
	SET @sqlColumn = '
		[id], 
		[id_cliente], 
		[status], 
		[qtd_produtos],
		[qtd_unidades],
		[valor_total],
		[data_emissao]
	 '
	--TABLES
	SET @sqlTable = 'FROM [Pedidos] WITH (NOLOCK)'

	--CONDITIONALS
	SET @sqlWhere = ' WHERE 1=1 '

	IF @id IS NOT NULL
		SET @sqlWhere = @sqlWhere + ' AND [id] = @id'

	--ORDER BY
	SET @sqlOrderBy = ' ORDER BY [id]'
	IF @orderby IS NOT NULL AND @orderby <> ''
	BEGIN
		SET @sqlOrderBy = ' ORDER BY ' + @orderby
	END

	--PAGINATION
	IF @pag < 1
		SET @pag = 1
	SET @sqlOffset = ' OFFSET ((@pag - 1) * @regporpag) ROWS FETCH NEXT @regporpag ROWS ONLY '

	--TOTAL OF RECORDS
	IF @qtdtotal IS NULL OR @qtdtotal = 0
	BEGIN
		SET @qtdtotal = 0;
		SET @query = N'SELECT @count = COUNT(*) ' + @sqlTable + @sqlWhere
		EXECUTE sp_executesql @query,
		N'@id Int, 
		@count int output',
		@id,
		@count = @qtdtotal output
	END

	--DATASET RESULT
	SET @query = N'SELECT ' + @sqlColumn + @sqlTable + @sqlWhere + @sqlOrderBy + @sqlOffset
	EXECUTE sp_executesql @query,
	N'@id Int, 
	@regporpag INT, 
	@pag INT, 
	@qtdtotal INT OUTPUT',
	@id,
	@regporpag,
	@pag,
	@qtdtotal
END
