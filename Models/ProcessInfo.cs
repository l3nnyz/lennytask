namespace ProcessKiller.Models
{
    public class ProcessInfo
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Path { get; set; } = string.Empty;
        public long MemoryUsage { get; set; }
        public DateTime StartTime { get; set; }
        
        public override string ToString()
        {
            return $"ID: {Id}, Name: {Name}, Memory: {MemoryUsage / 1024 / 1024}MB, Started: {StartTime:HH:mm:ss}";
        }
    }
}
