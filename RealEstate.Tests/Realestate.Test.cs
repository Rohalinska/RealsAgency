using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using Xunit;
// Переконуйтеся, що ці namespace збігаються з вашим проектом
using RealEstate.Domain;
using RealEstate.Application;
using RealEstate.Infrastructure; 

namespace RealEstate.Tests
{
    public class UnitTest1
    {
        #region 1-10: ТЕСТИ ДОМЕННОЇ МОДЕЛІ APARTMENT (Комісії та Валідація)

        [Fact]
        public void Apartment_Commission_IsCorrect_ForStandardPrice()
        {
            var apt = new Apartment("Тестова 1", 100000, 1);
            var commission = apt.CalculateCommission();
            Assert.Equal(5000, commission); // 5% від 100 000
        }

        [Fact]
        public void Apartment_Commission_IsCorrect_ForHighPrice()
        {
            var apt = new Apartment("Люкс", 500000, 2);
            var commission = apt.CalculateCommission();
            Assert.Equal(25000, commission);
        }

        [Fact]
        public void Apartment_Commission_IsZero_WhenPriceIsZero()
        {
            var apt = new Apartment("Безкоштовна", 0, 1);
            var commission = apt.CalculateCommission();
            Assert.Equal(0, commission);
        }

        [Fact]
        public void Apartment_WithNegativePrice_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => new Apartment("Test", -10, 5));
        }

        [Fact]
        public void Apartment_WithEmptyName_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => new Apartment("", 100000, 1));
        }

        [Fact]
        public void Apartment_WithNullName_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => new Apartment(null, 100000, 1));
        }

        [Fact]
        public void Apartment_WithNegativeRooms_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => new Apartment("Test", 100000, -1));
        }

        [Fact]
        public void Apartment_WithZeroRooms_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => new Apartment("Test", 100000, 0));
        }

        [Fact]
        public void Apartment_MarkAsSold_ShouldChangeStatus()
        {
            var apt = new Apartment("Київ", 1000, 1);
            apt.MarkAsSold();
            Assert.True(apt.IsSold);
        }

        [Fact]
        public void MarkAsSold_ShouldThrow_IfAlreadySold()
        {
            var apt = new Apartment("Київ", 1000, 1);
            apt.MarkAsSold();
            Assert.Throws<AlreadySoldException>(() => apt.MarkAsSold());
        }

        #endregion

        #region 11-20: ТЕСТИ ДОМЕННОЇ МОДЕЛІ HOUSE (Валідація та Специфічна логіка)

        [Fact]
        public void House_WithNegativePrice_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => new House("Test", -10, 5));
        }

        [Fact]
        public void House_WithEmptyName_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => new House("", 200000, 2));
        }

        [Fact]
        public void House_WithNullName_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => new House(null, 200000, 2));
        }

        [Fact]
        public void House_WithNegativeFloors_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => new House("Будинок", 150000, -2));
        }

        [Fact]
        public void House_WithZeroFloors_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => new House("Будинок", 150000, 0));
        }

        [Fact]
        public void House_Commission_IsCorrect()
        {
            var house = new House("Дача", 200000, 2);
            // Припустимо, у Будинків комісія 7% або вираховується інакше
            var commission = house.CalculateCommission(); 
            Assert.Equal(14000, commission); // 7% від 200 000 (або змініть під вашу бізнес-логіку)
        }

        [Fact]
        public void House_MarkAsSold_ShouldChangeStatus()
        {
            var house = new House("Львів", 50000, 2);
            house.MarkAsSold();
            Assert.True(house.IsSold);
        }

        [Fact]
        public void House_MarkAsSold_ShouldThrow_IfAlreadySold()
        {
            var house = new House("Львів", 50000, 2);
            house.MarkAsSold();
            Assert.Throws<AlreadySoldException>(() => house.MarkAsSold());
        }

        [Fact]
        public void House_PriceUpdate_ShouldWorkCorrectly()
        {
            var house = new House("Центр", 100000, 1);
            house.UpdatePrice(120000);
            Assert.Equal(120000, house.Price);
        }

        [Fact]
        public void House_UpdatePrice_ToNegative_ThrowsException()
        {
            var house = new House("Центр", 100000, 1);
            Assert.Throws<ArgumentException>(() => house.UpdatePrice(-500));
        }

        #endregion

        #region 21-35: ІНТЕГРАЦІЙНІ ТА СЕРВІСНІ ТЕСТИ (PropertyService + Mock Репозиторію)

        [Fact]
        public async Task Service_ShouldCorrectlyAddAndRetrieveData()
        {
            var mockRepo = new Mock<IPropertyRepository>();
            var data = new List<Property>();
            
            mockRepo.Setup(r => r.AddAsync(It.IsAny<Property>()))
                    .Callback<Property>(p => data.Add(p))
                    .Returns(Task.CompletedTask);
            
            mockRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(data);

            var service = new PropertyService(mockRepo.Object);
            var apt = new Apartment("Тестова 1", 5000, 2);

            await service.AddPropertyAsync(apt);
            var result = await service.GetAllPropertiesAsync();

            Assert.Single(result);
            Assert.Equal("Тестова 1", result[0].Address);
        }

        [Fact]
        public async Task Analytics_TotalValue_ShouldBeCorrect()
        {
            var mockRepo = new Mock<IPropertyRepository>();
            mockRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Property> {
                new Apartment("A", 1000, 1),
                new Apartment("B", 2000, 1)
            });

            var service = new PropertyService(mockRepo.Object);
            var total = await service.GetTotalPortfolioValueAsync();

            Assert.Equal(3000, total);
        }

        [Fact]
        public async Task Service_GetAllProperties_ReturnsEmptyList_WhenNoProperties()
        {
            var mockRepo = new Mock<IPropertyRepository>();
            mockRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Property>());

            var service = new PropertyService(mockRepo.Object);
            var result = await service.GetAllPropertiesAsync();

            Assert.Empty(result);
        }

        [Fact]
        public async Task Service_GetTotalPortfolioValue_ReturnsZero_WhenNoProperties()
        {
            var mockRepo = new Mock<IPropertyRepository>();
            mockRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Property>());

            var service = new PropertyService(mockRepo.Object);
            var total = await service.GetTotalPortfolioValueAsync();

            Assert.Equal(0, total);
        }

        [Fact]
        public async Task Service_ShouldCallRepository_WhenAddingProperty()
        {
            var mockRepo = new Mock<IPropertyRepository>();
            var service = new PropertyService(mockRepo.Object);
            var apt = new Apartment("Test", 1000, 1);

            await service.AddPropertyAsync(apt);

            mockRepo.Verify(r => r.AddAsync(apt), Times.Once);
        }

        [Fact]
        public async Task Service_GetSoldPropertiesCount_ShouldReturnCorrectCount()
        {
            var apt1 = new Apartment("A", 1000, 1);
            var apt2 = new Apartment("B", 2000, 1);
            apt1.MarkAsSold(); // Один проданий, один ні

            var mockRepo = new Mock<IPropertyRepository>();
            mockRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Property> { apt1, apt2 });

            var service = new PropertyService(mockRepo.Object);
            var soldCount = await service.GetSoldPropertiesCountAsync();

            Assert.Equal(1, soldCount);
        }

        [Fact]
        public async Task Service_GetAvailableProperties_ShouldNotIncludeSold()
        {
            var apt1 = new Apartment("A", 1000, 1);
            var apt2 = new Apartment("B", 2000, 1);
            apt1.MarkAsSold(); 

            var mockRepo = new Mock<IPropertyRepository>();
            mockRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Property> { apt1, apt2 });

            var service = new PropertyService(mockRepo.Object);
            var available = await service.GetAvailablePropertiesAsync();

            Assert.Single(available);
            Assert.Equal("B", available[0].Address);
        }

        [Fact]
        public async Task Service_GetTotalCommission_ShouldSumAllCommissions()
        {
            var mockRepo = new Mock<IPropertyRepository>();
            mockRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Property> {
                new Apartment("A", 100000, 1), // 5000
                new Apartment("B", 200000, 1)  // 10000
            });

            var service = new PropertyService(mockRepo.Object);
            var totalCommission = await service.GetTotalExpectedCommissionAsync();

            Assert.Equal(15000, totalCommission);
        }

        [Fact]
        public async Task Service_FindPropertyByAddress_ShouldReturnCorrectProperty()
        {
            var mockRepo = new Mock<IPropertyRepository>();
            mockRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Property> {
                new Apartment("Target Address", 1000, 1),
                new Apartment("Other Address", 2000, 1)
            });

            var service = new PropertyService(mockRepo.Object);
            var result = await service.FindPropertyByAddressAsync("Target Address");

            Assert.NotNull(result);
            Assert.Equal("Target Address", result.Address);
        }

        [Fact]
        public async Task Service_FindPropertyByAddress_ShouldReturnNull_IfNotFound()
        {
            var mockRepo = new Mock<IPropertyRepository>();
            mockRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Property>());

            var service = new PropertyService(mockRepo.Object);
            var result = await service.FindPropertyByAddressAsync("NonExistent");

            Assert.Null(result);
        }

        [Fact]
        public async Task Service_GetCheapestProperty_ShouldReturnCorrectProperty()
        {
            var mockRepo = new Mock<IPropertyRepository>();
            mockRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Property> {
                new Apartment("Expensive", 500000, 1),
                new Apartment("Cheap", 20000, 1)
            });

            var service = new PropertyService(mockRepo.Object);
            var result = await service.GetCheapestPropertyAsync();

            Assert.Equal("Cheap", result.Address);
        }

        [Fact]
        public async Task Service_GetMostExpensiveProperty_ShouldReturnCorrectProperty()
        {
            var mockRepo = new Mock<IPropertyRepository>();
            mockRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Property> {
                new Apartment("Expensive", 500000, 1),
                new Apartment("Cheap", 20000, 1)
            });

            var service = new PropertyService(mockRepo.Object);
            var result = await service.GetMostExpensivePropertyAsync();

            Assert.Equal("Expensive", result.Address);
        }

        [Fact]
        public async Task Service_DeleteProperty_ShouldCallRepositoryDelete()
        {
            var mockRepo = new Mock<IPropertyRepository>();
            var service = new PropertyService(mockRepo.Object);
            
            await service.DeletePropertyAsync(1); // Припустимо видалення по Id

            mockRepo.Verify(r => r.DeleteAsync(1), Times.Once);
        }

        [Fact]
        public async Task Service_UpdateProperty_ShouldCallRepositoryUpdate()
        {
            var mockRepo = new Mock<IPropertyRepository>();
            var service = new PropertyService(mockRepo.Object);
            var apt = new Apartment("Test", 1000, 1);

            await service.UpdatePropertyAsync(apt);

            mockRepo.Verify(r => r.UpdateAsync(apt), Times.Once);
        }

        [Fact]
        public async Task Service_GetPropertiesByRoomCount_ShouldFilterCorrectly()
        {
            var mockRepo = new Mock<IPropertyRepository>();
            mockRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Property> {
                new Apartment("1-Room", 50000, 1),
                new Apartment("2-Room", 80000, 2),
                new Apartment("1-Room-Gold", 60000, 1)
            });

            var service = new PropertyService(mockRepo.Object);
            var result = await service.GetPropertiesByRoomsAsync(1);

            Assert.Equal(2, result.Count);
        }

        #endregion
    }
}