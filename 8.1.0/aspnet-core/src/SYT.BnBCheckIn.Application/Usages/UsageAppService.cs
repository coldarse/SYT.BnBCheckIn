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

       
        //private async Task<PagedResultDto<DayUsageWithUnitsNotNested>> GetUpdatedAll(PagedUpdatedUsageResultRequestDto input)
        //{
        //    try
        //    {
        //        var tempUsages = await GetNotNestedUsage(new DateRange()
        //        {
        //            startDate = input.StartTime,
        //            endDate = input.EndTime
        //        });

        //        tempUsages.notNested = tempUsages.notNested.WhereIf(input.Unit != null, x => x.Unit.ToLower().Contains(input.Unit.ToLower())).ToList();

        //        var totalCount = tempUsages.notNested.Count();

        //        return new PagedResultDto<DayUsageWithUnitsNotNested>(
        //            totalCount,
        //            tempUsages.notNested
        //        );
        //    }
        //    catch(Exception ex)
        //    {
        //        Console.WriteLine(ex.Message);
        //        return null;
        //    }
            
        //}

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

                        int dateindex = tempByDate.FindIndex(x => x.StartTime.Date == u.StartTime.Date);
                        if (dateindex == -1)
                        {
                            //Check if duration is more than 1 day
                            if (ts.TotalSeconds > 86400)
                            {
                                //Get how many days
                                double duration_days = ts.TotalSeconds / 86400;

                                for (int day = 0; day <= duration_days; day++)
                                {
                                    DateTime tempStartDate = u.StartTime.AddDays(day);
                                    if (day != 0)
                                    {
                                        tempStartDate = new DateTime(tempStartDate.Date.Year, tempStartDate.Date.Month, tempStartDate.Date.Day, 00, 00, 00);
                                    }
                                    DateTime tempEndDate = u.EndTime;
                                    if (tempStartDate.Day != u.EndTime.Day)
                                    {
                                        tempEndDate = new DateTime(tempStartDate.Date.Year, tempStartDate.Date.Month, tempStartDate.Date.Day, 23, 59, 59);
                                    }

                                    TimeSpan tempSpan = tempEndDate - tempStartDate;
                                    double tempDuration = tempSpan.TotalSeconds;

                                    tempByDate.Add(new tempDaysUsage
                                    {
                                        Id = u.Id,
                                        Unit = u.Unit,
                                        Pico = u.Pico,
                                        RFID = u.RFID,
                                        Building = u.Building,
                                        StartTime = tempStartDate,
                                        EndTime = tempEndDate,
                                        Duration = tempDuration,
                                    });
                                }
                            }
                            else
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
                                    Duration = ts.TotalSeconds,
                                });
                            }
                        }
                        else
                        {
                            tempByDate[dateindex].Duration += ts.TotalSeconds;
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
                        TimeSpan time = TimeSpan.FromSeconds(b.value);
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

                int totalCount = notNested.Count();

                var un_nested = new PagedResultDto<DayUsageWithUnitsNotNested>(
                    totalCount,
                    notNested
                );

                TimeSpan totalDuration_TS = TimeSpan.FromSeconds(totalDuration);

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
                var todayDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 23, 59, 59);
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

                        int dateindex = tempByDate.FindIndex(x => x.StartTime.Date == u.StartTime.Date);
                        if (dateindex == -1)
                        {
                            //Check if duration is more than 1 day
                            if(ts.TotalSeconds > 86400)
                            {
                                //Get how many days
                                double duration_days = ts.TotalSeconds / 86400;

                                for (int day = 0; day <= duration_days; day++)
                                {
                                    DateTime tempStartDate = u.StartTime.AddDays(day);
                                    if (day != 0)
                                    {
                                        tempStartDate = new DateTime(tempStartDate.Date.Year, tempStartDate.Date.Month, tempStartDate.Date.Day, 00, 00, 00);
                                    }
                                    DateTime tempEndDate = u.EndTime;
                                    if (tempStartDate.Day != u.EndTime.Day)
                                    {
                                        tempEndDate = new DateTime(tempStartDate.Date.Year, tempStartDate.Date.Month, tempStartDate.Date.Day, 23, 59, 59);
                                    }

                                    TimeSpan tempSpan = tempEndDate - tempStartDate;
                                    double tempDuration = tempSpan.TotalSeconds;

                                    tempByDate.Add(new tempDaysUsage
                                    {
                                        Id = u.Id,
                                        Unit = u.Unit,
                                        Pico = u.Pico,
                                        RFID = u.RFID,
                                        Building = u.Building,
                                        StartTime = tempStartDate,
                                        EndTime = tempEndDate,
                                        Duration = tempDuration,
                                    });
                                }
                            }
                            else
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
                                    Duration = ts.TotalSeconds,
                                });
                            }
                        }
                        else
                        {
                            tempByDate[dateindex].Duration += ts.TotalSeconds;
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
                        TimeSpan time = TimeSpan.FromSeconds(b.value);
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
