using EnvyLogging.Models;
using EnvyLogging.Storage;
using Rocket.Core.Logging;
using Steamworks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnvyLogging.Database
{
    public class DatabaseManager
    {
        private List<Account> Data;



        private DataStorage<List<Account>> AccountStorage { get; set; }
        public DatabaseManager()
        {

            System.IO.Directory.CreateDirectory($"{Environment.CurrentDirectory}/Plugins/EnvyLogging/Database");
            this.AccountStorage = new DataStorage<List<Account>>(EnvyLoggingPlugin.Instance.Directory + "/Database", "Accounts.json");
            Logger.Log("Connected");
        }

        public void Reload()
        {
            Data = AccountStorage.Read();
            if (Data == null)
            {
                Data = new List<Account>();
                AccountStorage.Save(Data);
            }
        }

        public void AddAccount(CSteamID id, string name, string lastname, string sexo, int year)
        {
            Account account = new Account
            {
                Id = id,
                Name = name,
                Lastname = lastname,
                Age = year,
                Sexo = sexo
            };

            Data.Add(account);
            AccountStorage.Save(Data);
        }

        public void RemoveAccount(CSteamID id)
        {
            var data = Data.Find(x => x.Id == id);
            if (data != null) Data.Remove(data);
            AccountStorage.Save(Data);
        }

        public Account GetAccount(CSteamID id) {return Data.Find(x => x.Id == id);}
    }
}
