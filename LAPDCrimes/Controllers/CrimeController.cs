using LAPDCrimes.Models;
using LAPDCrimes.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;

namespace LAPDCrimes.Controllers
{
    public enum QueryType
    {
        Query01 = 1,
        Query02 = 2,
        Query03 = 3,
        Query04 = 4,
        Query05 = 5,
        Query06 = 6,
        Query07 = 7,
        Query08 = 8,
        Query09 = 9,
        Query10 = 10,
        Query11 = 11,
        Query12 = 12,


    }
    public static class QueryDefinitions
    {
        // For simplicity, we’ll represent required fields as a list of (Name, Type).
        // In a real scenario, you might define custom classes or attributes.
        public static Dictionary<QueryType, (string DisplayName, (string FieldName, string FieldType)[] Parameters)> Queries
            = new()
        {
            { QueryType.Query01, ("Query01", new [] {("TimeFrom", "number"), ("TimeTo","number")} ) },
            { QueryType.Query02, ("Query02", new [] {("CrimeCode","text"),("StartDate", "date"), ("EndDate", "date")} ) },
            { QueryType.Query03, ("Query03", new [] {("TheDate", "date")} ) },
            { QueryType.Query04, ("Query04", new [] {("StartDate", "date"), ("EndDate", "date")} )},
            { QueryType.Query05, ("Query05", new [] {("MinLon", "text"), ("MinLat", "text"), ("MaxLon", "text"), ("MaxLat", "text"), ("TheDate", "date") } )},

        };
    }

    [Authorize("AuthenticatedOnly")]
    public class CrimeController : Controller
    {
        private QueryService _queryService;
        public CrimeController( QueryService queryService)
        {
            _queryService = queryService;
        }

        public IActionResult Index()
        {
            var availableQueries = QueryDefinitions.Queries.Select(q => new QueryViewModel
            {
                Value = (int)q.Key,
                Text = q.Value.DisplayName
            }).ToList();

            return View(availableQueries);
        }


        [HttpGet("Crime/GetQueryForm")]
        public IActionResult GetQueryForm(int queryType)
        {
            Console.WriteLine("It is invoked");

            if (!QueryDefinitions.Queries.TryGetValue((QueryType)queryType, out var queryInfo))
                return BadRequest("Invalid query type.");

            var parameters = queryInfo.Parameters
            .Select(p => new QueryParameterDefinition
            {
                FieldName = p.Item1, // Replace with the correct property or tuple field
                FieldType = p.Item2  // Replace with the correct property or tuple field
            })
            .ToList();

            // We return a partial view that loops over the parameters and creates form fields
            return PartialView("_QueryForm", parameters);
        }

        [HttpPost("Crime/ExecuteQuery")]
        public async Task<IActionResult> ExecuteQuery(int queryType, [FromForm] Dictionary<string, string> parameters)
        {
            Console.WriteLine($"Execute is invoked");
            if (!QueryDefinitions.Queries.ContainsKey((QueryType)queryType))
                return BadRequest("Invalid query type.");

            object result = null;
            string partialViewName = string.Empty;

            switch ((QueryType)queryType)
            {
                case QueryType.Query01:
                    if (int.TryParse(parameters["TimeFrom"], out var TimeFrom) && int.TryParse(parameters["TimeTo"], out var TimeTo))
                    {
                        result = await _queryService.GetQuery01Async(TimeFrom,TimeTo);
                        partialViewName = "_QueryResults";
                    }
                    break;
                // Add cases for the remaining queries
                case QueryType.Query02:
                    if (parameters.ContainsKey("CrimeCode") && DateTime.TryParse(parameters["StartDate"], out var DateFrom) && DateTime.TryParse(parameters["EndDate"], out var DateTo))
                    {
                        
                        result = await _queryService.GetQuery02Async(parameters["CrimeCode"],DateFrom, DateTo);
                        partialViewName = "_QueryResults02";
                    }
                    break;
                case QueryType.Query03:
                    if ( DateTime.TryParse(parameters["TheDate"], out var TheDate))
                    {

                        result = await _queryService.GetQuery03Async(TheDate);
                        partialViewName = "_QueryResults03";
                    }
                    break;
                case QueryType.Query04:
                    if (DateTime.TryParse(parameters["StartDate"], out var DateFromQ4) && DateTime.TryParse(parameters["EndDate"], out var DateToQ4))
                    {

                        result = await _queryService.GetQuery04Async(DateFromQ4, DateToQ4);
                        partialViewName = "_QueryResults04";
                    }
                    break;
                case QueryType.Query05:
                    if (double.TryParse(parameters["MinLon"], CultureInfo.InvariantCulture, out var MinLon) && double.TryParse(parameters["MinLat"], CultureInfo.InvariantCulture, out var MinLat) &&
                        double.TryParse(parameters["MaxLon"], CultureInfo.InvariantCulture, out var MaxLon) && double.TryParse(parameters["MaxLat"], CultureInfo.InvariantCulture, out var MaxLat) && DateTime.TryParse(parameters["TheDate"], out var TheDateQ5))
                    {

                        result = await _queryService.GetQuery05Async(MinLon, MinLat, MaxLon, MaxLat, TheDateQ5);
                        partialViewName = "_QueryResults05";
                    }
                    break;
                default:
                    return BadRequest("Query not implemented.");
            }

            if (result == null)
            {
                return BadRequest("Invalid parameters or query execution failed.");
            }

            return PartialView(partialViewName, result);
        }

    }
}
