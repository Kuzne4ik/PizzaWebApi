using MapsterMapper;
using Moq;
using PizzaWebApi.Core.Models;
using PizzaWebApi.Core.Interfaces;
using System.Linq.Expressions;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using System;
using PizzaWebApi.Core;
using Microsoft.Extensions.Logging;
using PizzaWebApi.Infrastructure.Services;
using MockQueryable.Moq;
using PizzaWebApi.Core.Requests;

namespace PizzaWebApi.Tests.Services
{

    public class CategoryServiceTest
    {
        IMapper _mapper = null;
        private Mock<ILogger<CategoryService>> _loggerCategoryService;
        List<Category> _categoriesDB = new()
        {
            new Category
            {
                Id = 1,
                Name = "Cats",
                Title = "Cats",
                Products = new List<Product>()
            },
            new Category
            {
                Id = 2,
                Name = "Dogs",
                Title = "Dogs",
                Products = new List<Product>()
            },
            new Category
            {
                Id = 3,
                Name = "Ships",
                Title = "Ships",
                Products = new List<Product>()
            },
            new Category
            {
                Id = 4,
                Name = "Horses",
                Title = "Horses",
                Products = new List<Product>()
            },
            new Category
            {
                Id = 5,
                Name = "Pigs",
                Title = "Pigs",
                Products = new List<Product>()
            },
            new Category
            {
                Id = 6,
                Name = "Salamanders",
                Title = "Salamanders",
                Products = new List<Product>()
            }
        };

        public CategoryServiceTest()
        {
            _mapper = new Mapper(MapsterMapperSetup.GetTypeAdapterConfig());
            _loggerCategoryService = new Mock<ILogger<CategoryService>>();
        }

        [Fact]
        public async Task GetAll()
        {
            // # Arrange
            var categoriesMock = _categoriesDB.BuildMock();

            var categoryRepositoryMock = new Mock<ICategoryRepository>();
            categoryRepositoryMock
                .Setup(x => x.ListQuery())
                .Returns(categoriesMock);

            SearchCriteriaRequest searchCriteriaRequest = new SearchCriteriaRequest()
            {
                Keyword = String.Empty,
                Page = 1,
                PageSize = 5
            };

            var categoriesService = new CategoryService(categoryRepositoryMock.Object, _mapper, _loggerCategoryService.Object);

            // # Act
            var categoriesSearchResult = await categoriesService.GetAllAsync(searchCriteriaRequest);

            // # Assert
            Assert.Equal(_categoriesDB.Count, categoriesSearchResult.Total);
            foreach (var categoryDTO in categoriesSearchResult.Results)
            {
                var categoryDB = _categoriesDB.FirstOrDefault(t => t.Id == categoryDTO.Id);
                Assert.NotNull(categoryDB);
                Assert.Equal(categoryDB.Name, categoryDTO.Name);
                Assert.Equal(categoryDB.Title, categoryDTO.Title);
            }
        }

        [Fact]
        public async Task GetOne()
        {
            // # Arrange
            var categoriesMock = _categoriesDB.BuildMock();
            var testId = _categoriesDB.First().Id;

            var categoryRepositoryMock = new Mock<ICategoryRepository>();
            categoryRepositoryMock
                .Setup(x => x.FindByIdAsync(It.IsAny<int>))
                .ReturnsAsync(_categoriesDB.FirstOrDefault(t => t.Id == testId));

            Expression<Func<Category, bool>> predicate = t => t.Id == testId;

            categoryRepositoryMock
                .Setup(x => x.AnyAsync(It.IsAny<Expression<Func<Category, bool>>>()))
                .Callback<Expression<Func<Category, bool>>>( s => predicate = s)
                .ReturnsAsync(categoriesMock.Any(predicate));

            categoryRepositoryMock
                .Setup(x => x.FindByConditionQuery(It.IsAny<Expression<Func<Category, bool>>>()))
                    .Returns(categoriesMock.Where(t => t.Id == testId));

            var categoriesService = new CategoryService(categoryRepositoryMock.Object, _mapper, _loggerCategoryService.Object);

            // # Act
            var categoryDTO = await categoriesService.GetByIdAsync(testId);

            // # Assert
            Assert.NotNull(categoryDTO);
            var categoryDB = _categoriesDB.FirstOrDefault(t => t.Id == testId);
            Assert.Equal(categoryDB.Name, categoryDTO.Name);
            Assert.Equal(categoryDB.Title, categoryDTO.Title);
        }

        [Fact]
        public async Task GetOneWrongId()
        {
            // # Arrange
            var testId = -1;
            var categoriesMock = _categoriesDB.BuildMock();

            var categoryRepositoryMock = new Mock<ICategoryRepository>();

            categoryRepositoryMock.Setup(x => x.AnyAsync(It.IsAny<Expression<Func<Category, bool>>>())).ReturnsAsync(categoriesMock.Any(t => t.Id == testId));
            categoryRepositoryMock
                .Setup(x => x.FindByConditionQuery(It.IsAny<Expression<Func<Category, bool>>>()))
                    .Returns(categoriesMock.Where(t => t.Id == testId));

            var categoriesService = new CategoryService(categoryRepositoryMock.Object, _mapper, _loggerCategoryService.Object);

            // # Act & Assert
            await Assert.ThrowsAnyAsync<KeyNotFoundException>(() =>  categoriesService.GetByIdAsync(testId));
        }

        [Fact]
        public async Task FindAll()
        {
            // # Arrange
            var categoriesMock = _categoriesDB.BuildMock();
            var limit = 5;

            var categoryRepositoryMock = new Mock<ICategoryRepository>();
            categoryRepositoryMock
                .Setup(x => x.ListQuery())
                .Returns(categoriesMock);

            var pageCriteriaRequest1 = new SearchCriteriaRequest()
            {
                Page = 1,
                PageSize = limit,
            };

            var pageCriteriaRequest2 = new SearchCriteriaRequest()
            {
                Page = 2,
                PageSize = limit,
            };

            var categoriesService = new CategoryService(categoryRepositoryMock.Object, _mapper, _loggerCategoryService.Object);

            // # Act
            var categoryiesSearchResult = await categoriesService.GetAllAsync(pageCriteriaRequest1);

            // # Assert
            Assert.Equal(5, categoryiesSearchResult.Results.Count());
            Assert.Equal(_categoriesDB.Count, categoryiesSearchResult.Total);

            // # Act
            categoryiesSearchResult = await categoriesService.GetAllAsync(pageCriteriaRequest2);

            // # Assert
            Assert.Equal(_categoriesDB.Count - limit, categoryiesSearchResult.Results.Count());
            Assert.Equal(_categoriesDB.Count, categoryiesSearchResult.Total);
        }
    }
}