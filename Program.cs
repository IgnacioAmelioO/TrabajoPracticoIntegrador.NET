using Domain.Services;
using Domain.Model;



var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpLogging(o => { });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    //Falta configurar de manera correcta        
    app.UseHttpLogging();
}

app.UseHttpsRedirection();




app.MapGet("/personas/{id_persona}", (int id_persona) =>
{
    PersonaService personaService = new PersonaService();

    Persona persona = personaService.Get(id_persona);

    if (persona == null)
    {
        return Results.NotFound();
    }

    var dto = new DTOs.Persona
    {
        Id_persona = persona.Id_persona,
        Legajo = persona.Legajo,
        Nombre = persona.Nombre,
        Apellido = persona.Apellido,
        Email = persona.Email,
        Direccion= persona.Direccion,
        Telefono= persona.Telefono,
        Tipo_persona=persona.Tipo_persona,
        Fecha_nac= persona.Fecha_nac,
        Id_plan=persona.Id_plan,
    };

    return Results.Ok(dto);
})
.WithName("GetPersona")
.Produces<DTOs.Persona>(StatusCodes.Status200OK)
.Produces(StatusCodes.Status404NotFound);

app.MapGet("/personas", () =>
{
    PersonaService personaService = new PersonaService();

    var personas = personaService.GetAll();

    var dtos = personas.Select(persona => new DTOs.Persona
    {
        Id_persona = persona.Id_persona,
        Nombre = persona.Nombre,
        Apellido = persona.Apellido,
        Email = persona.Email,
        Fecha_nac = persona.Fecha_nac,
        Direccion = persona.Direccion,
        Telefono = persona.Telefono,
        Legajo = persona.Legajo,
        Tipo_persona = persona.Tipo_persona,
        Id_plan = persona.Id_plan
    }).ToList();


    return Results.Ok(dtos);
})
.WithName("GetAllPersonas")
.Produces<List<DTOs.Persona>>(StatusCodes.Status200OK);

app.MapPost("/personas", (DTOs.Persona dto) =>
{
    try
    {
        var personaService = new PersonaService();

        var persona = new Persona(
            dto.Id_persona,
            dto.Nombre,
            dto.Apellido,
            dto.Direccion,
            dto.Email,
            dto.Telefono,
            dto.Fecha_nac,
            dto.Legajo,
            dto.Tipo_persona,
            dto.Id_plan
        );

        personaService.Add(persona);

        var dtoResultado = new DTOs.Persona
        {
            Id_persona = persona.Id_persona,
            Legajo = persona.Legajo,
            Nombre = persona.Nombre,
            Apellido = persona.Apellido,
            Email = persona.Email,
            Direccion = persona.Direccion,
            Telefono = persona.Telefono,
            Tipo_persona = persona.Tipo_persona,
            Fecha_nac = persona.Fecha_nac,
            Id_plan = persona.Id_plan,
        };

        return Results.Created($"/personas/{dtoResultado.Id_persona}", dtoResultado);
    }
    catch (ArgumentException ex)
    {
        return Results.BadRequest(new { error = ex.Message });
    }
})
.WithName("AddPersona")
.Produces<DTOs.Persona>(StatusCodes.Status201Created)
.Produces(StatusCodes.Status400BadRequest);


app.MapPut("/personas", (DTOs.Persona dto) =>
{
    try
    {
        var personaService = new PersonaService();

        var persona = new Persona(
            dto.Id_persona,
            dto.Nombre,
            dto.Apellido,
            dto.Direccion,
            dto.Email,
            dto.Telefono,
            dto.Fecha_nac,
            dto.Legajo,
            dto.Tipo_persona,
            dto.Id_plan
        );

        var found = personaService.Update(persona);

        if (!found)
            return Results.NotFound();

        return Results.NoContent();
    }
    catch (ArgumentException ex)
    {
        return Results.BadRequest(new { error = ex.Message });
    }
})
.WithName("UpdatePersona")
.Produces(StatusCodes.Status204NoContent)
.Produces(StatusCodes.Status404NotFound)
.Produces(StatusCodes.Status400BadRequest);

// DELETE /personas/{id_persona}
app.MapDelete("/personas/{id_persona}", (int id_persona) =>
{
    var personaService = new PersonaService();

    var deleted = personaService.Delete(id_persona);

    if (!deleted)
        return Results.NotFound();

    return Results.NoContent();
})
.WithName("DeletePersona")
.Produces(StatusCodes.Status204NoContent)
.Produces(StatusCodes.Status404NotFound);


app.Run();