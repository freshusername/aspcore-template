using System;

namespace service.DTOs.stberry
{
    public class RecordDto
    {
        public int Id { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTime RecordDate { get; set; }
        public uint AmountKg { get; set; }

        public string StberryTypeName { get; set; }
    }
}
