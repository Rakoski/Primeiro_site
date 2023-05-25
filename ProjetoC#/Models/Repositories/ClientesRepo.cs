using Microsoft.Extensions.FileProviders;
using Newtonsoft.Json;
using System.Reflection.Metadata;
using System.Security.Cryptography.X509Certificates;

namespace ProjetoC_.Models.Repositories
{
    // Criado o "banco de dados" dos clientes, aka um repositório
    public class ClientesRepo
    {

        // convertendo as coisas do site em formato de json no arquivo .txt abaixo
        public void Salvar(Clientes clientes)
        {
            // Vai listar tudo do banco de dados
            var ClientesLista = Listar();
            // vai achar ele
            var item = ClientesLista.Where(a => a.Documento == clientes.Documento).FirstOrDefault();

            // Se ele existe eu deleto
            if (item != null)
            {
                Deletar(clientes.Documento);
            }

            // se ele não existir, já vai salvar de qualquer jeito mesmo
            string retorno_json = JsonConvert.SerializeObject(clientes, Formatting.Indented) + " ," + Environment.NewLine;

            File.AppendAllText(@"C:\\Users\\mastr\\OneDrive\\Área de Trabalho\\c#\\CadastroClientes\\Banco\\banco_dados.txt", retorno_json);
        }


        //criando a lista de todos os clientes que vai ser usada depois de ser convertida em json no arquivo .txt
        public List<Clientes> Listar()
        {

            // mano, pega essa lista desse arquivo aqui
            var tipo_da_variavel = File.ReadAllText("C:\\Users\\mastr\\OneDrive\\Área de Trabalho\\c#\\CadastroClientes\\Banco\\banco_dados.txt");

            // e coloca ela dentro dessa lista (lista Clientes criada antes)
            List<Clientes>? clientes = JsonConvert.DeserializeObject<List<Clientes>>("[" + tipo_da_variavel + "]");
            List<Clientes> clientes_lista = clientes;

            return clientes_lista.OrderByDescending(t => t.Nome).ToList();
        }

        
        public bool Deletar(string Document)
        {
            var lista_clientes_document = Listar();
            var item_encontrado = lista_clientes_document.Where(a=>a.Documento == Document).FirstOrDefault();
            // Encontrando a variável no arquivo .txt para ser deletada depois. FirstOrDefault() significa o primeiro que encontrar é pra trazer

            if (item_encontrado != null)
            {

                lista_clientes_document.Remove(item_encontrado);
                // Remove e deixa o arquivo vazio (String.Empty)
                File.WriteAllText("C:\\Users\\mastr\\OneDrive\\Área de Trabalho\\c#\\CadastroClientes\\Banco\\banco_dados.txt", string.Empty);

                // Agora, temos que escrever tudo de novo, só que sem o item_encontrado que foi excluido
                // foreach é a mesma coisa que um for em python, percorrendo uma lista, item por item
                foreach (var cliente in lista_clientes_document)
                {

                    Salvar(cliente);

                }

                // Isso é basicamente um alert("true")
                return true;
            }

            return false;

        }

        // Lembrando que ainda precisamos ter a função de editar o tal "banco de dados" que no momento é só um .txt 
        // Agora, nós temos que ir até o banco, achar o cliente que é pra editar e Pegar o Cliente
        public Clientes GetClient(string Document)
        {
            // Vai listar tudo do banco de dados
            var ClientesLista = Listar();
            // vai achar ele
            var item = ClientesLista.Where(a => a.Documento == Document).FirstOrDefault();

            return item;
            // e dai vai retornar o item selecionado
        } 
    }
}
