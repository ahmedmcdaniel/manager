using Microsoft.EntityFrameworkCore;
using SchoolManager.Application.Interfaces;
using SchoolManager.Models;
using SchoolManager.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SchoolManager.Infrastructure.Services
{
    public class AreaService : IAreaService
    {
        private readonly SchoolDbContext _context;

        public AreaService(SchoolDbContext context)
        {
            _context = context;
        }

        public async Task<Area> GetOrCreateAsync(string name)
        {
            name = name.Trim().ToUpper();

            var area = await _context.Areas
                .FirstOrDefaultAsync(a => a.Name.ToUpper() == name);

            if (area == null)
            {
                area = new Area
                {
                    Id = Guid.NewGuid(),
                    Name = name,
                    CreatedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified)
                };

                _context.Areas.Add(area);
                await _context.SaveChangesAsync();
            }

            return area;
        }

        public async Task<List<Area>> GetAllAsync()
        {
            return await _context.Areas.ToListAsync();
        }

        public async Task<Area?> GetByIdAsync(Guid id)
        {
            return await _context.Areas.FindAsync(id);
        }

        public async Task<Area> CreateAsync(Area area)
        {
            area.Id = Guid.NewGuid();
            area.CreatedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified);

            _context.Areas.Add(area);
            await _context.SaveChangesAsync();

            return area;
        }

        public async Task<Area> UpdateAsync(Area area)
        {
            var existing = await _context.Areas.FindAsync(area.Id);
            if (existing == null)
                throw new InvalidOperationException("Área no encontrada.");

            existing.Name = area.Name.Trim();
            // puedes actualizar más propiedades aquí

            _context.Areas.Update(existing);
            await _context.SaveChangesAsync();

            return existing;
        }

        public async Task DeleteAsync(Guid id)
        {
            var area = await _context.Areas.FindAsync(id);
            if (area != null)
            {
                _context.Areas.Remove(area);
                await _context.SaveChangesAsync();
            }
        }
    }
}
