using TaskList;
using TaskList.Core;

if (args.Length > 0)
{
    TaskListCommandLine.Main(args);
}
else
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Services.AddSingleton<TaskManager>();

    // Add services to the container.
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();
    builder.Services.AddControllers(); // Register controllers

    var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseHttpsRedirection();

    app.UseAuthorization();

    app.MapControllers(); // Map controllers

    app.Run();
}