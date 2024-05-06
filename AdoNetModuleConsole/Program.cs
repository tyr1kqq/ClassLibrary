using ClassLibrary;
using System.Data;
using System.Reflection.PortableExecutable;
using System.Security.Cryptography;

namespace AdoNetModuleConsole
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var manager = new Manager();

            manager.Connect();

            manager.ShowData();

            Console.WriteLine("Введите логин для добавления:");

            var login = Console.ReadLine();

            Console.WriteLine("Введите имя для добавления:");
            var name = Console.ReadLine();
            manager.AddUser(login, name);

            manager.ShowData();
            manager.Disconnect();
            Console.ReadKey();

        }
    }
    
    public class Manager
    {
        private Table userTable;
        private MainConnector connector;
        private DbExecutor dbExecutor;
        public Manager()
        {
            connector = new MainConnector();

           userTable = new Table();
            userTable.name = "NetworkUser";
            userTable.ImportantField = "Login";
            userTable.Fields.Add("Id");
            userTable.Fields.Add("Login");
            userTable.Fields.Add("Name");
        }

        public int DeleteUserByLogin(string value)
        {
            return dbExecutor.DeleteByColumn(userTable.name,userTable.ImportantField,value);
        }

        public void AddUser(string login, string name)
        {
            dbExecutor.ExecProcedureAdding(name, login);
        }

        public void Connect()
        {
            var result = connector.ConnectAsync();
            
            if(result.Result)
            {
                Console.WriteLine("Подключение успешно");

                dbExecutor = new DbExecutor(connector);
            }
            else
            {
                Console.WriteLine("Ошибка подключения");
            }
        }
        public void Disconnect()
        {
            Console.WriteLine("Отключаем БД!");
            connector.DisconnectAsync();
        }
        public void ShowData()
        {
            var tablename = "NetworkUser";

            Console.WriteLine("Получаем данные из таблицы" + tablename);

            var data = dbExecutor.SelectAll(tablename);
            Console.WriteLine("Количесвтво строк" + tablename + ": " + data.Rows.Count);

            Console.WriteLine();
            foreach(DataColumn column in data.Columns)
            {
                Console.Write(column.ColumnName + "\t");
            }
            Console.WriteLine();

            foreach(DataRow row in data.Rows)
            {
                var cells = row.ItemArray;
                foreach(var cell in cells)
                {
                    Console.Write(cell + "\t");
                }
                Console.WriteLine();
            }
            Console.WriteLine();
        }
    }

    public class Table
    {
        public Table()
        {
            Fields = new List<string>();
        }
        public string name { get; set; }
        public List<string> Fields { get; set; }
        public string ImportantField { get; set; }
    }
}
