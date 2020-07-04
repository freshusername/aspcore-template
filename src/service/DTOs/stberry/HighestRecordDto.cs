using System;

namespace service.DTOs.stberry
{
    public class HighestRecordDto
    {
        public int Id { get; set; }
        public DateTime RecordDate { get; set; }
        public uint AmountKg { get; set; }

        public string StberryTypeName { get; set; }
        //public string UserId { get; set; }
    }
}
