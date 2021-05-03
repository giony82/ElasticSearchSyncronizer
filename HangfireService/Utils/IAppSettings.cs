namespace SchoolUtils
{
    public interface IAppSettings
    {
        T Get<T>(string name, T defaultValue = default);
    }
}