namespace WinNetworkProcessMonitor.Models
{
    public class Filename
    {
        public string? Path { get; set; }
        public Hashes? Hashes { get; set; }
    }

    public class Hashes
    {
        public string? Md5 { get; set; }
        public string? Sha256 { get; set; }
        public string? Sha512 { get; set; }
    }
}
