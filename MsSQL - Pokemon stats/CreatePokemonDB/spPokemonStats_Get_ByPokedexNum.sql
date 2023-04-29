CREATE PROCEDURE spPokemonStats_Get_ByPokedexNum @PokedexNum int
AS
SELECT * FROM PokemonStats WHERE pokedex_number = @PokedexNum
GO