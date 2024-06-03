using Domain;
using ExcelDataReader;
using Repository;
using System.Data;
using System.Text;

namespace StrategyRepository
{
	// Interface to Import strategy
	public interface IDataImportStrategy
	{
		bool ImportData(string filePath, IRepository<Item> repository);
	}

	// Context Class managing the various strategies and coordinating their execution
	public class ImporterStrategyContext
	{

		private IDataImportStrategy importStrategy;

		public void SetImportStrategy(IDataImportStrategy importStrategy)
		{
			this.importStrategy = importStrategy;
		}

		public bool ImportData(string filePath, IRepository<Item> repository)
		{
			if (importStrategy == null)
			{
				Console.WriteLine("Import strategy not set.");
				return false;
			}

			return importStrategy.ImportData(filePath, repository);
		}
	}

	// JSON Implementation
	public class JSONImportStrategy : IDataImportStrategy
	{
		public bool ImportData(string filePath, IRepository<Item> repository)
		{
			try
			{
				// Logic to import JSON data
				Console.WriteLine($"Importing JSON data from file: {filePath}");
				// Placeholder logic - replace with actual implementation
				// For example, parsing JSON and storing it in repository
				repository.SaveChanges();
				return true;
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Error importing JSON data from file: {filePath}. Error: {ex.Message}");
				return false;
			}
		}
	}

	// CSV Implementation
	public class CSVImportStrategy : IDataImportStrategy
	{
		public bool ImportData(string filePath, IRepository<Item> repository)
		{
			try
			{
				// Logic to import CSV data
				Console.WriteLine($"Importing CSV data from file: {filePath}");
				// Placeholder logic - replace with actual implementation
				// For example, parsing CSV and storing it in repository
				repository.SaveChanges();
				return true;
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Error importing CSV data from file: {filePath}. Error: {ex.Message}");
				return false;
			}
		}
	}

	// Excel Implementation
	public class ExcelImportStrategy : IDataImportStrategy
	{

		static string ExtractTime(string input)
		{
			// Parse the input string into a DateTime object
			DateTime dateTime = DateTime.ParseExact(input, "dd/MM/yyyy HH:mm:ss", null);

			// Extract the time part and format it as HH:mm:ss
			string time = dateTime.ToString("HH:mm:ss");

			return time;
		}

		private List<Item> ReadExcelFile(string filePath)
		{
			#region Windows and.Net config
			// registers an encoding provider in .NET applications that are not natively supported by the framework
			Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
			// Create the configuration object and set the encoding
			var configuration = new ExcelReaderConfiguration
			{
				FallbackEncoding = Encoding.GetEncoding(1252) // Use Windows-1252 encoding
			};

			#endregion

			List<Item> list = new List<Item>();
			// Create an instance of FileStream to read the Excel file
			using (var stream = File.Open(filePath, FileMode.Open, FileAccess.Read))
			{
				// Create an ExcelDataReader reader based on the stream using the configuration
				// 
				using (var reader = ExcelReaderFactory.CreateReader(stream, configuration))
				{
					// Read the data from the Excel file
					var result = reader.AsDataSet(new ExcelDataSetConfiguration
					{
						ConfigureDataTable = _ => new ExcelDataTableConfiguration
						{
							UseHeaderRow = true // Treat the first row as header
						}
					});

					// Get the first DataTable from the DataSet
					var dataTable = result.Tables[0];

					// Iterate through each row in the DataTable
					foreach (DataRow row in dataTable.Rows)
					{
						// Access the 'Name' and 'Time' columns from the DataRow
						string name = row["Name"].ToString();
						//TimeSpan time = TimeSpan.Parse(row["Time"].ToString());
						// string time = row["Time"].ToString();

						// time = ExtractTime(time);
						// Now you can do whatever you want with the data
						// For example, you can create instances of a custom class
						// and populate them with the data from the Excel file
						Item customObject = new Item
						{

							Name = name,

						};
						list.Add(customObject);
						// Do something with the custom object
						// For now, I'm just printing it out
						Console.WriteLine($"Name: {customObject.Name}");
					}
					return list;
				}
			}
		}

		public bool ImportData(string filePath, IRepository<Item> repository)
		{
			try
			{
				// Logic to import Excel data
				var listOfItems = ReadExcelFile(filePath);
				repository.Clear();
				foreach (var item in listOfItems)
				{
					repository.Add(item);
				}
				repository.SaveChanges();

				return true;
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Error importing Excel data from file: {filePath}. Error: {ex.Message}");
				return false;
			}
		}
	}
}
