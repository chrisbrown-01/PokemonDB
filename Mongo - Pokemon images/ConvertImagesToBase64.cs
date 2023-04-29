using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using ThirdParty.Json.LitJson;

namespace MongoDockerTest
{
    public class ConvertImagesToBase64
    {
        public ConvertImagesToBase64()
        {
            string imagesPath = @"C:\Users\chris\Documents\Programming\C#\2023\03152023_PokemonApp\Pokemon images\images";
            string jsonPath = @"C:\Users\chris\Documents\Programming\C#\2023\03152023_PokemonApp\Pokemon images\mongoJsonDB.json";

            var filePaths = Directory.EnumerateFiles(imagesPath).ToList();
            var fileNames = Directory.EnumerateFiles(imagesPath).Select(Path.GetFileName).ToList();

            List<ImageModel> imageList = new();

            for (int i = 0; i < filePaths.Count; i++)
            {
                imageList.Add(new ImageModel
                {
                    Name = fileNames[i].Split(".").First(),
                    FileName = fileNames[i],
                    ImageBase64Data = Convert.ToBase64String(File.ReadAllBytes(filePaths[i]))
                });
            }

            var json = JsonSerializer.Serialize(imageList);

            File.WriteAllText(jsonPath, json);
        }
    }
}
