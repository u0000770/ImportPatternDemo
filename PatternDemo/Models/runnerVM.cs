using Domain;

namespace PatternDemo.Models
{
	public class runnerVM
	{
		public string Name { get; set; }
		public string Time { get; set; }

		public static List<runnerVM> buildVM(List<Item> items)
		{
			var vm = items.Select(item => new runnerVM
			{
				Name = item.Name,
				Time = item.Time.ToString(@"hh\:mm\:ss")
			}).ToList();
			return vm;

		}

	}
}
