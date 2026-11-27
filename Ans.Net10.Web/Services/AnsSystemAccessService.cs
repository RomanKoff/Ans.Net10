// rev 2026-11-27

using Ans.Net10.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System.Globalization;
using System.Security.Cryptography;

namespace Ans.Net10.Web.Services
{

    /// <summary>
    /// Служба проверки прав доступа к системным эндпоинтам и диагностическим представлениям библиотеки.
    /// </summary>
    public sealed partial class AnsSystemAccessService
    {

        private const string COOKIE_NAME = ".Ans.SystemAccess";
        private const string PROTECTOR_PURPOSE = "Ans.Net10.Web.SystemAccess.v1";
        private const int MAX_FAILED_ATTEMPTS = 5;
        private static readonly TimeSpan BLOCK_DURATION = TimeSpan.FromMinutes(10);

        private readonly CurrentContext _current;
        private readonly IAuthorizationService _authorizationService;
        private readonly IDataProtector _protector;
        private readonly IMemoryCache _cache;
        private readonly ILogger<AnsSystemAccessService> _logger;


        /* ctor */


        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="AnsSystemAccessService"/>.
        /// </summary>
        /// <param name="current">Текущий оркестровый контекст обработки HTTP-запроса.</param>
        /// <param name="authorizationService">Служба проверки политик авторизации ASP.NET Core.</param>
        /// <param name="dataProtectionProvider">Провайдер криптографической защиты данных.</param>
        /// <param name="cache">Кэш в памяти для отслеживания неудачных попыток входа.</param>
        /// <param name="logger">Экземпляр логгера.</param>
        public AnsSystemAccessService(
            CurrentContext current,
            IAuthorizationService authorizationService,
            IDataProtectionProvider dataProtectionProvider,
            IMemoryCache cache,
            ILogger<AnsSystemAccessService> logger)
        {
            _current = current;
            _authorizationService = authorizationService;
            _protector = dataProtectionProvider.CreateProtector(PROTECTOR_PURPOSE);
            _cache = cache;
            _logger = logger;
        }


        /* readonly properties */


        /// <summary>
        /// Возвращает признак наличия записей в списке персональных ключей доступа (<see cref="SystemHelpersOptions.AccessList"/>).
        /// </summary>
        public bool HasAccessList
            => _current.Options.SystemHelpers.AccessList is { Count: > 0 };


        /// <summary>
        /// Возвращает имя пользователя, авторизованного по персональному ключу через сессионную куку, либо <see langword="null"/>.
        /// </summary>
        public string? AuthenticatedKeyUser
            => _getAuthenticatedCookieUser(_current.HttpContext);


        /* functions */


        /// <summary>
        /// Асинхронно проверяет, разрешен ли текущему запросу доступ к системным ресурсам.
        /// </summary>
        /// <returns><see langword="true"/>, если доступ разрешен; иначе — <see langword="false"/>.</returns>
        public async ValueTask<bool> HasAccessAsync()
        {
            var ctx1 = _current.HttpContext;
            var sys1 = _current.Options.SystemHelpers;

            if (sys1.AllowInDeveloperMode && _current.Options.UseDeveloperMode)
                return true;

            if (_checkSubnet(sys1.AllowedSubnet))
                return true;

            if (!string.IsNullOrWhiteSpace(sys1.RequiredPolicy)
                && ctx1.User.Identity?.IsAuthenticated == true)
            {
                var authRes1 = await _authorizationService
                    .AuthorizeAsync(ctx1.User, null, sys1.RequiredPolicy)
                    .ConfigureAwait(false);
                if (authRes1.Succeeded)
                    return true;
            }

            if (HasAccessList && _getAuthenticatedCookieUser(ctx1) is not null)
                return true;

            return false;
        }


        /// <summary>
        /// Асинхронно выполняет аутентификацию пользователя по имени и персональному ключу с установкой защищенной куки.
        /// </summary>
        /// <param name="username">Имя пользователя из списка доступа.</param>
        /// <param name="key">Персональный ключ доступа.</param>
        /// <returns>Результат попытки входа <see cref="SystemAccessLoginResult"/>.</returns>
        public async ValueTask<SystemAccessLoginResult> TryLoginAsync(
            string? username,
            string? key)
        {
            await Task.CompletedTask;
            var ctx1 = _current.HttpContext;
            if (!HasAccessList)
                return SystemAccessLoginResult.InvalidCredentials;

            var ip1 = ctx1.Connection.RemoteIpAddress?.ToString() ?? "unknown";
            var cacheKey1 = $"ans_sys_fail:{ip1}";
            if (_cache.TryGetValue<int>(cacheKey1, out var fails1) && fails1 >= MAX_FAILED_ATTEMPTS)
            {
                _log.BlockedByRateLimit(_logger, ip1);
                return SystemAccessLoginResult.TooManyAttempts;
            }

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(key))
            {
                _registerFailure(cacheKey1, fails1);
                return SystemAccessLoginResult.InvalidCredentials;
            }

            var trimmedUser1 = username.Trim();
            string? matchedUser1 = null;
            string? storedHash1 = null;

            foreach (var kv1 in _current.Options.SystemHelpers.AccessList!)
            {
                if (string.Equals(kv1.Key, trimmedUser1, StringComparison.OrdinalIgnoreCase))
                {
                    matchedUser1 = kv1.Key;
                    storedHash1 = kv1.Value;
                    break;
                }
            }

            if (matchedUser1 is null
                || string.IsNullOrWhiteSpace(storedHash1)
                || !SuppCrypto.VerifyPassword(key, storedHash1))
            {
                _registerFailure(cacheKey1, fails1);
                _log.FailedLogin(_logger, trimmedUser1, ip1);
                return SystemAccessLoginResult.InvalidCredentials;
            }

            _cache.Remove(cacheKey1);

            var lifetimeHours1 = Math.Clamp(_current.Options.SystemHelpers.CookieLifetimeHours, 1, 168);
            var expiresUtc1 = DateTimeOffset.UtcNow.AddHours(lifetimeHours1);
            var payload1 = $"{matchedUser1}|{expiresUtc1.ToUnixTimeSeconds()}";
            var protected1 = _protector.Protect(payload1);

            ctx1.Response.Cookies.Append(COOKIE_NAME, protected1, new CookieOptions
            {
                HttpOnly = true,
                Secure = ctx1.Request.IsHttps,
                SameSite = SameSiteMode.Strict,
                Expires = expiresUtc1,
                IsEssential = true
            });

            _log.SucceededLogin(_logger, matchedUser1, ip1);
            return SystemAccessLoginResult.Success;
        }


        /* methods */


        /// <summary>
        /// Завершает сессию доступа по персональному ключу и удаляет защищенную куку.
        /// </summary>
        public void Logout()
        {
            var ctx1 = _current.HttpContext;
            var user1 = _getAuthenticatedCookieUser(ctx1);
            ctx1.Response.Cookies.Delete(COOKIE_NAME, new CookieOptions
            {
                HttpOnly = true,
                Secure = ctx1.Request.IsHttps,
                SameSite = SameSiteMode.Strict
            });

            if (user1 is not null)
                _log.LogoutUser(_logger, user1);
        }


        /* privates */


        private bool _checkSubnet(
            string? allowedSubnet)
        {
            if (string.IsNullOrWhiteSpace(allowedSubnet))
                return false;

            return allowedSubnet.Trim().ToLowerInvariant() switch
            {
                "admin" => _current.Network.IsAdmin,
                "safe" => _current.Network.IsSafe,
                "allow" => _current.Network.IsAllow,
                _ => false
            };
        }


        private string? _getAuthenticatedCookieUser(
            HttpContext ctx)
        {
            if (!HasAccessList)
                return null;

            if (!ctx.Request.Cookies.TryGetValue(COOKIE_NAME, out var rawCookie1)
                || string.IsNullOrWhiteSpace(rawCookie1))
                return null;

            try
            {
                var unprotected1 = _protector.Unprotect(rawCookie1);
                var sep1 = unprotected1.LastIndexOf('|');
                if (sep1 <= 0)
                    return null;

                var user1 = unprotected1[..sep1];
                if (!long.TryParse(unprotected1[(sep1 + 1)..], CultureInfo.InvariantCulture, out var expUnix1))
                    return null;

                if (DateTimeOffset.UtcNow.ToUnixTimeSeconds() >= expUnix1)
                    return null;

                foreach (var kv1 in _current.Options.SystemHelpers.AccessList!)
                {
                    if (string.Equals(kv1.Key, user1, StringComparison.OrdinalIgnoreCase))
                        return kv1.Key;
                }
                return null;
            }
            catch (CryptographicException)
            {
                return null;
            }
        }


        private void _registerFailure(
            string cacheKey,
            int currentFails)
        {
            _cache.Set(cacheKey, currentFails + 1, BLOCK_DURATION);
        }


        /* logs */


        private static partial class _log
        {
            [LoggerMessage(
                Level = LogLevel.Warning,
                Message = "System access login failed for user '{Username}' from IP {IpAddress}.")]
            public static partial void FailedLogin(
                ILogger logger, string username, string ipAddress);

            [LoggerMessage(
                Level = LogLevel.Warning,
                Message = "System access blocked due to too many failed attempts from IP {IpAddress}.")]
            public static partial void BlockedByRateLimit(
                ILogger logger, string ipAddress);

            [LoggerMessage(
                Level = LogLevel.Information,
                Message = "System access granted to user '{Username}' from IP {IpAddress}.")]
            public static partial void SucceededLogin(
                ILogger logger, string username, string ipAddress);

            [LoggerMessage(
                Level = LogLevel.Information,
                Message = "System access session closed for user '{Username}'.")]
            public static partial void LogoutUser(
                ILogger logger, string username);
        }

    }



    /// <summary>
    /// Перечисление результатов попытки аутентификации по персональному ключу системного доступа.
    /// </summary>
    public enum SystemAccessLoginResult
    {
        /// <summary>
        /// Вход выполнен успешно.
        /// </summary>
        Success = 0,

        /// <summary>
        /// Неверное имя пользователя или персональный ключ.
        /// </summary>
        InvalidCredentials = 1,

        /// <summary>
        /// Превышено количество неудачных попыток входа.
        /// </summary>
        TooManyAttempts = 2
    }

}
