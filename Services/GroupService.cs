using Microsoft.EntityFrameworkCore;
using ExpenseSharing.Data;
using ExpenseSharing.Models;

namespace ExpenseSharing.Services
{
    public class GroupService
    {
        private readonly ExpenseSharingContext _context;

        public GroupService(ExpenseSharingContext context)
        {
            _context = context;
        }

        public async Task<List<GroupDto>> GetAllGroupsAsync()
        {
            return await _context.Groups
                .Include(g => g.CreatedBy)
                .Include(g => g.Members)
                .Select(g => new GroupDto
                {
                    Id = g.Id,
                    Name = g.Name,
                    Description = g.Description,
                    CreatedById = g.CreatedById,
                    CreatedByName = g.CreatedBy.Name,
                    Members = g.Members.Select(m => new UserDto
                    {
                        Id = m.Id,
                        Username = m.Username,
                        Email = m.Email,
                        Name = m.Name
                    }).ToList()
                })
                .ToListAsync();
        }

        public async Task<GroupDto?> GetGroupByIdAsync(int id)
        {
            var group = await _context.Groups
                .Include(g => g.CreatedBy)
                .Include(g => g.Members)
                .FirstOrDefaultAsync(g => g.Id == id);

            if (group == null) return null;

            return new GroupDto
            {
                Id = group.Id,
                Name = group.Name,
                Description = group.Description,
                CreatedById = group.CreatedById,
                CreatedByName = group.CreatedBy.Name,
                Members = group.Members.Select(m => new UserDto
                {
                    Id = m.Id,
                    Username = m.Username,
                    Email = m.Email,
                    Name = m.Name
                }).ToList()
            };
        }

        public async Task<List<GroupDto>> GetGroupsByUserIdAsync(int userId)
        {
            return await _context.Groups
                .Include(g => g.CreatedBy)
                .Include(g => g.Members)
                .Where(g => g.Members.Any(m => m.Id == userId))
                .Select(g => new GroupDto
                {
                    Id = g.Id,
                    Name = g.Name,
                    Description = g.Description,
                    CreatedById = g.CreatedById,
                    CreatedByName = g.CreatedBy.Name,
                    Members = g.Members.Select(m => new UserDto
                    {
                        Id = m.Id,
                        Username = m.Username,
                        Email = m.Email,
                        Name = m.Name
                    }).ToList()
                })
                .ToListAsync();
        }

        public async Task<GroupDto> CreateGroupAsync(GroupDto groupDto)
        {
            var creator = await _context.Users.FindAsync(groupDto.CreatedById);
            if (creator == null) throw new ArgumentException("Creator not found");

            var group = new Group
            {
                Name = groupDto.Name,
                Description = groupDto.Description,
                CreatedById = groupDto.CreatedById
            };

            _context.Groups.Add(group);
            await _context.SaveChangesAsync();

            // Add members
            var members = await _context.Users
                .Where(u => groupDto.MemberIds.Contains(u.Id))
                .ToListAsync();

            if (!members.Any(m => m.Id == groupDto.CreatedById))
            {
                members.Add(creator);
            }

            group.Members = members;
            await _context.SaveChangesAsync();

            return new GroupDto
            {
                Id = group.Id,
                Name = group.Name,
                Description = group.Description,
                CreatedById = group.CreatedById,
                CreatedByName = creator.Name,
                Members = members.Select(m => new UserDto
                {
                    Id = m.Id,
                    Username = m.Username,
                    Email = m.Email,
                    Name = m.Name
                }).ToList()
            };
        }

        public async Task<bool> DeleteGroupAsync(int id)
        {
            var group = await _context.Groups.FindAsync(id);
            if (group == null) return false;

            _context.Groups.Remove(group);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}