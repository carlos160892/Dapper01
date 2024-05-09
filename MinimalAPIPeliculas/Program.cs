using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.OutputCaching;
using MinimalAPIPeliculas.EndPoints;
using MinimalAPIPeliculas.Entidades;
using MinimalAPIPeliculas.Repositorios;

var builder = WebApplication.CreateBuilder(args);
var OrigenesPermitidos = builder.Configuration.GetValue<string>("OrigenesPermitidos")!;

//Inicio del area de los servicios

builder.Services.AddOutputCache();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAutoMapper(typeof(Program));

builder.Services.AddScoped<IRepositorioGeneros, RepositorioGeneros>();

//Fin del area de los servicios

builder.Services.AddCors(opciones =>
{
    opciones.AddDefaultPolicy(configuracion =>
    {
        configuracion.WithOrigins(OrigenesPermitidos).AllowAnyHeader().AllowAnyMethod();
    });

    opciones.AddPolicy("Libre", configuracion =>
    {
        configuracion.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
    });
});

var app = builder.Build();
// Inicio de área de los middleware


if (builder.Environment.IsDevelopment()) {
    app.UseSwagger();
    app.UseSwaggerUI();
}  


app.UseCors();

app.UseOutputCache();


app.MapGet("/", () => "Hello World!");

//EndPoint para obtener listado de generos

app.MapGroup("/generos").MapGeneros();



app.Run();


