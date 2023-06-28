using Microsoft.AspNetCore.Mvc;
using NoSQLApp.Services;
using Microsoft.Azure.Cosmos;

namespace no_sql_app.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly DbService _dbService;

        public ProductsController(DbService dbService)
        {
            _dbService = dbService;
        }

        [HttpGet]
        public async Task<IActionResult> GetProducts()
        {
            try
            {
                var items = await _dbService.GetAllProductsAsync();
                return Ok(items);
            }
            catch
            {
                return StatusCode(500, "An error occurred while processing the request.");
            }
        }


        [HttpGet("{itemId}")]
        public async Task<IActionResult> GetProduct(string itemId)
        {
            try
            {
                var item = await _dbService.GetProductAsync(itemId);
                return item != null ? Ok(item) : NotFound("Product not found.");
            }
            catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return NotFound("Product not found in the database.");
            }
            catch
            {
                return StatusCode(500, "An error occurred while processing the request.");
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateProduct(ProductInputModel inputModel)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var newProduct = new Product
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = inputModel.Name,
                    Price = inputModel.Price,
                    Description = inputModel.Description
                };

                await _dbService.CreateProductAsync(newProduct);

                return CreatedAtAction(nameof(GetProduct), new { itemId = newProduct.Id }, newProduct);
            }
            catch
            {
                return StatusCode(500, "An error occurred while processing the request.");
            }
        }

        [HttpPut("{itemId}")]
        public async Task<IActionResult> UpdateProduct(string itemId, ProductInputModel inputModel)
        {
            try
            {
                var existingProduct = await _dbService.GetProductAsync(itemId);
                if (existingProduct == null)
                {
                    return NotFound("Product not found.");
                }

                var updatedProduct = new Product
                {
                    Id = existingProduct.Id,
                    Name = inputModel.Name,
                    Price = inputModel.Price,
                    Description = inputModel.Description
                };

                await _dbService.UpdateProductAsync(updatedProduct);
                return NoContent();
            }
            catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return NotFound("Product not found in the database.");
            }
            catch
            {
                return StatusCode(500, "An error occurred while processing the request.");
            }
        }

        [HttpDelete("{itemId}")]
        public async Task<IActionResult> DeleteProduct(string itemId)
        {
            try
            {
                await _dbService.DeleteProductAsync(itemId);
                return NoContent();
            }
            catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return NotFound("Product not found in the database.");
            }
            catch
            {
                return StatusCode(500, "An error occurred while processing the request.");
            }
        }
    }
}
