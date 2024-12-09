using LAPDCrimes.Data;
using LAPDCrimes.Models.QueryModels;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace LAPDCrimes.Services
{
    public class QueryService
    {
        private readonly LACrimesDbContext _context;

        public QueryService(LACrimesDbContext context)
        {
            _context = context;
        }

        public async Task<List<Query01>> GetQuery01Async(int TimeFrom,int TimeTo)
        {
            var sql = @"
                    SELECT crime_code, COUNT(*) as counter
                    FROM ""CrimesSchema"".crimes_commited as jun
                    INNER JOIN ""CrimesSchema"".crimes as main
                    ON jun.drno = main.drno
                    WHERE CAST(main.time_occ AS INT) > @TimeFrom and CAST(main.time_occ AS INT) < @TimeTo
                    GROUP BY crime_code
                    ORDER BY counter DESC;";

            var results = new List<Query01>();

            // Execute raw SQL and use a data reader to map results manually
            using (var command = _context.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = sql;
                command.CommandType = System.Data.CommandType.Text;

                var parameter = command.CreateParameter();
                parameter.ParameterName = "@TimeFrom";
                parameter.Value = TimeFrom;
                command.Parameters.Add(parameter);
                var parameter2 = command.CreateParameter();
                parameter2.ParameterName = "@TimeTo";
                parameter2.Value = TimeTo;
                command.Parameters.Add(parameter2);

                await _context.Database.OpenConnectionAsync();
                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        results.Add(new Query01
                        {
                            CrimeCode = reader["crime_code"].ToString(),
                            Counter = (long)reader["counter"]
                        });
                    }
                }
            }
            return results;
        }

        public async Task<List<Query02>> GetQuery02Async(string CrimeCode, DateTime DateFrom, DateTime DateTo)
        {
            var sql = @"
                    SELECT crime_code,date_occ, COUNT(crime_code) as counter
                    FROM ""CrimesSchema"".crimes_commited as cr_c
                    INNER JOIN ""CrimesSchema"".crimes as cr
                    ON cr.drno = cr_c.drno
                    WHERE cr_c.crime_code = @CrimeCode AND date_occ >= @DateFrom AND date_occ <= @DateTo
                    GROUP BY crime_code,date_occ";

            var results = new List<Query02>();

            // Execute raw SQL and use a data reader to map results manually
            using (var command = _context.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = sql;
                command.CommandType = System.Data.CommandType.Text;

                var parameter = command.CreateParameter();
                parameter.ParameterName = "@CrimeCode";
                parameter.Value = CrimeCode;
                command.Parameters.Add(parameter);

                parameter = command.CreateParameter();
                parameter.ParameterName = "@DateFrom";
                parameter.Value = DateFrom;
                command.Parameters.Add(parameter);
                var parameter2 = command.CreateParameter();
                parameter2.ParameterName = "@DateTo";
                parameter2.Value = DateTo;
                command.Parameters.Add(parameter2);

                await _context.Database.OpenConnectionAsync();
                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        results.Add(new Query02
                        {
                            CrimeCode = reader["crime_code"].ToString(),
                            DateOcc = reader["date_occ"].ToString(),
                            Counter = (long)reader["counter"],
                        });
                    }
                }
            }
            return results;
        }

        public async Task<List<Query03>> GetQuery03Async(DateTime TheDate)
        {
            var sql = @"
                        WITH x AS (
	                        SELECT area_code,crime_code,count(crime_code) as counter
	                        FROM ""CrimesSchema"".crimes_commited as cr_c
	                        INNER JOIN ""CrimesSchema"".crimes as cr
	                        ON cr.drno = cr_c.drno
	                        WHERE cr.date_occ = @TheDate
	                        GROUP BY area_code,crime_code
                        ),
                         maximum_table AS 
                        (SELECT  area_code, MAX(x.counter) as maximum
                        FROM x
                        GROUP BY area_code )

                        SELECT maximum_table.area_code, x.crime_code, maximum
                        FROM x 
                        INNER JOIN maximum_table
                        ON maximum = x.counter AND maximum_table.area_code = x.area_code";

            var results = new List<Query03>();

            // Execute raw SQL and use a data reader to map results manually
            using (var command = _context.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = sql;
                command.CommandType = System.Data.CommandType.Text;

                var parameter = command.CreateParameter();
                parameter.ParameterName = "@TheDate";
                parameter.Value = TheDate;
                command.Parameters.Add(parameter);

                await _context.Database.OpenConnectionAsync();
                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        results.Add(new Query03
                        {
                            CrimeCode = reader["crime_code"].ToString(),
                            AreaCode = reader["area_code"].ToString(),
                            Maximum = (long)reader["maximum"],
                        });
                    }
                }
            }
            return results;
        }

        public async Task<List<Query04>> GetQuery04Async(DateTime DateFrom,DateTime DateTo)
        {
            var sql = @"
                SELECT COUNT(cr_c.crime_code)/((@DateTo::DATE - @DateFrom::DATE)+1)  as average_per_hour
                FROM ""CrimesSchema"".crimes_commited as cr_c
                INNER JOIN ""CrimesSchema"".crimes as cr
                ON cr.drno = cr_c.drno
                WHERE  date_occ >= @DateFrom AND date_occ <= @DateTo
                GROUP BY LEFT(cr.time_occ, 2)";

            var results = new List<Query04>();

            // Execute raw SQL and use a data reader to map results manually
            using (var command = _context.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = sql;
                command.CommandType = System.Data.CommandType.Text;

                var parameter = command.CreateParameter();
                parameter.ParameterName = "@DateFrom";
                parameter.Value = DateFrom;
                command.Parameters.Add(parameter);
                parameter = command.CreateParameter();
                parameter.ParameterName = "@DateTo";
                parameter.Value = DateTo;
                command.Parameters.Add(parameter);

                await _context.Database.OpenConnectionAsync();
                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        results.Add(new Query04
                        {
                            AvgCrimes = (long)reader["average_per_hour"],
                        });
                    }
                }
            }
            return results;
        }

        public async Task<List<Query05>> GetQuery05Async(double minLon, double minLat, double maxLon, double maxLat, DateTime TheDate)
        {
            var sql = @"
                SELECT cr_c.crime_code as crime_code, COUNT(*) AS crime_count
                FROM ""CrimesSchema"".crimes AS cr
                INNER JOIN ""CrimesSchema"".crimes_commited AS cr_c
                ON cr.drno = cr_c.drno
                INNER JOIN ""CrimesSchema"".location AS loc 
                ON cr.loc_id = loc.loc_id
                WHERE loc.loc_point && ST_MakeEnvelope(@minLon, @minLat, @maxLon, @maxLat, 4326) AND cr.date_occ = @TheDate
                GROUP BY cr_c.crime_code
                ORDER BY crime_count DESC";

            var results = new List<Query05>();

            // Execute raw SQL and use a data reader to map results manually
            using (var command = _context.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = sql;
                command.CommandType = System.Data.CommandType.Text;

                var parameter = command.CreateParameter();
                parameter.ParameterName = "@minLon";
                parameter.Value = minLon;
                command.Parameters.Add(parameter);
                parameter = command.CreateParameter();
                parameter.ParameterName = "@minLat";
                parameter.Value = minLat;
                command.Parameters.Add(parameter);
                parameter = command.CreateParameter();
                parameter.ParameterName = "@maxLon";
                parameter.Value = maxLon;
                command.Parameters.Add(parameter);
                parameter = command.CreateParameter();
                parameter.ParameterName = "@maxLat";
                parameter.Value = maxLat;
                command.Parameters.Add(parameter);
                parameter = command.CreateParameter();
                parameter.ParameterName = "@TheDate";
                parameter.Value = TheDate;
                command.Parameters.Add(parameter);

                await _context.Database.OpenConnectionAsync();
                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        results.Add(new Query05
                        {
                            CrimeCode = reader["crime_code"].ToString(),
                            CrimeCount = (long)reader["crime_count"],

                        });
                    }
                }
            }
            return results;
        }

    }
}
