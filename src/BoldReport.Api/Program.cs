using BoldReports.Writer;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Bold.Licensing.BoldLicenseProvider.RegisterLicense("YOUR LICENSE KEY");

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast")
.WithOpenApi();


app.MapPost("/reports", async (string writerFormat) =>
{
    FileStream inputStream = new FileStream("sales-order-detail.rdl", FileMode.Open, FileAccess.Read);
    MemoryStream reportStream = new MemoryStream();
    inputStream.CopyTo(reportStream);
    reportStream.Position = 0;
    inputStream.Close();
    ReportWriter writer = new ReportWriter();

    string fileName = null;
    WriterFormat format;
    string type = null;

    if (writerFormat == "PDF")
    {
        fileName = "sales-order-detail.pdf";
        type = "pdf";
        format = WriterFormat.PDF;
    }
    else if (writerFormat == "Word")
    {
        fileName = "sales-order-detail.docx";
        type = "docx";
        format = WriterFormat.Word;
    }
    else if (writerFormat == "CSV")
    {
        fileName = "sales-order-detail.csv";
        type = "csv";
        format = WriterFormat.CSV;
    }
    else
    {
        fileName = "sales-order-detail.xlsx";
        type = "xlsx";
        format = WriterFormat.Excel;
    }

    var monthlySalesData = new[]
    {
        new { Month = "January", Sales = 15000 },
        new { Month = "February", Sales = 18000 },
        new { Month = "March", Sales = 16000 },
        new { Month = "April", Sales = 17000 },
        new { Month = "May", Sales = 29000 }
    };



    writer.ReportProcessingMode = ProcessingMode.Local;
    // Pass the dataset collection for report
    writer.DataSources.Clear();
    writer.DataSources.Add(new BoldReports.Web.ReportDataSource { Name = "DataSet1", Value = monthlySalesData });
    writer.LoadReport(reportStream);
    MemoryStream memoryStream = new MemoryStream();
    writer.Save(memoryStream, format);

    // Download the generated export document to the client side.
    memoryStream.Position = 0;
    FileStreamResult fileStreamResult = new FileStreamResult(memoryStream, "application/" + type);
    fileStreamResult.FileDownloadName = fileName;

    memoryStream.Position = 0;
    byte[] byteArray = memoryStream.ToArray();
    File.WriteAllBytes(fileName, byteArray);

    return fileStreamResult;
})
.WithName("GetReports")
.WithOpenApi();


app.Run();

internal record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
