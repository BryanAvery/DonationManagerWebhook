using DonationManagerWebHooks.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace DonationManagerWebHooks.WordPressService
{
    public class DonationManagerService : IDonationManagerService
    {
        private MySqlDatabase _mySqlDatabase { get; set; }
        private readonly string prefix;
        private readonly string suffix;
        private readonly ILogger<DonationManagerService> logger;

        public DonationManagerService(MySqlDatabase mySqlDatabase, ILogger<DonationManagerService> logger, IConfiguration configuration)
        {
            _mySqlDatabase = mySqlDatabase;
            this.logger = logger;
            prefix = configuration["prefix"];
            suffix = configuration["suffix"];
        }

        public async Task<Supporter> SubscriptionAsync(Supporter supporter)
        {
            if (supporter == null) throw new ArgumentNullException(nameof(supporter));

            if (supporter.mailinglist.ToLower() == "false")
            {
                logger.LogInformation("Supporter does not want to be added to the Mailing list");
                return supporter;
            }

            var id = await InsertSubscriberAsync(supporter?.email);

            if (id == 0)
            {
                logger.LogWarning($"Conflict - {supporter.email}");
                return supporter;
            }

            await InsertSubscriber_MetaAsync(id);

            await InsertSubscriber_FieldsAsync(id, supporter);

            await InsertLists_SubscribersAsync(id, "Donation Manager");

            logger.LogInformation($"Subscriber added - {supporter.email}");

            return supporter;
        }

        public async Task<Appeal> AppealAsync(Appeal appeal)
        {
            if (appeal == null) throw new ArgumentNullException(nameof(Appeal));

            logger.LogWarning("Appeal is not implemented");

            return appeal;
        }

        internal async Task<int> InsertSubscriberAsync(string email)
        {
            if (string.IsNullOrEmpty(email)) throw new ArgumentNullException(nameof(email));

            using var cmd = _mySqlDatabase.Connection.CreateCommand();

            var emailId = await FindEmailListAsync(email);

            if (emailId == 0)
            {
                cmd.CommandText = @$"INSERT INTO swimtayk_{suffix}.{prefix}mailster_subscribers (hash, email, status, added, updated, signup, confirm) VALUES (SHA2(@email, 256),@email, 1, UNIX_TIMESTAMP(@dt), UNIX_TIMESTAMP(@dt), UNIX_TIMESTAMP(@dt), UNIX_TIMESTAMP(@dt))";

                cmd.Parameters.AddWithValue("@email", email);
                cmd.Parameters.AddWithValue("@dt", DateTime.Now.ToString("yyyy-MM-dd H:mm:ss"));

                await cmd.ExecuteNonQueryAsync();
                return (int)cmd.LastInsertedId;
            }
            else
            {
                return 0;
            }
        }

        internal async Task<int> InsertSubscriber_MetaAsync(int id)
        {
            using (var cmd = _mySqlDatabase.Connection.CreateCommand())
            {
                cmd.CommandText = @$"INSERT INTO swimtayk_{suffix}.{prefix}mailster_subscriber_meta (subscriber_id, meta_key, meta_value) VALUES (@id, 'confirmation', '0');";

                cmd.Parameters.AddWithValue("@id", id);

                await cmd.ExecuteNonQueryAsync();

                return (int)cmd.LastInsertedId;
            }
        }

        internal async Task<int> InsertSubscriber_FieldsAsync(int id, Supporter supporter)
        {
            foreach (PropertyInfo pi in supporter.GetType().GetProperties())
            {
                using (var cmd = _mySqlDatabase.Connection.CreateCommand())
                {
                    var key = pi.Name;
                    var value = pi.GetValue(supporter, null);

                    cmd.CommandText = @$"INSERT INTO swimtayk_{suffix}.{prefix}mailster_subscriber_fields (subscriber_id, meta_key, meta_value) VALUES (@id, @key, @value);";

                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.Parameters.AddWithValue("@key", key);
                    cmd.Parameters.AddWithValue("@value", value);

                    if (value != null)
                        await cmd.ExecuteNonQueryAsync();
                    else
                        logger.LogWarning($"the subscriber - {id}, key:{key} does not have a value so not recorded");
                }
            }
            return id;
        }

        internal async Task<int> InsertLists_SubscribersAsync(int subscriberId, string name)
        {
            var listId = await FindListAsync(name);
            if (listId == 0)
            {
                listId = await InsertListAsync(name);
            }
            await InsertLists(listId, subscriberId);

            return 0;
        }

        internal async Task<int> InsertLists(int list_id, int subscriberId)
        {
            using (var cmd = _mySqlDatabase.Connection.CreateCommand())
            {
                cmd.CommandText = @$"INSERT INTO swimtayk_{suffix}.{prefix}mailster_lists_subscribers (list_id, subscriber_id, added) VALUES (@list_id, @subscriberId, UNIX_TIMESTAMP(@dt));";

                cmd.Parameters.AddWithValue("@list_id", list_id);
                cmd.Parameters.AddWithValue("@subscriberId", subscriberId);
                cmd.Parameters.AddWithValue("@dt", DateTime.Now.ToString("yyyy-MM-dd H:mm:ss"));

                await cmd.ExecuteNonQueryAsync();

                return (int)cmd.LastInsertedId;
            }
        }

        internal async Task<int> InsertListAsync(string name)
        {
            using (var cmd = _mySqlDatabase.Connection.CreateCommand())
            {
                cmd.CommandText = @$"INSERT INTO swimtayk_{suffix}.{prefix}mailster_lists (name, slug, added, updated) VALUES (@name, @name2, UNIX_TIMESTAMP(@dt),  UNIX_TIMESTAMP(@dt));";

                cmd.Parameters.AddWithValue("@name", name);
                cmd.Parameters.AddWithValue("@name2", name.Replace(" ", "_"));
                cmd.Parameters.AddWithValue("@dt", DateTime.Now.ToString("yyyy-MM-dd H:mm:ss"));

                await cmd.ExecuteNonQueryAsync();
                return (int)cmd.LastInsertedId;
            }
        }

        internal async Task<int> FindEmailListAsync(string email)
        {
            using (var cmd = _mySqlDatabase.Connection.CreateCommand())
            {
                cmd.CommandText = @$"SELECT ID FROM swimtayk_{suffix}.{prefix}mailster_subscribers  WHERE email = @email;";

                cmd.Parameters.AddWithValue("@email", email);

                var result = await ReadAllAsync(await cmd.ExecuteReaderAsync());
                return result.Count > 0 ? result.Select(int.Parse).ToList()[0] : 0;
            }
        }

        internal async Task<int> FindListAsync(string name)
        {
            using (var cmd = _mySqlDatabase.Connection.CreateCommand())
            {
                cmd.CommandText = @$"SELECT ID FROM swimtayk_{suffix}.{prefix}mailster_lists WHERE name = @name;";

                cmd.Parameters.AddWithValue("@name", name);

                var result = await ReadAllAsync(await cmd.ExecuteReaderAsync());
                return result.Count > 0 ? result.Select(int.Parse).ToList()[0] : 0;
            }
        }

        internal async Task<List<string>> ReadAllAsync(DbDataReader reader)
        {
            var ids = new List<string>();
            using (reader)
            {
                while (await reader.ReadAsync())
                {
                    var id = reader.GetInt32(0).ToString();
                    ids.Add(id);
                }
            }
            return ids;
        }
    }
}