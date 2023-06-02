using Microsoft.Extensions.FileProviders;
using Newtonsoft.Json;
using System.Reflection.Metadata;
using System.Security.Cryptography.X509Certificates;
using MySql.Data.MySqlClient;
using System.Data;

namespace ProjetoC_.Models.Repositories
{
    // Criado o "banco de dados" dos clientes, aka um repositório
    public class ClientesRepo
    {

        // convertendo as coisas do site em formato de json no arquivo .txt abaixo
        public void Salvar(Clientes clientes)
        {

            string server = "localhost";
            string database = "cadastro_cliente";
            string username = "root";
            string password = "";
            string constring = "SERVER=" + server + ";" + "DATABASE=" + database + ";" +
                "UID=" + username + ";" + "PASSWORD=" + password + ";";

            // mano, pega essa lista desse arquivo aqui
            var tipo_da_variavel = File.ReadAllText("C:\\Users\\mastr\\OneDrive\\Área de Trabalho\\c#\\CadastroClientes\\Banco\\banco_dados.txt");

            // e coloca ela dentro dessa lista (lista Clientes criada antes)
            List<Clientes>? data = JsonConvert.DeserializeObject<List<Clientes>>("[" + tipo_da_variavel + "]");

            // Primeiro eu tenho que conectar com o MySQL workbench
            using (MySqlConnection conn = new MySqlConnection(constring))
            {
                conn.Open();

                // Depois eu cologo, pra cada item nos dados (data), sendo que os dados é o deserialized object.
                // I'm running into an issue here. The website is saving the already listed data from Listar(), not
                // the new saved cadastro when I try to save another person
                foreach (var item in data)
                {

                    string query = "INSERT INTO clientes (Documento, nome, sexo, email, telefone, fax, UF) " +
                                   "VALUES (@Documento, @Nome, @Sexo, @Email, @Telefone, @Fax, @UF)";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        // Set the parameter values
                        cmd.Parameters.AddWithValue("@Documento", item.Documento);
                        cmd.Parameters.AddWithValue("@Nome", item.Nome);
                        cmd.Parameters.AddWithValue("@Sexo", item.Sexo);
                        cmd.Parameters.AddWithValue("@Email", item.Email);
                        cmd.Parameters.AddWithValue("@Telefone", item.Telefone);
                        cmd.Parameters.AddWithValue("@Fax", item.Fax);
                        cmd.Parameters.AddWithValue("@UF", item.UF);

                        // Execute the query
                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            Console.WriteLine("Data inserted successfully!");
                        }
                        else
                        {
                            Console.WriteLine("Failed to insert data.");
                        }
                    }
                }
            }

            // Vai listar tudo do banco de dados
            // var ClientesLista = Listar();
            // vai achar ele
            // var item = ClientesLista.Where(a => a.Documento == clientes.Documento).FirstOrDefault();

            // Se ele existe eu deleto
            //if (item != null)
            //{
            //Deletar(clientes.Documento);
            //}

            // se ele não existir, já vai salvar de qualquer jeito mesmo
            // string retorno_json = JsonConvert.SerializeObject(clientes, Formatting.Indented) + " ," + Environment.NewLine;

            // File.AppendAllText(@"C:\\Users\\mastr\\OneDrive\\Área de Trabalho\\c#\\CadastroClientes\\Banco\\banco_dados.txt", retorno_json);
        }

        // Here, I'll make a function that only retrieves the data so that I can list it in the Website using MySQL
        public List<Clientes> Listar() {

            string server = "localhost";
            string database = "cadastro_cliente";
            string username = "root";
            string password = "";
            string constring = "SERVER=" + server + ";" + "DATABASE=" + database + ";" +
                "UID=" + username + ";" + "PASSWORD=" + password + ";";

            using (MySqlConnection conn = new MySqlConnection(constring))
            {
                conn.Open();

                string query = "SELECT * FROM clientes";

                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    using (MySqlDataAdapter adapter = new MySqlDataAdapter(cmd))
                    {
                        DataTable dataTable = new DataTable();
                        adapter.Fill(dataTable);

                        List<Clientes> clientesList = new List<Clientes>();

                        foreach (DataRow row in dataTable.Rows)
                        {
                            Clientes cliente = new Clientes
                            {
                                IdCliente = Convert.ToInt32(row["id_cliente"]),
                                Documento = row["Documento"].ToString(),
                                Nome = row["nome"].ToString(),
                                Sexo = row["sexo"].ToString(),
                                Email = row["email"].ToString(),
                                Telefone = row["telefone"].ToString(),
                                Fax = row["fax"].ToString(),
                                UF = row["UF"].ToString()
                            };

                            clientesList.Add(cliente);
                        }

                        return clientesList.OrderByDescending(t => t.Nome).ToList();
                    }
                }
            }
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
