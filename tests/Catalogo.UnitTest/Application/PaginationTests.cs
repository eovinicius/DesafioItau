using CatalogoItau.Application.Abstractions.Pagination;

using FluentAssertions;

namespace Catalogo.UnitTest.Application;

public sealed class PaginationTests
{
    [Fact]
    public void PaginationParams_ShouldNormalizeInvalidValues()
    {
        // Arrange

        // Act
        var paramsInvalid = new PaginationParams(0, 101);

        // Assert
        paramsInvalid.Page.Should().Be(1);
        paramsInvalid.PageSize.Should().Be(10);
        paramsInvalid.GetSkip().Should().Be(0);
    }

    [Fact]
    public void PaginationParams_GetSkip_ShouldCalculateOffset()
    {
        // Arrange
        var pagination = new PaginationParams(3, 20);

        // Act
        var skip = pagination.GetSkip();

        // Assert
        skip.Should().Be(40);
    }

    [Fact]
    public void PagedResult_ShouldCalculateNavigationProperties()
    {
        // Arrange
        var items = Enumerable.Range(1, 10);

        // Act
        var result = new PagedResult<int>(items, page: 2, pageSize: 10, totalItems: 25);

        // Assert
        result.TotalPages.Should().Be(3);
        result.HasPreviousPage.Should().BeTrue();
        result.HasNextPage.Should().BeTrue();
    }
}
