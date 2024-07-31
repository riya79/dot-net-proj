using LanguageWorker;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<LanguageWorker>();

var host = builder.Build();
host.Run();
