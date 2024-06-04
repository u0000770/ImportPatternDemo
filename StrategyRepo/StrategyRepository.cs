using Domain;
using ExcelDataReader;
using MongoDB.Bson.IO;
using Repository;
using System;
using System.Data;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

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

}
