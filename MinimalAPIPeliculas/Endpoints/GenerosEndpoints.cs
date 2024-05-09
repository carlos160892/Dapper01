using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.OutputCaching;
using MinimalAPIPeliculas.DTOs;
using MinimalAPIPeliculas.Entidades;
using MinimalAPIPeliculas.Repositorios;

namespace MinimalAPIPeliculas.EndPoints
{
    public static class GenerosEndpoints
    {

        public static RouteGroupBuilder MapGeneros(this RouteGroupBuilder group) 
        {


            group.MapGet("", ObtenerGeneros).CacheOutput(c => c.Expire(TimeSpan.FromSeconds(15)).Tag("generos-get"));
            group.MapGet("/{id:int}", ObtenerGeneroPorId);
            group.MapPost("", CrearGenero);
            group.MapPut("/{id:int}", ActualizarGenero);
            group.MapDelete("/{id:int}", EliminarGenero);
            return group;

        }



        static async Task<Ok<List<GeneroDTO>>> ObtenerGeneros(
            IRepositorioGeneros repositorio,
            IMapper mapper)
        {
            var generos = await repositorio.ObtenerTodos();

            var generosDTO = mapper.Map<List<GeneroDTO>>(generos);

            return TypedResults.Ok(generosDTO);
        }

        static async Task<Results<Ok<GeneroDTO>, NotFound>> ObtenerGeneroPorId(int id, 
            IRepositorioGeneros repositorio,
            IMapper mapper)
        {
            var genero = await repositorio.ObtenerPorId(id);

            if (genero is null)
            {
                return TypedResults.NotFound();
            }

            var generoDTO = new GeneroDTO
            {
                Id = id,
                Nombre = genero.Nombre
            };


            return TypedResults.Ok(generoDTO);

        }



        static async Task<Created<GeneroDTO>> CrearGenero(CrearGeneroDTO crearGeneroDTO,
            IRepositorioGeneros repositorioGeneros,
            IOutputCacheStore outputCacheStore,
            IMapper mapper)
        {

            var genero = mapper.Map<Genero>(crearGeneroDTO); 
            var id = await repositorioGeneros.CrearGenero(genero);
            await outputCacheStore.EvictByTagAsync("generos-get", default);
            var generoDTO = mapper.Map<GeneroDTO>(genero);

            return TypedResults.Created($"/Generos/{id}", generoDTO);
        }

        static async Task<Results<NoContent, NotFound>> ActualizarGenero(int id, 
            CrearGeneroDTO crearGeneroDTO,
            IRepositorioGeneros repositorio,
            IOutputCacheStore outputCacheStore)
        {
            var existe = await repositorio.Existe(id);
            var genero = new Genero
            {

                Id = id,
                Nombre = crearGeneroDTO.Nombre

            };

            if (!existe)
            {
                return TypedResults.NotFound();
            }


            await repositorio.Actualizar(genero);
            await outputCacheStore.EvictByTagAsync("generos-get", default);
            return TypedResults.NoContent();
        }

        static async Task<Results<NotFound, NoContent>> EliminarGenero(int id, IRepositorioGeneros repositorio,
            IOutputCacheStore outputCacheStore)
        {

            var existe = await repositorio.Existe(id);

            if (!existe)
            {
                return TypedResults.NotFound();
            }

            await repositorio.Eliminar(id);
            await outputCacheStore.EvictByTagAsync("generos-get", default);
            return TypedResults.NoContent();


        }

    }
}
