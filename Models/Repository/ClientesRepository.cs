using Newtonsoft.Json;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace CadastroClientes.Models.Repository
{
    public class ClientesRepository
    {
        public void Salvar(Clientes clientes)
        {
            //LISTAMOS TODOS OS ITENS CADASTRO
            var listaClientes = Listar();

            //ENCONTEAMOS O ITEM A SER EXCLUIDO
            var item = listaClientes.Where(t => t.Documento == clientes.Documento).FirstOrDefault();

            //SE ELE EXISTE EU DELETO
            if (item != null)
            {
                Deletar(clientes.Documento);
            }

            var clientesTexto = JsonConvert.SerializeObject(clientes) + "," + Environment.NewLine;
            File.AppendAllText("C:\\Users\\fernanda.alves.PLURALCAPITAL\\source\\repos\\CadastroClientes\\BancoDados\\bancodados.txt", clientesTexto);
        }

        public List<Clientes> Listar()
        {
            var clientes = File.ReadAllText("C:\\Users\\fernanda.alves.PLURALCAPITAL\\source\\repos\\CadastroClientes\\BancoDados\\bancodados.txt");

            List<Clientes> clientesLista = JsonConvert.DeserializeObject<List<Clientes>>("["+clientes+"]");

            return clientesLista.OrderByDescending(t=>t.Nome).ToList();
        }

        public bool Deletar(string Documento)
        {
            //LISTAMOS TODOS OS ITENS CADASTRO
            var listaClientes = Listar();

            //ENCONTEAMOS O ITEM A SER EXCLUIDO
            var item = listaClientes.Where(t => t.Documento == Documento).FirstOrDefault();

            if (item != null)
            {
                //REMOVEMOS O ITEM DA LISTA
                listaClientes.Remove(item);

                //LIMPAMOS O NOSSO BANCO DE DADOS
                File.WriteAllText("C:\\Users\\fernanda.alves.PLURALCAPITAL\\source\\repos\\CadastroClientes\\BancoDados\\bancodados.txt", string.Empty);

                //ESCREVER NO BANCO DE DADOS NOSSA LISTA SEM O ITEM EXCLUIDO
                foreach (var cliente in listaClientes)
                {
                    Salvar(cliente);
                }

                return true;
            }

            return false;
        }

        public Clientes GetCliente(string Documento)
        {
            var clienteLista = Listar();
            var item = clienteLista.Where(t => t.Documento == Documento).FirstOrDefault();

            return item;
        }
    }
}
