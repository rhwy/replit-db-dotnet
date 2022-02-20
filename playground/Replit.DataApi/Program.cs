var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.MapGet("/v1.0/{key}", (string key) => {
  return $"value of key {key}";
});
app.Run();
