using EHandelApi.Models;
using EHandelApi.Repositories;
using Microsoft.EntityFrameworkCore;

namespace EHandelApi.Tests
{
    public class ProductRepositoryTests
    {
        private EHandelContext CreateContext(string dbName)
        {
            var options = new DbContextOptionsBuilder<EHandelContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;
            return new EHandelContext(options);
        }

        [Fact]
        public async Task AddAsync_ShouldSaveProductToDatabase()
        {
            // Arrange
            using var context = CreateContext("AddProduct");
            var repository = new ProductRepository(context);
            var product = new Product { Name = "Testprodukt", Price = 99.99m, Stock = 10 };

            // Act
            await repository.AddAsync(product);

            // Assert
            var saved = await context.Products.FirstOrDefaultAsync(p => p.Name == "Testprodukt");
            Assert.NotNull(saved);
            Assert.Equal(99.99m, saved.Price);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnAllProducts()
        {
            // Arrange
            using var context = CreateContext("GetAllProducts");
            var repository = new ProductRepository(context);
            await repository.AddAsync(new Product { Name = "Produkt 1", Price = 10m, Stock = 5 });
            await repository.AddAsync(new Product { Name = "Produkt 2", Price = 20m, Stock = 3 });

            // Act
            var result = await repository.GetAllAsync();

            // Assert
            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnCorrectProduct()
        {
            // Arrange
            using var context = CreateContext("GetProductById");
            var repository = new ProductRepository(context);
            await repository.AddAsync(new Product { Name = "Testprodukt", Price = 99.99m, Stock = 10 });

            // Act
            var result = await repository.GetByIdAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Testprodukt", result.Name);
        }

        [Fact]
        public async Task SearchAsync_ShouldFindProductsByName()
        {
            // Arrange
            using var context = CreateContext("SearchProducts");
            var repository = new ProductRepository(context);
            await repository.AddAsync(new Product { Name = "Röd tröja", Price = 199m, Stock = 5 });
            await repository.AddAsync(new Product { Name = "Blå byxor", Price = 299m, Stock = 3 });

            // Act
            var result = await repository.SearchAsync("tröja");

            // Assert
            Assert.Single(result);
            Assert.Equal("Röd tröja", result.First().Name);
        }

        [Fact]
        public async Task DeleteAsync_ShouldRemoveProduct()
        {
            // Arrange
            using var context = CreateContext("DeleteProduct");
            var repository = new ProductRepository(context);
            await repository.AddAsync(new Product { Name = "Testprodukt", Price = 99.99m, Stock = 10 });

            // Act
            await repository.DeleteAsync(1);

            // Assert
            var result = await repository.GetAllAsync();
            Assert.Empty(result);
        }

        [Fact]
        public async Task UpdateAsync_ShouldUpdateProduct()
        {
            // Arrange
            using var context = CreateContext("UpdateProduct");
            var repository = new ProductRepository(context);
            await repository.AddAsync(new Product { Name = "Gammalt namn", Price = 99.99m, Stock = 10 });
            var product = await repository.GetByIdAsync(1);

            // Act
            product!.Name = "Nytt namn";
            await repository.UpdateAsync(product);

            // Assert
            var updated = await repository.GetByIdAsync(1);
            Assert.Equal("Nytt namn", updated!.Name);
        }
    }

    public class OrderRepositoryTests
    {
        private EHandelContext CreateContext(string dbName)
        {
            var options = new DbContextOptionsBuilder<EHandelContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;
            return new EHandelContext(options);
        }

        [Fact]
        public async Task AddAsync_ShouldSaveOrderToDatabase()
        {
            // Arrange
            using var context = CreateContext("AddOrder");
            var userRepo = new UserRepository(context);
            var orderRepo = new OrderRepository(context);

            var user = new User { Username = "testuser", Email = "test@test.com", PasswordHash = "hash" };
            await userRepo.AddAsync(user);

            var order = new Order { UserId = 1, TotalAmount = 299m };

            // Act
            await orderRepo.AddAsync(order);

            // Assert
            var saved = await context.Orders.FirstOrDefaultAsync(o => o.UserId == 1);
            Assert.NotNull(saved);
            Assert.Equal(299m, saved.TotalAmount);
        }

        [Fact]
        public async Task GetByUserIdAsync_ShouldReturnUserOrders()
        {
            // Arrange
            using var context = CreateContext("GetOrdersByUser");
            var userRepo = new UserRepository(context);
            var orderRepo = new OrderRepository(context);

            var user = new User { Username = "testuser", Email = "test@test.com", PasswordHash = "hash" };
            await userRepo.AddAsync(user);
            await orderRepo.AddAsync(new Order { UserId = 1, TotalAmount = 100m });
            await orderRepo.AddAsync(new Order { UserId = 1, TotalAmount = 200m });

            // Act
            var result = await orderRepo.GetByUserIdAsync(1);

            // Assert
            Assert.Equal(2, result.Count());
        }
    }

    public class UserRepositoryTests
    {
        private EHandelContext CreateContext(string dbName)
        {
            var options = new DbContextOptionsBuilder<EHandelContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;
            return new EHandelContext(options);
        }

        [Fact]
        public async Task AddAsync_ShouldSaveUserToDatabase()
        {
            // Arrange
            using var context = CreateContext("AddUser");
            var repository = new UserRepository(context);
            var user = new User { Username = "testuser", Email = "test@test.com", PasswordHash = "hash" };

            // Act
            await repository.AddAsync(user);

            // Assert
            var saved = await context.Users.FirstOrDefaultAsync(u => u.Email == "test@test.com");
            Assert.NotNull(saved);
            Assert.Equal("testuser", saved.Username);
        }

        [Fact]
        public async Task GetByEmailAsync_ShouldReturnCorrectUser()
        {
            // Arrange
            using var context = CreateContext("GetUserByEmail");
            var repository = new UserRepository(context);
            await repository.AddAsync(new User { Username = "testuser", Email = "test@test.com", PasswordHash = "hash" });

            // Act
            var result = await repository.GetByEmailAsync("test@test.com");

            // Assert
            Assert.NotNull(result);
            Assert.Equal("testuser", result.Username);
        }
    }
}