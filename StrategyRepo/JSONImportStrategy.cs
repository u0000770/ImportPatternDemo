using Domain;
using Newtonsoft.Json.Linq;
using Repository;
using StrategyRepository;

namespace StrategyRepo
{
	public class JSONImportStrategy : IDataImportStrategy
	{
		public bool ImportData(string filePath, IRepository<Item> repository)
		{
			try
			{
				// Logic to import JSON data
				var items = ReadJsonFile(filePath);
				repository.ClearAndAddRange(items);
				return true;
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Error importing JSON data from file: {filePath}. Error: {ex.Message}");
				return false;
			}
		}

		private List<Item> ReadJsonFile(string filePath)
		{
			string json = File.ReadAllText(filePath);
			var jsonArray = JArray.Parse(json);
			var people = new List<Item>();
			foreach (var item in jsonArray)
			{
				var timeS = "00:" + item.Value<string>("Time");
                var time = TimeSpan.Parse(timeS);

				var person = new Item
				{
					Name = item.Value<string>("Name"),
					Time = time
				};
				people.Add(person);
			}
			return people;
		}
	}
}
