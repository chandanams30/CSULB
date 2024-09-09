using CSULB_COE.Controllers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThoughtFocus.Common.Utilities.Interfaces;
using ThoughtFocus.DataAccess.DBHelper;
using ThoughtFocus.Service.Implementation;
using ThoughtFocus.Service.Interfaces;

namespace ThoughtFocus.CSULB_CED.App.UnitTest
{
    public class StudentProfile
    {
        private readonly IConfiguration _configuration;
        private readonly StudentProfileImpl _studentProfileService;
        private readonly StudentProfileController _controller;
        private readonly ILogger<StudentProfileImpl> _serviceLogger;
        private readonly ISqlDBUtility _sqlDBUtility;
        private readonly ISendMail _sendMail;
        public StudentProfile()
        {
            // Setup configuration for the database connection
            _configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory()) // Set the base path to the current directory
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true) // Load the appsettings.json file
    .Build();
            var sqlDbLogger = new LoggerFactory().CreateLogger<SqlDBUtility>();
            _sqlDBUtility = new SqlDBUtility(sqlDbLogger, _configuration);
            // Initialize service logger
            _serviceLogger = new LoggerFactory().CreateLogger<StudentProfileImpl>();

            // Initialize StudentProfileImpl with actual dependencies
            _studentProfileService = new StudentProfileImpl(
                _sqlDBUtility,
                _configuration,
                _sendMail,
                _serviceLogger
            );

            // Initialize StudentProfileController with the actual StudentProfileImpl
            _controller = new StudentProfileController(
                _studentProfileService,
                new LoggerFactory().CreateLogger<StudentProfileController>(),
                _configuration
            );
        }
        [Fact]
        public void Encrypt_Decrypt_String()
        {
            // Arrange
            string clearText = "123-45-6789";

            // Act
            string encryptedText = _studentProfileService.EncryptSSNNumber(clearText);

            // Assert
            Assert.NotNull(encryptedText);
            Assert.NotEqual(clearText, encryptedText);
            Assert.False(string.IsNullOrWhiteSpace(encryptedText));

            // Decrypt the result and verify it matches the input
            string decryptedText = _studentProfileService.DecryptSSNNumber(encryptedText);
            Assert.Equal(clearText, decryptedText);
        }

    }
}
