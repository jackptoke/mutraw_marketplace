using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using mutraw_marketplace_api.Models;
using mutraw_marketplace_api.Repositories;

namespace mutraw_marketplace_api.Controllers;

[Route("api/v1/[controller]")]
[ApiController]
public class ProductsController(IRepository<Product> productRepo) : ControllerBase
{
    [HttpPost]
    [Authorize(policy: "AdminOnly")]
    public async Task<IActionResult> Create([FromBody]Product product)
    {
        await productRepo.CreateAsync(product);
        return CreatedAtAction(nameof(GetOne), new { id = product.Id }, product);
    }
    
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        return Ok(await productRepo.GetAllAsync());
    }
    
    [HttpGet("{id:long}")]
    public async Task<IActionResult> GetOne([FromRoute] long id)
    {
        var product = await productRepo.GetIdAsync(id);
        if (product == null)
            return NotFound();
        return Ok(await productRepo.GetIdAsync(id));
    }
    
    [HttpPut("{id:long}")]
    [Authorize(policy: "AdminOnly")]
    public async Task<IActionResult> Update([FromRoute] long id, [FromBody] Product product)
    {
        var existingProduct = await productRepo.GetIdAsync(id);
        if (existingProduct == null)
            return NotFound();
        existingProduct.Name = product.Name;
        existingProduct.Description = product.Description;
        existingProduct.ProductSubCategoryId = product.ProductSubCategoryId;
        existingProduct.SellPrice = product.SellPrice;
        existingProduct.DefaultUom = product.DefaultUom;
        existingProduct.ImageUrl = product.ImageUrl;
        
        await productRepo.UpdateAsync(existingProduct);
        return NoContent();
    }

    [HttpDelete("{id:long}")]
    [Authorize(policy: "AdminOnly")]
    public async Task<IActionResult> Delete([FromRoute] long id)
    {
        await productRepo.DeleteAsync(id);
        return NoContent();
    }
}