using Microsoft.EntityFrameworkCore;
using MvcCoreSessionEmpleados.Data;
using MvcCoreSessionEmpleados.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

//LO AÑADIMOS SIEMPRE QUE USEMOS SESSION
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession();

//AHORA SÍ VAMOS A AÑADIR CACHE PERSONALIZADO
builder.Services.AddMemoryCache();

//LO AÑADIMOS PARA EL REPO
builder.Services.AddTransient<RepositoryEmpleados>();

string connectionString = builder.Configuration.GetConnectionString("SqlHospital");
builder.Services.AddDbContext<HospitalContext>
    (options => options.UseSqlServer(connectionString));




var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();
//--------------debemos de incluir useSession para que funcione----------------
app.UseSession();


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
