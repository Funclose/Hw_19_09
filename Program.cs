using Microsoft.EntityFrameworkCore;

namespace HW_19_04
{
     class Program
    {
        static void Main(string[] args)
        {
            using (ApplicationContext db = new ApplicationContext()) 
            {
                db.Database.EnsureDeleted();
                db.Database.EnsureCreated();

                    List<Client> clients = new List<Client>()
                    { 
                        new Client {Name = "Artem" , Email = "Artem@gmail.com", Address = "bojino Main street"},
                        new Client {Name = "Alex" , Email = "Alex@gmail.com", Address = "bojino Second Street"}
                    };

                    
                    List<Products> products = new List<Products>()
                {
                    new Products { Name = "Orange", Price = 10.5m },
                    new Products { Name = "Banana", Price = 20m },
                    new Products { Name = "Sneck", Price = 15m }
                };

               
                    db.Clients.AddRange(clients);
                    db.Products.AddRange(products);
                    db.SaveChanges();

                
                    List<Order> orders = new List<Order>()
                {
                    new Order
                    {
                        Client = clients[0], 
                        OrderDetails = new List<OrderDetails>()
                        {
                            new OrderDetails { Products = products[0], Quantity = 2 }, 
                            new OrderDetails { Products = products[1], Quantity = 1 }  
                        }
                    },
                    new Order
                    {
                        Client = clients[1],
                        OrderDetails = new List<OrderDetails>()
                        {
                            new OrderDetails { Products = products[1], Quantity = 3 }, 
                            new OrderDetails { Products = products[2], Quantity = 2 }  
                        }
                    }
                };
                db.Orders.AddRange(orders);
                db.SaveChanges();
            }
            using (ApplicationContext context =new ApplicationContext())
            {
                var Query = context.Clients.Select(c => new
                {
                    Name = c.Name,
                    Email = c.Email,
                    Address = c.Address,
                    totalorder = c.Orders.Count(),
                    totalSpent = c.Orders.SelectMany(order => order.OrderDetails).Sum(detail => detail.Quantity * detail.Products.Price),
                    ExpensivProduct = c.Orders.SelectMany(order => order.OrderDetails).Select(price => price.Products.Price).Max()

                }).ToList();

                foreach (var item in Query)
                {
                    Console.WriteLine($"Name:  {item.Name}");
                    Console.WriteLine($"Email:  {item.Email}");
                    Console.WriteLine($"Address:  {item.Address}");
                    Console.WriteLine($"TotalOrder:  {item.totalorder}");
                    Console.WriteLine($"TotalSpent:  {item.totalSpent}");
                }
            }
            
        }
    }
    public class Client
    { 
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public ICollection<Order> Orders { get; set; }
    }
    public class Products
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Decimal Price { get; set; }
    }
    public class Order
    {
        public int Id { get; set; } 
        public DateTime OrderDate { get; set; }
        

        public int ClientId { get; set; }
        public Client Client { get; set; }

        public ICollection<OrderDetails> OrderDetails { get; set; }
    }
    public class OrderDetails
    {
        public int Id { get; set; }
        public Products Products { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }

        public Order Orders { get; set; }
        public int OrderId { get; set; }

    }

    public class ApplicationContext : DbContext
    {
        public DbSet<Client> Clients { get; set; }
        public DbSet<Products> Products { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetails>OrderDetails  { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=DESKTOP-TBASQVJ;Database=testdb;Trusted_Connection=True;TrustServerCertificate=True;");
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Order>().Property(e => e.OrderDate).HasDefaultValueSql("GETDATE()");
        }
    }



}
