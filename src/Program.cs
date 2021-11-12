using API;
using API.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<AppDbContext>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors();
builder.Services.AddAuthorization();

var key = Encoding.ASCII.GetBytes(Settings.Secret);

builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
    .AddJwtBearer(x =>
    {
        x.RequireHttpsMetadata = false;
        x.SaveToken = true;
        x.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = false,
            ValidateAudience = false
        };
    });

var app = builder.Build();
app.UseSwagger();

if (app.Environment.IsDevelopment())
    app.UseDeveloperExceptionPage();


#region Login
app.MapPost("/login", async ([FromServices] AppDbContext context, [FromBody] FuncionarioDTO usuario) =>
{
    var funcionario = await context.Funcionarios.FirstOrDefaultAsync(x => x.Matricula == usuario.Matricula && x.Senha == usuario.Senha);

    Random generator = new Random();
    String r = generator.Next(0, 1000000).ToString("D6");

    if (funcionario == null)
        return Results.NotFound(new { message = "Matrícula ou Senha incorretos" });

    var token = TokenService.Generate(funcionario);

    funcionario.Senha = string.Empty;

    return Results.Ok(new
    {
        funcionario = funcionario,
        token = token,
    });
}).AllowAnonymous();
#endregion

#region Funcionarios
app.MapGet("/funcionarios", async ([FromServices] AppDbContext context) =>
{
    var funcionarios = await context.Funcionarios.ToListAsync();

    return Results.Ok(funcionarios);
}).AllowAnonymous();

app.MapGet("/funcionarios/{id}", async ([FromServices] AppDbContext context, [FromRoute] Guid id) =>
{
    var funcionario = await context.Funcionarios.FirstOrDefaultAsync(x => x.Id == id);

    return Results.Ok(funcionario);
}).AllowAnonymous();

app.MapPost("/funcionarios", async ([FromServices] AppDbContext context, [FromBody] FuncionarioViewModel model) =>
{
    if (!model.IsValid)
        return Results.BadRequest(model.Notifications);

    var funcionario = model.MapTo();

    context.Funcionarios.Add(funcionario);
    await context.SaveChangesAsync();

    return Results.Ok(funcionario);
}).AllowAnonymous();

app.MapPut("/funcionarios/{id}", async ([FromServices] AppDbContext context, [FromRoute] Guid id, [FromBody] FuncionarioViewModel model) =>
{
    if (!model.IsValid)
        return Results.BadRequest(model.Notifications);

    var funcionario = model.MapTo();

    context.Funcionarios.Update(funcionario);
    await context.SaveChangesAsync();

    return Results.Ok(funcionario);
}).AllowAnonymous();

app.MapDelete("/funcionarios/{id}", async ([FromServices] AppDbContext context, [FromRoute] Guid id, [FromBody] FuncionarioViewModel model) =>
{
    if (!model.IsValid)
        return Results.BadRequest(model.Notifications);

    var funcionario = model.MapTo();

    context.Funcionarios.Remove(funcionario);
    await context.SaveChangesAsync();

    return Results.Ok(funcionario);
}).AllowAnonymous();
#endregion

#region Pontos
app.MapGet("/funcionarios/{id}/pontos", async ([FromServices] AppDbContext context, [FromRoute] Guid id) =>
{
    var pontos = await context.Pontos.Where(x => x.funcionarioId == id).ToListAsync();

    return Results.Ok(pontos);
}).AllowAnonymous();

app.MapPost("/funcionarios/{id}/pontos", async ([FromServices] AppDbContext context, [FromRoute] Guid id, [FromBody] PontoViewModel model) =>
{
    if (!model.IsValid)
        return Results.BadRequest(model.Notifications);

    model.FuncionarioId = id;

    var ponto = model.MapTo();

    await context.Pontos.AddAsync(ponto);
    await context.SaveChangesAsync();

    return Results.Ok(ponto);
}).AllowAnonymous();

app.MapPut("/funcionarios/{id}/pontos", async ([FromServices] AppDbContext context, [FromRoute] Guid id, [FromBody] PontoViewModel model) =>
{
    model.FuncionarioId = id;

    var ponto = model.MapTo();

    context.Pontos.Update(ponto);
    await context.SaveChangesAsync();

    return Results.Ok(ponto);
}).AllowAnonymous();

app.MapDelete("/funcionarios/{id}/pontos", async ([FromServices] AppDbContext context, [FromRoute] Guid id, [FromBody] PontoViewModel model) =>
{
    model.FuncionarioId = id;

    var ponto = model.MapTo();

    context.Pontos.Remove(ponto);
    await context.SaveChangesAsync();

    return Results.Ok(ponto);
}).AllowAnonymous();
#endregion

app.UseAuthorization();

app.UseCors(x => x.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());

app.UseAuthentication();

app.Run();
