using Microsoft.EntityFrameworkCore;
using MinimalApi.Api;
using MinimalAPI;

var builder = WebApplication.CreateBuilder(args);
// DI - Dependency Injection (Injeção de Independencia)
builder.Services.AddDbContext<StudentDb>(opt => opt.UseInMemoryDatabase("StudentList"));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();
// Add services to the container.

builder.Services.AddControllers();
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

app.UseAuthorization();

app.MapControllers();

// MapGroup - Reduz a repetição de código
var studentItems = app.MapGroup("/studentitems");

studentItems.MapGet("/", ReadAllStudents);
studentItems.MapGet("/Active", ReadActiveStudents);
studentItems.MapGet("/{id}", ReadStudentById);
studentItems.MapPost("/{id}", CreateStudent);
studentItems.MapPut("/", UpdateStudent);
studentItems.MapPut("/{id}Delete", DeleteStudent);
studentItems.MapDelete("/", RemoveStudent);


app.Run();

static async Task<IResult> ReadAllStudents(StudentDb db)
{
    return TypedResults.Ok(await db.Students.ToArrayAsync());
}

static async Task<IResult> ReadActiveStudents(StudentDb db)
{
    return TypedResults.Ok(await db.Students.Where(i => i.Active == true).ToListAsync());
}

static async Task<IResult> ReadStudentById(int id, StudentDb db)
{
    return await db.Students.FindAsync(id)
        is Student student
            ?TypedResults.Ok(student)
            :TypedResults.NotFound();
}

static async Task<IResult> CreateStudent(StudentDTO studentDTO, StudentDb db)
{
    var student = new Student
    {
        Name = studentDTO.Name,
        Surname = studentDTO.Surname,
        Age = studentDTO.Age,
    };
    db.Students.Add(student);
    await db.SaveChangesAsync();
    return TypedResults.Created($"/studentitems/{student.Id}", student);
}

static async Task<IResult> UpdateStudent(int id, StudentDTO inputStudent, StudentDb db)
{
    var student = await db.Students.FindAsync(id);

    if (student is null) return TypedResults.NotFound();
    student.Name = inputStudent.Name;
    student.Surname = inputStudent.Surname;
    student.Age = inputStudent.Age;
    await db.SaveChangesAsync();
    return TypedResults.Ok("Aluno Atualizado com Sucesso");
}

static async Task<IResult> DeleteStudent(int id, StudentDb db)
{
    var student = await db.Students.FindAsync(id);

    if(student is null) return TypedResults.NotFound();

    student.Active = false;
    await db.SaveChangesAsync();
    return TypedResults.Ok("Aluno deletado com Sucesso");
}

static async Task<IResult> RemoveStudent(int id, StudentDb db)
{
    if(await db.Students.FindAsync(id) is Student student)
    {
        db.Remove(student);
        await db.SaveChangesAsync();
        return TypedResults.NoContent();
    }
    return TypedResults.NotFound();
}
