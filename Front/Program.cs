var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();

builder.Services.AddHttpClient("Api", client =>
{
    client.BaseAddress = new Uri("https://localhost:5001");
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}
app.UseStaticFiles();
app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();
app.MapFallbackToPage("/Dashboard/Index");

app.Run();
