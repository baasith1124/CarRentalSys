# Enhanced FluentValidation Behavior Implementation for CarRentalSystem

This document explains the enhanced implementation of automatic validation using FluentValidation Behavior with MediatR in the CarRentalSystem project, including async validation support and localized error messages.

## Overview

The implementation provides automatic validation for all MediatR commands and queries through a pipeline behavior, ensuring consistent validation across the application without manual validation in each handler. The enhanced version includes:

- **Async Validation Support**: Full support for asynchronous validation rules
- **Localized Error Messages**: Multi-language support (English, Sinhala, Tamil)
- **Enhanced Logging**: Comprehensive logging for validation operations
- **Performance Optimizations**: Parallel validation execution

## Components Implemented

### 1. Enhanced Validation Behavior (`CarRentalSystem.Application/Behaviors/ValidationBehavior.cs`)

The `ValidationBehavior<TRequest, TResponse>` class implements `IPipelineBehavior<TRequest, TResponse>` and automatically validates requests before they reach the handlers.

**Key Features:**
- Automatically finds and executes all validators for a request
- **Async Support**: Full support for asynchronous validation rules
- **Parallel Execution**: Validators run in parallel for better performance
- **Enhanced Logging**: Comprehensive logging with structured logging
- **Error Handling**: Robust error handling for validation failures
- Collects all validation errors and throws `ValidationException` if validation fails
- Allows the request to proceed if validation passes

### 2. Localization Support

#### Resource Files
- **English**: `ValidationMessages.resx` (default)
- **Sinhala**: `ValidationMessages.si-LK.resx`
- **Tamil**: `ValidationMessages.ta-LK.resx`

#### Localization Service
- `ILocalizationService`: Interface for getting localized strings
- `LocalizationService`: Implementation using `IStringLocalizer`
- Automatic culture detection from request headers, cookies, and query strings

### 3. Enhanced Validators

Updated existing validators with comprehensive validation rules and localized messages:

#### CreateBookingCommandValidator
- Validates pickup date is not in the past
- Ensures return date is after pickup date
- Validates minimum booking duration (30 minutes)
- Validates required fields with localized error messages
- Uses `ILocalizationService` for multi-language support

#### CreateBookingCommandAsyncValidator
- **Async validation** for car availability
- Checks database for existing bookings
- Provides localized error messages for availability conflicts

#### RegisterCarValidator
- Validates car name and model with length constraints
- Validates year range (1900 to current year + 1)
- Validates rate per day is positive
- Validates date ranges for availability
- Validates optional fields with conditional rules
- **Localized messages** for all validation rules

#### CreateCustomerCommandValidator
- Validates full name format (letters and spaces only)
- Validates email format and length
- Validates NIC format (Sri Lankan format)
- Validates address and profile image path lengths
- **Localized messages** for all validation rules

#### UpdateCarCommandValidator
- Validates all required fields for car updates
- Ensures date consistency
- Validates approval status ID
- **Localized messages** for all validation rules

### 4. Async Validation Extensions

#### AsyncValidationExtensions
- Custom extension methods for async validation
- `MustAsync` methods for async predicate validation
- Support for async error message functions

#### AsyncCarAvailabilityValidator
- Example of custom async validator
- Demonstrates async validation patterns
- Can be used as a reference for other async validators

### 5. Global Exception Handling

#### Enhanced GlobalExceptionHandlingMiddleware
- Handles `ValidationException` specifically
- Returns structured error responses for AJAX requests
- Provides detailed validation error information
- Maintains existing error handling for other exceptions

#### ValidationExceptionFilter
- Controller-level exception filter for validation errors
- Returns `BadRequestObjectResult` with validation details
- Provides consistent error response format

### 6. Registration in Program.cs

```csharp
// Register Validation Behavior
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

// Register Exception Filter
builder.Services.AddControllersWithViews(options =>
{
    options.Filters.Add<ValidationExceptionFilter>();
});

// Add localization services
builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");
builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    var supportedCultures = new[]
    {
        new CultureInfo("en-US"),
        new CultureInfo("si-LK"), // Sinhala
        new CultureInfo("ta-LK")  // Tamil
    };

    options.DefaultRequestCulture = new RequestCulture("en-US");
    options.SupportedCultures = supportedCultures;
    options.SupportedUICultures = supportedCultures;
});

// Register localization service
builder.Services.AddScoped<ILocalizationService, LocalizationService>();
```

## How It Works

### 1. Request Flow
1. Controller receives request
2. MediatR sends command/query
3. **ValidationBehavior** intercepts the request
4. Finds all validators for the request type
5. **Executes validation rules in parallel** (including async validators)
6. **Logs validation progress** and results
7. If validation fails: throws `ValidationException` with localized messages
8. If validation passes: continues to handler

### 2. Localization Flow
1. **Culture Detection**: Request culture detected from headers, cookies, or query strings
2. **Resource Selection**: Appropriate resource file selected based on culture
3. **Message Localization**: Validation messages retrieved from localized resources
4. **Error Response**: Localized error messages returned to client

### 3. Error Handling
- **ValidationExceptionFilter**: Catches validation exceptions in controllers
- **GlobalExceptionHandlingMiddleware**: Catches unhandled validation exceptions
- Both return structured error responses with **localized validation details**

### 4. Error Response Format
```json
{
  "Message": "Validation failed.",
  "Errors": [
    {
      "PropertyName": "PickupDate",
      "ErrorMessage": "Pickup date cannot be in the past.", // Localized message
      "AttemptedValue": "2023-01-01T10:00:00Z"
    }
  ]
}
```

**Localized Examples:**
- **English**: "Pickup date cannot be in the past."
- **Sinhala**: "අරගෙන යන දිනය අතීතයේ විය නොහැක."
- **Tamil**: "எடுக்கும் தேதி கடந்த காலத்தில் இருக்க முடியாது."

## Benefits

### 1. Automatic Validation
- No need to manually validate in each handler
- Consistent validation across all commands/queries
- Centralized validation logic
- **Async validation support** for complex business rules

### 2. Clean Handlers
- Handlers focus on business logic only
- Removed manual validation code from handlers
- Improved code maintainability
- **Async validation** handles complex checks automatically

### 3. Comprehensive Error Handling
- Structured error responses
- Detailed validation information
- Consistent error format across the application
- **Localized error messages** for better user experience

### 4. Enhanced Validators
- More comprehensive validation rules
- **Localized error messages** in multiple languages
- Conditional validation for optional fields
- **Async validation** for database-dependent rules

### 5. Performance Optimizations
- **Parallel validation execution** for better performance
- **Enhanced logging** for debugging and monitoring
- Efficient async validation handling

### 6. Multi-Language Support
- **English, Sinhala, and Tamil** language support
- Automatic culture detection
- Easy to add more languages
- Consistent user experience across different locales

## Usage Examples

### Controller Action (No Changes Required)
```csharp
[HttpPost]
public async Task<IActionResult> CreateBooking([FromBody] CreateBookingDto dto)
{
    var command = _mapper.Map<CreateBookingCommand>(dto);
    var result = await _mediator.Send(command); // Validation happens automatically
    return Ok(result);
}
```

### Command Handler (Simplified)
```csharp
public async Task<Guid> Handle(CreateBookingCommand request, CancellationToken cancellationToken)
{
    // Only business logic validation remains
    bool isAvailable = await _bookingRepository.IsCarAvailableAsync(
        request.CarId, request.PickupDate, request.ReturnDate, cancellationToken);
    
    if (!isAvailable)
        throw new InvalidOperationException("This car is already booked for the selected period.");
    
    // Create and save booking...
}
```

## Validation Rules Examples

### Date Validation
```csharp
RuleFor(x => x.PickupDate)
    .NotEmpty()
    .WithMessage("Pickup date is required.")
    .GreaterThan(DateTime.UtcNow)
    .WithMessage("Pickup date cannot be in the past.")
    .LessThan(x => x.ReturnDate)
    .WithMessage("Pickup date must be before return date.");
```

### Conditional Validation
```csharp
RuleFor(x => x.NIC)
    .MaximumLength(20)
    .WithMessage("NIC cannot exceed 20 characters.")
    .Matches(@"^[0-9]{9}[vVxX]?$|^[0-9]{12}$")
    .WithMessage("Invalid NIC format.")
    .When(x => !string.IsNullOrWhiteSpace(x.NIC));
```

### Localized Validation
```csharp
RuleFor(x => x.PickupDate)
    .NotEmpty()
    .WithMessage(_localizationService.GetString("PickupDateRequired"))
    .GreaterThan(DateTime.UtcNow)
    .WithMessage(_localizationService.GetString("PickupDatePast"));
```

### Async Validation
```csharp
RuleFor(x => x.CarId)
    .MustAsync(async (command, carId, cancellation) =>
    {
        return await _bookingRepository.IsCarAvailableAsync(
            carId, command.PickupDate, command.ReturnDate, cancellation);
    })
    .WithMessage(_localizationService.GetString("CarNotAvailable"));
```

### Custom Validation with Localization
```csharp
RuleFor(x => x.ReturnDate)
    .Must((request, returnDate) => (returnDate - request.PickupDate).TotalMinutes >= 30)
    .WithMessage(_localizationService.GetString("BookingMinDuration"));
```

## Testing

The validation behavior can be tested by:

1. **Unit Tests**: Test validators directly
2. **Integration Tests**: Test the complete pipeline including validation
3. **API Tests**: Test validation error responses
4. **Localization Tests**: Test validation messages in different languages
5. **Async Validation Tests**: Test async validation rules with mocked dependencies

## Future Enhancements

1. **More Languages**: Add support for additional languages (Arabic, Chinese, etc.)
2. **Custom Validators**: Create more reusable custom validators
3. **Validation Groups**: Implement validation groups for different scenarios
4. **Caching**: Add caching for frequently used validation results
5. **Metrics**: Add performance metrics for validation operations
6. **Dynamic Validation**: Support for dynamic validation rules based on configuration

## Conclusion

This enhanced implementation provides a robust, automatic validation system that ensures data integrity while keeping the code clean and maintainable. The validation behavior runs automatically for all MediatR requests, providing consistent validation across the entire application.

**Key Achievements:**
- ✅ **Async Validation Support**: Full support for asynchronous validation rules
- ✅ **Multi-Language Support**: English, Sinhala, and Tamil localization
- ✅ **Performance Optimizations**: Parallel validation execution
- ✅ **Enhanced Logging**: Comprehensive logging for debugging and monitoring
- ✅ **Clean Architecture**: Separation of concerns with dependency injection
- ✅ **Extensible Design**: Easy to add new validators and languages

The system now provides a world-class validation experience with automatic validation, localized error messages, and async support for complex business rules.
