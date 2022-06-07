namespace HoaMokuBot.Config
{
    using Contracts;
    using Newtonsoft.Json;

    public class Config : IConfig
    {
        private readonly ConfigRoot configRoot;

        public Config()
        {
            this.configRoot = this.Load();
        }

        public string GetBotPrefix(ulong guildId)
        {
            return this.GetOrAddGuildConfig(guildId).BotPrefix;
        }

        public void SetBotPrefix(ulong guildId, string prefix)
        {
            this.GetOrAddGuildConfig(guildId).BotPrefix = prefix;
            this.Save();
        }

        public ConfigAutoJoin? GetAutoJoin(ulong guildId)
        {
            return this.GetOrAddGuildConfig(guildId).AutoJoin;
        }

        public void SetAutoJoin(ulong guildId, ulong voiceChannelId, ulong textChannelId)
        {
            this.GetOrAddGuildConfig(guildId).AutoJoin = new ConfigAutoJoin { VoiceChannelId = voiceChannelId, TextChannelId = textChannelId };
            this.Save();
        }

        public void DeleteAutoJoin(ulong guildId)
        {
            this.GetOrAddGuildConfig(guildId).AutoJoin = default;
            this.Save();
        }

        private ConfigGuild GetOrAddGuildConfig(ulong guildId)
        {
            if (!this.configRoot.Guilds.ContainsKey(guildId))
            {
                this.configRoot.Guilds.Add(guildId, this.GetDefaultGuildConfig());
                this.Save();
            }

            return this.configRoot.Guilds[guildId];
        }

        private ConfigGuild GetDefaultGuildConfig()
        {
            return new ConfigGuild
            {
                BotPrefix = "$"
            };
        }

        private ConfigRoot Load()
        {
            if (!Directory.Exists("Resources"))
            {
                Directory.CreateDirectory("Resources");
            }

            if (File.Exists("Resources/Config.json"))
            {
                var configFileContents = File.ReadAllText("Resources/Config.json");

                if (!String.IsNullOrWhiteSpace(configFileContents))
                {
                    var serializedConfig = JsonConvert.DeserializeObject<ConfigRoot>(configFileContents);

                    if (serializedConfig != null)
                    {
                        return serializedConfig;
                    }
                }
            }

            return new ConfigRoot
            {
                Guilds = new Dictionary<ulong, ConfigGuild>()
            };
        }

        private void Save()
        {
            var jsonConfig = JsonConvert.SerializeObject(this.configRoot, Formatting.Indented);
            File.WriteAllText("Resources/Config.json", jsonConfig);
        }
    }
}