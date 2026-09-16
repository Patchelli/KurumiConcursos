using KurumiConcursos.Domain.Entities;
using KurumiConcursos.Domain.Entities.IdentityEntities;
using KurumiConcursos.Domain.Providers;
using KurumiConcursos.Domain.UserPolicies;
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
    private readonly PrivateMaterialsAccessOptions _privateMaterials;

    public DbInitializer(ApplicationContext context, IUserRepository userRepository,
        IPasswordHasher<User> passwordHasher, PrivateMaterialsAccessOptions privateMaterials)
    {
        _context = context;
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _privateMaterials = privateMaterials;
    }

    public async Task Seed()
    {
        await EnsureRolesAsync();
        await EnsureUsersAsync();
        await EnsurePersonalDataAsync();
        await EnsureAdminProfilesAsync();
        await EnsurePrivateMaterialsPermissionsAsync();
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
                throw new InvalidOperationException(string.Join("; ",
                    result.Errors.Select(error => error.Description)));
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

    private async Task EnsurePrivateMaterialsPermissionsAsync()
    {
        if (_privateMaterials.Emails.Count == 0) return;
        var users = await _context.Set<User>().Where(user => user.NormalizedEmail != null &&
                                                             _privateMaterials.Emails.Contains(user.NormalizedEmail))
            .ToListAsync();
        var ids = users.Select(user => user.Id).ToHashSet();
        var existing = await _context.Set<UserClaim>().Where(claim => ids.Contains(claim.UserId) &&
                                                                      claim.ClaimType == PrivateMaterials.ClaimType &&
                                                                      claim.ClaimValue == PrivateMaterials.Permission)
            .ToListAsync();
        var granted = existing.Select(claim => claim.UserId).ToHashSet();
        var missing = users.Where(user => !granted.Contains(user.Id)).Select(user => new UserClaim
        {
            UserId = user.Id, ClaimType = PrivateMaterials.ClaimType, ClaimValue = PrivateMaterials.Permission
        }).ToList();
        if (missing.Count == 0) return;
        AddRange(missing);
        await SaveAsync();
    }
}