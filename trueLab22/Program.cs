using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;
using System.Threading;

namespace trueLab22
{
    class Program
    {
        static SqlConnection connection = new SqlConnection(SQLConnect.SqlConnection());
        static int id = 0;
        static void Main(string[] args)
        {
            try
            {
                connection.Open();
                Initialization();
            }
            catch (Exception e)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Не удалось подключиться к базе данных.");
                Console.ResetColor();
            }
            
            

            id = GetLastId();

            Console.WriteLine("ID : " + id);

            Thread myThread = new Thread(new ThreadStart(Count));
            myThread.Start();


            while (true)
            {
                AddMessage("user2");
            }
        }

        static void Count()
        {
            while (true)
            {
                int id1 = GetLastId();

                if (id1 > id)
                {
                    OutputLines(id1 - id);
                }

                id = id1;

                Thread.Sleep(500);
            }
        }

        static void Initialization()
        {
            // Create a dependency connection.
            SqlDependency.Start(SQLConnect.SqlConnection(), null);
        }

        static void AddMessage(string login)
        {
            string message = Console.ReadLine();
            //SqlConnection connection = DBUtils.GetDBConnection();
            //connection.Open();

            string sql = "INSERT INTO dbo.Employees(LFM, PassportData) VALUES(@lfm, @passportData)";

            SqlCommand cmd = connection.CreateCommand();
            cmd.CommandText = sql;

            // Добавить параметр (Написать короче).
            cmd.Parameters.Add("@lfm", SqlDbType.VarChar).Value = login;
            cmd.Parameters.Add("@passportData", SqlDbType.VarChar).Value = message;

            // Выполнить Command (Используется для delete, insert, update).
            int rowCount = cmd.ExecuteNonQuery();
        }

        static int GetLastId()
        {
            string id2 = "";

            SqlDataAdapter da = new SqlDataAdapter(@"WITH SRC AS (SELECT TOP (1) EmployeeId, LFM, " +
                "PassportData FROM Employees ORDER BY EmployeeId DESC) SELECT * FROM SRC ORDER BY EmployeeId", connection);
            DataSet ds = new DataSet();
            da.Fill(ds);

            foreach (DataTable dt in ds.Tables)
            {
                // перебор всех строк таблицы
                foreach (DataRow row in dt.Rows)
                {
                    // получаем все ячейки строки
                    var cells = row.ItemArray;
                    int i = 0;
                    foreach (object cell in cells)
                    {
                        if (i == 0)
                        {
                            id2 = cell.ToString();
                        }
                        i++;
                    }
                }
            }

            return Convert.ToInt32(id2);
        }

        static void OutputLines(int count)
        {
            SqlDataAdapter da = new SqlDataAdapter(@"WITH SRC AS (SELECT TOP (" + count + ") EmployeeId, LFM, " +
                "PassportData FROM Employees ORDER BY EmployeeId DESC) SELECT * FROM SRC ORDER BY EmployeeId", connection);
            DataSet ds = new DataSet();
            da.Fill(ds);

            foreach (DataTable dt in ds.Tables)
            {
                // перебор всех строк таблицы
                foreach (DataRow row in dt.Rows)
                {
                    // получаем все ячейки строки
                    var cells = row.ItemArray;
                    int i = 0;
                    foreach (object cell in cells)
                    {
                        if (i != 0)
                        {
                            Console.Write("{0}", cell);
                        }
                        if (i == 1)
                        {
                            Console.Write(": ");
                        }
                        i++;
                    }
                    Console.WriteLine();
                }
            }
        }
    }
}
