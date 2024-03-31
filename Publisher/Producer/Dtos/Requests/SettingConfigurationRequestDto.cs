namespace Producer.Dtos.Requests
{
    public class SettingConfigurationRequestDto
    {
        public string? Name { get; set; }
        public string? Type { get; set; }
        public string? Value { get; set; }
        public bool IsActive { get; set; }
        public string? ApplicationName { get; set; }
    }
}
