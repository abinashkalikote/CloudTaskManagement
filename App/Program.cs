using App.Web;

var builder = WebApplication.CreateBuilder(args);

builder.UseApp();

var app = builder.Build();

app.HttpPipelineConfiguration();