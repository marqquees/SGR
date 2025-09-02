using Microsoft.EntityFrameworkCore;
using SGR.Components;
using SGR.Data;
using SGR.Models;
using SGR.Service;

var builder = WebApplication.CreateBuilder(args);

// Adicionar serviços ao container.
builder.Services.AddRazorComponents().AddInteractiveServerComponents();
builder.Services.AddControllers();

// Configurar o DbContext para usar SQL Server com a string de conexão.
builder.Services.AddDbContext<EquipmentContext>(options => 
    options.UseSqlServer(builder.Configuration.GetConnectionString("DbConnectionWindows")));

// Serviço de Gerenciamento do Inventário.
builder.Services.AddScoped<OperationService>();
// Serviço Gerenciador de Modais para a interface do usuário.
builder.Services.AddScoped<ModalManager>();
// Serviço de Exportação de Dados para ficheiros em PDF.
builder.Services.AddScoped<ExportToPdf>(); 
// Serviço de Limpeza de Arquivos Temporários.
builder.Services.AddHostedService<TempFileCleanupService>();

var app = builder.Build();

// Configurar o papeline de solicitações HTTP.
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
