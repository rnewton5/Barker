using System;

namespace Barker.Models
{
    public class BarkerPost
    {
        public int Id { get; set; }
        public string Message { get; set; }
        public DateTime PostDate { get; set; }
        public BarkerUser Author { get; set; }
    }
}