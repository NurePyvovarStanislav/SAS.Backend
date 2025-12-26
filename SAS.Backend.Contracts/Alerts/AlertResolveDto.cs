namespace SAS.Backend.Contracts.Alerts
{
    public record AlertResolveDto
    {
        public bool IsResolved { get; init; } = true;
    }
}

