using Microsoft.AspNetCore.Mvc;
using ProductApi.Models;
using ProductApi.Repositories;

namespace ProductApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly ProductRepository _repository;

    public ProductsController()
    {
        _repository = new ProductRepository();
    }

    [HttpGet]
    public ActionResult<IEnumerable<Product>> GetAll()
    {
        return Ok(_repository.GetAll());
    }

    [HttpGet("{id}")]
    public ActionResult<Product> GetById(int id)
    {
        var product = _repository.GetById(id);
        if (product == null)
            return NotFound();

        return Ok(product);
    }

    [HttpGet("search")]
    public ActionResult<IEnumerable<Product>> Search([FromQuery] string term)
    {
        return Ok(_repository.Search(term));
    }

    [HttpPost]
    public ActionResult<Product> Create(Product product)
    {
        var newProduct = _repository.Add(product);
        return CreatedAtAction(nameof(GetById), new { id = newProduct.Id }, newProduct);
    }
}