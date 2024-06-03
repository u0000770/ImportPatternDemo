namespace Domain
{
    public class Item
    {
        public int Id { get; set; }
        public required string Name { get; set; }

        // An implicit operator in C# allows you to define custom implicit conversions
        // between two types.
        // This implicit operator Item(string v) method allows you to define how an
        // instance of the Item class can be implicitly converted from a string.
        public static implicit operator Item(string v)
        {
            // Split the string assuming it is in the format "Id,Name"
            string[] parts = v.Split(',');
            if (parts.Length != 2)
            {
                throw new ArgumentException("Invalid string format. Expected format: 'Id,Name'");
            }

            // Parse Id
            int id;
            if (!int.TryParse(parts[0], out id))
            {
                throw new ArgumentException("Invalid Id format.");
            }

            // Return a new Item instance
            return new Item { Id = id, Name = parts[1] };
        }
    }
}
