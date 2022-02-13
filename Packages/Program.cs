using BaGet.Protocol;
using BaGet.Protocol.Models;
using CzSoft.Database;
using Microsoft.EntityFrameworkCore;
using NuGet.Versioning;

namespace Packages;
public class Program
{
    public static void Main(string[] args)
    {
        var cts = new CancellationTokenSource();
        var builder = WebApplication.CreateBuilder(args);
        builder.Services.AddDbContext<CzSoftDatabaseContext>(options =>
        {
            options.UseSqlServer(builder.Configuration["CzSoftDatabase"]);
        }, contextLifetime: ServiceLifetime.Transient, optionsLifetime: ServiceLifetime.Singleton);

        var app = builder.Build();

        NuGetClient client = new("https://nuget.czompisoftware.hu/v3/index.json");
        var db = builder.Configuration["CzSoftDatabase"];
        CzSoftDatabaseContext databaseContext = new(db);
        PackageList packageList = new();
        int AllPackages = 0;
        TaskHelper.RecurringTask(async () =>
        {
            var packages = await client.SearchAsync();
            AllPackages = packages.Select(x => x.Versions.Count).Sum();
            foreach (var package in packages)
            {
                foreach (var version in package.Versions)
                {

                }
            }
            foreach (var prodVer in await databaseContext.ProductVersions.ToListAsync())
            {
                var pkg = packages.FirstOrDefault(pkg => pkg.PackageId == prodVer.Product.Name);
                var pkgVer = pkg.Versions.First(ver => ver.Version.Equals(prodVer.Name));
                packageList.Add(new(prodVer, pkgVer.Downloads, pkg.TotalDownloads));
                AddProductAndVersion(client, databaseContext, package, version);
            }
        }, 60, cts.Token);

        app.MapGet("/updates/{amount?}", (CzSoftDatabaseContext db, int? amount) =>
        {
            //return db.Products.ToList();
            //if (AllPackages != packageList.Count)
            //{
            //    return new ErrorRequest { Status = "LoadingInprogress", Message = "Database is currently updating. Please try again later" };
            //}
            return PackageList.FromEnumerable(packageList.OrderByDescending(order => order.Published));
        });

        app.MapGet("/latest", () =>
        {
            if (AllPackages != packageList.Count)
            {
                return new ErrorRequest { Status = "LoadingInprogress", Message = "Database is currently updating. Please try again later" };
            }
            return PackageList.FromEnumerable(packageList.GroupBy(pkg => pkg.Id).Select(grp => grp.OrderByDescending(order => order.Published).First()));
        });

        
        app.Run();
    }

    private static async void AddProductAndVersion(NuGetClient client, CzSoftDatabaseContext databaseContext, SearchResult package, SearchResultVersion version)
    {
        if (!await databaseContext.Products.AnyAsync(prod => prod.Name.ToLower().Equals(package.PackageId.ToLower())))
        {
            await databaseContext.Products.AddAsync(new()
            {
                Name = package.PackageId,
                Summary = package.Summary,
                Description = package.Description,
                ProgrammingLanguage = "C#",
                Authors = package.Authors,
                Source = package.ProjectUrl,
                Tags = package.Tags,

                IsPackage = true,
                IsPublic = false,
            });
            await databaseContext.SaveChangesAsync();
        }

        var prod = await databaseContext.Products.FirstAsync(prod => prod.Name.ToLower().Equals(package.PackageId.ToLower()));

        if (!await databaseContext.ProductVersions.AnyAsync(prodver => prodver.Product.Name.ToLower().Equals(package.PackageId.ToLower()) && prodver.Name.ToLower().Equals(version.Version.ToLower())))
        {
            try
            {
                PackageMetadata metadata = await client.GetPackageMetadataAsync(package.PackageId, new NuGetVersion(version.Version));
                if (metadata is null)
                {
                    Console.WriteLine($"Package '{package.PackageId}' with version '{version.Version}' does not exist");
                    return;
                }
                await databaseContext.ProductVersions.AddAsync(new()
                {
                    Product = prod,
                    Published = metadata.Published.DateTime,
                    Name = version.Version,
                });
                await databaseContext.SaveChangesAsync();
                Console.WriteLine($"Package '{package.PackageId}' with version '{version.Version}' added successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error occurred during package adding process. Package '{package.PackageId}' with version '{version.Version}' cannot be added.\r\nException: {ex}");
            }
        }
        else
        {
            Console.WriteLine($"Package '{package.PackageId}' with version '{version.Version}' already added, skipping.");
        }
    }

    internal static bool StringEqualsIgnoreCase(string a, string b)
    {
        return a.ToLower().Equals(b.ToLower());
    }
}
