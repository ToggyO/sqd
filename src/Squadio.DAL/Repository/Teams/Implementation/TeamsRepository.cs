﻿using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Squadio.Common.Models.Responses;
using Squadio.Domain.Models.Teams;
using Squadio.DTO.Pages;

namespace Squadio.DAL.Repository.Teams.Implementation
{
    public class TeamsRepository : ITeamsRepository
    {
        private readonly SquadioDbContext _context;
        public TeamsRepository(SquadioDbContext context)
        {
            _context = context;
        }

        public async Task<TeamModel> GetById(Guid id)
        {
            var item = await _context.Teams.FindAsync(id);
            return item;
        }

        public async Task<TeamModel> Create(TeamModel entity)
        {
            var item = _context.Teams.Add(entity);
            await _context.SaveChangesAsync();
            return item.Entity;
        }

        public Task<TeamModel> Delete(Guid id)
        {
            throw new NotImplementedException();
        }

        public async Task<TeamModel> Update(TeamModel entity)
        {
            var item = await _context.Teams.FindAsync(entity.Id);
            item.Name = entity.Name;
            _context.Update(item);
            await _context.SaveChangesAsync();
            return item;
        }

        public async Task<PageModel<TeamModel>> GetTeams(PageModel model)
        {
            var total = await _context.Teams.CountAsync();
            var items = await _context.Teams
                .Skip((model.Page - 1) * model.PageSize)
                .Take(model.PageSize)
                .ToListAsync();
            var result = new PageModel<TeamModel>
            {
                Page = model.Page,
                PageSize = model.PageSize,
                Total = total,
                Items = items
            };
            return result;
        }
    }
}