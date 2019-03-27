namespace TimetableLoader
{
    public interface ILoaderConfig
    {
        string TimetableArchiveFile { get; }
        bool IsRdgZip { get; }
        string ConnectionString { get; }
    }
}