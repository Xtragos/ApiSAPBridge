using Microsoft.EntityFrameworkCore;
using ApiSAPBridge.Configuration.Data;
using ApiSAPBridge.Configuration.Models;
using ApiSAPBridge.Configuration.Models.DTOs;

namespace ApiSAPBridge.Configuration.Services
{
    public interface ISecurityService
    {
        Task<AuthenticationResult> AuthenticateAsync(string password);
        Task<OperationResult> ChangePasswordAsync(string currentPassword, string newPassword);
        Task<bool> IsLockedOutAsync();
        Task ClearLockoutAsync();
    }

    public class SecurityService : ISecurityService
    {
        private readonly ConfigurationContext _context;
        private readonly IConfigurationService _configurationService;

        public SecurityService(ConfigurationContext context, IConfigurationService configurationService)
        {
            _context = context;
            _configurationService = configurationService;
        }

        public async Task<AuthenticationResult> AuthenticateAsync(string password)
        {
            var securityConfig = await _context.SecurityConfigurations.FirstOrDefaultAsync();

            if (securityConfig == null)
            {
                return new AuthenticationResult
                {
                    IsAuthenticated = false,
                    Message = "Configuración de seguridad no encontrada"
                };
            }

            // Verificar si está bloqueado
            if (await IsLockedOutAsync())
            {
                return new AuthenticationResult
                {
                    IsAuthenticated = false,
                    Message = "Cuenta bloqueada temporalmente. Intente más tarde."
                };
            }

            // Verificar contraseña
            if (!BCrypt.Net.BCrypt.Verify(password, securityConfig.PasswordHash))
            {
                // Incrementar intentos fallidos
                securityConfig.LoginAttempts++;

                var maxAttempts = int.Parse(await _configurationService.GetSystemConfigurationAsync("MaxLoginAttempts") ?? "3");

                if (securityConfig.LoginAttempts >= maxAttempts)
                {
                    var lockoutMinutes = int.Parse(await _configurationService.GetSystemConfigurationAsync("LockoutDurationMinutes") ?? "15");
                    securityConfig.LockedUntil = DateTime.UtcNow.AddMinutes(lockoutMinutes);
                }

                securityConfig.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();

                return new AuthenticationResult
                {
                    IsAuthenticated = false,
                    Message = "Contraseña incorrecta"
                };
            }

            // Autenticación exitosa
            securityConfig.LastLogin = DateTime.UtcNow;
            securityConfig.LoginAttempts = 0;
            securityConfig.LockedUntil = null;
            securityConfig.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return new AuthenticationResult
            {
                IsAuthenticated = true,
                Message = "Autenticación exitosa",
                ExpiresAt = DateTime.UtcNow.AddHours(8) // Sesión válida por 8 horas
            };
        }

        public async Task<OperationResult> ChangePasswordAsync(string currentPassword, string newPassword)
        {
            var authResult = await AuthenticateAsync(currentPassword);
            if (!authResult.IsAuthenticated)
            {
                return OperationResult.Failure("Contraseña actual incorrecta");
            }

            try
            {
                var securityConfig = await _context.SecurityConfigurations.FirstOrDefaultAsync();
                if (securityConfig != null)
                {
                    securityConfig.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
                    securityConfig.UpdatedAt = DateTime.UtcNow;
                    await _context.SaveChangesAsync();
                }

                return OperationResult.Success("Contraseña cambiada correctamente");
            }
            catch (Exception ex)
            {
                return OperationResult.Failure("Error al cambiar contraseña", ex.Message);
            }
        }

        public async Task<bool> IsLockedOutAsync()
        {
            var securityConfig = await _context.SecurityConfigurations.FirstOrDefaultAsync();

            if (securityConfig?.LockedUntil == null)
                return false;

            if (securityConfig.LockedUntil > DateTime.UtcNow)
                return true;

            // El bloqueo ha expirado, limpiarlo
            await ClearLockoutAsync();
            return false;
        }

        public async Task ClearLockoutAsync()
        {
            var securityConfig = await _context.SecurityConfigurations.FirstOrDefaultAsync();
            if (securityConfig != null)
            {
                securityConfig.LockedUntil = null;
                securityConfig.LoginAttempts = 0;
                securityConfig.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }
        }
    }
}