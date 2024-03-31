namespace Producer.Databases
{
    public interface IDatabaseSetting
    {
        public string? CollectionName { get; set; }
        public string? DatabaseName { get; set; }
        public string? ConnectionString { get; set; }
    }
}
