using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProveedorApp.IBusiness;
using ProveedorApp.Model;

namespace ProveedorApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductoController : ControllerBase
    {

        public ProductoController(IProductoBusiness iProductoBusiness)
        {
            _productoBusiness = iProductoBusiness;
        }

        private readonly IProductoBusiness _productoBusiness;


        [HttpPost]
        public async Task<IActionResult> CreateProducto([FromBody] Producto producto)
        {
            await _productoBusiness.CreateProducto(producto);
            return Ok("Producto creado correctamente");
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Producto>>> GetAllProductos()
        {
            var productos = await _productoBusiness.GetAllProductos();
            return Ok(productos);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Producto>> GetProductoById(int id)
        {
            var producto = await _productoBusiness.GetProductoById(id);
            if (producto == null)
                return NotFound("Producto no encontrado");

            return Ok(producto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProducto(int id, [FromBody] Producto producto)
        {
            if (id != producto.Id)
                return BadRequest("El ID del producto no coincide");

            await _productoBusiness.UpdateProducto(producto);
            return Ok("Producto actualizado correctamente");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProducto(int id)
        {
            await _productoBusiness.DeleteProducto(id);
            return Ok("Producto eliminado correctamente");
        }
    }
    
}
