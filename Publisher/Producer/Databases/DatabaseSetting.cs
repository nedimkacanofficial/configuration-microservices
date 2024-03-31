namespace Producer.Databases
{
    public class DatabaseSetting : IDatabaseSetting
    {
        public string? CollectionName { get; set; }
        public string? DatabaseName { get; set; }
        public string? ConnectionString { get; set; }
    }
}
