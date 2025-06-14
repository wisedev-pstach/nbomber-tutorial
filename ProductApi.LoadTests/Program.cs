using NBomber.Contracts.Stats;
using NBomber.CSharp;
using NBomber.Http.CSharp;
using System.Text;
using System.Text.Json;

// 1. Create HTTP client
var httpClient = new HttpClient();

// Base URL for our API
var baseUrl = "http://localhost:5062";

// 2. Define scenarios for different API operations

// Scenario 1: Test the GET all products endpoint
var getAllProductsScenario = Scenario.Create("get_all_products_scenario", async context =>
{
    var step1 = await Step.Run("get_all_products", context, async () =>
    {
        var request = Http.CreateRequest("GET", $"{baseUrl}/api/products");
        var response = await Http.Send(httpClient, request);
        
        return !response.IsError 
            ? Response.Ok() 
            : Response.Fail();
    });

    return Response.Ok();
})
.WithWarmUpDuration(TimeSpan.FromSeconds(5))
.WithLoadSimulations(
    Simulation.KeepConstant(copies: 50, during: TimeSpan.FromSeconds(30))
);

// Scenario 2: Test the search endpoint (which has our deliberate inefficiency)
var searchProductsScenario = Scenario.Create("search_products_scenario", async context =>
{
    var step1 = await Step.Run("search_products", context, async () =>
    {
        // Generate a random search term
        var searchTerms = new[] { "product", "1", "2", "3", "4", "5" };
        var term = searchTerms[Random.Shared.Next(searchTerms.Length)];
        
        var request = Http.CreateRequest("GET", $"{baseUrl}/api/products/search?term={term}");
        var response = await Http.Send(httpClient, request);
        
        return !response.IsError 
            ? Response.Ok() 
            : Response.Fail();
    });

    return Response.Ok();
})
.WithWarmUpDuration(TimeSpan.FromSeconds(5))
.WithLoadSimulations(
    Simulation.KeepConstant(copies: 50, during: TimeSpan.FromSeconds(30))
);

// Scenario 3: Mixed usage scenario
var mixedScenario = Scenario.Create("mixed_usage_scenario", async context =>
{
    // Get all products step
    var step1 = await Step.Run("get_all_products", context, async () =>
    {
        var request = Http.CreateRequest("GET", $"{baseUrl}/api/products");
        var response = await Http.Send(httpClient, request);
        
        return !response.IsError 
            ? Response.Ok() 
            : Response.Fail();
    });

    // Get product by ID step
    var step2 = await Step.Run("get_product_by_id", context, async () =>
    {
        // Generate a random ID between 1 and 1000
        var id = Random.Shared.Next(1, 1000);
        
        var request = Http.CreateRequest("GET", $"{baseUrl}/api/products/{id}");
        var response = await Http.Send(httpClient, request);
        
        return !response.IsError 
            ? Response.Ok() 
            : Response.Fail();
    });

    // Search products step
    var step3 = await Step.Run("search_products", context, async () =>
    {
        // Generate a random search term
        var searchTerms = new[] { "product", "1", "2", "3", "4", "5" };
        var term = searchTerms[Random.Shared.Next(searchTerms.Length)];
        
        var request = Http.CreateRequest("GET", $"{baseUrl}/api/products/search?term={term}");
        var response = await Http.Send(httpClient, request);
        
        return !response.IsError 
            ? Response.Ok() 
            : Response.Fail();
    });

    // Create a product step
    var step4 = await Step.Run("create_product", context, async () =>
    {
        // Create a random product
        var product = new
        {
            Name = $"Test Product {Guid.NewGuid()}",
            Price = Random.Shared.Next(10, 100),
            Description = "A test product created during load testing",
            StockQuantity = Random.Shared.Next(1, 100)
        };
        
        var json = JsonSerializer.Serialize(product);
        var request = Http.CreateRequest("POST", $"{baseUrl}/api/products")
            .WithHeader("Content-Type", "application/json")
            .WithBody(new StringContent(json, Encoding.UTF8, "application/json"));
        
        var response = await Http.Send(httpClient, request);
        
        return !response.IsError 
            ? Response.Ok() 
            : Response.Fail();
    });

    return Response.Ok();
})
.WithWarmUpDuration(TimeSpan.FromSeconds(5))
.WithLoadSimulations(
    Simulation.KeepConstant(copies: 20, during: TimeSpan.FromSeconds(60))
);

// 3. Run the tests
NBomberRunner
    .RegisterScenarios(getAllProductsScenario, searchProductsScenario, mixedScenario)
    .WithReportFormats(ReportFormat.Html, ReportFormat.Csv)
    .WithReportFileName("product_api_load_test_report")
    .Run();

Console.WriteLine("Load test complete. Press any key to exit...");
Console.ReadKey();