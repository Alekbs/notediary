namespace ServerApp.Models
{
    public class Subject
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public ICollection<TeacherData>? Teachers { get; set; } = new List<TeacherData>();
    }
}
