using System.Xml.Serialization;

namespace CzSoft.Database;

[XmlRoot(ElementName = "Root")]
public class Tags
{

    [XmlElement(ElementName = "tag")]
    public List<string> Tag { get; set; }

    internal static Tags Parse(IReadOnlyList<string> to)
    {
        return new() { Tag = to.ToList() };
    }
}
