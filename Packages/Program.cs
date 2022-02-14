using BaGet.Protocol;
using BaGet.Protocol.Models;
using CzSoft.Database;
using CzSoft.Database.Model;
using Microsoft.EntityFrameworkCore;
using NuGet.Versioning;

namespace Packages;
public class Program
{
    static PackageList PackageList = new();
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
        //CzSoftDatabaseContext databaseContext = new(db);

        TaskHelper.RecurringTask(() => UpdatePackages(client, new(db)), 600, cts.Token);
        TaskHelper.RecurringTask(() => GetPackageList(new(db)), 600, cts.Token);

        app.MapGet("/latest", GetLatest);
        app.MapGet("/updates/{amount?)", GetUpdates);

        app.Run();
    }

    #region Update packages
    private static async void UpdatePackages(NuGetClient client, CzSoftDatabaseContext databaseContext)
    {
        var tmpPkgList = new PackageList();
        var packages = await client.SearchAsync();

        foreach (var package in packages)
        {
            foreach (var version in package.Versions)
            {
                var name = !string.IsNullOrEmpty(package.Title.ToLower()) ? package.PackageId.ToLower() : package.PackageId.ToLower();
                var hasProd = databaseContext.Products.Any(prod => prod.Name.ToLower().Equals(name));
                //var hasProdVer = databaseContext.ProductVersions.Any(prodver => prodver.Product.Name.ToLower().Equals(package.Name.ToLower()) && prodver.Name.ToLower().Equals(package.Version.ToLower()));
                if (!hasProd /*&& !hasProdVer*/) // If product does not exist, than it shouldn't have any versions
                {
                    tmpPkgList.Add(new(client, package, version.Version));
                }
            }
        }

        foreach (var package in tmpPkgList.OrderBy(pkg => pkg.Published))
        {
            AddProductAndVersion(databaseContext, package);
        }

        databaseContext.Dispose();
    }

    private static void AddProductAndVersion(CzSoftDatabaseContext databaseContext, Package package)
    {
        if (!databaseContext.Products.Any(prod => prod.Name.ToLower().Equals(package.Name.ToLower())))
        {
            databaseContext.Products.Add(new()
            {
                Name = package.Id,
                Summary = package.Summary,
                Description = package.Description,
                ProgrammingLanguage = "C#",
                Authors = package.Authors,
                Source = package.Source,
                Tags = package.Tags,

                IsPackage = true,
                IsPublic = false,
            });
            databaseContext.SaveChanges();
        }

        var prod = databaseContext.Products.First(prod => prod.Name.ToLower().Equals(package.Name.ToLower()));

        if (!databaseContext.ProductVersions.Any(prodver => prodver.Product.Name.ToLower().Equals(package.Name.ToLower()) && prodver.Name.ToLower().Equals(package.Version.ToLower())))
        {
            try
            {
                databaseContext.ProductVersions.Add(new()
                {
                    Product = prod,
                    Published = package.Published,
                    Name = package.Version,
                });
                databaseContext.SaveChanges();
                Console.WriteLine($"[MSSQL] Package '{package.Name}' with version '{package.Version}' added successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error occurred during package adding process. Package '{package.Name}' with version '{package.Version}' cannot be added.\r\nException: {ex}");
            }
        }
        else
        {
            Console.WriteLine($"Package '{package.Name}' with version '{package.Version}' already added, skipping.");
        }
        var prodVer = databaseContext.ProductVersions.First(prodVer => prodVer.Product.Name.ToLower().Equals(package.Name.ToLower()) && prodVer.Name.ToLower().Equals(package.Version.ToLower()));

    }
    #endregion

    #region Get package list
    private static void GetPackageList(CzSoftDatabaseContext databaseContext)
    {
        var items = databaseContext.ProductVersions.OrderByDescending(pkg => pkg.Published).ToList();
        
        foreach (var item in items)
        {
            AddPackage(item);
        }

        databaseContext.Dispose();
    }

    private static void AddPackage(ProductVersion productVersion)
    {
        PackageList.Add(new()
        {
            Name = productVersion.Product.Name,
            Version = productVersion.Name,
            Authors = productVersion.Product.Authors,
            Description = productVersion.Product.Description,
            Id = productVersion.Product.Name,
            IsPublic = productVersion.Product.IsPublic,
            LogoUrl = productVersion.Product.LogoUrl,
            Published = productVersion.Published,
            Source = productVersion.Product.Source,
            Summary = productVersion.Product.Summary,
            Tags = productVersion.Product.Tags,
            //Downloads = package.Downloads,
            //TotalDownloads = package.TotalDownloads,
        });
    }
    #endregion

    #region Responses
    private static IResponse GetLatest(HttpContext context, int? amount)
    {
        //if (AllPackages != PackageList.Count)
        //{
        //    return new ErrorResponse { Status = "LoadingInprogress", Message = "Database is currently updating. Please try again later" };
        //}
        var items = PackageList.GroupBy(pkg => pkg.Id).Select(grp => grp.OrderByDescending(order => order.Published).First());
        if (amount != null && amount > 0) items = items.Take(amount ?? 0).ToList();
        return PackageList.FromEnumerable(items.OrderByDescending(order => order.Published));
    }

    private static IResponse GetUpdates(HttpContext context, int? amount)
    {
        //if (AllPackages != PackageList.Count)
        //{
        //    return new ErrorResponse { Status = "LoadingInprogress", Message = "Database is currently updating. Please try again later" };
        //}
        var items = PackageList.OrderByDescending(order => order.Published).ToList();
        if (amount != null && amount > 0) items = items.Take(amount ?? 0).ToList();
        return PackageList.FromEnumerable(items.OrderByDescending(order => order.Published));
    }
    #endregion

}
