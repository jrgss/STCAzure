using Azure.Storage.Blobs;
using Microsoft.EntityFrameworkCore;
using STC.Data;
using STC.Repository.Interfaces;
using STC.Repository.ReposSql;
using STC.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
    
string connectionString = builder.Configuration.GetConnectionString("SqlAzure");
//string connectionString = builder.Configuration.GetConnectionString("SqlProyecto");
builder.Services.AddTransient<IRepositoryInsertarDeApi, RepositoryInsertarDeApiSql>();
builder.Services.AddTransient<IRepositoryCompeticion, RepositoryCompeticionSql>();
builder.Services.AddSingleton<ServiceApi>();
builder.Services.AddTransient<ServiceApiSTC>();

string azureKeys =
    builder.Configuration.GetValue<string>
    ("AzureKeys:StorageAccount");
BlobServiceClient blobServiceClient =
    new BlobServiceClient(azureKeys);
builder.Services.AddTransient<BlobServiceClient>
    (x => blobServiceClient);
builder.Services.AddTransient<ServiceStorageBlobs>();


builder.Services.AddDbContext<StcContext>(options => options.UseSqlServer(connectionString));




builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

//LO DE LOS ENDPOINTS ES AÑADIDO NUEVO PARA PONER TENER DOS IACTIONRESULT INDEX
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
      name: "default",
      pattern: "{controller=Home}/{action=Index}/{id?}");
});
app.Run();
