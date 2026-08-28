using KurumiConcursos.Domain.Entities;
using KurumiConcursos.Domain.Entities.IdentityEntities;
using KurumiConcursos.Infra.Interfaces.RepositoryContracts;
using KurumiConcursos.Infra.ORM.DataSeeds;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace KurumiConcursos.Infra.ORM.Context;

public sealed class DbInitializer
{
    private readonly ApplicationContext _context;
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher<User> _passwordHasher;

    public DbInitializer(ApplicationContext context, IUserRepository userRepository,
        IPasswordHasher<User> passwordHasher)
    {
        _context = context;
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
    }

    public async Task Seed()
    {
        await EnsureRolesAsync();
        await EnsureUsersAsync();
        await EnsurePersonalDataAsync();
        await EnsureAdminProfilesAsync();
    }

    private Task<int> SaveAsync() => _context.SaveChangesAsync();
    private void AddRange<T>(IEnumerable<T> entities) where T : class => _context.Set<T>().AddRange(entities);

    private async Task EnsureRolesAsync()
    {
        var existingTypes = await _context.Set<Role>()
            .Select(role => role.Type)
            .ToListAsync();
        var missingRoles = RoleSeed.CreateRolesSeed()
            .Where(role => !existingTypes.Contains(role.Type))
            .ToList();
        if (missingRoles.Count == 0) return;
        AddRange(missingRoles);
        await SaveAsync();
    }

    private async Task EnsureUsersAsync()
    {
        if (await _context.Set<User>().AnyAsync(user => user.NormalizedEmail == "ADMIN@KURUMICONCURSOS.COM")) return;

        foreach (var user in UserSeed.CreateUserSeed())
        {
            if (UserSeed.DefaultPasswords.TryGetValue(user.Id, out var plainPassword))
            {
                user.PasswordHash = _passwordHasher.HashPassword(user, plainPassword);
            }

            var result = await _userRepository.SaveAsync(user);
            if (!result.Succeeded)
                throw new InvalidOperationException(string.Join("; ", result.Errors.Select(error => error.Description)));
        }

        await SaveAsync();
    }

    private async Task EnsurePersonalDataAsync()
    {
        if (await _context.Set<PersonalData>().AnyAsync()) return;
        var data = UserSeed.CreatePersonalDataSeed();
        AddRange(data);
        await SaveAsync();
    }

    private async Task EnsureAdminProfilesAsync()
    {
        if (await _context.Set<AdminProfile>().AnyAsync()) return;
        var admins = UserSeed.CreateAdminProfilesSeed();
        AddRange(admins);
        await SaveAsync();
    }
}
