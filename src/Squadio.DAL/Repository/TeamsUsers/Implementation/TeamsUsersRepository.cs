﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Squadio.Domain.Enums;
using Squadio.Domain.Models.Teams;

namespace Squadio.DAL.Repository.TeamsUsers.Implementation
{
    public class TeamsUsersRepository : ITeamsUsersRepository
    {
        private readonly SquadioDbContext _context;
        public TeamsUsersRepository(SquadioDbContext context)
        {
            _context = context;
        }

        public async Task<TeamUserModel> GetTeamUser(Guid teamId, Guid userId)
        {
            var item = await _context.TeamsUsers
                .Include(x => x.Team)
                .Include(x => x.User)
                .Where(x => x.TeamId == teamId && x.UserId == userId)
                .FirstOrDefaultAsync();
            return item;
        }

        public async Task AddTeamUser(Guid teamId, Guid userId, UserStatus userStatus)
        {
            var item = new TeamUserModel
            {
                TeamId = teamId,
                UserId = userId,
                Status = userStatus,
                CreatedDate = DateTime.UtcNow
            };
            _context.TeamsUsers.Add(item);
            await _context.SaveChangesAsync();
        }

        public async Task AddRangeTeamUser(Guid teamId, IEnumerable<Guid> userIds, UserStatus userStatus)
        {
            var items = userIds.Select(userId => new TeamUserModel
                {
                    TeamId = teamId, 
                    UserId = userId, 
                    Status = userStatus, 
                    CreatedDate = DateTime.UtcNow
                })
                .ToList();

            _context.TeamsUsers.AddRange(items);
            await _context.SaveChangesAsync();
        }

        public async Task ChangeStatusTeamUser(Guid teamId, Guid userId, UserStatus newUserStatus)
        {
            var item = await _context.TeamsUsers
                .Where(x => x.TeamId == teamId && x.UserId == userId)
                .FirstOrDefaultAsync();
            item.Status = newUserStatus;
            _context.TeamsUsers.Update(item);
            await _context.SaveChangesAsync();
        }
    }
}