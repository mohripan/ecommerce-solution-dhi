using Moq;
using ProductService.Application.DTOs.Requests;
using ProductService.Application.Services.Impls;
using ProductService.Contracts.Interfaces;
using ProductService.Domain.Factories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductService.Tests
{
    [TestClass]
    public class ProductAppServiceTests
    {
        private Mock<IProductRepository> _productRepositoryMock;
        private Mock<IUnitOfWork> _unitOfWorkMock;
        private Mock<IProductFactory> _productFactoryMock;
        private ProductAppService _productAppService;

        [TestInitialize]
        public void Setup()
        {
            _productRepositoryMock = new Mock<IProductRepository>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _productFactoryMock = new Mock<IProductFactory>();

            _productAppService = new ProductAppService(
                _productRepositoryMock.Object,
                _unitOfWorkMock.Object,
                _productFactoryMock.Object
            );
        }

        [TestMethod]
        public async Task CreateProductAsync_ValidRequest_ReturnsProductResponseDto()
        {
            // Arrange
            var requestDto = new ProductRequestDto { Name = "Test Product", CategoryId = 1, Price = 100 };
            var userId = 1;
            var product = new Product(1, userId, 100, userId, "Test Product");

            _productFactoryMock.Setup(f => f.CreateProduct(requestDto.CategoryId, userId, requestDto.Price, userId, requestDto.Name))
                               .Returns(product);

            _productRepositoryMock.Setup(r => r.AddAsync(product)).Returns(Task.CompletedTask);
            _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

            _productRepositoryMock.Setup(r => r.GetByIdAsync(product.Id))
                .ReturnsAsync(product);

            // Act
            var result = await _productAppService.CreateProductAsync(requestDto, userId);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(product.Name, result.ProductName);
            _productRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Product>()), Times.Once);
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
        }

        [TestMethod]
        public async Task UpdateProductAsync_ValidRequest_UpdatesProduct()
        {
            // Arrange
            var requestDto = new ProductRequestDto { Name = "Updated Product", CategoryId = 1, Price = 200 };
            var productId = 1;
            var userId = 1;

            var existingProduct = new Product(1, userId, 100, userId, "Original Product")
            {
                Id = productId
            };

            _productRepositoryMock.Setup(r => r.GetByIdAsync(productId))
                .ReturnsAsync(existingProduct);
            _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

            // Act
            var result = await _productAppService.UpdateProductAsync(productId, requestDto, userId);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(requestDto.Name, result.ProductName);
            Assert.AreEqual(requestDto.Price, result.Price);
            _productRepositoryMock.Verify(r => r.Update(It.IsAny<Product>()), Times.Once);
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
        }

        [TestMethod]
        public async Task UpdateProductAsync_ProductNotFound_ReturnsNull()
        {
            // Arrange
            var requestDto = new ProductRequestDto { Name = "Non-existent Product", CategoryId = 1, Price = 150 };
            var productId = 99;
            var userId = 1;

            _productRepositoryMock.Setup(r => r.GetByIdAsync(productId))
                .ReturnsAsync((Product)null);

            // Act
            var result = await _productAppService.UpdateProductAsync(productId, requestDto, userId);

            // Assert
            Assert.IsNull(result);
            _productRepositoryMock.Verify(r => r.Update(It.IsAny<Product>()), Times.Never);
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Never);
        }

        [TestMethod]
        public async Task DeleteProductAsync_ValidRequest_DeletesProduct()
        {
            // Arrange
            var productId = 1;
            var userId = 1;

            var existingProduct = new Product(1, userId, 100, userId, "Test Product")
            {
                Id = productId
            };

            _productRepositoryMock.Setup(r => r.GetByIdAsync(productId))
                .ReturnsAsync(existingProduct);

            _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

            // Act
            var result = await _productAppService.DeleteProductAsync(productId, userId);

            // Assert
            Assert.IsTrue(result);
            _productRepositoryMock.Verify(r => r.Delete(It.IsAny<Product>()), Times.Once);
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
        }

        [TestMethod]
        [ExpectedException(typeof(UnauthorizedAccessException))]
        public async Task DeleteProductAsync_ProductNotOwnedByUser_ThrowsUnauthorizedAccessException()
        {
            // Arrange
            var productId = 1;
            var userId = 1;

            var existingProduct = new Product(1, 2, 100, 2, "Test Product")
            {
                Id = productId
            };

            _productRepositoryMock.Setup(r => r.GetByIdAsync(productId))
                .ReturnsAsync(existingProduct);

            // Act
            await _productAppService.DeleteProductAsync(productId, userId);

            // Assert
        }
    }
}
