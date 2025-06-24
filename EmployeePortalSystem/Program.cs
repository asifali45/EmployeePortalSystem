using EmployeePortalSystem.Repositories;
using EmployeePortalSystem.Context;
var builder = WebApplication.CreateBuilder(args);


// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddScoped<UserAccessRepository>();
builder.Services.AddScoped<EmployeeCreationRepository>();
builder.Services.AddScoped<AwardRepository>();
builder.Services.AddScoped<AwardContext>();


builder.Services.AddSession();
builder.Services.AddHttpContextAccessor();

builder.Services.AddScoped<IDepartmentRepository, DepartmentRepository>();

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

app.UseSession();


app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=UserAccess}/{action=Login}/{id?}");

app.Run();
