
// Add services to the container.


// Configure the HTTP request pipeline.



using BaGet.Protocol;
using BaGet.Protocol.Models;
using NuGet.Versioning;

public class Package
{
    public Package()
    {
    }

    internal Package(NuGetClient client, SearchResult searchResult, string version = null) : base()
    {
        version ??= searchResult.Version;
        Id = searchResult.PackageId;
        Name = !string.IsNullOrEmpty(searchResult.Title) ? searchResult.Title : searchResult.PackageId;
        Summary = searchResult.Summary;
        Tags = searchResult.Tags;
        Source = searchResult.ProjectUrl;
        LogoUrl = searchResult.IconUrl;
        Authors = searchResult.Authors;
        Description = searchResult.Description;
        Version = version;
        TotalDownloads = searchResult.TotalDownloads;
        try
        {
            PackageMetadata metadata = client.GetPackageMetadataAsync(Id, new NuGetVersion(Version)).GetAwaiter().GetResult();
            if (metadata is null)
            {
                //Console.WriteLine($"Package '{Id}' with version '{Version}' does not exist");
                //return;
                throw new PackageNotFoundException(Id, new(Version));
            }
            Published = metadata.Published.DateTime;
            IsPublic = metadata.IsListed();
            Downloads = searchResult.Versions.First(ver => ver.Version.Equals(Version)).Downloads;
        }
        catch (Exception)
        {
            throw new PackageNotFoundException(Id, new(Version));
        }

        Console.WriteLine($"[NUGET] Package '{Name}' with version '{Version}' added successfully.");
    }

    public string Id { get; set; }
    public string Name { get; set; }
    public string Summary { get; set; }
    public IReadOnlyList<string> Tags { get; set; }
    public string Source { get; set; }
    public string LogoUrl { get; set; }
    public string Description { get; set; }
    public IReadOnlyList<string> Authors { get; set; }
    public string Version { get; set; }
    public long TotalDownloads { get; set; }
    public long Downloads { get; set; }
    public DateTime Published { get; set; }
    public bool IsPublic { get; set; }

}