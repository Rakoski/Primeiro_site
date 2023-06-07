using Microsoft.Extensions.FileProviders;
using Newtonsoft.Json;
using System.Reflection.Metadata;
using System.Security.Cryptography.X509Certificates;
using MySql.Data.MySqlClient;
using System.Data;
using MySqlX.XDevAPI;

namespace ProjetoC_.Models.Repositories
{
    // Criado o "banco de dados" dos clientes, aka um repositório
    public class ClientesRepo
    {

        public void Salvar(Clientes clientes)
        
        {

            string server = "localhost";
            string database = "cadastro_cliente";
            string username = "root";
            string password = "";
            string constring = "SERVER=" + server + ";" + "DATABASE=" + database + ";" +
                "UID=" + username + ";" + "PASSWORD=" + password + ";";

            List<Clientes> clientesList = new List<Clientes> { clientes };

            // First, I have to connect to MySQL workbench
            using (MySqlConnection conn = new MySqlConnection(constring))
            {
                conn.Open();

                foreach (var cliente in clientesList)
                {
                    // Checking if the cliente already exists in the database based on the Documento field
                    string query = "SELECT COUNT(*) FROM clientes WHERE Documento = @Documento";
                    using (MySqlCommand checkCmd = new MySqlCommand(query, conn))
                    {
                        checkCmd.Parameters.AddWithValue("@Documento", cliente.Documento);
                        int existingCount = Convert.ToInt32(checkCmd.ExecuteScalar());

                        if (existingCount > 0)
                        {
                            // If the cliente exists, delete the existing record
                            query = "DELETE FROM clientes WHERE Documento = @Documento";
                            using (MySqlCommand deleteCmd = new MySqlCommand(query, conn))
                            {
                                deleteCmd.Parameters.AddWithValue("@Documento", cliente.Documento);
                                deleteCmd.ExecuteNonQuery();
                            }
                        }
                    }

                    // Insert the cliente record into the database
                    query = "INSERT INTO clientes (Documento, nome, sexo, email, telefone, fax, UF) " +
                            "VALUES (@Documento, @Nome, @Sexo, @Email, @Telefone, @Fax, @UF)";
                    using (MySqlCommand insertCmd = new MySqlCommand(query, conn))
                    {
                        insertCmd.Parameters.AddWithValue("@Documento", cliente.Documento);
                        insertCmd.Parameters.AddWithValue("@Nome", cliente.Nome);
                        insertCmd.Parameters.AddWithValue("@Sexo", cliente.Sexo);
                        insertCmd.Parameters.AddWithValue("@Email", cliente.Email);
                        insertCmd.Parameters.AddWithValue("@Telefone", cliente.Telefone);
                        insertCmd.Parameters.AddWithValue("@Fax", cliente.Fax);
                        insertCmd.Parameters.AddWithValue("@UF", cliente.UF);
                        insertCmd.ExecuteNonQuery();
                    }
                }
            }

            Console.WriteLine("Data transfer completed.");
            Console.WriteLine("Press any key to exit...");
            Console.ReadLine();
        }


        // Here, I'll make a function that only retrieves the data so that I can list it in the Website using MySQL
        public List<Clientes> Listar()
        {

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

        public bool Deletar(string Documento)
        {
            string server = "localhost";
            string database = "cadastro_cliente";
            string username = "root";
            string password = "";
            string constring = "SERVER=" + server + ";" + "DATABASE=" + database + ";" +
                            "UID=" + username + ";" + "PASSWORD=" + password + ";";

            var conexao = new MySqlConnection(constring);
            conexao.Open();

            var query = "DELETE FROM clientes WHERE documento = @Documento";
            var comando = new MySqlCommand(query, conexao);
            comando.Parameters.AddWithValue("@Documento", Documento);

            var linhasAfetadas = comando.ExecuteNonQuery();

            conexao.Close();

            if (linhasAfetadas > 0)
            {
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
