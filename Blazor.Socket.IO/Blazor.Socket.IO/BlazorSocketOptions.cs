namespace Blazor.Socket.IO;

public class BlazorSocketOptions
{
    public bool ForceNew { get; set; } = false;
    public bool Multiplex { get; set; } = true;
    public bool AddTrailingSlash { get; set; } = true;
    public bool AutoUnref { get; set; } = false;
    public bool CloseOnBeforeunload { get; set; } = true;
    public Dictionary<string, string>? ExtraHeaders { get; set; }
    public bool ForceBase64 { get; set; } = false;
    public string? Path { get; set; }
    public string[]? Protocols { get; set; }
    public string? Query { get; set; }
    public bool RememberUpgrade { get; set; } = false;
    public string TimestampParam { get; set; } = "t";
    public bool TimestampRequests { get; set; } = true;
    public string[] Transports { get; set; } = { "polling", "websocket" };
    public bool Upgrade { get; set; } = true;
    public bool WithCredentials { get; set; } = false;
}