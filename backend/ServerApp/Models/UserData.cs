namespace ServerApp.Models
{
    public class UserData    {
        public int Id { get; set; }
        public string ChildName { get; set; }
        public string ChildAddress { get; set; }
        public string LearningWorkplace { get; set; }
        public string ChildContact { get; set; }

        public string MotherName { get; set; }
        public string MotherPassport { get; set; }
        public string MotherAddress { get; set; }
        public string MotherWorkplace { get; set; }
        public string MotherContact { get; set; }

        public string FatherName { get; set; }
        public string FatherPassport { get; set; }
        public string FatherAddress { get; set; }
        public string FatherWorkplace { get; set; }
        public string FatherContact { get; set; }

        // Внешний ключ для связи с таблицей User
        public int UserId { get; set; }
        public User User { get; set; } // Навигационное свойство
    }
}