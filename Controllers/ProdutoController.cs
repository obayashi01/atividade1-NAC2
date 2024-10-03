using Dapper;
using Microsoft.AspNetCore.Mvc;
using MySqlConnector;
using Newtonsoft.Json;
using StackExchange.Redis;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InventarioAPI.Models;

namespace InventarioAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProdutosController : ControllerBase
    {
        private readonly MySqlConnection _db;
        private readonly IConnectionMultiplexer _redis;

        public ProdutosController(MySqlConnection db, IConnectionMultiplexer redis)
        {
            _db = db;
            _redis = redis;
        }

        // GET /produtos
        [HttpGet]
        public async Task<IActionResult> GetProdutos()
        {
            var cacheKey = "produtos";
            var dbRedis = _redis.GetDatabase();
            var produtosCache = await dbRedis.StringGetAsync(cacheKey);

            if (!produtosCache.IsNullOrEmpty)
            {
                var produtos = JsonConvert.DeserializeObject<List<Produto>>( produtosCache);
                return Ok(produtos);
            }

            var produtosDb = _db.Query<Produto>("SELECT * FROM produtos").ToList();
            await dbRedis.StringSetAsync(cacheKey, JsonConvert.SerializeObject(produtosDb), TimeSpan.FromMinutes(10));

            return Ok(produtosDb);
        }

        // POST /produtos
        [HttpPost]
        public async Task<IActionResult> AddProduto([FromBody] Produto produto)
        {
            var query = "INSERT INTO produtos (nome, preco, quantidade_estoque) VALUES (@Nome, @Preco, @QuantidadeEstoque)";
            var result = _db.Execute(query, produto);

            if (result > 0)
            {
                var dbRedis = _redis.GetDatabase();
                await dbRedis.KeyDeleteAsync("produtos"); // Invalida o cache
                return Ok("Produto adicionado com sucesso.");
            }

            return BadRequest("Erro ao adicionar produto.");
        }

        // PUT /produtos/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduto(int id, [FromBody] Produto produto)
        {
            var query = "UPDATE produtos SET nome = @Nome, preco = @Preco, quantidade_estoque = @QuantidadeEstoque WHERE id = @Id";
            var result = _db.Execute(query, new { produto.Nome, produto.Preco, produto.QuantidadeEstoque, Id = id });

            if (result > 0)
            {
                var dbRedis = _redis.GetDatabase();
                await dbRedis.KeyDeleteAsync("produtos"); // Invalida o cache
                return Ok("Produto atualizado com sucesso.");
            }

            return BadRequest("Erro ao atualizar produto.");
        }

        // DELETE /produtos/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduto(int id)
        {
            var query = "DELETE FROM produtos WHERE id = @Id";
            var result = _db.Execute(query, new { Id = id });

            if (result > 0)
            {
                var dbRedis = _redis.GetDatabase();
                await dbRedis.KeyDeleteAsync("produtos"); // Invalida o cache
                return Ok("Produto removido com sucesso.");
            }

            return BadRequest("Erro ao remover produto.");
        }
    }
}
