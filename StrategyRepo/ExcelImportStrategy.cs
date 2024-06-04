using Domain;
using ExcelDataReader;
using Repository;
using StrategyRepository;
using System.Data;
using System.Text;

namespace StrategyRepo
{
	public class ExcelImportStrategy : IDataImportStrategy
	{
	
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
						string timeS = row["Time"].ToString();
                        string[] parts = timeS.Split(' ');
						TimeSpan time = TimeSpan.Parse(parts[1]);

                     
                        Item customObject = new Item
						{

							Name = name,
							Time = time,

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
				repository.ClearAndAddRange(listOfItems);

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
