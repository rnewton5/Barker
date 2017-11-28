using System;

namespace Barker.Models
{
    public class BarkerPost
    {
        public int Id { get; set; }
        public string Message { get; set; }
        public string Author { get; set; }
        public DateTime PostDate { get; set; }

        public BarkerUser User { get; set; }
    }
}