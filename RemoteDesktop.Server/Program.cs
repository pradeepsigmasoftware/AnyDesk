var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();
app.MapGet("/health", () => Results.Ok(new { status = "ok", service = "RemoteDesktop.Server", version = "0.1.0" }));
app.MapGet("/", () => Results.Redirect("/health"));
app.Run();
