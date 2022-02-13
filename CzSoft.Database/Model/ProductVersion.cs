namespace CzSoft.Database.Model
{
    public class ProductVersion
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ChangeLog { get; set; }
        public string Build { get; set; }
        public DateTime Published { get; set; }

        public Product Product { get; set; }
    }
}