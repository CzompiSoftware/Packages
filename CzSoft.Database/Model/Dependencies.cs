using System.Xml.Serialization;

namespace CzSoft.Database.Model;

[XmlRoot(ElementName = "Root")]
public class Dependencies
{

    [XmlElement(ElementName = "dependency")]
    public List<Dependency> Dependency { get; set; }

    internal static Dependencies Parse(IReadOnlyList<ProductVersion> to)
    {
        return new() { Dependency = to.Select(x=> new Dependency { ProductId = x.Product.Id, VersionId = x.Id}).ToList() };
    }
}

public class Dependency
{
    [XmlAttribute(AttributeName = "product")]
    public int ProductId { get; set; }

    [XmlAttribute(AttributeName = "version")]
    public int VersionId { get; set; }
}