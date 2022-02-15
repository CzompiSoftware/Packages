using CzomPack.Attributes;

namespace CzSoft.Database.Model;

[ToString]
public partial class ProductVersion
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public string Name { get; set; }
    public string ChangeLog { get; set; }
    public string Build { get; set; }
    public DateTime Published { get; set; }

    public virtual Product Product { get; set; }
}
