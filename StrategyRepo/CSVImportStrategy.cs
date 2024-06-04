using Domain;
using Repository;
using StrategyRepository;


namespace StrategyRepo
{
	public class CSVImportStrategy : IDataImportStrategy
	{
		public bool ImportData(string filePath, IRepository<Item> repository)
		{
			try
			{
				List<Item> listofItems = new List<Item>();
				using (var stream = File.Open(filePath, FileMode.Open, FileAccess.Read))
				{
					listofItems = ReadCSVFile(stream);
				}

				repository.ClearAndAddRange(listofItems);

				return true;
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Error importing CSV data from file: {filePath}. Error: {ex.Message}");
				return false;
			}
		}

		private static List<Item> ReadCSVFile(FileStream stream)
		{
			List<Item> list = new List<Item>();
			using (var reader = new StreamReader(stream))
			{
				var headerLine = reader.ReadLine();

				// Loop through each line in the CSV file
				try
				{
					while (!reader.EndOfStream)
					{
						var line = reader.ReadLine();
						var values = line.Split(',');
						TimeSpan time = TimeSpan.Parse("00:" + values[1]);

						// Assuming the CSV columns are: Id, Name, Price
						var item = new Item
						{
							Name = values[0],
							Time = time
						};

						list.Add(item);
					}
				}
				catch (Exception ex)
				{
					string message = ex.Message;
				}
			}
			return list;
		}
	}
}
