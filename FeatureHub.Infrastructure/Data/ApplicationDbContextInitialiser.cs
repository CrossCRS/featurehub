using FeatureHub.Domain.Constants;
using FeatureHub.Domain.Entities;
using FeatureHub.Infrastructure.Identity;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace FeatureHub.Infrastructure.Data;

public static class InitialiserExtensions
{
    public static async Task InitialiseDatabaseAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();

        var initialiser = scope.ServiceProvider.GetRequiredService<ApplicationDbContextInitialiser>();

        //await initialiser.InitialiseAsync();
        await initialiser.SeedAsync();
    }
}

public class ApplicationDbContextInitialiser
{
    private readonly ILogger<ApplicationDbContextInitialiser> _logger;
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public ApplicationDbContextInitialiser(ILogger<ApplicationDbContextInitialiser> logger, ApplicationDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        _logger = logger;
        _context = context;
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public async Task InitialiseAsync()
    {
        try
        {
            await _context.Database.EnsureDeletedAsync();
            await _context.Database.EnsureCreatedAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while initialising the database.");
            throw;
        }
    }

    public async Task SeedAsync()
    {
        try
        {
            await TrySeedAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while seeding the database.");
            throw;
        }
    }

    public async Task TrySeedAsync()
    {
        // Roles
        var administratorRole = new IdentityRole(Roles.Administrator);

        if (!_roleManager.Roles.Any(r => r.Name == administratorRole.Name))
        {
            await _roleManager.CreateAsync(administratorRole);
        }

        var userRole = new IdentityRole(Roles.User);

        if (!_roleManager.Roles.Any(r => r.Name == userRole.Name))
        {
            await _roleManager.CreateAsync(userRole);
        }

        var demoRole = new IdentityRole(Roles.Demo);

        if (!_roleManager.Roles.Any(r => r.Name == demoRole.Name))
        {
            await _roleManager.CreateAsync(demoRole);
        }

        // Users
        var administrator = _userManager.Users.FirstOrDefault(u => u.UserName == "administrator");

        if (administrator == null)
        {
            administrator = new ApplicationUser { UserName = "administrator", Email = "administrator@localhost" };
            await _userManager.CreateAsync(administrator, "P@ssw0rd!");
            await _userManager.AddToRolesAsync(administrator, [Roles.Administrator]);
        }

        var user = _userManager.Users.FirstOrDefault(u => u.UserName == "user");

        if (user == null)
        {
            user = new ApplicationUser { UserName = "user", Email = "user@localhost" };
            await _userManager.CreateAsync(user, "P@ssw0rd!");
            await _userManager.AddToRolesAsync(user, [Roles.User]);
        }

        var demoUser = _userManager.Users.FirstOrDefault(u => u.UserName == "demo");

        if (demoUser == null)
        {
            demoUser = new ApplicationUser { UserName = "demo", Email = "demo@localhost" };
            await _userManager.CreateAsync(demoUser, "P@ssw0rd!");
            await _userManager.AddToRolesAsync(demoUser, [Roles.Demo]);
        }

        // Data here, if necessary
        if (!_context.Projects.Any())
        {
            _context.Projects.Add(new Project { OwnerId = administrator.Id, Name = "Admin Project 1" });
            _context.Projects.Add(new Project { OwnerId = user.Id, Name = "User Project 1" });
            _context.Projects.Add(new Project { OwnerId = demoUser.Id, Name = "Demo Project 1" });
        }

        var demoProject = _context.Projects.FirstOrDefault(p => p.Name == "Demo Project 1");

        if (demoProject != null && !_context.Environments.Any(e => e.ProjectId == demoProject.Id))
        {
            _context.Environments.Add(new Domain.Entities.Environment { ProjectId = demoProject.Id, Name = "Demo Environment 1", Token = "123e4567e89b12d3a456426614174000" });
        }

        await _context.SaveChangesAsync();
    }
}
