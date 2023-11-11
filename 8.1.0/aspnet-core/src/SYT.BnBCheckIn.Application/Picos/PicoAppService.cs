using Abp;
using Abp.Application.Services;
using Abp.Extensions;
using Abp.Collections.Extensions;
using Abp.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SYT.BnBCheckIn.Picos.Dto;
using SYT.BnBCheckIn.Units;
using SYT.BnBCheckIn.RFIDS;
using Abp.Application.Services.Dto;
using System.Drawing;

namespace SYT.BnBCheckIn.Picos
{
    public class PicoAppService : CrudAppService<Pico, PicoDto, Guid, PagedPicoResultRequestDto>
    {
        private readonly IRepository<Unit, Guid> _unitRepository;

        public PicoAppService(IRepository<Pico, Guid> repository, IRepository<Unit, Guid> unitRepository) : base(repository)
        {
            _unitRepository = unitRepository;
        }
        protected override IQueryable<Pico>CreateFilteredQuery(PagedPicoResultRequestDto input)
        {
            var picos = Repository.GetAll();
            var units = _unitRepository.GetAll();

            var joinedQuery = from pico in picos
                              join unit in units
                              on pico.UnitId equals unit.Id
                              select new
                              {
                                  pico.Id,
                                  pico.Name,
                                  pico.UnitId,
                                  unit.UnitNo
                              };

            var orderedQuery = joinedQuery.OrderBy(x => x.UnitNo);

            if (!input.Keyword.IsNullOrWhiteSpace())
            {
                var filteredQuery = joinedQuery.Where(x =>
                    x.Name.ToLower().Contains(input.Keyword.ToLower()) ||
                    x.UnitNo.ToLower().Contains(input.Keyword.ToLower())
                    );

                orderedQuery = filteredQuery.OrderBy(x => x.UnitNo);
            }

            var mapQuery = orderedQuery.Select(a => new Pico()
            {
                Id = a.Id,
                Name = a.Name,
                UnitId = a.UnitId
            });

            return mapQuery.AsQueryable();
        }

        public override PagedResultDto<PicoDto> GetAll(PagedPicoResultRequestDto input)
        {
            CheckGetAllPermission();

            var query = CreateFilteredQuery(input);

            var totalCount = query.Count();

            var units = _unitRepository.GetAll();

            var joinedQuery = from pico in query
                              join unit in units
                              on pico.UnitId equals unit.Id
                              select new
                              {
                                  pico.Id,
                                  pico.Name,
                                  pico.UnitId,
                                  unit.UnitNo
                              };

            joinedQuery = joinedQuery.OrderBy(x => x.UnitNo);

            var mapQuery = joinedQuery.Select(a => new Pico()
            {
                Id = a.Id,
                Name = a.Name,
                UnitId = a.UnitId
            }).ToList();

            return new PagedResultDto<PicoDto>(
                totalCount,
                mapQuery.Select(MapToEntityDto).ToList()
                );
        }

        public async Task<PicoDto> CreatePico(PicoDto input)
        {
            var temppico = await Repository.FirstOrDefaultAsync(x => x.Name.ToLower().Equals(input.Name.ToLower()));

            if (temppico is not null) return new PicoDto() { Name = "ERR501" };

            var createpico = await Repository.InsertAsync(MapToEntity(input));

            return MapToEntityDto(createpico);
        }

        public async Task<PicoDto> UpdatePico(PicoDto input)
        {
            try
            {
                var temppico = await Repository.FirstOrDefaultAsync(x => x.Name.ToLower().Equals(input.Name.ToLower()));

                if (temppico is not null)
                {
                    if (!temppico.Name.ToLower().Equals(input.Name.ToLower())) return new PicoDto() { Name = "ERR501" };
                };

                var updatepico = await Repository.UpdateAsync(MapToEntity(input));

                return MapToEntityDto(updatepico);
            }
            catch(Exception ex)
            {
                Console.Write("");
                return new PicoDto();
            }
            
        }

        public async Task<Pico> getPicoByName(string pico)
        {
            return await Repository.FirstOrDefaultAsync(x => x.Name == pico);
        }

        public async Task<Pico> getPicoByID(Guid pico)
        {
            return await Repository.FirstOrDefaultAsync(x => x.Id == pico);
        }

        public async Task<List<Guid>> GetPicoUnits()
        {
            var picos = await Repository.GetAllListAsync();

            List<Guid> units = new List<Guid>();

            foreach(var pico in picos)
            {
                units.Add(pico.UnitId);
            }

            return units;
        }
    }
}
