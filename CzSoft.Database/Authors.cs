using System.Xml.Serialization;

namespace CzSoft.Database;

[XmlRoot(ElementName = "Root")]
public class Authors
{

    [XmlElement(ElementName = "author")]
    public List<string> Author { get; set; }

    internal static Authors Parse(IReadOnlyList<string> to)
    {
        return new() { Author = to.ToList() };
    }
}
