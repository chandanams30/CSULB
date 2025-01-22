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
using ThoughtFocus.Domain.Response.Milestones;

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
        // Setup configuration for the database connection
        _configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory()) // Set the base path to the current directory
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true) // Load the appsettings.json file
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
        // Arrange - to intialiaze mock response
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
    public void GetMilestoneApproverTypes_ReturnsSuccess_WhenDataIsPresent()
    {
        // Arrange
        var mockResponse = new MilestonesApproverTypesResponse
        {
            IsSuccess = true,
            Message = "Data Retrieved Successfully",
            ApproverTypes = new List<MilestonesApproverTypes>
        {
            new MilestonesApproverTypes { ApproverTypeID = 1, Name = "Internal" },
            new MilestonesApproverTypes { ApproverTypeID = 2, Name = "External" },
            new MilestonesApproverTypes { ApproverTypeID = 3, Name = "External Approver by Student" }
        }
        };

        // Act
        var result = _controller.GetMilestoneApproverTypes();

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(mockResponse.Message, result.Message);

        // Additional assertions based on your expected data in the database
        Assert.NotEmpty(result.ApproverTypes);
        Assert.Equal(mockResponse.ApproverTypes.Count, result.ApproverTypes.Count);

        // Check each appover type item
        for (int i = 0; i < mockResponse.ApproverTypes.Count; i++)
        {
            Assert.Equal(mockResponse.ApproverTypes[i].ApproverTypeID, result.ApproverTypes[i].ApproverTypeID);
            Assert.Equal(mockResponse.ApproverTypes[i].Name, result.ApproverTypes[i].Name);
        }

    }
    [Fact]
    public void GetMilestoneTypes_ReturnsSuccess_WhenDataIsPresent()
    {
        // Arrange
        var mockResponse = new MilestoneTypesResponse
        {
            IsSuccess = true,
            Message = "Data Retrieved Successfully",
            MilestoneTypes = new List<MilestoneTypes>
        {
            new MilestoneTypes { ID = 1, Name = "Student Milestone" },
            new MilestoneTypes { ID = 2, Name = "Faculty Milestone" }
        }
        };

        // Act
        var result = _controller.GetMilestoneTypes();

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(mockResponse.Message, result.Message);

        // Additional assertions based on your expected data in the database
        Assert.NotEmpty(result.MilestoneTypes);
        Assert.Equal(mockResponse.MilestoneTypes.Count, result.MilestoneTypes.Count);

        // Check each milestone type item
        for (int i = 0; i < mockResponse.MilestoneTypes.Count; i++)
        {
            Assert.Equal(mockResponse.MilestoneTypes[i].ID, result.MilestoneTypes[i].ID);
            Assert.Equal(mockResponse.MilestoneTypes[i].Name, result.MilestoneTypes[i].Name);
        }

    }
    [Fact]
    public void GetApplicationProgramsByTermCode_ReturnsSuccess_WhenDataIsPresent()
    {
        // Arrange
        string termCode = "2243";
        var mockResponse = new ApplicationProgramsResponse
        {
            IsSuccess = true,
            Message = "Data Retrieved Successfully",
            ProgramsList = new List<ApplicationProgram>
        {
            new ApplicationProgram { ProgramID = 1, ProgramName = "Education Specialist Credential Program (ESCP" },
            new ApplicationProgram { ProgramID = 2, ProgramName = "Multiple Subject Credential Program (MSCP)" },
            new ApplicationProgram { ProgramID = 3, ProgramName=  "PK-3 Early Childhood Education Specialist Instruction Credential Program (PK-3CP)"},
            new ApplicationProgram { ProgramID = 4, ProgramName = "Single Subject Credential Program (SSCP)" },
            new ApplicationProgram { ProgramID = 5, ProgramName = "Urban Dual Credential Program (UDCP)" },
            }
        };

        // Act
        var result = _controller.GetApplicationProgramsByTermCode(termCode);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(mockResponse.Message, result.Message);

        // Additional assertions based on your expected data
        Assert.NotEmpty(result.ProgramsList);
        Assert.Equal(mockResponse.ProgramsList.Count, result.ProgramsList.Count);
    }


    //[Fact]
    //public void GetDistinctSemesterList_ReturnsFailure_WhenNoDataIsPresent()
    //{
    //    // Arrange
    //    var mockResponse = new SemesterListResponse
    //    {
    //        IsSuccess = false,
    //        Message = "No Data Present",
    //        Semesters = new List<Semester>()
    //    };

        //    //_mockMilestonesService.Setup(service => service.GetDistinctSemesterList())
        //    //                      .Returns(mockResponse);

        //    // Act
        //    var result = _controller.GetDistinctSemesterList();

        //    // Assert
        //    Assert.False(result.IsSuccess);
        //    Assert.Equal(mockResponse.Message, result.Message);
        //    Assert.Empty(result.Semesters);
        //}

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
