using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.AutoMock;
using Plooto.Payments.Application.UnitTests.Extensions;
using Plooto.Payments.Application.UseCases.Bill.Commands;
using Plooto.Payments.Domain.Interfaces;

namespace Plooto.Payments.Application.UnitTests.UseCases.Bill.Commands
{
    public class CreateBillCommandTests
    {
        private readonly AutoMocker _mocker;
        private readonly CreateBillCommandHandler _handler;
        private readonly CreateBillCommandValidator _validator;

        public CreateBillCommandTests()
        {
            _mocker = new AutoMocker();

            _handler = _mocker.CreateInstance<CreateBillCommandHandler>();
            _validator = _mocker.CreateInstance<CreateBillCommandValidator>();
        }

        [Fact]
        public async Task Validator_ValidCommand_ShouldPassValidation()
        {
            var command = new CreateBillCommand
            {
                Amount = 1,
                VendorName = "test"
            };

            // Act
            var result = await _validator.ValidateAsync(command);

            // Assert
            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public async Task Validator_InvalidCommand_ShouldNotPassValidation()
        {
            var command = new CreateBillCommand
            {
                Amount = 0,
                VendorName = string.Empty
            };

            // Act
            var result = await _validator.ValidateAsync(command);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Count.Should().Be(2);
            result.Errors.Select(x => x.ErrorMessage).Should().Contain("'Vendor Name' must not be empty.");
            result.Errors.Select(x => x.ErrorMessage).Should().Contain("'Amount' must be greater than '0'.");
        }

        [Fact]
        public async Task Handler_ValidCommand_ShouldCreate()
        {
            var command = new CreateBillCommand
            {
                Amount = 1,
                VendorName = "test"
            };

            var billId = 0;

            var repository = _mocker.GetMock<IBillRepository>();
            var unitOfWork = _mocker.GetMock<IUnitOfWork>();

            repository
                .Setup(s => s.AddAsync(It.IsAny<Domain.Entities.Bill>(), It.IsAny<CancellationToken>()))
                .Callback(() => { billId = 1; });

            unitOfWork.Setup(s => s.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .Verifiable();

            // Act
            await _handler.Handle(command, CancellationToken.None);

            billId.Should().Be(1);
            unitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()));
        }

        [Fact]
        public async Task Handler_RepositoryThrowsException_ShouldLogTheExceptionAndThrow()
        {
            var command = new CreateBillCommand
            {
                Amount = 1,
                VendorName = "test"
            };

            var exceptionMessage = "Error to connect to database";

            var repository = _mocker.GetMock<IBillRepository>();
            var logger = _mocker.GetMock<ILogger<CreateBillCommandHandler>>();

            repository
                 .Setup(s => s.AddAsync(It.IsAny<Domain.Entities.Bill>(), It.IsAny<CancellationToken>()))
                 .Throws(new Exception(exceptionMessage));

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _handler.Handle(command, CancellationToken.None));

            logger.VerifyLog(Times.Once, LogLevel.Error);
            repository.Verify(x => x.AddAsync(It.IsAny<Domain.Entities.Bill>(), It.IsAny<CancellationToken>()), exceptionMessage);
        }
    }
}
