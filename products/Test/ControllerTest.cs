using Api;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;


namespace Test
{
    [TestFixture]
    public class ProductsControllerTests
    {
        private Mock<IService> _serviceMock;
        private Mock<ILogger<ProductsController>> _loggerMock;
        private ProductsController _controller;

        [SetUp]
        public void Setup()
        {
            _serviceMock = new Mock<IService>();
            _loggerMock = new Mock<ILogger<ProductsController>>();
            _controller = new ProductsController(_serviceMock.Object, _loggerMock.Object);
        }

        [Test]
        public void GetAllProducts_ReturnsOkResult_WithListOfProducts()
        {
            // Arrange
            var products = new List<Product> { new Product { Id = 1, Name = "Product1", Price = 100, CategoryId = 1 } };
            _serviceMock.Setup(s => s.GetAll()).Returns(products);

            // Act
            var result = _controller.GetAllProducts();

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.That(products, Is.EqualTo(okResult.Value));
        }

        [Test]
        public void GetProduct_ExistingId_ReturnsOkResult_WithProduct()
        {
            // Arrange
            var product = new Product { Id = 1, Name = "Product1", Price = 100, CategoryId = 1 };
            _serviceMock.Setup(s => s.GetById(1)).Returns(product);

            // Act
            var result = _controller.GetProduct(1);

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.That(product, Is.EqualTo(okResult.Value));
        }

        [Test]
        public void GetProduct_NonExistingId_ReturnsNotFoundResult()
        {
            // Arrange
            _serviceMock.Setup(s => s.GetById(1)).Returns((Product)null);

            // Act
            var result = _controller.GetProduct(1);

            // Assert
            Assert.IsInstanceOf<NotFoundResult>(result);
        }

        [Test]
        public async Task CreateProduct_ValidProduct_ReturnsCreatedAtActionResult()
        {
            // Arrange
            var product = new Product { Id = 0, Name = "Product1", Price = 100, CategoryId = 1 };
            _serviceMock.Setup(s => s.CategoryExists(1)).ReturnsAsync(true);
            _serviceMock.Setup(s => s.Add(It.IsAny<Product>())).Returns(product);

            // Act
            var result = await _controller.CreateProduct(product);

            // Assert
            Assert.IsInstanceOf<CreatedAtActionResult>(result);
            var createdAtActionResult = result as CreatedAtActionResult;
            Assert.IsNotNull(createdAtActionResult);
            Assert.That(nameof(_controller.GetProduct), Is.EqualTo(createdAtActionResult.ActionName));
            Assert.That(product, Is.EqualTo(createdAtActionResult.Value));
        }

        [Test]
        public async Task CreateProduct_InvalidCategoryId_ReturnsBadRequest()
        {
            // Arrange
            var product = new Product { Id = 0, Name = "Product1", Price = 100, CategoryId = 999 };
            _serviceMock.Setup(s => s.CategoryExists(999)).ReturnsAsync(false);

            // Act
            var result = await _controller.CreateProduct(product);

            // Assert
            Assert.IsInstanceOf<BadRequestObjectResult>(result);
        }

        [Test]
        public async Task UpdateProduct_ValidProduct_ReturnsOkResult()
        {
            // Arrange
            var product = new Product { Id = 1, Name = "UpdatedProduct", Price = 150, CategoryId = 1 };
            _serviceMock.Setup(s => s.GetById(1)).Returns(product);
            _serviceMock.Setup(s => s.CategoryExists(1)).ReturnsAsync(true);

            // Act
            var result = await _controller.UpdateProduct(1, product);

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.That(product, Is.EqualTo(okResult.Value));
        }

        [Test]
        public async Task UpdateProduct_MismatchedId_ReturnsBadRequest()
        {
            // Arrange
            var product = new Product { Id = 2, Name = "Product1", Price = 100, CategoryId = 1 };

            // Act
            var result = await _controller.UpdateProduct(1, product);

            // Assert
            Assert.IsInstanceOf<BadRequestResult>(result);
        }

        [Test]
        public async Task UpdateProduct_NonExistingProduct_ReturnsNotFound()
        {
            // Arrange
            var product = new Product { Id = 1, Name = "Product1", Price = 100, CategoryId = 1 };
            _serviceMock.Setup(s => s.GetById(1)).Returns((Product)null);

            // Act
            var result = await _controller.UpdateProduct(1, product);

            // Assert
            Assert.IsInstanceOf<NotFoundResult>(result);
        }

        [Test]
        public void DeleteProduct_ExistingProduct_ReturnsNoContentResult()
        {
            // Arrange
            _serviceMock.Setup(s => s.Delete(1));

            // Act
            var result = _controller.DeleteProduct(1);

            // Assert
            Assert.IsInstanceOf<NoContentResult>(result);
        }
    }
}