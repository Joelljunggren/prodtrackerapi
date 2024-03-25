using System.ComponentModel.DataAnnotations;

namespace API.Models
{
    public class Entry
    {
        public int EntryId { get; set; }

        [Range(0, 7, ErrorMessage = "Chose between 0 and 7.")]
        public int ProductivityLevel { get; set; }

        [Range(0, 7, ErrorMessage = "Chose between 0 and 7.")]
        public int StressLevel { get; set; }
        public string? Message { get; set; }
        public DateTime TimeOfEntry { get; set; }
    }
}
