using OrderServerClient.Components;

var builder = WebApplication.CreateBuilder(args);

// A ÚNICA LINHA QUE FALTAVA PARA OS CONTROLLERS ACORDAREM
builder.Services.AddControllers();

// O TEU CÓDIGO ORIGINAL INTACTO
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

var app = builder.Build();

// O TEU CÓDIGO ORIGINAL INTACTO
app.MapControllers();
app.UseAntiforgery();
app.MapStaticAssets();

app.Run();