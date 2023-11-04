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
using SYT.BnBCheckIn.Units;
using SYT.BnBCheckIn.Units.Dto;
using Abp.Application.Services.Dto;
using Abp.Linq.Extensions;

namespace SYT.BnBCheckIn.Usages
{
    public class UsageAppService : CrudAppService<Usage, UsageDto, Guid, PagedUsageResultRequestDto>
    {
        private readonly IRepository<Unit, Guid> _unitRepository;

        public UsageAppService(IRepository<Usage, Guid> repository, IRepository<Unit, Guid> unitRepository) : base(repository)
        {
            _unitRepository = unitRepository;
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

            DateTime DateTimeUTC = DateTime.UtcNow;
            TimeZoneInfo cstZone = TimeZoneInfo.FindSystemTimeZoneById("Singapore Standard Time");
            DateTime cstDateTime = TimeZoneInfo.ConvertTimeFromUtc(DateTimeUTC, cstZone);

            usage.EndTime = cstDateTime;

            await Repository.UpdateAsync(usage);

            var tempUnit = await _unitRepository.FirstOrDefaultAsync(x => x.UnitNo.Equals(usage.Unit));

            if(tempUnit != null)
            {
                tempUnit.Status = "Vacant";

                await _unitRepository.UpdateAsync(tempUnit);
            }

            return usage;
        }

        private int CalculateDaysFromTwoDates(DateTime startDate, DateTime endDate)
        {
            // Check if startDate and endDate is same day
            bool sameDay = startDate.Date.Equals(endDate.Date);

            // If not same day
            if (!sameDay)
            {
                bool reachedSameDay = false;
                int day = 0;

                while (!reachedSameDay)
                {
                    DateTime tempStartDate = startDate.AddDays(day);
                    day++;
                    if (tempStartDate.Date.Equals(endDate.Date)) reachedSameDay = true;
                }

                return day;

            }
            // If same day
            else return 0;
        }


        public async Task<UsageDataTable> GetNotNestedUsage(PagedUpdatedUsageResultRequestDto input)
        {
            try
            {
                var usage = await Repository.GetAllListAsync(x => x.EndTime != DateTime.MinValue);

                usage = usage.Where(x => (x.StartTime <= input.EndTime && x.StartTime >= input.StartTime)).ToList();

                if (usage.Count() == 0) return new UsageDataTable();

                List<string> tempUnits = new List<string>();

                foreach (var u in usage)
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
                        Name = u.Unit,
                        Building = "",
                        Series = new List<DayUsage>()
                    });
                }

                foreach (var u in usage)
                {
                    int index = unitsinfo.FindIndex(x => x.Unit == u.Unit);
                    if (index != -1)
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

                        List<tempDaysUsage> internalTempByDate = new List<tempDaysUsage>();

                        //Get how many days
                        int duration_days = CalculateDaysFromTwoDates(u.StartTime, u.EndTime);

                        if (duration_days > 1)
                        {
                            for (int day = 0; day < duration_days; day++)
                            {
                                DateTime tempStartDate = u.StartTime.AddDays(day);
                                if (day != 0)
                                {
                                    tempStartDate = new DateTime(tempStartDate.Year, tempStartDate.Month, tempStartDate.Day, 00, 00, 00);
                                }

                                DateTime tempEndDate = u.EndTime;
                                if (tempStartDate.Day != tempEndDate.Day)
                                {
                                    tempEndDate = new DateTime(tempStartDate.Year, tempStartDate.Month, tempStartDate.Day, 23, 59, 59);
                                }

                                TimeSpan tempSpan = tempEndDate - tempStartDate;

                                internalTempByDate.Add(new tempDaysUsage
                                {
                                    Id = u.Id,
                                    Unit = u.Unit,
                                    Pico = u.Pico,
                                    RFID = u.RFID,
                                    Building = u.Building,
                                    StartTime = tempStartDate,
                                    EndTime = tempEndDate,
                                    Duration = tempSpan.TotalHours,
                                });
                            }
                        }
                        else
                        {
                            internalTempByDate.Add(new tempDaysUsage
                            {
                                Id = u.Id,
                                Unit = u.Unit,
                                Pico = u.Pico,
                                RFID = u.RFID,
                                Building = u.Building,
                                StartTime = u.StartTime,
                                EndTime = u.EndTime,
                                Duration = ts.TotalHours,
                            });
                        }

                        foreach (var du in internalTempByDate)
                        {
                            int dateindex = tempByDate.FindIndex(x => x.StartTime.Date.Equals(du.StartTime.Date));
                            if (dateindex == -1)
                            {
                                tempByDate.Add(du);
                            }
                            else
                            {
                                tempByDate[dateindex].Duration += du.Duration;
                                tempByDate[dateindex].EndTime = du.EndTime;
                            }
                        }
                    }

                    tempByDate = tempByDate.OrderBy(x => x.StartTime).ToList();

                    int unitindex = units.FindIndex(x => x.Name == v.Unit);
                    foreach (var u in tempByDate)
                    {
                        units[unitindex].Series.Add(new DayUsage
                        {
                            name = u.StartTime.Date.ToString("MMM dd"),
                            value = u.Duration,
                            start = u.StartTime.ToString(),
                            end = u.EndTime.ToString()
                        });

                        if (units[unitindex].Building == "")
                        {
                            units[unitindex].Building = u.Building;
                        }
                    }
                }

                double totalDuration = 0;

                List<DayUsageWithUnitsNotNested> notNested = new();
                foreach (var a in units)
                {
                    foreach (var b in a.Series)
                    {
                        totalDuration += b.value;
                        TimeSpan time = TimeSpan.FromHours(b.value);
                        notNested.Add(new DayUsageWithUnitsNotNested()
                        {
                            Unit = a.Name,
                            Building = a.Building,
                            StartTime = b.start,
                            EndTime = b.end,
                            Duration = time.ToString("hh':'mm':'ss"),
                        });
                    }
                }

                notNested = notNested.WhereIf(input.Unit != null, x => x.Unit.ToLower().Contains(input.Unit.ToLower())).ToList();

                notNested = notNested.OrderBy(d => d.StartTime).ToList();

                int totalCount = notNested.Count();

                var un_nested = new PagedResultDto<DayUsageWithUnitsNotNested>(
                    totalCount,
                    notNested
                );

                TimeSpan totalDuration_TS = TimeSpan.FromHours(totalDuration);

                return new UsageDataTable()
                {
                    notNested = un_nested,
                    Duration = totalDuration_TS.ToString("dd':'hh':'mm':'ss")
                };

            }
            catch
            {
                return new UsageDataTable();
            }
        }

        public async Task<DayUsageWithAndWithoutNested> GetDayUsage(int days)
        {
            try
            {
                DateTime DateTimeUTC = DateTime.UtcNow;
                TimeZoneInfo cstZone = TimeZoneInfo.FindSystemTimeZoneById("Singapore Standard Time");
                DateTime cstDateTime = TimeZoneInfo.ConvertTimeFromUtc(DateTimeUTC, cstZone);

                var todayDate = new DateTime(cstDateTime.Year, cstDateTime.Month, cstDateTime.Day, 23, 59, 59);
                var aMonthAgo = todayDate.AddDays(-days).Date;

                var usage = await Repository.GetAllListAsync(x => x.EndTime != DateTime.MinValue);

                usage = usage.Where(x => (x.StartTime <= todayDate && x.StartTime >= aMonthAgo)).ToList();

                if (usage.Count() == 0) return new DayUsageWithAndWithoutNested();

                List<string> tempUnits = new List<string>();

                foreach (var u in usage)
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
                        Name = u.Unit,
                        Building = "",
                        Series = new List<DayUsage>()
                    });
                }

                foreach (var u in usage)
                {
                    int index = unitsinfo.FindIndex(x => x.Unit == u.Unit);
                    if (index != -1)
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

                        List<tempDaysUsage> internalTempByDate = new List<tempDaysUsage>();

                        //Get how many days
                        int duration_days = CalculateDaysFromTwoDates(u.StartTime, u.EndTime);

                        if (duration_days > 1)
                        {
                            for (int day = 0; day < duration_days; day++)
                            {
                                DateTime tempStartDate = u.StartTime.AddDays(day);
                                if (day != 0)
                                {
                                    tempStartDate = new DateTime(tempStartDate.Year, tempStartDate.Month, tempStartDate.Day, 00, 00, 00);
                                }

                                DateTime tempEndDate = u.EndTime;
                                if (tempStartDate.Day != tempEndDate.Day)
                                {
                                    tempEndDate = new DateTime(tempStartDate.Year, tempStartDate.Month, tempStartDate.Day, 23, 59, 59);
                                }

                                TimeSpan tempSpan = tempEndDate - tempStartDate;

                                internalTempByDate.Add(new tempDaysUsage
                                {
                                    Id = u.Id,
                                    Unit = u.Unit,
                                    Pico = u.Pico,
                                    RFID = u.RFID,
                                    Building = u.Building,
                                    StartTime = tempStartDate,
                                    EndTime = tempEndDate,
                                    Duration = tempSpan.TotalHours,
                                });
                            }
                        }
                        else
                        {
                            internalTempByDate.Add(new tempDaysUsage
                            {
                                Id = u.Id,
                                Unit = u.Unit,
                                Pico = u.Pico,
                                RFID = u.RFID,
                                Building = u.Building,
                                StartTime = u.StartTime,
                                EndTime = u.EndTime,
                                Duration = ts.TotalHours,
                            });
                        }

                        foreach (var du in internalTempByDate)
                        {
                            int dateindex = tempByDate.FindIndex(x => x.StartTime.Date.Equals(du.StartTime.Date));
                            if (dateindex == -1)
                            {
                                tempByDate.Add(du);
                            }
                            else
                            {
                                tempByDate[dateindex].Duration += du.Duration;
                                tempByDate[dateindex].EndTime = du.EndTime;
                            }
                        }
                    }

                    tempByDate = tempByDate.OrderBy(x => x.StartTime).ToList();

                    int unitindex = units.FindIndex(x => x.Name == v.Unit);
                    foreach (var u in tempByDate)
                    {
                        units[unitindex].Series.Add(new DayUsage
                        {
                            name = u.StartTime.Date.ToString("MMM dd"),
                            value = u.Duration,
                            start = u.StartTime.ToString(),
                            end = u.EndTime.ToString()
                        });

                        if (units[unitindex].Building == "")
                        {
                            units[unitindex].Building = u.Building;
                        }
                    }
                }

                List<DayUsageWithUnitsNotNested> notNested = new();
                foreach (var a in units)
                {
                    foreach (var b in a.Series)
                    {
                        TimeSpan time = TimeSpan.FromHours(b.value);
                        notNested.Add(new DayUsageWithUnitsNotNested()
                        {
                            Unit = a.Name,
                            Building = a.Building,
                            StartTime = b.start,
                            EndTime = b.end,
                            Duration = time.ToString("hh':'mm':'ss"),
                        });
                    }
                }

                notNested = notNested.OrderBy(d => d.StartTime).ToList();

                return new DayUsageWithAndWithoutNested()
                {
                    nested = units,
                    notNested = notNested,
                };
            }
            catch(Exception ex)
            {
                return new DayUsageWithAndWithoutNested();
            }
            
        }
    }
}
