using DotnetAPI.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options=> {
    
    options.AddPolicy("DevCors", corsBuilder => {
        corsBuilder.WithOrigins("http://localhost").AllowAnyMethod().AllowAnyHeader().AllowCredentials();
    });

     options.AddPolicy("ProdCors", corsBuilder => {
        corsBuilder.WithOrigins("https://Production").AllowAnyMethod().AllowAnyHeader().AllowCredentials();
    });
});
builder.Services.AddScoped<IUserRepository, UserRepository>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseCors("DevCors");
}else {
    app.UseCors("ProdCors");
    app.UseHttpsRedirection();
}

app.MapControllers();





app.Run();

