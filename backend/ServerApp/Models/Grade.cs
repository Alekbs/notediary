namespace ServerApp.Models
{
    public class Grade
    {
        public int Id { get; set; }

        public int StudentId { get; set; }
        public User Student { get; set; } = null!;

        public int TeacherId { get; set; }
        public User Teacher { get; set; } = null!;

        public int SubjectId { get; set; }
        public Subject Subject { get; set; } = null!;

        public int GradeValue { get; set; } // 2, 3, 4, 5
        public DateTime Date { get; set; }
    }
}
