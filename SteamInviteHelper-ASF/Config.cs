using ArchiSteamFarm.Steam;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;

namespace SteamInviteHelper_ASF
{
    class Config
    {
        private const string defaultConfig = @"{""SteamInviteHelper"":{""Enabled"":false,""ActionPriority"":[""block"",""ignore"",""add"",""none""],""PrivateProfile"":{""action"":""block""},""SteamRepScammer"":{""action"":""block""},""SteamLevel"":[{""condition"":""default"",""value"":-1,""action"":""none""},{""condition"":""less_than"",""value"":1,""action"":""block""},{""condition"":""less_than"",""value"":5,""action"":""ignore""}],""VACBanned"":[{""condition"":""default"",""value"":-1,""action"":""none""},{""condition"":""more_than"",""value"":1,""action"":""ignore""}],""GameBanned"":[{""condition"":""default"",""value"":-1,""action"":""none""},{""condition"":""more_than"",""value"":1,""action"":""ignore""}],""DaysSinceLastBan"":[{""condition"":""default"",""value"":-1,""action"":""none""},{""condition"":""less_than"",""value"":90,""action"":""ignore""}],""CommunityBanned"":{""action"":""none""},""EconomyBanned"":{""action"":""none""},""ProfileName"":[{""condition"":""default"",""value"":"""",""action"":""none""},{""condition"":""contain"",""value"":""shittygamblingsite.com"",""action"":""ignore""}],""Comments"":[{""condition"":""default"",""value"":"""",""action"":""none""},{""condition"":""less_than"",""value"":""1"",""action"":""ignore""}]}}";
        public static ConcurrentDictionary<Bot, Config> FriendInviteConfigs = new ConcurrentDictionary<Bot, Config>();

        public bool Enabled { get; set; }
        public List<string> ActionPriority { get; set; }
        public string PrivateProfile { get; set; }
        public string SteamRepScammer { get; set; }
        public List<ConfigItem> SteamLevel { get; set; }
        public List<ConfigItem> VacBanned { get; set; }
        public List<ConfigItem> GameBanned { get; set; }
        public List<ConfigItem> DaysSinceLastBan { get; set; }
        public string CommunityBanned { get; set; }
        public string EconomyBanned { get; set; }
        public List<ConfigItem> ProfileName { get; set; }
        public List<ConfigItem> Comments { get; set; }

        public Config(Bot bot, JToken jToken)
        {
            try
            {
                this.Enabled = jToken.Value<bool>("Enabled");
                this.ActionPriority = jToken["ActionPriority"].ToObject<List<string>>();

                this.PrivateProfile = jToken.Value<JToken>("PrivateProfile").Value<string>("action");
                this.SteamRepScammer = jToken.Value<JToken>("SteamRepScammer").Value<string>("action");

                this.SteamLevel = jToken["SteamLevel"].ToObject<List<ConfigItem>>();
                this.VacBanned = jToken["VACBanned"].ToObject<List<ConfigItem>>();
                this.GameBanned = jToken["GameBanned"].ToObject<List<ConfigItem>>();
                this.DaysSinceLastBan = jToken["DaysSinceLastBan"].ToObject<List<ConfigItem>>();

                this.CommunityBanned = jToken.Value<JToken>("CommunityBanned").Value<string>("action");
                this.EconomyBanned = jToken.Value<JToken>("EconomyBanned").Value<string>("action");

                this.ProfileName = jToken["ProfileName"].ToObject<List<ConfigItem>>();

                this.Comments = jToken["Comments"].ToObject<List<ConfigItem>>();
            }
            catch (Exception e)
            {
                bot.ArchiLogger.LogGenericError("Error when loading config file");
                bot.ArchiLogger.LogGenericError("Exception: " + e.Message);
                bot.ArchiLogger.LogGenericError("Exiting in 5 seconds...");
                Thread.Sleep(5000);
                Environment.Exit(1);
            }
        }

        public static void AppendDefaultConfig(Bot bot)
        {
            string configpath = "./config/" + bot.BotName + ".json";
            string json = File.ReadAllText(configpath);

            try
            {
                JObject o = JObject.Parse(json);

                if (!o.ContainsKey("SteamInviteHelper"))
                {
                    JObject defaultConfigJson = JObject.Parse(defaultConfig);
                    o.Merge(defaultConfigJson);

                    File.WriteAllText(configpath, o.ToString());
                    bot.ArchiLogger.LogGenericWarning("Config not found! Loading default config...");
                    bot.ArchiLogger.LogGenericWarning("Saved default config, please review and edit your bot's config!");
                }
            }
            catch (Exception e)
            {
                bot.ArchiLogger.LogGenericError("Something went wrong while trying to add the default config...");
                bot.ArchiLogger.LogGenericError("Exception: " + e.Message);
                bot.ArchiLogger.LogGenericError("Exiting in 5 seconds...");
                Thread.Sleep(5000);
                Environment.Exit(1);
            }
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("{");
            foreach (var property in this.GetType().GetProperties())
            {
                sb.AppendFormat("\"{0}\": \"{1}\", ", property.Name, property.GetValue(this, null));
            }
            sb.Length -= 2;
            sb.Append("}");
            return sb.ToString();
        }
    }
}
