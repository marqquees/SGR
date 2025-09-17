using Microsoft.EntityFrameworkCore;
using SGR.Components;
using SGR.Data;
using SGR.Service;

var builder = WebApplication.CreateBuilder(args);

// Adicionar servi�os ao container.
builder.Services.AddRazorComponents().AddInteractiveServerComponents();
builder.Services.AddControllers();

// Configurar o DbContext para usar SQL Server com a string de conex�o.
builder.Services.AddDbContext<EquipmentContext>(options => 
    options.UseSqlServer(builder.Configuration.GetConnectionString("DbConnectionWindows")));

// Servi�o de Gerenciamento do Invent�rio.
builder.Services.AddScoped<EquipmentOperation>();
// Servi�o Gerenciador de Modais para a interface do usu�rio.
builder.Services.AddScoped<ModalManager>();
// Servi�o de Limpeza de Arquivos Tempor�rios.
builder.Services.AddHostedService<TempFileCleanup>();

var app = builder.Build();

// Configurar o papeline de solicita��es HTTP.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Erro", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.MapControllers();
app.UseStaticFiles();
app.UseAntiforgery();
app.MapRazorComponents<App>().AddInteractiveServerRenderMode();
app.Run();
