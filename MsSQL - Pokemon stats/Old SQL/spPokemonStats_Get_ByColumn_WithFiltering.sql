CREATE PROCEDURE spPokemonStats_Get_ByColumn_WithFiltering
    @ColumnName NVARCHAR(128),
    @ColumnValue NVARCHAR(128),
    @Operator NVARCHAR(2)
AS
BEGIN
    DECLARE @sql NVARCHAR(MAX)
    SET @sql = N'SELECT name, pokedex_number, ' + QUOTENAME(@ColumnName) + ' FROM PokemonTable WHERE ' + QUOTENAME(@ColumnName) + ' ' + @Operator + ' @ColumnValue'
    EXEC sp_executesql @sql, N'@ColumnName NVARCHAR(128), @ColumnValue NVARCHAR(128), @Operator NVARCHAR(2)', @ColumnName, @ColumnValue, @Operator
END
GO