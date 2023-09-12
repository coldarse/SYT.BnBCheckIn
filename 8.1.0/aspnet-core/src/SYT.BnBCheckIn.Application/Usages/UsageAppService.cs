using Abp;
using Abp.Application.Services;
using Abp.Extensions;
using Abp.Collections.Extensions;
using Abp.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SYT.BnBCheckIn.Usages.Dto;

namespace SYT.BnBCheckIn.Usages
{
    public class UsageAppService : CrudAppService<Usage, UsageDto, Guid, PagedUsageResultRequestDto>
    {

        public UsageAppService(IRepository<Usage, Guid> repository) : base(repository)
        {
        }
        protected override IQueryable<Usage> CreateFilteredQuery(PagedUsageResultRequestDto input)
        {
            return Repository.GetAllIncluding()
                .WhereIf(!input.Keyword.IsNullOrWhiteSpace(), x => 
                    x.CheckInRef.Contains(input.Keyword)).AsQueryable();
        }

        public async Task<Usage> endUsage(Guid id)
        {
            var usage = await Repository.FirstOrDefaultAsync(x => x.Id == id);
            usage.EndTime = DateTime.UtcNow;
            Repository.Update(usage);
            return usage;
        }

        public async Task<List<DayUsageWithUnits>> GetDayUsage()
        {
            var todayDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 23, 59, 59);
            var aMonthAgo = todayDate.AddDays(-30).Date;

            var usage = Repository.GetAll().Where(x => x.EndTime != DateTime.MinValue);

            usage = usage.Where(x => (x.StartTime <= todayDate && x.StartTime >= aMonthAgo));

            List<string> tempUnits = new List<string>();

            foreach(var u in usage)
            {
                if (!tempUnits.Contains(u.Unit)) tempUnits.Add(u.Unit);
            }

            List<UnitsWithInfo> unitsinfo = new List<UnitsWithInfo>();

            foreach (var u in tempUnits)
            {
                unitsinfo.Add(new UnitsWithInfo
                {
                    Unit = u,
                    Infos = new List<tempDaysUsage>(),
                });
            }

            List<DayUsageWithUnits> units = new List<DayUsageWithUnits>();

            foreach (var u in unitsinfo)
            {
                units.Add(new DayUsageWithUnits
                {
                    Unit = u.Unit,
                    Usages = new List<DayUsage>()
                });
            }

            foreach (var u in usage)
            {
                int index = unitsinfo.FindIndex(x => x.Unit == u.Unit);
                if(index != -1)
                {
                    unitsinfo[index].Infos.Add(new tempDaysUsage
                    {
                        Id = u.Id,
                        Unit = u.Unit,
                        Pico = u.Pico,
                        RFID = u.RFID,
                        Building = u.Building,
                        StartTime = u.StartTime,
                        EndTime = u.EndTime
                    });
                }
            }

            

            foreach (var v in unitsinfo)
            {
                List<tempDaysUsage> tempByDate = new List<tempDaysUsage>();

                foreach (var u in v.Infos)
                {
                    

                    TimeSpan ts = u.EndTime - u.StartTime;

                    int dateindex = tempByDate.FindIndex(x => x.StartTime.Date == u.StartTime.Date);
                    if (dateindex == -1)
                    {
                        tempByDate.Add(new tempDaysUsage
                        {
                            Id = u.Id,
                            Unit = u.Unit,
                            Pico = u.Pico,
                            RFID = u.RFID,
                            Building = u.Building,
                            StartTime = u.StartTime,
                            EndTime = u.EndTime,
                            Duration = ts.Hours,
                        });
                    }
                    else
                    {
                        tempByDate[dateindex].Duration += ts.Hours;
                    }
                }

                tempByDate = tempByDate.OrderBy(x => x.StartTime).ToList();

                int unitindex = units.FindIndex(x => x.Unit == v.Unit);
                foreach(var u in tempByDate)
                {
                    units[unitindex].Usages.Add(new DayUsage
                    {
                        name = u.StartTime.Date.ToString("MMM dd"),
                        value = u.Duration
                    });
                }
            }

            return units;
        }
    }
}
