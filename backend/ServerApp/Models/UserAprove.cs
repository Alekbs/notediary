namespace ServerApp.Models
{
    public class UserAprrove    {
        public int Id { get; set; }
        public bool IsApproved { get; set; }

        // Внешний ключ для связи с таблицей User
        public int UserId { get; set; }
        public User User { get; set; } // Навигационное свойство
    }
}