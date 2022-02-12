using CzomPack.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CzSoft.Database.Model;

[ToString]
public partial class Package
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string IconUrl { get; set; }
    public string Description { get; set; }
    public IReadOnlyList<string> Authors { get; set; }
    public string Version { get; set; }
    public long VersionDownloads { get; set; }
    public long Downloads { get; set; }
    public DateTime Published { get; set; }
    public bool IsListed { get; set; }

}
