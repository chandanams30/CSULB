using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using ThoughtFocus.Service.Interfaces;
using CSULB_COE.Controllers;
using ThoughtFocus.Domain.Response.GraduateProgram;
using ThoughtFocus.Service.Implementation;
using ThoughtFocus.Common.Utilities.Interfaces;
namespace ThoughtFocus.CSULB_CED.App.UnitTest;
using Microsoft.Data.SqlClient;
using Serilog.Core;
using System.Data;
using ThoughtFocus.Common.Utilities.Implementation;
using ThoughtFocus.DataAccess.DBHelper;

public class MilestonesControllerTests
{
    private readonly IConfiguration _configuration;
    private readonly ISqlDBUtility _sqlDBUtility;
    private readonly ISendMail _sendMail;
    private readonly ILogger<MilestonesServiceImpl> _serviceLogger;
    private readonly ICommonUtils _commonUtils;
    private readonly MilestonesServiceImpl _milestonesService;
    private readonly MilestonesController _controller;

    public MilestonesControllerTests()
    {
        // Setup configuration for the database connection and mail settings
        var inMemorySettings = new Dictionary<string, string> {
            {"ConnectionStrings:AppDBConnection", "Data Source=20.25.58.133;Initial Catalog=CSULB_DB_DEV;User ID=csulbsql; Password=c$ulb@D3vSql;"},
            
        };

        _configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings)
            .Build();
        var sqlDbLogger = new LoggerFactory().CreateLogger<SqlDBUtility>();
        _sqlDBUtility = new SqlDBUtility(sqlDbLogger, _configuration);
        var sendMailLogger = new LoggerFactory().CreateLogger<SendMail>();
        _sendMail = new SendMail(_configuration, sendMailLogger);
        _commonUtils = new CommonUtils(); 

        // Initialize service logger
        _serviceLogger = new LoggerFactory().CreateLogger<MilestonesServiceImpl>();

        // Initialize MilestonesServiceImpl with actual dependencies
        _milestonesService = new MilestonesServiceImpl(
            _sqlDBUtility,
            _configuration,
            _sendMail,
            _serviceLogger,
            _commonUtils
        );

        // Initialize MilestonesController with the actual MilestonesServiceImpl
        _controller = new MilestonesController(
            _milestonesService,
            new LoggerFactory().CreateLogger<MilestonesController>(),
            _configuration
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
            new Semester { TermCode = "2252", TermName = "Spring 2025" },
            new Semester { TermCode = "2254", TermName = "Fall 2025" }
        }
        };

        // Act
        var result = _controller.GetDistinctSemesterList();

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(mockResponse.Message, result.Message);

        // Additional assertions based on your expected data in the database
        Assert.NotEmpty(result.Semesters);
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

        //_mockMilestonesService.Setup(service => service.GetDistinctSemesterList())
        //                      .Returns(mockResponse);

        // Act
        var result = _controller.GetDistinctSemesterList();

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(mockResponse.Message, result.Message);
        Assert.Empty(result.Semesters);
    }

    //[Fact]
    //public void GetDistinctSemesterList_ReturnsFailure_WhenExceptionIsThrown()
    //{
    //    // Arrange
    //    //_mockMilestonesService.Setup(service => service.GetDistinctSemesterList())
    //    //                      .Throws(new Exception("Database error"));

    //    // Act
    //    var result = _controller.GetDistinctSemesterList();

    //    // Assert
    //    Assert.False(result.IsSuccess);
    //    Assert.Equal("Failed to retrieve data , please try after sometime", result.Message);
    //    Assert.Equal("Database error", result.StackTrace);
    //}
}
