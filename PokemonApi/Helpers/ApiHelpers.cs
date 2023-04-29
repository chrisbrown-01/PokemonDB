namespace PokemonApi.Helpers
{
    public class ApiHelpers
    {
        public static List<KeyValuePair<string, List<string>>> CleanQueryParameters(IDictionary<string, List<string>> queryParams)
        {
            var cleanedQueryParams = new List<KeyValuePair<string, List<string>>>();

            foreach (var param in queryParams)
            {
                // If multiple parameters of the same property, need to extract them and add as a
                // single KeyValuePair to the list where the value length is 2
                if (param.Value.Count > 2)
                {
                    var chunks = param.Value.Chunk(2).ToList();

                    foreach (var chunk in chunks)
                    {
                        cleanedQueryParams.Add(new KeyValuePair<string, List<string>>(param.Key, chunk.ToList()));
                    }
                }
                else cleanedQueryParams.Add(param);
            }

            if (cleanedQueryParams.Count > 5) cleanedQueryParams = cleanedQueryParams.Take(5).ToList(); // Limit to only first 5 query conditions

            return cleanedQueryParams;
        }
    }
}