using Microsoft.EntityFrameworkCore;
using Pedidos.Domain;
using Pedidos.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pedidos
{
    class Program
    {
        static void Main(string[] args)
        {
            using var db = new Data.ApplicationContext();
            var existe = db.Database.GetPendingMigrations().Any();
            if(existe)
            {
                db.Database.Migrate();
            }

            //InserirDados();
            //InserirDadosEmMassa();
            //InserirDadosEmMassaRange();
            //ConsultaDados();
            //ConsultaDadosAdNoTracking();
            //CadastrarPedido();
            //ConsultaPedidoCarregamentoAdiantado();
            //AtualizarDados();
            //AtualizarDadosDesconectado();
            //AtualizarDadosDesconectado2();
            RemoverRegistro();

        }

        private static void RemoverRegistro()
        {
            using var db = new Data.ApplicationContext();

            //var cliente = db.Cliente.Find(2);
            var cliente = new Cliente { Id = 2 };


            //db.Cliente.Remove(cliente);
            //db.Remove(cliente);
            db.Entry(cliente).State = EntityState.Deleted;
            db.SaveChanges();
        }

        private static void AtualizarDados()
        {
            using var db = new Data.ApplicationContext();

            var cliente = db.Cliente.Find(1);
            cliente.Nome = "Nome alterado 2";

            //db.Cliente.Update(cliente); // se este metodo nao for utilizado, o efCore atualizara apenas a propriedade alterada
            db.SaveChanges();
        }

        private static void AtualizarDadosDesconectado()
        {
            using var db = new Data.ApplicationContext();

            var cliente = db.Cliente.Find(1);
            var clienteDesconectado = new
            {
                Nome = "Nome desconectado",
                Telefone = "14554545454"
            };

            db.Entry(cliente).CurrentValues.SetValues(clienteDesconectado);

            //db.Cliente.Update(cliente); // se este metodo nao for utilizado, o efCore atualizara apenas a propriedade alterada
            db.SaveChanges();
        }

        private static void AtualizarDadosDesconectado2()
        {
            using var db = new Data.ApplicationContext();

            var cliente = new Cliente
            {
                Id = 1
            };

            var clienteDesconectado = new
            {
                Nome = "Nome desconectado",
                Telefone = "14554545454"
            };
            db.Attach(cliente);

            db.Entry(cliente).CurrentValues.SetValues(clienteDesconectado);

            db.SaveChanges();// somente aqui vai no banco de dados
        }

        private static void ConsultaPedidoCarregamentoAdiantado()
        {
            using var db = new Data.ApplicationContext();

            var pedidos = db.Pedidos
                .Include(i => i.Itens)
                //.Include("Itens")
                    .ThenInclude(i => i.Produto)
                .ToList();
            Console.WriteLine(pedidos.Count);
        }

        private static void CadastrarPedido()
        {
            using var db = new Data.ApplicationContext();

            var cliente = db.Cliente.FirstOrDefault();
            var produto = db.Produto.FirstOrDefault();

            var pedido = new Pedido
            {
                ClienteId = cliente.Id,
                IniciadoEm = DateTime.Now,
                FinalizadoEm = DateTime.Now,
                Observacao = "Pedido Teste",
                Status = StatusPedido.Analise,
                TipoFrete = TipoFrete.SemFrete,
                Itens = new List<PedidoItem>
                {
                    new PedidoItem
                    {
                        ProdutoId = produto.Id,
                        Desconto = 0,
                        Quantidade = 1,
                        Valor = 10
                    }
                }
            };

            db.Pedidos.Add(pedido);
            db.SaveChanges();
        }

        private static void ConsultaDados()
        {
            using var db = new Data.ApplicationContext();
            var consultaPorSintaxe = (from c in db.Cliente where c.Id > 0 select c).ToList();
            var consultaPorMetodo = db.Cliente.Where(p => p.Id > 0).ToList();
        }


        private static void ConsultaDadosAdNoTracking()
        {
            using var db = new Data.ApplicationContext();
            //var consultaPorSintaxe = (from c in db.Cliente where c.Id > 0 select c).ToList();
            var consultaPorMetodo = db.Cliente
                .AsNoTracking()// fara com que cada consulta seja executada uma a uma no foreach abaixo
                .Where(p => p.Id > 0).ToList();
            foreach (var item in consultaPorMetodo)
            {
                Console.WriteLine($"Consultando Cliente: { item.Id }");
                db.Cliente.Find(item.Id);
                //db.Cliente.FirstOrDefault(p => p.Id == item.Id);
            }
        }

        private static void InserirDadosEmMassaRange()
        {
            var produto = new Produto
            {
                Descricao = "Produto Teste 2",
                CodigoBarras = "23423434",
                Valor = 10m,
                TipoProduto = TipoProduto.MercadoriaParaREvenda,
                Ativo = true
            };

            var clientes = new []
            {
                new Cliente
                {
                    Nome = "Samuel Lemes",
                    CEP = "45454545",
                    Cidade = "Rio Verde",
                    Estado = "GO",
                    Telefone = "6464646464",
                },
                new Cliente
                {
                    Nome = "Lidiane",
                    CEP = "54545454",
                    Cidade = "Rio Verde",
                    Estado = "GO",
                    Telefone = "646488888",
                }
            };

            using var db = new Data.ApplicationContext();
            db.Cliente.AddRange(clientes);

            var registros = db.SaveChanges();
            Console.WriteLine($"Total Registro(s): {registros}");
        }


        private static void InserirDadosEmMassa()
        {
            var produto = new Produto
            {
                Descricao = "Produto Teste 2",
                CodigoBarras = "23423434",
                Valor = 10m,
                TipoProduto = TipoProduto.MercadoriaParaREvenda,
                Ativo = true
            };

            var cliente = new Cliente
            {
                Nome = "Samuel Lemes",
                CEP = "45454545",
                Cidade = "Rio Verde",
                Estado = "GO",
                Telefone = "6464646464",
            };

            using var db = new Data.ApplicationContext();
            db.AddRange(produto, cliente);

            var registros = db.SaveChanges();
            Console.WriteLine($"Total Registro(s): {registros}");
        }

        private static void InserirDados()
        {
            var produto = new Produto
            {
                Descricao = "Produto Teste",
                CodigoBarras = "21212121",
                Valor = 10m,
                TipoProduto = TipoProduto.MercadoriaParaREvenda,
                Ativo = true
            };

            using var db = new Data.ApplicationContext();
            db.Produto.Add(produto);
            //db.Set<Produto>().Add(produto);
            //db.Entry(produto).State = EntityState.Added;
            //db.Add(produto); // menor performance

            var registros = db.SaveChanges();
            Console.WriteLine($"Total Registro(s): {registros}");
        }
    }
}
