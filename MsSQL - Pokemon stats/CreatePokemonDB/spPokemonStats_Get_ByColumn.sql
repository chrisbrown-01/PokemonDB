CREATE PROCEDURE spPokemonStats_Get_ByColumn
    @ColumnName NVARCHAR(128)
AS
BEGIN
    DECLARE @sql NVARCHAR(MAX)
    SET @sql = N'SELECT name, pokedex_number, ' + QUOTENAME(@ColumnName) + '  FROM PokemonStats'
    EXEC sp_executesql @sql, N'@ColumnName NVARCHAR(128)', @ColumnName
END
GO