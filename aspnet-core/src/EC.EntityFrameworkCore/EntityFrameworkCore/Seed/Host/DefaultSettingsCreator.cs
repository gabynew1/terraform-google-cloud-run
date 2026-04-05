using System.Linq;
using Microsoft.EntityFrameworkCore;
using Abp.Configuration;
using Abp.Localization;
using Abp.MultiTenancy;
using Abp.Net.Mail;
using EC.Configuration;

namespace EC.EntityFrameworkCore.Seed.Host
{
    public class DefaultSettingsCreator
    {
        private readonly ECDbContext _context;

        public DefaultSettingsCreator(ECDbContext context)
        {
            _context = context;
        }

        public void Create()
        {
            int? tenantId = null;

            // Emailing
            AddSettingIfNotExists(EmailSettingNames.DefaultFromAddress, "admin@mydomain.com", tenantId);
            AddSettingIfNotExists(EmailSettingNames.DefaultFromDisplayName, "mydomain.com mailer", tenantId);

            // Languages
            AddSettingIfNotExists(LocalizationSettingNames.DefaultLanguage, "en", tenantId);

            // Login — use AddOrUpdateSetting to ensure these are always correctly set,
            // even in databases that were seeded before these values were configured.
            AddOrUpdateSetting(AppSettingNames.EnableNormalLogin, "true", tenantId);
            AddOrUpdateSetting(AppSettingNames.EnableLoginGoogle, "true", tenantId);
            AddOrUpdateSetting(AppSettingNames.GoogleClientId, "985569266142-c30ivcffpd9fmji9i3t6fds1hordh1ks.apps.googleusercontent.com", tenantId);
        }

        private void AddSettingIfNotExists(string name, string value, int? tenantId = null)
        {
            if (_context.Settings.IgnoreQueryFilters().Any(s => s.Name == name && s.TenantId == tenantId && s.UserId == null))
            {
                return;
            }

            _context.Settings.Add(new Setting(tenantId, null, name, value));
            _context.SaveChanges();
        }

        private void AddOrUpdateSetting(string name, string value, int? tenantId = null)
        {
            var existing = _context.Settings.IgnoreQueryFilters()
                .FirstOrDefault(s => s.Name == name && s.TenantId == tenantId && s.UserId == null);

            if (existing == null)
            {
                _context.Settings.Add(new Setting(tenantId, null, name, value));
            }
            else if (string.IsNullOrEmpty(existing.Value) || existing.Value != value)
            {
                existing.Value = value;
            }

            _context.SaveChanges();
        }
    }
}
