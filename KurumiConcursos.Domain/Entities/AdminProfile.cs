namespace KurumiConcursos.Domain.Entities;

public class AdminProfile
{
    public long Id { get; set; }
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;
}