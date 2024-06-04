using API.Data;
using API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EntryController : ControllerBase
    {
        private readonly AppDbContext appDbContext;

        public EntryController(AppDbContext appDbContext)
        {
            this.appDbContext = appDbContext;
        }

        //Create
        [HttpPost]
        public async Task<ActionResult<List<Entry>>> CreateEntry(Entry newEntry)
        {
            if (newEntry != null)
            {
                appDbContext.Entries.Add(newEntry);
                await appDbContext.SaveChangesAsync();
                return Ok(await appDbContext.Entries.ToListAsync());
            }
            //Clearer error message needed
            return BadRequest("Error");
        }

        //Read
        [HttpGet]
        public async Task<ActionResult<List<Entry>>> GetAllEntries()
        {
            var entries = await appDbContext.Entries.OrderByDescending(e => e.TimeOfEntry).ToListAsync();
            return Ok(entries);
        }

        [HttpGet("EntriesByMonth")]
        public async Task<ActionResult<List<Entry>>> GetEntriesSortByMonth()
        {
            var entries = await appDbContext.Entries.ToListAsync();

            //var januarylist = entries
            //    .Where(entry => entry.TimeOfEntry.Year == 2024 && entry.TimeOfEntry.Month == 2)
            //    .ToList();

            //return Ok(januarylist);

            var entriesByMonthList = new List<List<Entry>>();

            for (int month = 1; month <= 12; month++)
            {
                var entriesInMonth = entries.Where(entry => entry.TimeOfEntry.Month == month).ToList();

                entriesByMonthList.Add(entriesInMonth);
            }
            return Ok(entriesByMonthList);
        }

        [HttpGet("id:int")]
        public async Task<ActionResult<Entry>> GetEntryById(int id)
        {
            var entry = await appDbContext.Entries.FirstOrDefaultAsync(e => e.EntryId == id);
            if (entry != null)
            {
                return Ok(entry);
            }
            return NotFound("Entry with that ID does not exist");
        }

        [HttpGet("Average Productivity")]
        public async Task<ActionResult<double>> CalculateAverageProductivity()
        {
            var entries = await appDbContext.Entries.ToListAsync();

            var totalProductivityLevel = entries
                .Where(e => e.ProductivityLevel != null)
                .Select(e => e.ProductivityLevel)
                .Average();

            string formatToOneDecimal = totalProductivityLevel.ToString("F1");

            return Ok(formatToOneDecimal);
        }

        [HttpGet("Average Stress")]
        public async Task<ActionResult<double>> CalculateAverageStress()
        {
            var entries = await appDbContext.Entries.ToListAsync();

            var totalStressLevel = entries
                .Where(e => e.StressLevel != null)
                .Select(e => e.StressLevel)
                .Average();

            string formatToOneDecimal = totalStressLevel.ToString("F1");

            return Ok(formatToOneDecimal);
        }


        //Update
        [HttpPut]
        public async Task<ActionResult<Entry>> UpdateEntry(Entry updateEntry)
        {
            if (updateEntry != null)
            {
                var entry = await appDbContext.Entries.FirstOrDefaultAsync(e => e.EntryId == updateEntry.EntryId);
                entry!.Message = updateEntry.Message;
                entry.StressLevel = updateEntry.StressLevel;
                entry.ProductivityLevel = updateEntry.ProductivityLevel;
                await appDbContext.SaveChangesAsync();
                return Ok(entry);
            }
            return BadRequest("Entry with that ID does not exist.");
        }

        //Delete
        [HttpDelete]
        public async Task<ActionResult<List<Entry>>> DeleteEntry(int id)
        {
            var entry = await appDbContext.Entries.FirstOrDefaultAsync(e => e.EntryId == id);
            if(entry != null)
            {
                appDbContext.Entries.Remove(entry);
                await appDbContext.SaveChangesAsync();
                return Ok(await appDbContext.Entries.ToListAsync());
            }
            return NotFound();
        }
    }
}
