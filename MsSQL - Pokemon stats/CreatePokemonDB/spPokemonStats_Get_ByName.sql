CREATE PROCEDURE spPokemonStats_Get_ByName @Name VARCHAR(12)
AS
SELECT * FROM PokemonStats WHERE name = @Name
GO