using API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<AppDataContext>();
builder.Services.AddCors(options =>
{
    options.AddPolicy(
       "AcessoTotal", builder => builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()
    );
});
var app = builder.Build();


app.MapGet("/", () => "Prova A1");

//ENDPOINTS DE CATEGORIA
//GET: http://localhost:5000/categoria/listar
app.MapGet("/categoria/listar", ([FromServices] AppDataContext ctx) =>
{
    if (ctx.Categorias.Any())
    {
        return Results.Ok(ctx.Categorias.ToList());
    }
    return Results.NotFound("Nenhuma categoria encontrada");
});

//POST: http://localhost:5000/categoria/cadastrar
app.MapPost("/categoria/cadastrar", ([FromServices] AppDataContext ctx, [FromBody] Categoria categoria) =>
{
    ctx.Categorias.Add(categoria);
    ctx.SaveChanges();
    return Results.Created("", categoria);
});

//ENDPOINTS DE TAREFA
//GET: http://localhost:5000/tarefas/listar
app.MapGet("/tarefas/listar", ([FromServices] AppDataContext ctx) =>
{
    if (ctx.Tarefas.Any())
    {
        return Results.Ok(ctx.Tarefas.ToList());
    }
    return Results.NotFound("Nenhuma tarefa encontrada");
});

//POST: http://localhost:5000/tarefas/cadastrar
app.MapPost("/tarefas/cadastrar", ([FromServices] AppDataContext ctx, [FromBody] Tarefa tarefa) =>
{
    Categoria? categoria = ctx.Categorias.Find(tarefa.CategoriaId);
    if (categoria == null)
    {
        return Results.NotFound("Categoria não encontrada");
    }
    tarefa.Categoria = categoria;
    ctx.Tarefas.Add(tarefa);
    ctx.SaveChanges();
    return Results.Created("", tarefa);
});

//PUT: http://localhost:5000/tarefas/alterar/{id}
app.MapPut("/tarefas/alterar/{id}", ([FromServices] AppDataContext ctx, [FromRoute] string id, [FromBody] Tarefa taskAtualizada) =>
{

    Categoria? categoria = ctx.Categorias.Find(taskAtualizada.CategoriaId);
    if (categoria == null)
    {
        return Results.NotFound("Categoria não encontrada");
    }

    Tarefa? tarefa = ctx.Tarefas.FirstOrDefault(x => x.TarefaId == id);

    if (tarefa is not null)
    {
        tarefa.Titulo = taskAtualizada.Titulo;
        tarefa.Descricao = taskAtualizada.Descricao;
        tarefa.Status = taskAtualizada.Status;
        tarefa.Categoria = categoria;

        ctx.Tarefas.Update(tarefa);
        ctx.SaveChanges();
        return Results.Ok("Tarrefa editada com suceso");

    }
    return Results.NotFound("Tarefa não Encotrada");
});

//GET: http://localhost:5000/tarefas/naoconcluidas
app.MapGet("/tarefas/naoconcluidas", ([FromServices] AppDataContext ctx ) =>
{

    Tarefa? tarefa = ctx.Tarefas.ToList().Find(x => x.Status != "Concluída");

    if (tarefa is not null)
    {
        return Results.Ok(tarefa);
    }
    return Results.NotFound("Nenhuma tarefa encontrada");
});

//GET: http://localhost:5000/tarefas/concluidas
app.MapGet("/tarefas/concluidas", ([FromServices] AppDataContext ctx) =>
{
    Tarefa? tarefa = ctx.Tarefas.ToList().Find(x => x.Status == "Concluída");

    if (tarefa is not null)
    {
        return Results.Ok(tarefa);
    }
    return Results.NotFound("Nenhuma tarefa encontrada");
});


//GET  http://localhost:{porta}/tarefas/buscar{product.id}
app.MapGet("/tarefas/buscar/{id}", ([FromRoute] string id, [FromServices] AppDataContext context) =>
{
    //Endpoint com várias linhas de código 
    Tarefa? tarefa = context.Tarefas.FirstOrDefault(x => x.TarefaId == id);

    if (tarefa is null) return Results.NotFound("Tarefa não Encotrada");
    return Results.Ok(tarefa);

});

app.UseCors("AcessoTotal");
app.Run();
