using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProjetoC_.Models;
using ProjetoC_.Models.Repositories;
using Newtonsoft.Json;


// If you see any annotations, please ignore them. Since I mainly do Python programming, C# is a new language for me and
// it's mostly just "converting" the code back to Python 


namespace ProjetoC_.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CientesController : ControllerBase
    {
        [HttpPost("Salvar")]
        // "httpPost" significa que tudo que acontecer no site será mandado para essa classe aqui embaixo
        public object Salvar([FromBody] Clientes cadastro)
            // "object" é o tipo comum do C, aka ele aceita tudo, como um input normal, pode ser int, string, etc
            // Então, quando salvar essa classe quer falar pro usuário que o input foi salvo com sucesso
        {
            try
            {
                ClientesRepo repo = new ClientesRepo();
                repo.Salvar(cadastro);
            }
            catch 
            {
            Exception ex; 
            }
            {
                // Qualquer mensagem de erro no site vai cair aqui
            }
            return null;
        }

        [HttpPost("Alterar")]
        public object Alterar([FromBody] Clientes clientes)
        {
            try
            {

            }
            catch { Exception ex; }
            {
            }
            return null;
        }

        [HttpGet("Listar")]
        public object Listar()
        {
            List<Clientes> lista_de_clientes = null;
            try
            {
                ClientesRepo Repository = new ClientesRepo();
                lista_de_clientes = Repository.Listar();
            }
            catch { Exception ex; }
            {
            }
            return lista_de_clientes.OrderByDescending(t=>t.Nome).ToList();
        }

        [HttpDelete("Deletar")]
        public object Deletar(string Document)
        {
            try
            {
                ClientesRepo clientes = new ClientesRepo();
                bool deleteReturn = clientes.Deletar(Document);

                return deleteReturn;
            }
            catch { Exception ex; }
            {
                // Qualquer mensagem de erro no site vai cair aqui
            }
            return null;
        }
        [HttpGet("GetClient")]
        public object GetClient(string Document)
        {
            try
            {
                ClientesRepo clientes = new ClientesRepo();
                var return_this = clientes.GetClient(Document);
                return return_this;
            }
            catch { Exception ex; }
            {
                // Qualquer mensagem de erro no site vai cair aqui
            }
            return null;
        }

    }
}
