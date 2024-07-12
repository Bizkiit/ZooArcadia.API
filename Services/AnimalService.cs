using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using ZooArcadia.API.Models.DbModels;
using ZooArcadia.API.Settings;

public class AnimalService
{
    private readonly ZooArcadiaDbContext _context;
    private readonly IMongoCollection<AnimalMongoDB> _animalsCollection;

    public AnimalService(IOptions<MongoDBSettings> mongoDBSettings, IMongoClient mongoClient, ZooArcadiaDbContext context)
    {
        var database = mongoClient.GetDatabase(mongoDBSettings.Value.DatabaseName);
        _animalsCollection = database.GetCollection<AnimalMongoDB>("ZooArcadia");
        _context = context;
    }

    public async Task IncrementClickAsync(int id)
    {
        var filter = Builders<AnimalMongoDB>.Filter.Eq(a => a.animalid, id);
        var update = Builders<AnimalMongoDB>.Update.Inc(a => a.clickcount, 1)
                                                   .SetOnInsert(a => a.animalid, id);

        var options = new UpdateOptions { IsUpsert = true };

        try
        {
            var result = await _animalsCollection.UpdateOneAsync(filter, update, options);
        }
        catch (MongoWriteException ex)
        {
            throw new Exception($"Failed to increment click count for animal with id {id}.", ex);
        }
    }



    public async Task<Animal> GetClickStatisticsAsync(int id)
    {
        return await _animalsCollection.Find(animal => animal.animalid == id).FirstOrDefaultAsync();
    }

    public async Task<List<AnimalMongoDB>> GetAllClickStatisticsAsync()
    {
        List<AnimalMongoDB> animalObject = new List<AnimalMongoDB>();

        try
        {
            var clickStatistics = await _animalsCollection.Find(animal => true).ToListAsync();
            foreach (var animal in clickStatistics)
            {
                Animal? animalFromDb = _context.animal
                    .Include(r => r.race)
                    .FirstOrDefault(a => a.animalid == animal.animalid);

                AnimalMongoDB animalMongoDBObject = new AnimalMongoDB
                {
                    Id = animal.Id,
                    animalid = animalFromDb.animalid,
                    rapportveterinaireid = animalFromDb.rapportveterinaireid,
                    habitatid = animalFromDb.habitatid,
                    name = animalFromDb.name,
                    status = animalFromDb.status,
                    raceid = animalFromDb.raceid,
                    race = animalFromDb.race,
                    habitat = animalFromDb.habitat,
                    rapportveterinaire = animalFromDb.rapportveterinaire,
                    animalimagerelation = animalFromDb.animalimagerelation,
                    animalfeeding = animalFromDb.animalfeeding,
                    clickcount = animal.clickcount
                };
                animalObject.Add(animalMongoDBObject);
            }

            return animalObject;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error retrieving click statistics: {ex.Message}");
            throw;
        }
    }

}
