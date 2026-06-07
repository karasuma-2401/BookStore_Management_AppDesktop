using System.Globalization;
using BookStoreManagement.API.Data;
using BookStoreManagement.API.Models.DTOs;
using BookStoreManagement.API.Models.Entities;
using BookStoreManagement.API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BookStoreManagement.API.Services
{
    public class SettingService : ISettingService
    {
        private readonly ApplicationDBContext _context;

        public SettingService(ApplicationDBContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<SettingResponseDTO>> GetAll()
        {
            return await _context.Settings
                .Select(x => new SettingResponseDTO
                {
                    SettingName = x.SettingName,
                    Value = x.Value
                })
                .ToListAsync();
        }

        public async Task<SettingResponseDTO?> GetByName(string name)
        {
            return await _context.Settings
                .Where(x => x.SettingName == name)
                .Select(x => new SettingResponseDTO
                {
                    SettingName = x.SettingName,
                    Value = x.Value
                })
                .FirstOrDefaultAsync();
        }

        public async Task<SettingResponseDTO> Create(string name, string value)
        {
            var exists = await _context.Settings.AnyAsync(x => x.SettingName == name);
            if (exists)
                throw new Exception("Setting already exists");

            var setting = new Settings
            {
                SettingName = name,
                Value = value
            };

            _context.Settings.Add(setting);
            await _context.SaveChangesAsync();

            return new SettingResponseDTO
            {
                SettingName = setting.SettingName,
                Value = setting.Value
            };
        }

        public async Task<SettingResponseDTO?> Update(string name, string value)
        {
            var setting = await _context.Settings.FindAsync(name);
            if (setting == null)
                return null;

            setting.Value = value;

            await _context.SaveChangesAsync();

            return new SettingResponseDTO
            {
                SettingName = setting.SettingName,
                Value = setting.Value
            };
        }

        public async Task<bool> Delete(string name)
        {
            var setting = await _context.Settings.FindAsync(name);
            if (setting == null)
                return false;

            _context.Settings.Remove(setting);
            await _context.SaveChangesAsync();

            return true;
        }
        public async Task<int> GetInt(string key)
        {
            var value = await _context.Settings
                .Where(x => x.SettingName == key)
                .Select(x => x.Value)
                .FirstOrDefaultAsync();

            if (value == null)
                throw new Exception($"Setting {key} not found");

            return int.Parse(value, CultureInfo.InvariantCulture);
        }

        public async Task<decimal> GetDecimal(string key)
        {
            var value = await _context.Settings
                .Where(x => x.SettingName == key)
                .Select(x => x.Value)
                .FirstOrDefaultAsync();

            if (value == null)
                throw new Exception($"Setting {key} not found");

            return decimal.Parse(value, CultureInfo.InvariantCulture);
        }
    }
}