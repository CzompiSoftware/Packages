using CzomPack.Attributes;
using System.Xml;

namespace CzSoft.Database.Model;

[ToString]
public partial class Product
{
    public int Id { get; set; }
    public string ShortName { get; set; }
    public string Name { get; set; }
    public string Summary { get; set; }
    public string Description { get; set; }
    public IReadOnlyList<string> Authors { get; set; }
    public string ProgrammingLanguage { get; set; }
    public string Source { get; set; }
    public string LogoUrl { get; set; }
    public string Url { get; set; }
    public int Price { get; set; }
    public string PriceCurrency { get; set; }
    public string BuyLink { get; set; }
    public bool IsPublic { get; set; }
    public bool IsPackage { get; set; }

    public IReadOnlyList<ProductVersion> Dependencies { get; set; }
    public IReadOnlyList<string> Tags { get; set; }

}