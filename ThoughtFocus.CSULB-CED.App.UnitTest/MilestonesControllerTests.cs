using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using ThoughtFocus.Service.Interfaces;
using CSULB_COE.Controllers;
using ThoughtFocus.Domain.Response.GraduateProgram;

public class MilestonesControllerTests
{
    private readonly Mock<IMilestonesService> _mockMilestonesService;
    private readonly Mock<ILogger<MilestonesController>> _mockLogger;
    private readonly Mock<IConfiguration> _mockConfiguration;
    private readonly MilestonesController _controller;

    public MilestonesControllerTests()
    {
        _mockMilestonesService = new Mock<IMilestonesService>();
        _mockLogger = new Mock<ILogger<MilestonesController>>();
        _mockConfiguration = new Mock<IConfiguration>();

        _controller = new MilestonesController(
            _mockMilestonesService.Object,
            _mockLogger.Object,
            _mockConfiguration.Object
        );
    }

    [Fact]
    public void GetDistinctSemesterList_ReturnsSuccess_WhenDataIsPresent()
    {
        // Arrange
        var mockResponse = new SemesterListResponse
        {
            IsSuccess = true,
            Message = "Data Retrieved Successfully",
            Semesters = new List<Semester>
        {
            new Semester { TermCode = "2234", TermName = "Fall 2023" },
            new Semester { TermCode = "2242", TermName = "Spring 2024" },
            new Semester { TermCode = "2243", TermName = "Summer 2024" },
            new Semester { TermCode = "2244", TermName = "Fall 2024" },
            new Semester { TermCode = "2252", TermName = "Spring 2025" }
            //new Semester { TermCode = "2254", TermName = "Fall 2025" }
        }
        };

        _mockMilestonesService.Setup(service => service.GetDistinctSemesterList())
                              .Returns(mockResponse);

        // Act
        var result = _controller.GetDistinctSemesterList();
        Console.WriteLine("Expected Result - :",mockResponse.Semesters.Count);
        Console.WriteLine("Actual Result - :", result.Semesters.Count);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("Data Retrieved Successfully", result.Message);

        // Check the count of semesters
        Assert.Equal(mockResponse.Semesters.Count, result.Semesters.Count);

        // Check each semester item
        for (int i = 0; i < mockResponse.Semesters.Count; i++)
        {
            Assert.Equal(mockResponse.Semesters[i].TermCode, result.Semesters[i].TermCode);
            Assert.Equal(mockResponse.Semesters[i].TermName, result.Semesters[i].TermName);
        }
    }

    [Fact]
    public void GetDistinctSemesterList_ReturnsFailure_WhenNoDataIsPresent()
    {
        // Arrange
        var mockResponse = new SemesterListResponse
        {
            IsSuccess = false,
            Message = "No Data Present",
            Semesters = new List<Semester>()
        };

        _mockMilestonesService.Setup(service => service.GetDistinctSemesterList())
                              .Returns(mockResponse);

        // Act
        var result = _controller.GetDistinctSemesterList();

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("No Data Present", result.Message);
        Assert.Empty(result.Semesters);
    }

    [Fact]
    public void GetDistinctSemesterList_ReturnsFailure_WhenExceptionIsThrown()
    {
        // Arrange
        _mockMilestonesService.Setup(service => service.GetDistinctSemesterList())
                              .Throws(new Exception("Database error"));

        // Act
        var result = _controller.GetDistinctSemesterList();

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("Failed to retrieve data , please try after sometime", result.Message);
        Assert.Equal("Database error", result.StackTrace);
    }
}
