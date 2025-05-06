namespace SchoolManager.Dtos
{
    public class StudentReportDto
    {
        public Guid StudentId { get; set; }
        public string StudentName { get; set; }
        public string Grade { get; set; }
        public string Trimester { get; set; }
        public List<GradeDto> Grades { get; set; } = new();
        public List<AttendanceDto> AttendanceByTrimester { get; set; } = new();
        public List<AttendanceDto> AttendanceByMonth { get; set; } = new();
        public List<AvailableTrimesters> AvailableTrimesters { get; set; } = new();
    }


    public class AvailableTrimesters
    {
        public string Trimester { get; set; }   
    }

}
