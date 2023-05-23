using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace ptmkcs
{
    public class Peoples
    {
        [Key]
        public int id { get; set; }
        [Display(Name = "ФИО")]
        public string? Name { get; set; }
        [Display(Name = "Дата рождения")]
        public string? DoB { get; set; }
        [Display(Name = "Пол")]
        public string? Sex { get; set; }
        public PeopleFull GetFull()
        {
            return new PeopleFull(this);
        }
        public string print()
        {
            return $"{Name} | {DoB} | {Sex}";
        }
    }
    public class PeopleFull : Peoples
    {
        public PeopleFull(Peoples people) {
            id = people.id;
            Name = people.Name;
            DoB= people.DoB;
            Sex= people.Sex;
        }
        public string? Years
        {
            get
            {
                if (DoB == null) return "0";
                var year = int.Parse(DoB.Replace(',', '.').Split('.')[^1]);
                return (DateTime.Now.Year - year).ToString();
            }
        }
        public new string print()
        {
            return $"{Name} | {DoB} | {Sex} | {Years}";
        }
    }
    public class DataBaseContext : DbContext
    {
        public DbSet<Peoples> Peoples { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Filename=db.db");
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Peoples>().ToTable("Peoples");
        }
    }
    public class DataBase
    {
        public DataBaseContext db = new();
        public void Create()
        {
            db.Database.EnsureCreated();
        }
        public Peoples AddPeoples(string name, string DoB, string sex)
        {
            var people = new Peoples() { Name = name, DoB = DoB, Sex = sex };
            db.Peoples.Add(people);
            db.SaveChanges();
            return people;
        }
        public void AddPeopleAsync(string name, string DoB, string sex)
        {
            var people = new Peoples() { Name = name, DoB = DoB, Sex = sex };
            db.Peoples.AddAsync(people);
        }
        public List<PeopleFull> GetUnique()
        {
            var peoples = db.Peoples.OrderBy(p => p.Name).GroupBy(p => p.Name + p.DoB).ToList().ConvertAll(p => p.FirstOrDefault());
            return peoples.ConvertAll(p => p.GetFull());
        }
        public void Generate()
        {
            var abc = "A B C D E F G H I J K L M N O P Q R S T U V W X Y Z".Split(" ");
            Random rnd = new();
            for (int i = 0; i < 1000001; i++)
            {
                var f = rnd.Next(0, 25);
                var m = rnd.Next(0, 25);
                var l = rnd.Next(0, 25);
                var name = $"{abc[f]} {abc[m]} {abc[l]}";
                var year = $"01.01.{rnd.Next(1900, 2022)}";
                var sex = rnd.Next(0, 1) == 1 ? "М" : "Ж";
                AddPeopleAsync(name, year, sex);
            }
            for (int i = 0; i < 101; i++)
            {
                var m = rnd.Next(0, 25);
                var l = rnd.Next(0, 25);
                var name = $"F {abc[m]} {abc[l]}";
                var year = $"01.01.{rnd.Next(1900, 2022)}";
                var sex =  "М";
                AddPeopleAsync(name, year, sex);
            }
            db.SaveChanges();
        }
        public double GetPeoples()
        {
            var date = DateTime.Now;
            var qwe = db.Peoples.Where(p => EF.Functions.Like(p.Name, "F%")).ToList();
            return (DateTime.Now - date).TotalMilliseconds;
        }
        public string Acceleration()
        {
            var last = GetPeoples();
            //Режим OFF (или 0) означает: SQLite считает, что данные фиксированы на диске сразу после того как он передал их ОС (то есть сразу после вызова соот-го API ОС).
            db.Database.ExecuteSql($"PRAGMA synchronous = OFF"); 
            var news = GetPeoples();
            return $"was {last} ms, became {news} ms";
        }
    }
}
