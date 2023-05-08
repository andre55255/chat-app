using Chat.API.Extensions;
using Chat.API.Filters;
using Chat.API.Hub;
using Chat.Helpers;
using Microsoft.Extensions.FileProviders;

var builder = WebApplication.CreateBuilder(args);

//// Configs services
// Add Controllers
builder.Services.AddControllers(x =>
{
    x.Filters.Add(typeof(AuthorizeFilter));
})
.AddCustomResponse();

// Config Endpoints
builder.Services.AddEndpointsApiExplorer();

// Add MongoContexts
builder.Services.AddMongoDbContext(builder.Configuration);

// Add SignalR config
builder.Services.AddSignalRServiceApp();

// Add Config Jwt
builder.Services.AddAuthJwt(builder.Configuration);

// Add Cors
builder.Services.AddCors(builder.Configuration);

// Add Repositories
builder.Services.AddRepositories();

// Add Services
builder.Services.AddServices();

// Add Swagger
builder.Services.AddSwaggerGeneration(builder.Configuration);

// Config AutoMapper
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

////Configs app middlewares
var app = builder.Build();

// Config Is Dev
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
// Config Swagger page
app.UseSwagger()
   .UseSwaggerUI(c => c.SwaggerEndpoint($"/swagger/v{builder.Configuration[ConfigAppSettings.VersionApi]}/swagger.json", "Chat.API"));

// Config HttpContext
app.UseHttpContext();

// Config Cors
app.UseCors(ConfigPolicy.Cors);

// Config routing
app.UseRouting();

// Config static files
app.UseStaticFiles();
app.UseStaticFiles(new StaticFileOptions()
{
    FileProvider = new PhysicalFileProvider(
               Path.Combine(Directory.GetCurrentDirectory(), @"Directory")),
    RequestPath = new PathString("/Static")
});
app.UseDirectoryBrowser(new DirectoryBrowserOptions()
{
    FileProvider = new PhysicalFileProvider(
    Path.Combine(Directory.GetCurrentDirectory(), @"Directory")),
    RequestPath = new PathString("/Static")
});

// Config authentication/authorization
app.UseAuthentication();
app.UseAuthorization();

// Config endpoints
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
    endpoints.MapHub<MessageHub>("/MessagesAllClientsToString");
});

// Config mapping controllers app
app.MapControllers();

app.Run();
