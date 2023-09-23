using Microsoft.EntityFrameworkCore;
using CreekRiver.Models;

public class CreekRiverDbContext : DbContext
{

    public DbSet<Reservation> Reservations { get; set; }
    public DbSet<UserProfile> UserProfiles { get; set; }
    public DbSet<Campsite> Campsites { get; set; }
    public DbSet<CampsiteType> CampsiteTypes { get; set; }

    public CreekRiverDbContext(DbContextOptions<CreekRiverDbContext> context) : base(context)
    {
        
    }



// "This (below) is the first use in the course of the access modifier protected. Up until now, you have made all of your classes and properties public, meaning that they are accessible anywhere in the code that you can reference the class or a property of the class. protected, in contrast, means that the method can only be called from code inside the class itself, or by any class that inherits it. This is a form of encapsulation, which means keeping code that is only safe or useful to use inside a particular context inaccessible to other parts of the program. We will see why this is important later.
    //override indicates that OnModelCreating is actually replacing a method of the same name that is inherited from the DbContext class. Such methods, like the one in the DbContext class, are marked with the virtual keyword. See the extra chapters for a deeper dive on override. 
    //The rest of the code in this method will check - every time we create or update the database schema - whether this data is in the database or not, and will attempt to add it if it doesn't find it all. This is very useful for seeding the database when it is created for the first time with test data."
    protected override void OnModelCreating(ModelBuilder modelBuilder) 
{
    // seed data with campsite types
    modelBuilder.Entity<CampsiteType>().HasData(new CampsiteType[]
    {
        new CampsiteType {Id = 1, CampsiteTypeName = "Tent", FeePerNight = 15.99M, MaxReservationDays = 7},
        new CampsiteType {Id = 2, CampsiteTypeName = "RV", FeePerNight = 26.50M, MaxReservationDays = 14},
        new CampsiteType {Id = 3, CampsiteTypeName = "Primitive", FeePerNight = 10.00M, MaxReservationDays = 3},
        new CampsiteType {Id = 4, CampsiteTypeName = "Hammock", FeePerNight = 12M, MaxReservationDays = 7}
    });

// "This adds campsites to the database."
    modelBuilder.Entity<Campsite>().HasData(new Campsite[]
{
    new Campsite {Id = 1, CampsiteTypeId = 1, Nickname = "Barred Owl", ImageUrl="https://tnstateparks.com/assets/images/content-images/campgrounds/249/colsp-area2-site73.jpg"},
    new Campsite {Id = 2, CampsiteTypeId = 2, Nickname = "Screechy Rooster Mornings Campgrounds", ImageUrl="https://hipcamp-res.cloudinary.com/f_auto,c_limit,w_1120,q_60/v1683846899/land-photos/cylaydbzi96nta6h5oms.jpg"},
    new Campsite {Id = 3, CampsiteTypeId = 3, Nickname = "Explorer's Respite", ImageUrl="https://explorerchick.com/wp-content/uploads/2023/08/campsite1.jpg"},
    new Campsite {Id = 4, CampsiteTypeId = 4, Nickname = "The Sleepy Sloth", ImageUrl="https://en.pimg.jp/084/266/196/1/84266196.jpg"},
    new Campsite {Id = 5, CampsiteTypeId = 2, Nickname = "Wrangled Wildlife Campsites", ImageUrl="https://www.visitarizona.com/places/parks-monuments/patagonia-lake-state-park/"},
    new Campsite {Id = 6, CampsiteTypeId = 3, Nickname = "FreeCamp Campgrounds", ImageUrl="https://static01.nyt.com/images/2021/04/25/multimedia/25ah-camping/merlin_186621867_547397c8-d887-4bbb-a094-d17f15b6cd95-jumbo.jpg?quality=75&auto=webp"},
});

// Add a user profile with the .HasData method
modelBuilder.Entity<UserProfile>().HasData(new UserProfile[]
{
    new UserProfile {Id = 1, FirstName = "Roger", LastName = "Rogers", Email = "Roger@Rogers.com"},
    new UserProfile {Id = 2, FirstName = "Bill", LastName = "Billington", Email = "Bill@Billington.com"}
});

modelBuilder.Entity<Reservation>().HasData(new Reservation[]
{
    new Reservation {Id = 1, CampsiteId = 2, UserProfileId = 1, CheckinDate = new DateTime(2022, 12, 10)},
    new Reservation {Id = 2, CampsiteId = 1, UserProfileId = 2, CheckinDate = new DateTime(2022, 12, 12)}
});

}
}



// "In this chapter we will create the class that is able to access a PostgreSQL database and seed the database with data. We will then create the database using the dotnet cli migration tool."

// "DbContext is a class that comes from EF Core that represents our database as .NET objects that we can access."

//* Inheritance
// "...this class is actually called CreekRiverDbContext, and after its name you see : DbContext. This is how inheritance is indicated in C#. Inheritance means that a class inherits all of the properties, fields, and methods of another class. In this case, we want our CreekRiverDbContext class to be a DbContext as well. Inheritance indicates an "is-a" relationship between two types. All of the properties of DbContext allow this class to connect to the database with no other code that you have to write."

// "DbSet is another class from EF Core, which is like other collections such as List and Array, in that we can write Linq queries to get data from them. What is special about DbSet is that our Linq queries will be transformed into a SQL query, which will be run against the database to get the data for which we are querying."

//* Constructor
// "Finally, there is something that looks like a method called CreekRiverDbContext. This is a constructor, which is a method-like member of a class that allows us to write extra logic to configure the class, so that it is ready for use when it is created. You can always tell that something is a constructor in a class when: 1. It is public, 2. has the same name as the class itself, and 3. has no return type. In this case, our CreekRiverDbContext class actually doesn't need any special setup, but the DbContext class does. DbContext is our class's base class, and it requires an options object to set itself up properly. We use the base keyword to pass that object down to DbContext when ASP.NET creates the CreekRiverDbContext class."

//* Access Modifier
// "This is the first use in the course of the access modifier protected. Up until now, you have made all of your classes and properties public, meaning that they are accessible anywhere in the code that you can reference the class or a property of the class. protected, in contrast, means that the method can only be called from code inside the class itself, or by any class that inherits it. This is a form of encapsulation, which means keeping code that is only safe or useful to use inside a particular context inaccessible to other parts of the program. We will see why this is important later."

//* Encapsulation
// "This is a form of encapsulation, which means keeping code that is only safe or useful to use inside a particular context inaccessible to other parts of the program."