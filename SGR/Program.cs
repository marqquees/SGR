using Microsoft.EntityFrameworkCore;
using SGR.Components;
using SGR.Data;
using SGR.Services;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Adicionar serviços ao container.
builder.Services.AddRazorComponents().AddInteractiveServerComponents();

// Configurar o DbContext para usar SQLite.
builder.Services.AddDbContext<EquipmentContext>(option => 
    option.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// Serviço de Gerenciamento do Inventário.
builder.Services.AddScoped<EquipmentOperation>();

// Serviço de Limpeza de Arquivos Temporários.
builder.Services.AddHostedService<TempFileCleanup>();

// Serviços de Exportação.
builder.Services.AddScoped<ExportToPdf>();

WebApplication app = builder.Build();

// Aplica as migrações pendentes a base de dados.
using (IServiceScope scope = app.Services.CreateScope())
{
    DbContext db = scope.ServiceProvider.GetRequiredService<EquipmentContext>();
    db.Database.Migrate();
}

// Configurar o papeline de solicitações HTTP.
if (!app.Environment.IsDevelopment()) app.UseHsts();

app.UseStaticFiles();
app.UseAntiforgery();
app.MapRazorComponents<App>().AddInteractiveServerRenderMode();
app.Run();