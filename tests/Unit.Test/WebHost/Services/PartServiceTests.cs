using Aspotus.Catalog.Api.Data.Entities;
using Aspotus.Catalog.Api.Data.Repositories.Interfaces;
using Aspotus.Catalog.Api.Enums;
using Aspotus.Catalog.Api.Exceptions;
using Aspotus.Catalog.Api.Models.Requests;
using Aspotus.Catalog.Api.Services.Implementations;
using Bogus;
using FluentAssertions;
using Moq;

namespace Unit.Test.WebHost.Services
{
    public class PartServiceTests
    {
        private readonly Mock<IPartRepository> _partRepositoryMock;
        private readonly Mock<IPartCategoryRepository> _categoryRepositoryMock;
        private readonly Mock<IPartManufacturerRepository> _manufacturerRepositoryMock;
        private readonly Mock<ICarRepository> _carRepositoryMock;

        private readonly PartService _service;

        public PartServiceTests()
        {
            _partRepositoryMock = new Mock<IPartRepository>();
            _categoryRepositoryMock = new Mock<IPartCategoryRepository>();
            _manufacturerRepositoryMock = new Mock<IPartManufacturerRepository>();
            _carRepositoryMock = new Mock<ICarRepository>();

            _service = new PartService(
                _partRepositoryMock.Object,
                _categoryRepositoryMock.Object,
                _manufacturerRepositoryMock.Object,
                _carRepositoryMock.Object
            );
        }

        [Fact]
        public async Task GetById_ShouldReturnPart_WhenExists()
        {
            // Arrange
            var category = GenerateCategory();
            var manufacturer = GenerateManufacturer();
            var part = PartWithRelations(GeneratePart(), category, manufacturer);

            _partRepositoryMock
                .Setup(x => x.GetByIdAsync(part.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(part);

            // Act
            var result = await _service.GetByIdAsync(part.Id);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(part.Id);

            _partRepositoryMock.Verify(x => x.GetByIdAsync(part.Id, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetById_ShouldReturnNull_WhenNotFound()
        {
            // Arrange
            var id = Guid.NewGuid();

            _partRepositoryMock
                .Setup(x => x.GetByIdAsync(id, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Part?)null);

            // Act
            var result = await _service.GetByIdAsync(id);

            // Assert
            result.Should().BeNull();

            _partRepositoryMock.Verify(x => x.GetByIdAsync(id, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Create_ShouldCreatePart_WithAllRelations()
        {
            // Arrange
            var category = GenerateCategory();
            var manufacturer = GenerateManufacturer();
            var request = GenerateCreatePartRequest(category.Id, manufacturer.Id);

            Part? savedPart = null;

            _partRepositoryMock
                .Setup(x => x.GetByArticleAsync(request.Article, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Part?)null);

            _categoryRepositoryMock
                .Setup(x => x.GetByIdAsync(category.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(category);

            _manufacturerRepositoryMock
                .Setup(x => x.GetByIdAsync(manufacturer.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(manufacturer);

            _partRepositoryMock
                .Setup(x => x.AddAsync(It.IsAny<Part>(), It.IsAny<CancellationToken>()))
                .Callback<Part, CancellationToken>((p, _) =>
                {
                    p.Category = category;
                    p.Manufacturer = manufacturer;

                    p.ReplacementArticles ??= new List<PartReplacement>();

                    savedPart = p;
                })
                .Returns(Task.CompletedTask);

            _partRepositoryMock
                .Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(() => savedPart);

            // Act
            var result = await _service.CreateAsync(request);

            // Assert
            result.Should().NotBeNull();
            result!.CategoryName.Should().Be(category.Name);
            result.ManufacturerName.Should().Be(manufacturer.Name);

            _partRepositoryMock.Verify(x => x.GetByArticleAsync(request.Article, It.IsAny<CancellationToken>()), Times.Once);
            _categoryRepositoryMock.Verify(x => x.GetByIdAsync(category.Id, It.IsAny<CancellationToken>()), Times.Once);
            _manufacturerRepositoryMock.Verify(x => x.GetByIdAsync(manufacturer.Id, It.IsAny<CancellationToken>()), Times.Once);
            _partRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Part>(), It.IsAny<CancellationToken>()), Times.Once);
            _partRepositoryMock.Verify(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Create_ShouldThrow_WhenCategoryNotFound()
        {
            // Arrange
            var request = GenerateCreatePartRequest(Guid.NewGuid(), Guid.NewGuid());

            _partRepositoryMock
                .Setup(x => x.GetByArticleAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Part?)null);

            _categoryRepositoryMock
                .Setup(x => x.GetByIdAsync(request.CategoryId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((PartCategory?)null);

            // Act
            var act = () => _service.CreateAsync(request);

            // Assert
            await act.Should().ThrowAsync<NotFoundException>()
                .Where(ex => ex.Message == "Указанная категория запчасти не существует.");

            _partRepositoryMock.Verify(x => x.GetByArticleAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
            _categoryRepositoryMock.Verify(x => x.GetByIdAsync(request.CategoryId, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Create_ShouldThrow_WhenManufacturerNotFound()
        {
            // Arrange
            var category = GenerateCategory();
            var request = GenerateCreatePartRequest(category.Id, Guid.NewGuid());

            _partRepositoryMock
                .Setup(x => x.GetByArticleAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Part?)null);

            _categoryRepositoryMock
                .Setup(x => x.GetByIdAsync(category.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(category);

            _manufacturerRepositoryMock
                .Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((PartManufacturer?)null);

            // Act
            var act = () => _service.CreateAsync(request);

            // Assert
            await act.Should().ThrowAsync<NotFoundException>()
                .Where(ex => ex.Message == "Указанный производитель запчасти не существует.");

            _partRepositoryMock.Verify(x => x.GetByArticleAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
            _categoryRepositoryMock.Verify(x => x.GetByIdAsync(category.Id, It.IsAny<CancellationToken>()), Times.Once);
            _manufacturerRepositoryMock.Verify(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Create_ShouldValidate_NewCondition()
        {
            // Arrange
            var category = GenerateCategory();
            var manufacturer = GenerateManufacturer();

            var request = GenerateCreatePartRequest(category.Id, manufacturer.Id);
            request.ConditionType = PartConditionType.New;
            request.ConditionPercent = 50;

            _categoryRepositoryMock
                .Setup(x => x.GetByIdAsync(category.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(category);

            _manufacturerRepositoryMock
                .Setup(x => x.GetByIdAsync(manufacturer.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(manufacturer);

            // Act
            Func<Task> act = () => _service.CreateAsync(request);

            // Assert
            await act.Should().ThrowAsync<Aspotus.Catalog.Api.Exceptions.ValidationException>()
                .Where(ex => ex.Message == "Для новой запчасти нельзя указывать состояние, описание состояния и пробег снятия.");

            _categoryRepositoryMock.Verify(x => x.GetByIdAsync(category.Id, It.IsAny<CancellationToken>()), Times.Once);
            _manufacturerRepositoryMock.Verify(x => x.GetByIdAsync(manufacturer.Id, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Create_ShouldValidate_UsedCondition()
        {
            // Arrange
            var category = GenerateCategory();
            var manufacturer = GenerateManufacturer();

            var request = GenerateCreatePartRequest(category.Id, manufacturer.Id);
            request.ConditionType = PartConditionType.Used;
            request.ConditionPercent = null;

            _categoryRepositoryMock
                .Setup(x => x.GetByIdAsync(category.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(category);

            _manufacturerRepositoryMock
                .Setup(x => x.GetByIdAsync(manufacturer.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(manufacturer);

            // Act
            Func<Task> act = () => _service.CreateAsync(request);

            // Assert
            await act.Should().ThrowAsync<Aspotus.Catalog.Api.Exceptions.ValidationException>()
                .Where(ex => ex.Message == "Для БУ-запчасти необходимо указать процент состояния.");

            _categoryRepositoryMock.Verify(x => x.GetByIdAsync(category.Id, It.IsAny<CancellationToken>()), Times.Once);
            _manufacturerRepositoryMock.Verify(x => x.GetByIdAsync(manufacturer.Id, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Create_ShouldThrow_WhenArticleExists() 
        { 
            // Arrange
            var category = GenerateCategory(); 
            var manufacturer = GenerateManufacturer(); 

            var request = GenerateCreatePartRequest(category.Id, manufacturer.Id); 

            var existingPart = GeneratePart(); 

            existingPart.Article = request.Article; 

            _categoryRepositoryMock
                .Setup(x => x.GetByIdAsync(category.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(category);

            _manufacturerRepositoryMock
                .Setup(x => x.GetByIdAsync(manufacturer.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(manufacturer); 

            _partRepositoryMock
                .Setup(x => x.GetByArticleAsync(request.Article, It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingPart); 

            // Act
            var act = async () => await _service.CreateAsync(request); 

            // Assert
            await act.Should().ThrowAsync<AlreadyExistsException>()
                .Where(ex => ex.Message == $"Запчасть с артикулом '{existingPart.Article}' уже существует.");
         
            _partRepositoryMock.Verify(x => x.GetByArticleAsync(request.Article, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Update_ShouldUpdatePart_WithRelations()
        {
            // Arrange
            var category = GenerateCategory();
            var manufacturer = GenerateManufacturer();

            var part = PartWithRelations(GeneratePart(), category, manufacturer);

            var request = GenerateUpdatePartRequest(category.Id, manufacturer.Id);

            _partRepositoryMock
                .Setup(x => x.GetByIdAsync(part.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(part);

            _partRepositoryMock
                .Setup(x => x.GetByArticleAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Part?)null);

            _categoryRepositoryMock
                .Setup(x => x.GetByIdAsync(category.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(category);

            _manufacturerRepositoryMock
                .Setup(x => x.GetByIdAsync(manufacturer.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(manufacturer);

            _partRepositoryMock
                .Setup(x => x.UpdateAsync(It.IsAny<Part>(), It.IsAny<CancellationToken>()))
                .Callback<Part, CancellationToken>((updated, _) =>
                {
                    part.Name = updated.Name;
                    part.Article = updated.Article;

                    part.Category = category;
                    part.Manufacturer = manufacturer;
                })
                .Returns(Task.CompletedTask);

            // Act
            var result = await _service.UpdateAsync(part.Id, request);

            // Assert
            result.Should().NotBeNull();
            result!.Name.Should().Be(request.Name.Trim());
            result.CategoryName.Should().Be(category.Name);
            result.ManufacturerName.Should().Be(manufacturer.Name);

            _partRepositoryMock.Verify(x => x.GetByArticleAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
            _categoryRepositoryMock.Verify(x => x.GetByIdAsync(category.Id, It.IsAny<CancellationToken>()), Times.Once);
            _manufacturerRepositoryMock.Verify(x => x.GetByIdAsync(manufacturer.Id, It.IsAny<CancellationToken>()), Times.Once);
            _partRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<Part>(), It.IsAny<CancellationToken>()), Times.Once);
        }
        [Fact]
        public async Task Update_ShouldThrow_WhenPartNotFound()
        {
            // Arrange
            var id = Guid.NewGuid();    
            var request = GenerateUpdatePartRequest(Guid.NewGuid(), Guid.NewGuid());

            _partRepositoryMock
                .Setup(x => x.GetByIdAsync(id, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Part?)null);

            // Act
            var result = await _service.UpdateAsync(id, request);

            // Assert
            result.Should().BeNull();

            _partRepositoryMock.Verify(x => x.GetByIdAsync(id, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact] 
        public async Task Update_ShouldThrow_WhenCategoryNotFound() 
        { 
            // Arrange
            var part = GeneratePart(); 
            var request = GenerateUpdatePartRequest(Guid.NewGuid(), Guid.NewGuid()); 

            _partRepositoryMock
                .Setup(x => x.GetByIdAsync(part.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(part); 

            _partRepositoryMock
                .Setup(x => x.GetByArticleAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Part?)null); 

            _categoryRepositoryMock
                .Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((PartCategory?)null); 

            // Act
            var act = async () => await _service.UpdateAsync(part.Id, request);

            // Assert
            await act.Should().ThrowAsync<NotFoundException>()
                .Where(ex => ex.Message == "Указанная категория запчасти не существует.");

            _partRepositoryMock.Verify(x => x.GetByIdAsync(part.Id, It.IsAny<CancellationToken>()), Times.Once);
            _partRepositoryMock.Verify(x => x.GetByArticleAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
            _categoryRepositoryMock.Verify(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact] 
        public async Task Update_ShouldThrow_WhenManufacturerNotFound() 
        { 
            // Arrange
            var part = GeneratePart();
            var category = GenerateCategory(); 
            var request = GenerateUpdatePartRequest(category.Id, Guid.NewGuid()); 
            
            _partRepositoryMock
                .Setup(x => x.GetByIdAsync(part.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(part); 
            
            _partRepositoryMock
                .Setup(x => x.GetByArticleAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Part?)null);
            
            _categoryRepositoryMock
                .Setup(x => x.GetByIdAsync(category.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(category); 
            
            _manufacturerRepositoryMock
                .Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((PartManufacturer?)null); 
            
            // Act
            var act = async () => await _service.UpdateAsync(part.Id, request);

            // Assert
            await act.Should().ThrowAsync<NotFoundException>()
                .Where(ex => ex.Message == "Указанный производитель запчасти не существует.");

            _partRepositoryMock.Verify(x => x.GetByIdAsync(part.Id, It.IsAny<CancellationToken>()), Times.Once);
            _partRepositoryMock.Verify(x => x.GetByArticleAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
            _categoryRepositoryMock.Verify(x => x.GetByIdAsync(category.Id, It.IsAny<CancellationToken>()), Times.Once);
            _manufacturerRepositoryMock.Verify(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Once);
        }


        [Fact]
        public async Task Update_ShouldThrow_WhenArticleAlreadyExists()
        {
            // Arrange
            var category = GenerateCategory();
            var manufacturer = GenerateManufacturer();
            var existing = PartWithRelations(GeneratePart(), category, manufacturer);
            var another = GeneratePart();

            var request = GenerateUpdatePartRequest(category.Id, manufacturer.Id);
            request.Article = another.Article; 

            _partRepositoryMock.Setup(x => x.GetByIdAsync(existing.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(existing);

            _partRepositoryMock.Setup(x => x.GetByArticleAsync(request.Article, It.IsAny<CancellationToken>()))
                .ReturnsAsync(another);

            // Act
            Func<Task> act = () => _service.UpdateAsync(existing.Id, request);

            // Assert
            await act.Should().ThrowAsync<AlreadyExistsException>()
                .Where(ex => ex.Message == $"Запчасть с артикулом '{another.Article}' уже существует.");

            _partRepositoryMock.Verify(x => x.GetByIdAsync(existing.Id, It.IsAny<CancellationToken>()), Times.Once);
            _partRepositoryMock.Verify(x => x.GetByArticleAsync(request.Article, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Update_ShouldThrow_WhenNewPartHasInvalidState()
        {
            // Arrange
            var part = GeneratePart(); 
            var category = GenerateCategory(); 
            var manufacturer = GenerateManufacturer(); 

            var request = GenerateUpdatePartRequest(category.Id, manufacturer.Id); 
            request.ConditionType = PartConditionType.New; 
            request.ConditionPercent = 50;

            _partRepositoryMock
                .Setup(x => x.GetByIdAsync(part.Id, It.IsAny<CancellationToken>()))        
                .ReturnsAsync(part);    
            
            _partRepositoryMock
                .Setup(x => x.GetByArticleAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))        
                .ReturnsAsync((Part?)null);   
            
            _categoryRepositoryMock
                .Setup(x => x.GetByIdAsync(category.Id, It.IsAny<CancellationToken>()))        
                .ReturnsAsync(category);    
            
            _manufacturerRepositoryMock
                .Setup(x => x.GetByIdAsync(manufacturer.Id, It.IsAny<CancellationToken>()))        
                .ReturnsAsync(manufacturer);    
            
            // Act
            var act = async () => await _service.UpdateAsync(part.Id, request);   

            // Assert
            await act.Should().ThrowAsync<Aspotus.Catalog.Api.Exceptions.ValidationException>()
                .Where(ex => ex.Message == "Для новой запчасти нельзя указывать состояние, описание состояния и пробег снятия.");

            _partRepositoryMock.Verify(x => x.GetByIdAsync(part.Id, It.IsAny<CancellationToken>()), Times.Once);
            _partRepositoryMock.Verify(x => x.GetByArticleAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
            _categoryRepositoryMock.Verify(x => x.GetByIdAsync(category.Id, It.IsAny<CancellationToken>()), Times.Once);
            _manufacturerRepositoryMock.Verify(x => x.GetByIdAsync(manufacturer.Id, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Update_ShouldThrow_WhenUsedWithoutPercent()
        {
            // Arrange
            var part = GeneratePart(); 
            var category = GenerateCategory(); 
            var manufacturer = GenerateManufacturer(); 

            var request = GenerateUpdatePartRequest(category.Id, manufacturer.Id); 
            request.ConditionType = PartConditionType.Used; 
            request.ConditionPercent = null;  

            _partRepositoryMock
                .Setup(x => x.GetByIdAsync(part.Id, It.IsAny<CancellationToken>()))        
                .ReturnsAsync(part);    
            
            _partRepositoryMock
                .Setup(x => x.GetByArticleAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))        
                .ReturnsAsync((Part?)null);    
            
            _categoryRepositoryMock
                .Setup(x => x.GetByIdAsync(category.Id, It.IsAny<CancellationToken>()))       
                .ReturnsAsync(category);    
            
            _manufacturerRepositoryMock
                .Setup(x => x.GetByIdAsync(manufacturer.Id, It.IsAny<CancellationToken>()))        
                .ReturnsAsync(manufacturer);    
            
            // Act
            var act = async () => await _service.UpdateAsync(part.Id, request);   

            // Assert
            await act.Should().ThrowAsync<Aspotus.Catalog.Api.Exceptions.ValidationException>()
                .Where(ex => ex.Message == "Для БУ-запчасти необходимо указать процент состояния.");

            _partRepositoryMock.Verify(x => x.GetByIdAsync(part.Id, It.IsAny<CancellationToken>()), Times.Once);
            _partRepositoryMock.Verify(x => x.GetByArticleAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
            _categoryRepositoryMock.Verify(x => x.GetByIdAsync(category.Id, It.IsAny<CancellationToken>()), Times.Once);
            _manufacturerRepositoryMock.Verify(x => x.GetByIdAsync(manufacturer.Id, It.IsAny<CancellationToken>()), Times.Once);
        }


        [Fact]
        public async Task Delete_ShouldRemovePart_WhenExists()
        {
            // Arrange
            var part = GeneratePart();

            _partRepositoryMock
                .Setup(x => x.GetByIdAsync(part.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(part);

            _partRepositoryMock
                .Setup(x => x.DeleteAsync(part.Id, It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _service.DeleteAsync(part.Id);

            // Assert
            result.Should().BeTrue();

            _partRepositoryMock.Verify(x => x.DeleteAsync(part.Id, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Delete_ShouldThrow_WhenNotFound()
        {
            // Arrange
            var id = Guid.NewGuid();
            _partRepositoryMock
                .Setup(x => x.GetByIdAsync(id, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Part?)null);

            // Act
            var result = await _service.DeleteAsync(id);

            // Assert
            result.Should().BeFalse();

            _partRepositoryMock.Verify(x => x.GetByIdAsync(id, It.IsAny<CancellationToken>()), Times.Once);
        }

        private PartCategory GenerateCategory()
        {
            return new Faker<PartCategory>()
                .RuleFor(x => x.Id, f => f.Random.Guid())
                .RuleFor(x => x.Name, f => f.Commerce.Categories(1)[0])
                .Generate();
        }

        private PartManufacturer GenerateManufacturer()
        {
            return new Faker<PartManufacturer>()
                .RuleFor(x => x.Id, f => f.Random.Guid())
                .RuleFor(x => x.Name, f => f.Company.CompanyName())
                .Generate();
        }

        private Part GeneratePart(
            Guid? categoryId = null,
            Guid? manufacturerId = null,
            PartCategory? category = null,
            PartManufacturer? manufacturer = null)
        {
            category ??= GenerateCategory();
            manufacturer ??= GenerateManufacturer();

            return new Faker<Part>()
                .RuleFor(x => x.Id, f => f.Random.Guid())
                .RuleFor(x => x.Name, f => f.Commerce.ProductName())
                .RuleFor(x => x.Article, f => f.Commerce.Ean8())
                .RuleFor(x => x.Price, f => f.Random.Decimal(10, 1000))
                .RuleFor(x => x.StockQuantity, f => f.Random.Int(1, 100))
                .RuleFor(x => x.CategoryId, f => categoryId ?? category.Id)
                .RuleFor(x => x.ManufacturerId, f => manufacturerId ?? manufacturer.Id)

                .RuleFor(x => x.Category, f => category)
                .RuleFor(x => x.Manufacturer, f => manufacturer)

                .RuleFor(x => x.ReplacementArticles, f => new List<PartReplacement>
                {
            new PartReplacement
            {
                Id = Guid.NewGuid(),
                ReplacementArticle = f.Commerce.Ean8()
            }
                })
                .Generate();
        }

        private Part PartWithRelations(Part part, PartCategory category, PartManufacturer manufacturer)
        {
            part.Category = category;
            part.Manufacturer = manufacturer;
            part.ReplacementArticles ??= new List<PartReplacement>();
            return part;

        }

        private CreatePartRequest GenerateCreatePartRequest(Guid categoryId, Guid manufacturerId)
        {
            return new Faker<CreatePartRequest>()
                .RuleFor(x => x.Name, f => f.Commerce.ProductName())
                .RuleFor(x => x.Article, f => f.Random.AlphaNumeric(10))
                .RuleFor(x => x.Description, f => f.Lorem.Sentence())
                .RuleFor(x => x.Price, f => f.Random.Decimal(10, 1000))
                .RuleFor(x => x.StockQuantity, f => f.Random.Int(0, 100))
                .RuleFor(x => x.IsOriginal, f => f.Random.Bool())
                .RuleFor(x => x.ConditionType, f => PartConditionType.New)

                .RuleFor(x => x.ReplacementArticles, f => new List<string> { f.Random.AlphaNumeric(5) })
                .RuleFor(x => x.CategoryId, categoryId)
                .RuleFor(x => x.ManufacturerId, manufacturerId)
                .Generate();
        }

        private UpdatePartRequest GenerateUpdatePartRequest(Guid categoryId, Guid manufacturerId)
        {
            return new Faker<UpdatePartRequest>()
                .RuleFor(x => x.Name, f => f.Commerce.ProductName())
                .RuleFor(x => x.Article, f => f.Random.AlphaNumeric(10))
                .RuleFor(x => x.Description, f => f.Lorem.Sentence())
                .RuleFor(x => x.Price, f => f.Random.Decimal(10, 1000))
                .RuleFor(x => x.StockQuantity, f => f.Random.Int(0, 100))
                .RuleFor(x => x.IsOriginal, f => f.Random.Bool())
                .RuleFor(x => x.ConditionType, f => PartConditionType.New)

                .RuleFor(x => x.ReplacementArticles, f => new List<string> { f.Random.AlphaNumeric(5) })
                .RuleFor(x => x.CategoryId, categoryId)
                .RuleFor(x => x.ManufacturerId, manufacturerId)
                .Generate();
        }

    }
}