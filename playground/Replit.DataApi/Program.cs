using System.Collections;
using System.Text;
using Newtonsoft.Json;

var builder = WebApplication.CreateBuilder(args);
/*builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "Replit db c# apis",
                               Version = "v1" });
});
*/
var app = builder.Build();

/*
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json",
                                    $"{builder.Environment.ApplicationName} v1"));
}*/

var replitEnv = new ReplitEnv(
      Environment.GetEnvironmentVariable("REPL_ID"),
      Environment.GetEnvironmentVariable("REPLIT_DB_URL")
  );
Console.WriteLine($"== ℹ️ {replitEnv.Id} ==");

app.UseMinimalSecurity(replitEnv.Id);


app.MapGet("/", () => "Hello World!");

app.MapGet("/v1/dbinfo", async (HttpContext context) => {
  
  string content = $@"
<h1>Values</h1>
<ul>
<li>REPLIT_ID     : {replitEnv.Id}</li>
<li>REPLIT_DB_URL : <a href=""{replitEnv.DbUrl}"">{replitEnv.DbUrl}</a></li>
</ul>
";
  context.Response.ContentType = "text/html";
  context.Response.StatusCode = 200;
  await context.Response.WriteAsync(content);
});

app.MapGet("/v1/get/{key}", async (string key) => {
  var result = await GetValue(replitEnv.DbUrl, key);
  return result;
});


app.MapGet("/v1/set/{key}/{value}", async (string key, string value) => {
  var result = await SetValue(replitEnv.DbUrl, key, value);
  return result 
    ? $"ok, key [{key}] was set to [{value}]"
    : $"not ok, key [{key}] was NOT set to [{value}] ";
});


app.Run();

//----------------------------

async Task<bool> SetValue(string host, string key, string value){
  using var client = new HttpClient();
  client.DefaultRequestHeaders.Add("User-Agent", "Replit C# Client");
  var builder = new UriBuilder(host);
  builder.Query = $"{key}={value}";
  var url = builder.ToString();
  var json = $"{{ \"{key}\"=\"{value}\"}}";
  var data = new StringContent(json, Encoding.UTF8, "application/json");

  var result = await client.PostAsync(url,data);
  Console.WriteLine($"-- SET : {result.StatusCode} {key}={value}") ;
  return ((int)result.StatusCode) == 200;
}

async Task<string> GetValue(string host, string key){
  using var client = new HttpClient();
  client.DefaultRequestHeaders.Add("User-Agent", "Replit C# Client");
  var url = $"{host}/{key}";
  try 
  {
    var content = await client.GetStringAsync(url);
    return content;
  } catch (Exception e) {
    Console.WriteLine($"-- ERROR -- : {e.Message}");
    Console.WriteLine($"------------: url = {url}");
    return "not found";
  }
  
}
record ReplitEnv(string Id, string DbUrl);

public static class MiddleWares
{
  public static void UseMinimalSecurity(this WebApplication app, string uniqueKey)
  {
    app.Use(async (context, next) =>
    {
        //Console.WriteLine("== ⚠️ Security MiddleWare called! ==");
        var key = context.Request.Query["key"];
        if (!string.IsNullOrWhiteSpace(key))
        {
            if(key == uniqueKey)
            {
               await next(context);
               return;
            }
        }
        context.Response.StatusCode = 401;
        await context.Response.WriteAsync("you need a secret key to use this api");
    });
  }
}
