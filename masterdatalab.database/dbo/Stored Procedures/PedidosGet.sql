CREATE PROCEDURE [dbo].[PedidosGet]
@orderby VARCHAR(MAX),
@id Int,
@id_cliente Int,
@status Int,
@data_emissao_from DateTime2(0),
@data_emissao_to DateTime2(0),
@regporpag INT,
@pag INT,
@qtdtotal INT OUTPUT
AS
BEGIN
	DECLARE @sqlColumn   NVARCHAR(MAX)
	DECLARE @sqlTable    NVARCHAR(MAX)
	DECLARE @sqlWhere     NVARCHAR(MAX)
	DECLARE @sqlOrderBy    NVARCHAR(MAX)
	DECLARE @sqlOffset   NVARCHAR(MAX)
	DECLARE @query       NVARCHAR(MAX)
	DECLARE @count       INT

	--COLUMNS
	SET @sqlColumn = '
		[id],
		[id_cliente],
		[status],
		[data_emissao],
		[qtd_produtos],
		[qtd_unidades],
		[valor_total]
	 '
	--TABLES
	SET @sqlTable = 'FROM [Pedidos] WITH (NOLOCK)'

	--CONDITIONALS
	SET @sqlWhere = ' WHERE 1=1 '

	IF @id IS NOT NULL
		SET @sqlWhere = @sqlWhere + ' AND [id] = @id'
	IF @id_cliente IS NOT NULL
		SET @sqlWhere = @sqlWhere + ' AND [id_cliente] = @id_cliente'
	IF @status IS NOT NULL
		SET @sqlWhere = @sqlWhere + ' AND [status] = @status'


	IF @data_emissao_from IS NOT NULL
		SET @sqlWhere = @sqlWhere + ' AND [data_emissao] >= @data_emissao_from'
	IF @data_emissao_to IS NOT NULL
		SET @sqlWhere = @sqlWhere + ' AND [data_emissao] < DATEADD(DAY, 1, CAST(@data_emissao_to AS DATE))'

	--ORDER BY
	SET @sqlOrderBy  = ' ORDER BY [id]'
	IF @orderby IS NOT NULL AND @orderby <> ''
	BEGIN
		SET @sqlOrderBy  = ' ORDER BY ' + @orderby
	END

	--PAGINATION
	IF @pag < 1
		SET @pag = 1
	SET @sqlOffset = ' '
	SET @sqlOffset = @sqlOffset + ' OFFSET ('
	SET @sqlOffset = @sqlOffset + '(@pag - 1)'
	SET @sqlOffset = @sqlOffset + ' * '
	SET @sqlOffset = @sqlOffset + '@regporpag'
	SET @sqlOffset = @sqlOffset + ') ROWS FETCH NEXT '
	SET @sqlOffset = @sqlOffset + '@regporpag'
	SET @sqlOffset = @sqlOffset + ' ROWS ONLY '

	--TOTAL OF RECORDS
	IF @qtdtotal is null or @qtdtotal = 0
	BEGIN
		SET @qtdtotal = 0;
		SET @query = N'SELECT @count = COUNT(*) ' + @sqlTable + @sqlWhere
		EXECUTE sp_executesql @query,
		N'@id Int,
		@id_cliente Int,
		@status Int,
				@data_emissao_from DateTime2(0),
		@data_emissao_to DateTime2(0),
		@count int output',
		@id,
		@id_cliente,
		@status,
		@data_emissao_from,
		@data_emissao_to,
		@count = @qtdtotal output
	END

	--DATASET RESULT
	SET @query = N'SELECT ' + @sqlColumn + @sqlTable + @sqlWhere + @sqlOrderBy + @sqlOffset
	EXECUTE sp_executesql @query,
	N'@id Int,
	@id_cliente Int,
	@status Int,
		@data_emissao_from DateTime2(0),
	@data_emissao_to DateTime2(0),
	@regporpag INT,
	@pag INT,
	@qtdtotal INT OUTPUT ',

	@id,
	@id_cliente,
	@status,
	@data_emissao_from,
	@data_emissao_to,
	@regporpag,
	@pag,
	@qtdtotal

END