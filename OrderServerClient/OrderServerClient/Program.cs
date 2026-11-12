

var builder = WebApplication.CreateBuilder(args);

// A ÚNICA LINHA QUE FALTAVA PARA OS CONTROLLERS ACORDAREM
builder.Services.AddControllers();




var app = builder.Build();

// O TEU CÓDIGO ORIGINAL INTACTO
app.MapControllers();

app.MapStaticAssets();

app.UseDefaultFiles();
app.UseStaticFiles();

app.Run();