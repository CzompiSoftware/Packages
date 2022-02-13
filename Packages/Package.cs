
// Add services to the container.


// Configure the HTTP request pipeline.



using BaGet.Protocol;
using BaGet.Protocol.Models;
using CzSoft.Database.Model;
using NuGet.Versioning;
using System.Collections.Generic;

public class Package
{
    public Package(NuGetClient client, SearchResult searchResult, string version = null)
    {
        version ??= searchResult.Version;

        Id = searchResult.PackageId;
        Name = string.IsNullOrEmpty(searchResult.Title) ? searchResult.Title: searchResult.PackageId;
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
    }

    public string Id { get; set; }
    public string Name { get; set; }
    public string LogoUrl { get; set; }
    public string Description { get; set; }
    public IReadOnlyList<string> Authors { get; set; }
    public string Version { get; set; }
    public long TotalDownloads { get; set; }
    public long Downloads { get; set; }
    public DateTime Published { get; set; }
    public bool IsPublic { get; set; }

}