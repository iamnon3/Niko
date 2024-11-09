using Google.Apis.Sheets.v4;
using SQLite;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trak_IT.Scripts
{
    public class LocalDBServer
    {
        private const string DB_NAME = "trakitdb.db3";
        private readonly SQLiteAsyncConnection _connection;

        public LocalDBServer()
        {
            _connection = new SQLiteAsyncConnection(Path.Combine(FileSystem.AppDataDirectory, DB_NAME));
            _connection.CreateTableAsync<UserModel>();
            _connection.CreateTableAsync<StudentModel>();
        }

        public async Task<List<UserModel>> GetUsers()
        {
            return await _connection.Table<UserModel>().ToListAsync();
        }

        public async Task<UserModel> GetByUser(string user, string password)
        {
            try
            {
                var usersd = await _connection.Table<UserModel>().Where(x => x.Username == user && x.Password == password).FirstOrDefaultAsync();
                return usersd;
            }

            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return null;
            }

        }

        public async Task Create(UserModel user)
        {
            await _connection.InsertAsync(user);
        }

        public async Task Update(UserModel user)
        {
            await _connection.UpdateAsync(user);
        }

        public async Task Delete(UserModel user)
        {
            await _connection.DeleteAsync(user);
        }

        public async Task Resets()
        {
            await _connection.DropTableAsync<UserModel>();
            await _connection.DropTableAsync<StudentModel>();
            await _connection.CreateTableAsync<UserModel>();
            await _connection.CreateTableAsync<StudentModel>();
            Debug.WriteLine("Resets");
        }

        public async Task GetUser()
        {
            SheetService service = new SheetService();
            var data = await service.ReadSheetDataAsync("spreadsheetID", "Sheet1!A:E");

            foreach (var row in data)
            {
                await _connection.InsertAsync(new UserModel
                {
                    Username = row[1].ToString(),
                    Password = row[2].ToString(),
                    User_Role = row[3].ToString(),
                    PhotoString = row[4].ToString()
                });
            }

        }

        public async Task<StudentModel> GetByStudent(string bsit)
        {
            try
            {
                var usersd = await _connection.Table<StudentModel>().Where(x => x.Bsitid == bsit).FirstOrDefaultAsync();
                return usersd;
            }

            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return null;
            }

        }

        public async Task GetStudent()
        {
            SheetService service = new SheetService();
            var data = await service.ReadSheetDataAsync("spreadsheetID", "Sheet2!A:L");
            var time = await service.ReadSheetDataAsync("spreadsheetID", "Sheet3!A:G");
            
            foreach (var row in data)
            {
                if (row[7].ToString() == "")
                {
                    if (!string.IsNullOrWhiteSpace(row[5].ToString()))
                    {
                        await _connection.InsertAsync(new StudentModel
                        {
                            Bsitid = row[2].ToString(),
                            Name = row[3].ToString() + ", " + row[4].ToString(),
                            Section = row[5].ToString(),
                            Event = row[7].ToString(),
                            Timein = string.Empty,
                            Timeout = string.Empty
                        });
                    }

                    else
                    {
                        await _connection.InsertAsync(new StudentModel
                        {
                            Bsitid = row[2].ToString(),
                            Name = row[3].ToString() + ", " + row[4].ToString(),
                            Section = "Irreg",
                            Event = row[7].ToString(),
                            Timein = string.Empty,
                            Timeout = string.Empty
                        });
                    }
                }
            }

            if (time != null)
            {
                foreach (var row in time)
                {
                    await _connection.UpdateAsync(new StudentModel
                    {
                        Id = Convert.ToInt32(row[0]),
                        Bsitid = row[1].ToString(),
                        Name = row[2].ToString(),
                        Section = row[3].ToString(),
                        Event = row[4].ToString(),
                        Timein = row[5].ToString()
                    });
                }
            }
        }

        public async Task GetTime()
        {
            SheetService service = new SheetService();
            var data = await service.ReadSheetDataAsync("spreadsheetID", "Sheet3!A:G");
            if (data != null)
            {
                foreach (var row in data)
                {
                    await UpdateStudent(new StudentModel
                    {
                        Id = Convert.ToInt32(row[0].ToString()),
                        Timein = row[5].ToString(),
                    });
                }
            }
            
        }

        public async Task UpdateStudent(StudentModel user)
        {
            await _connection.UpdateAsync(user);
        }
    }
}
