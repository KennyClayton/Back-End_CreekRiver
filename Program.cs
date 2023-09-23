using CreekRiver.Models;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http.Json;

// allows passing datetimes without time zone data 
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

var builder = WebApplication.CreateBuilder(args);

//* allows our api endpoints to access the database through Entity Framework Core
// also below, "builder.Configuration["CreekRiverDbConnectionString"] retrieves the connection string that we stored in the secrets manager so that EF Core can use it to connect to the database. Don't worry about what the others are doing for now."
builder.Services.AddNpgsql<CreekRiverDbContext>(builder.Configuration["CreekRiverDbConnectionString"]);

// Set the JSON serializer options
builder.Services.Configure<JsonOptions>(options =>
{
    options.SerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
});


// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

//^ ENDPOINT to get all campsites
app.MapGet("/api/campsites", (CreekRiverDbContext db) => // "We provide the endpoint access to the DbContext for our database by adding another param to the handler"
{
    return db.Campsites.ToList(); //EF Core is turning this method chain into a SQL query...and then turning the tabular data that comes back from the database into .NET objects
});
// For the above endpoint..."A few things to notice about the endpoint:
//Linq methods can be chained to db.Campsites, like ToList. Underneath this seemingly simple line of code, EF Core, is turning this method chain into a SQL query: SELECT Id, Nickname, ImageUrl, CampsiteTypeId FROM "Campsites";, and then turning the tabular data that comes back from the database into .NET objects! ASP.NET is serializing those .NET objects into JSON to send back to the client.
//We provide the endpoint access to the DbContext for our database by adding another param to the handler. This is a rudimentary form of dependency injection, where the framework sees a dependency that the handler requires, and passes in an instance of it as an arg so that the handler can use it.""

//^ ENDPOINT to get a campsite by ID with CampsiteType
// app.MapGet("/api/campsites/{id}", (CreekRiverDbContext db, int id) =>
// {
//     return db.Campsites.Include(c => c.CampsiteType).Single(c => c.Id == id);
// });
//above version is original from curriculum. Below is the Chat version that uses "httpContext.Response.StatusCode is a property within the ASP.NET Core HttpContext object that represents the HTTP status code to be sent back to the client as part of an HTTP response"
app.MapGet("/api/campsites/{id}", async (HttpContext httpContext, CreekRiverDbContext db, int id) =>
{
    var campsite = db.Campsites.Include(c => c.CampsiteType).SingleOrDefault(c => c.Id == id);

    if (campsite == null)
    {
        httpContext.Response.StatusCode = 404;
        await httpContext.Response.WriteAsync("Campsite not found");
        return;
    }

    await httpContext.Response.WriteAsJsonAsync(campsite);
});


// For the above endpoint..."Include is a method that will add related data to an entity. Because our campsite has a CampsiteType property where we can store that data, Include will add a JOIN in the underlying SQL query to the CampsiteTypes table. This is the same functionality that JSON Server provided with the _expand query string param.
// Single is like First in that it looks for one matching item based on the expression, but unlike First will throw an Exception if it finds more than one that matches. For something like a primary key, where there is definitely only one row that should match the query, Single is a good way to express that.

//^ ENDPOINT to add a campsite to the database
app.MapPost("/api/campsites", (CreekRiverDbContext db, Campsite campsite) =>
{
    db.Campsites.Add(campsite);
    db.SaveChanges();
    return Results.Created($"/api/campsites/{campsite.Id}", campsite);
});



//^ ENDPOINT to delete a campsite
app.MapDelete("/api/campsites/{id}", (CreekRiverDbContext db, int id) =>
{
    Campsite campsite = db.Campsites.SingleOrDefault(campsite => campsite.Id == id);
    if (campsite == null)
    {
        return Results.NotFound();
    }
    db.Campsites.Remove(campsite);
    db.SaveChanges();
    return Results.NoContent();

});


//^ ENDPOINT to update campsite details
app.MapPut("/api/campsites/{id}", (CreekRiverDbContext db, int id, Campsite campsite) =>
{
    Campsite campsiteToUpdate = db.Campsites.SingleOrDefault(campsite => campsite.Id == id);
    if (campsiteToUpdate == null)
    {
        return Results.NotFound();
    }
    campsiteToUpdate.Nickname = campsite.Nickname;
    campsiteToUpdate.CampsiteTypeId = campsite.CampsiteTypeId;
    campsiteToUpdate.ImageUrl = campsite.ImageUrl;

    db.SaveChanges();
    return Results.NoContent();
});


//^ ENDPOINT to get reservations and related data
app.MapGet("/api/reservations", (CreekRiverDbContext db) =>
{
    return db.Reservations
        .Include(r => r.UserProfile)
        .Include(r => r.Campsite)
        .ThenInclude(c => c.CampsiteType)
        .OrderBy(res => res.CheckinDate)
        .ToList();
});


//^ ENDPOINT to create a reservation
app.MapPost("/api/reservations", (CreekRiverDbContext db, Reservation newRes) =>
{
    try
    {
        db.Reservations.Add(newRes);
        db.SaveChanges();
        return Results.Created($"/api/reservations/{newRes.Id}", newRes);
    }
    catch (DbUpdateException)
    {
        return Results.BadRequest("Invalid data submitted");
    }
});


//^ ENDPOINT to cancel a reservation
app.MapDelete("/api/reservations/{id}", (CreekRiverDbContext db, int id) =>
{
    Reservation reservation = db.Reservations.SingleOrDefault(reservation => reservation.Id == id);
    if (reservation == null)
    {
        return Results.NotFound();
    }
    db.Reservations.Remove(reservation);
    db.SaveChanges();
    return Results.NoContent();
});






app.Run();

