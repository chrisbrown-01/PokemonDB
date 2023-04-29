using AutoFixture.Kernel;
using MongoDB.Bson;

namespace PokemonApi_Tests.TestData
{
    public class ObjectIdGenerator : ISpecimenBuilder
    {
        public object Create(object request, ISpecimenContext context)
        {
            if (request is Type type && type == typeof(ObjectId))
            {
                return ObjectId.GenerateNewId();
            }

            return new NoSpecimen();
        }
    }
}