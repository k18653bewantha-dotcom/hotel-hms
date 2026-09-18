using HotelBookingSystem.Models;

namespace HotelBookingSystem.Data;

public static class DbSeeder
{
    public static void Seed(HotelDbContext context)
    {
        if (!context.Rooms.Any())
        {
            context.Rooms.AddRange(
                new Room { RoomNumber = "401", RoomType = "Double Mountain View", Capacity = 2, PricePerNight = 25000, Status = RoomStatuses.Ready, Floor = "4" },
                new Room { RoomNumber = "402", RoomType = "Double Mountain View", Capacity = 2, PricePerNight = 25000, Status = RoomStatuses.Ready, Floor = "4" },
                new Room { RoomNumber = "403", RoomType = "Double Mountain View", Capacity = 2, PricePerNight = 25000, Status = RoomStatuses.Ready, Floor = "4" },
                new Room { RoomNumber = "404", RoomType = "Double Mountain View", Capacity = 2, PricePerNight = 25000, Status = RoomStatuses.Ready, Floor = "4" },
                new Room { RoomNumber = "405", RoomType = "Double Mountain View", Capacity = 2, PricePerNight = 25000, Status = RoomStatuses.Ready, Floor = "4" },
                new Room { RoomNumber = "406", RoomType = "Double Mountain View", Capacity = 2, PricePerNight = 25000, Status = RoomStatuses.Ready, Floor = "4" },
                new Room { RoomNumber = "407", RoomType = "Triple Room", Capacity = 3, PricePerNight = 30000, Status = RoomStatuses.Ready, Floor = "4" },
                new Room { RoomNumber = "408", RoomType = "Triple Room", Capacity = 3, PricePerNight = 30000, Status = RoomStatuses.Ready, Floor = "4" },
                new Room { RoomNumber = "409", RoomType = "Family Room", Capacity = 4, PricePerNight = 36000, Status = RoomStatuses.Ready, Floor = "4" },
                new Room { RoomNumber = "410", RoomType = "Suite", Capacity = 2, PricePerNight = 40000, Status = RoomStatuses.Ready, Floor = "4" },
                new Room { RoomNumber = "411", RoomType = "Double Mountain View", Capacity = 2, PricePerNight = 25000, Status = RoomStatuses.Ready, Floor = "4" },
                new Room { RoomNumber = "412", RoomType = "Double Mountain View", Capacity = 2, PricePerNight = 25000, Status = RoomStatuses.Ready, Floor = "4" },
                new Room { RoomNumber = "413", RoomType = "Double Mountain View", Capacity = 2, PricePerNight = 25000, Status = RoomStatuses.Ready, Floor = "4" },
                new Room { RoomNumber = "414", RoomType = "Double Mountain View", Capacity = 2, PricePerNight = 25000, Status = RoomStatuses.Ready, Floor = "4" },
                new Room { RoomNumber = "415", RoomType = "Triple Room", Capacity = 3, PricePerNight = 30000, Status = RoomStatuses.Ready, Floor = "4" },
                new Room { RoomNumber = "416", RoomType = "Suite", Capacity = 2, PricePerNight = 40000, Status = RoomStatuses.Ready, Floor = "4" }
            );
        }

        if (!context.Customers.Any())
        {
            context.Customers.AddRange(
                new Customer
                {
                    FullName = "John Doe",
                    PassportNo = "P1234567",
                    Phone = "0771234567",
                    Email = "john@example.com",
                    Country = "United Kingdom"
                },
                new Customer
                {
                    FullName = "Sample Local Guest",
                    NicNo = "900000000V",
                    Phone = "0712345678",
                    Email = "guest@example.com",
                    Country = "Sri Lanka"
                }
            );
        }

        if (!context.MenuCategories.Any())
        {
            var food = new MenuCategory { Name = "Food" };
            var drinks = new MenuCategory { Name = "Drinks" };
            var desserts = new MenuCategory { Name = "Desserts" };

            context.MenuCategories.AddRange(food, drinks, desserts);
            context.SaveChanges();

            context.MenuItems.AddRange(
                new MenuItem { Name = "Fried Rice", MenuCategoryId = food.Id, Price = 850 },
                new MenuItem { Name = "Chicken Kottu", MenuCategoryId = food.Id, Price = 950 },
                new MenuItem { Name = "Grilled Chicken", MenuCategoryId = food.Id, Price = 1800 },
                new MenuItem { Name = "Coca Cola", MenuCategoryId = drinks.Id, Price = 300 },
                new MenuItem { Name = "Orange Juice", MenuCategoryId = drinks.Id, Price = 450 },
                new MenuItem { Name = "Ceylon Tea", MenuCategoryId = drinks.Id, Price = 350 },
                new MenuItem { Name = "Chocolate Mousse", MenuCategoryId = desserts.Id, Price = 650 }
            );
        }

        if (!context.RestaurantTables.Any())
        {
            context.RestaurantTables.AddRange(
                new RestaurantTable { TableNumber = "T1" },
                new RestaurantTable { TableNumber = "T2" },
                new RestaurantTable { TableNumber = "T3" },
                new RestaurantTable { TableNumber = "T4" },
                new RestaurantTable { TableNumber = "T5" }
            );
        }

        context.SaveChanges();
    }
}
