using Dapper;
using Solnet.Wallet;
using System.Data;
using System.Text.Json;

namespace Bundler.Services
{
    public class WalletRecord
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string SecretJson { get; set; } = string.Empty;
    }

    public class WalletService
    {
        private readonly IDbConnection _db;
        public WalletService(IDbConnection db)
        {
            _db = db;
            _db.Execute(
                "CREATE TABLE IF NOT EXISTS Wallets (Id INTEGER PRIMARY KEY, Name TEXT, SecretJson TEXT)");
        }
        public IEnumerable<WalletRecord> GetAll() =>
            _db.Query<WalletRecord>("SELECT * FROM Wallets");

        public void Add(string name, string secretJson)
        {
            _db.Execute(
                "INSERT INTO Wallets (Name, SecretJson) VALUES (@Name,@Json)",
                new { Name = name, Json = secretJson });
        }

        public void Delete(int id) =>
            _db.Execute("DELETE FROM Wallets WHERE Id=@Id", new { Id = id });

        public Account LoadAccount(int id)
        {
            var rec = _db.QuerySingle<WalletRecord>(
                "SELECT * FROM Wallets WHERE Id = @Id", new { Id = id });

            var secret = rec.SecretJson.Trim();

            // If you ever still have JSON-arrays in the DB, you can detect & parse:
            if (secret.StartsWith("["))
            {
                var arr = JsonSerializer.Deserialize<byte[]>(secret)!;
                int half = arr.Length / 2;
                var priv = arr.Take(half).ToArray();
                var pub = arr.Skip(half).ToArray();
                return new Account(priv, pub);
            }
            else
            {
                // Base58 seed
                return Account.FromSecretKey(secret);
            }
        }


        public string ExportAllJson()
        {
            var all = GetAll().Select(r => new {
                r.Name,
                PublicKey = LoadAccount(r.Id).PublicKey.Key,
                SecretJson = r.SecretJson
            });
            return JsonSerializer.Serialize(all, new JsonSerializerOptions { WriteIndented = true });
        }
    }
}
