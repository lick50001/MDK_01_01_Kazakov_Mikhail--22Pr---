using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.IO;
using System.Linq;

namespace ClassConnection
{
    public class Connection
    {
        public List<User> users = new List<User>();
        public List<Call> calls = new List<Call>();

        public enum Tabels
        {
            Users,
            Calls
        }

        public string localPath = "";

        public OleDbDataReader QueryAccess(string query)
        {
            try
            {
                localPath = Directory.GetCurrentDirectory();
                string connectionString = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" +
                    Path.Combine(localPath, "accesbase.accdb");

                OleDbConnection connect = new OleDbConnection(connectionString);
                connect.Open();
                OleDbCommand cmd = new OleDbCommand(query, connect);
                OleDbDataReader reader = cmd.ExecuteReader();
                return reader;
            }
            catch
            {
                return null;
            }
        }

        public int SetLastId(Tabels tabel)
        {
            try
            {
                LoadData(tabel);
                switch (tabel)
                {
                    case Tabels.Users:
                        if (users.Count >= 1)
                        {
                            return users.Max(x => x.Id) + 1;
                        }
                        else return 1;
                    case Tabels.Calls:
                        if (calls.Count >= 1)
                        {
                            return calls.Max(x => x.Id) + 1;
                        }
                        else return 1;
                }
                return -1;
            }
            catch { return -1; }
        }

        public void LoadData(Tabels zap)
        {
            OleDbDataReader itemQuery = null;
            try
            {
                itemQuery = QueryAccess("SELECT * FROM [" + zap.ToString() + "] ORDER BY [Код]");

                if (zap == Tabels.Users)
                {
                    users.Clear();
                    while (itemQuery != null && itemQuery.Read())
                    {
                        User newEl = new User();
                        newEl.Id = Convert.ToInt32(itemQuery.GetValue(0));
                        newEl.PhoneNum = Convert.ToString(itemQuery.GetValue(1));
                        newEl.FioUser = Convert.ToString(itemQuery.GetValue(2));
                        newEl.PassportData = Convert.ToString(itemQuery.GetValue(3));

                        users.Add(newEl);
                    }
                }
                if (zap == Tabels.Calls)
                {
                    calls.Clear();
                    while (itemQuery != null && itemQuery.Read())
                    {
                        Call newEl = new Call();
                        newEl.Id = Convert.ToInt32(itemQuery.GetValue(0));
                        newEl.UserId = Convert.ToInt32(itemQuery.GetValue(1));
                        newEl.CategoryCall = Convert.ToInt32(itemQuery.GetValue(2));
                        newEl.Date = Convert.ToString(itemQuery.GetValue(3));
                        newEl.TimeStart = Convert.ToString(itemQuery.GetValue(4));
                        newEl.TimeEnd = Convert.ToString(itemQuery.GetValue(5));
                        calls.Add(newEl);
                    }
                }
            }
            catch (Exception ex)
            {
                // Логирование ошибки
                Console.WriteLine($"Ошибка загрузки данных: {ex.Message}");
            }
            finally
            {
                if (itemQuery != null && !itemQuery.IsClosed)
                {
                    itemQuery.Close();
                }
            }
        }
    }

    // Временные классы для компиляции. В реальном проекте они должны быть в отдельном файле
    public class User
    {
        public int Id { get; set; }
        public string PhoneNum { get; set; }
        public string FioUser { get; set; }
        public string PassportData { get; set; }
    }

    public class Call
    {
        public int Id { get; set; }
        public int UserId { get; set; }  // Было user_id
        public int CategoryCall { get; set; }
        public string Date { get; set; }
        public string TimeStart { get; set; }
        public string TimeEnd { get; set; }
    }
}