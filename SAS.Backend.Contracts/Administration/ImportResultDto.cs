namespace SAS.Backend.Contracts.Administration
{
    public sealed record ImportResultDto(
        int Created,
        int Updated,
        int Skipped,
        IReadOnlyList<string> Warnings
    );
}
