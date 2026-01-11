
using Microsoft.Data.SqlClient;
using System.Text.RegularExpressions;

var builder = WebApplication.CreateBuilder(args);

// --- Log the effective connection string (mask the password) ---
var raw = builder.Configuration.GetConnectionString("DefaultConnection") ?? "<null>";
var masked = Regex.Replace(raw, @"(?i)(Password\s*=\s*)[^;]+", "$1****");
Console.WriteLine($"[Startup] ConnectionStrings:DefaultConnection = {masked}");

// Add MVC services
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Typical ASP.NET Core middleware pipeline
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

// ✅ Conventional MVC route: default points to Products/Index to match launchSettings.json
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Products}/{action=Index}/{id?}");

// ✅ Simple root endpoint to avoid 404s if you browse to "/"
app.MapGet("/", () => Results.Text("App running. Try /Products, /_dbinfo, or /_productsping"));

// ✅ Diagnostic endpoint: shows DB, login, schema your app is actually using
app.MapGet("/_dbinfo", async (IConfiguration config) =>
{
    var cs = config.GetConnectionString("DefaultConnection");
    await using var conn = new SqlConnection(cs);
    await conn.OpenAsync();

    await using var cmd = conn.CreateCommand();
    cmd.CommandText = "SELECT DB_NAME() AS CurrentDb, SUSER_SNAME() AS LoginName, SCHEMA_NAME() AS DefaultSchema;";
    await using var rdr = await cmd.ExecuteReaderAsync();

    var result = new List<Dictionary<string, object?>>();
    while (await rdr.ReadAsync())
    {
        result.Add(new Dictionary<string, object?>
        {
            ["CurrentDb"] = rdr["CurrentDb"],
            ["LoginName"] = rdr["LoginName"],
            ["DefaultSchema"] = rdr["DefaultSchema"]
        });
    }
    return Results.Json(result);
});

// ✅ Diagnostic endpoint: verifies dbo.Products exists and is queryable from the app
app.MapGet("/_productsping", async (IConfiguration config) =>
{
    var cs = config.GetConnectionString("DefaultConnection");
    await using var conn = new SqlConnection(cs);
    await conn.OpenAsync();

    await using var cmd = conn.CreateCommand();
    cmd.CommandText = "SELECT TOP 1 1 FROM dbo.Products";
    var ok = await cmd.ExecuteScalarAsync(); // throws if table not found
    return Results.Text("OK: dbo.Products is reachable.");
});

app.Run();
