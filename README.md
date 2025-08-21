# Subscription Tools - DNN Module

## Overview
This is a DotNetNuke (DNN) subscription management module that provides comprehensive subscription tools including plan management, user signups, payment processing, receipt management, and reporting capabilities. The module is designed to handle subscription-based business models with flexible billing cycles and user management.

## Project Information
- **Module Name**: Subscription Tools
- **DNN Version**: Compatible with DNN 6.x and higher
- **Language**: VB.NET
- **Framework**: .NET Framework
- **Module Type**: Business/Subscription Management

## Features

### 🎯 **Core Subscription Management**
- **Plan Management**: Create, edit, and manage subscription plans with different billing frequencies
- **User Signups**: Streamlined user registration and subscription enrollment
- **Payment Processing**: Integrated payment handling with IPN (Instant Payment Notification) support
- **Receipt Management**: Comprehensive receipt tracking and management
- **User Status**: Monitor and manage user subscription statuses
- **Reporting**: Built-in reporting tools for subscription analytics

### 🔧 **Technical Features**
- **Multi-Frequency Billing**: Support for various billing cycles (monthly, yearly, custom)
- **Role-Based Access**: Different user roles and permissions
- **IPN Handler**: PayPal and other payment processor integration
- **Database Providers**: SQL Server data providers with version management
- **Localization**: Multi-language support with resource files

## 📁 **File Structure**
```
Subscription-Tools/
├── App_LocalResources/           # Localization resource files
├── Base/                         # Base module classes
│   └── ModuleBase.vb
├── Entities/                     # Business logic and data models
│   ├── Constants.vb
│   ├── ContentController.vb
│   ├── ContentInfo.vb
│   ├── CrumbInfo.vb
│   ├── FrequencyType.vb
│   ├── OrderController.vb
│   ├── OrderInfo.vb
│   ├── OrderItemController.vb
│   ├── OrderItemInfo.vb
│   ├── PaymentType.vb
│   ├── PlanController.vb
│   ├── PlanInfo.vb
│   ├── ReceiptController.vb
│   ├── ReceiptInfo.vb
│   ├── ReminderJob.vb
│   ├── ReportController.vb
│   ├── ReportType.vb
│   ├── UserSettingController.vb
│   └── UserSettingInfo.vb
├── Providers/                    # Data providers and SQL scripts
│   └── DataProviders/
│       ├── DataProvider.vb
│       └── SqlDataProvider/
│           ├── 01.01.00.SqlDataProvider
│           ├── 01.02.00.SqlDataProvider
│           ├── 01.03.00.SqlDataProvider
│           ├── 01.04.00.SqlDataProvider
│           ├── 01.05.00.SqlDataProvider
│           └── Uninstall.SqlDataProvider
├── Tools/                        # Utility tools
│   ├── IPNHandler.aspx          # Payment notification handler
│   ├── IPNHandler.aspx.vb
│   └── IPNHandler.aspx.designer.vb
├── *.ascx                       # User control files for each module view
├── *.ascx.vb                    # Code-behind files
├── *.ascx.designer.vb           # Designer-generated files
├── Ventrian.SubscriptionTools.sln    # Solution file
├── Ventrian.SubscriptionTools.vbproj # Project file
├── SubscriptionTools.dnn        # DNN manifest
└── web.config                   # Configuration files
```

## 🚀 **Development Setup**

### **Prerequisites**
- Visual Studio 2019 or later
- .NET Framework 4.5 or higher
- DNN development environment
- SQL Server (for database operations)

### **Getting Started**
1. **Clone/Download** the module source code
2. **Open** `Ventrian.SubscriptionTools.sln` in Visual Studio
3. **Restore** NuGet packages if prompted
4. **Build** the solution to ensure all dependencies are resolved

### **Build Configuration**
```powershell
# Build the solution
& "C:\Program Files (x86)\Microsoft Visual Studio\2019\Community\MSBuild\Current\Bin\MSBuild.exe" Ventrian.SubscriptionTools.sln /p:Configuration=Release /p:Platform="Any CPU" /verbosity:minimal
```

## 🛠 **Module Views & Functionality**

### **1. Signup Management**
- **Signup.ascx**: Main signup interface for new subscriptions
- **SignupDetails.ascx**: Detailed signup information and processing
- **SignupPlan.ascx**: Plan selection and configuration
- **SignupRegister.ascx**: User registration during signup
- **SignupViewOptions.ascx**: Signup display options and settings

### **2. Plan Management**
- **EditPlan.ascx**: Individual plan editing interface
- **EditPlans.ascx**: Bulk plan management and overview

### **3. Receipt Management**
- **EditReceipt.ascx**: Individual receipt editing
- **EditReceipts.ascx**: Bulk receipt management
- **ViewInvoice.ascx**: Invoice viewing and management

### **4. User Management**
- **Status.ascx**: User subscription status overview
- **StatusEditRole.ascx**: Role editing for users
- **StatusViewOptions.ascx**: Status display configuration

### **5. Administrative Tools**
- **Reports.ascx**: Subscription and financial reporting
- **SendReminders.ascx**: Automated reminder system for payments

### **6. Payment Processing**
- **IPNHandler.aspx**: Handles payment notifications from payment processors

## 🔧 **Development Guidelines**

### **Code Structure**
- **VB.NET**: All code-behind files use VB.NET
- **DNN Pattern**: Follows DNN module development patterns
- **Separation of Concerns**: Business logic separated into controller classes
- **Data Access**: Uses DNN data provider pattern for database operations

### **Adding New Features**
1. **Create User Control**: Add new `.ascx` file
2. **Add Code-Behind**: Create corresponding `.ascx.vb` file
3. **Update Project**: Add files to `Ventrian.SubscriptionTools.vbproj`
4. **Register in DNN**: Update module manifest if needed

### **Database Changes**
1. **Create SQL Script**: Add new version in `SqlDataProvider` folder
2. **Update Version**: Increment version number in script filename
3. **Test Migration**: Verify upgrade process works correctly

## 📊 **Configuration & Settings**

### **Module Settings**
- **Payment Gateway**: Configure payment processor settings
- **Email Templates**: Customize reminder and notification emails
- **Billing Cycles**: Set up supported billing frequencies
- **User Roles**: Configure role-based access permissions

### **DNN Integration**
- **Portal Settings**: Module-specific portal configuration
- **User Permissions**: Role-based access control
- **Localization**: Multi-language support configuration

## 🚀 **Deployment**

### **Development Deployment**
1. **Build** the solution in Release mode
2. **Copy the DLL**: Copy `bin\Ventrian.SubscriptionTools.dll` to your DNN development site's `bin\` directory
3. **Copy ASCX files**: Copy only the `.ascx` user control files to your DNN development site's `DesktopModules\Ventrian.SubscriptionTools\` directory
4. **Recycle** the application pool

**Important Note**: Only copy the `.ascx` files, NOT the `.ascx.vb` or `.ascx.designer.vb` files, as these are compiled into the DLL.

### **Production Deployment**
1. **Build** the solution in Release mode
2. **Package** the module using DNN packaging tools
3. **Install** via DNN module installation process
4. **Configure** module settings for production environment
5. **Test** all functionality thoroughly

### **Quick Development Deployment Commands**
```powershell
# Build the solution
& "C:\Program Files (x86)\Microsoft Visual Studio\2019\Community\MSBuild\Current\Bin\MSBuild.exe" Ventrian.SubscriptionTools.sln /p:Configuration=Release /p:Platform="Any CPU" /verbosity:minimal

# Copy the DLL (contains all compiled code)
Copy-Item "bin\Ventrian.SubscriptionTools.dll" "C:\inetpub\packdev\bin\"

# Copy the ASCX file (contains the UI template)
Copy-Item "EditReceipt.ascx" "C:\inetpub\packdev\DesktopModules\SubscriptionSignup\"

# Recycle the app pool by touching web.config
(Get-Item "C:\inetpub\packdev\bin\web.config").LastWriteTime = Get-Date
```

### **Actual Deployment Example (Completed)**
The following deployment was successfully completed on August 20, 2025:

```powershell
# Navigate to project directory
cd "C:\Users\Administrator\source\repos\Subscription-Tools"

# Copy the DLL
Copy-Item "bin\Ventrian.SubscriptionTools.dll" "C:\inetpub\packdev\bin\"

# Copy the ASCX file to the SubscriptionSignup module
Copy-Item "EditReceipt.ascx" "C:\inetpub\packdev\DesktopModules\SubscriptionSignup\"

# Recycle the app pool
(Get-Item "C:\inetpub\packdev\web.config").LastWriteTime = Get-Date
```

**Note**: The module is located in `DesktopModules\SubscriptionSignup\`, not `Ventrian.SubscriptionTools\`.

### **Upgrade Process**
1. **Backup** existing data and configuration
2. **Install** new version via DNN
3. **Run** database upgrade scripts automatically
4. **Verify** all functionality works correctly

## 🧪 **Testing**

### **Unit Testing**
- Test individual controller methods
- Verify data validation logic
- Test error handling scenarios

### **Integration Testing**
- Test DNN module integration
- Verify database operations
- Test payment processing workflows

### **User Acceptance Testing**
- Test subscription signup flow
- Verify payment processing
- Test administrative functions
- Verify reporting functionality

## 🐛 **Troubleshooting**

### **Common Issues**
1. **Build Errors**: Ensure all dependencies are properly referenced
2. **Runtime Errors**: Check DNN logs for detailed error information
3. **Database Issues**: Verify connection strings and permissions
4. **Payment Issues**: Check IPN handler configuration

### **Deployment Issues**
1. **Changes Not Appearing**: Ensure you've recycled the application pool after copying files
2. **DLL Not Loading**: Verify the DLL was copied to the correct `bin\` directory
3. **UI Not Updating**: Ensure the `.ascx` files were copied to the correct `DesktopModules` directory
4. **Compilation Errors**: Check that all required dependencies are available in the target environment

### **Common Deployment Mistakes**
- ❌ **Copying `.ascx.vb` files** - These are compiled into the DLL and don't need to be copied
- ❌ **Copying `.ascx.designer.vb` files** - These are also compiled into the DLL
- ❌ **Not recycling the app pool** - Changes won't take effect until the app pool is recycled
- ❌ **Copying files to wrong directories** - DLL goes in `bin\`, ASCX files go in `DesktopModules\`
- ❌ **Wrong module folder** - This module is located in `DesktopModules\SubscriptionSignup\`, not `Ventrian.SubscriptionTools\`

### **Debug Mode**
- Enable detailed error messages in development
- Use DNN logging for debugging
- Check browser console for client-side issues

## 📈 **Performance Considerations**

### **Database Optimization**
- Use appropriate indexes on frequently queried fields
- Implement connection pooling
- Optimize complex queries

### **Caching Strategy**
- Cache frequently accessed data
- Use DNN caching mechanisms
- Implement appropriate cache invalidation

### **Scalability**
- Design for multiple concurrent users
- Optimize database queries for large datasets
- Consider asynchronous processing for long-running operations

## 🔒 **Security Considerations**

### **Data Protection**
- Validate all user inputs
- Use parameterized queries to prevent SQL injection
- Implement proper authentication and authorization

### **Payment Security**
- Secure payment processor integration
- Validate IPN notifications
- Protect sensitive payment information

## 📚 **API Reference**

### **Key Classes**
- **PlanController**: Manages subscription plans
- **OrderController**: Handles subscription orders
- **ReceiptController**: Manages receipts and invoices
- **UserSettingController**: Handles user preferences

### **Data Models**
- **PlanInfo**: Subscription plan information
- **OrderInfo**: Order details and status
- **ReceiptInfo**: Receipt and invoice data
- **UserSettingInfo**: User configuration settings

## 🤝 **Contributing**

### **Development Process**
1. **Fork** the repository
2. **Create** feature branch
3. **Implement** changes following coding standards
4. **Test** thoroughly
5. **Submit** pull request

### **Coding Standards**
- Follow VB.NET best practices
- Use consistent naming conventions
- Add appropriate comments and documentation
- Follow DNN module development patterns

## 📞 **Support & Contact**

### **Documentation**
- This README provides development guidance
- Check DNN documentation for platform-specific information
- Review code comments for implementation details

### **Community**
- DNN Community forums
- Module-specific support channels
- Development team contact information

## 📝 **Change Log**

### **Recent Updates (Current Development)**
- **EditReceipt User Selection**: Changed from username text input to user dropdown for better usability
- **UserID-based Processing**: Updated receipt creation to use UserID instead of username lookup
- **Improved User Experience**: Dropdown shows users by DisplayName (First Last) sorted alphabetically
- **URL Parameter Update**: Changed from `username` parameter to `UserID` parameter for better reliability
- **Enhanced Validation**: Built-in user selection validation eliminates username formatting issues
- **Interface Label Update**: Changed label from "UserName:" to "User:" for cleaner, more intuitive interface
- **Conditional Role Assignment**: Users are added to the "Paid Member" role only when "Add To Role" checkbox is checked
- **Robust Error Handling**: Comprehensive null reference protection and exception handling prevents crashes
- **Enhanced Search Interface**: Changed EditReceipts search from username text input to user dropdown selection

### **Deployment History - August 20, 2025**
- **Issue Resolved**: Username text input field causing errors with special characters and spaces
- **Solution Implemented**: Replaced text input with user dropdown populated from DNN user list
- **Files Updated**: 
  - `EditReceipt.ascx` - Changed from text input to dropdown
  - `EditReceipt.ascx.vb` - Added user binding and validation logic
  - `Ventrian.SubscriptionTools.dll` - Compiled with new functionality
- **Deployment Location**: `C:\inetpub\packdev\DesktopModules\SubscriptionSignup\`
- **Status**: ✅ Successfully deployed and tested

### **Role Assignment Fix - August 20, 2025**
- **Issue Identified**: Users were not being added to the "Paid Member" role after receipt creation due to null reference errors
- **Root Cause**: Null reference exceptions were preventing the role assignment code from executing
- **Solution Implemented**: Enhanced error handling and improved role assignment logic while maintaining checkbox control
- **Files Updated**:
  - `EditReceipt.ascx.vb` - Enhanced role assignment logic
  - `Ventrian.SubscriptionTools.dll` - Compiled with role assignment fix
- **Deployment Location**: `C:\inetpub\packdev\bin\`
- **Status**: ✅ Successfully deployed and tested

### **EditReceipts Search Interface Enhancement - August 20, 2025**
- **Issue Identified**: Search functionality in EditReceipts used text input for usernames, causing similar issues as EditReceipt
- **Solution Implemented**: Converted username text search to user dropdown selection with "FirstName LastName / DisplayName" format
- **Files Updated**:
  - `EditReceipts.ascx` - Changed from text input to dropdown
  - `EditReceipts.ascx.vb` - Added user binding and dropdown logic
  - `EditReceipts.ascx.designer.vb` - Updated control type from TextBox to DropDownList
  - `Ventrian.SubscriptionTools.dll` - Compiled with enhanced search functionality
- **Deployment Location**: 
  - DLL: `C:\inetpub\packdev\bin\`
  - ASCX: `C:\inetpub\packdev\DesktopModules\SubscriptionSignup\`
- **Status**: ✅ Successfully deployed and tested

### **EditReceipts Auto-Search Enhancement - August 20, 2025**
- **Enhancement**: Added automatic search triggering when user selection changes in dropdown
- **User Experience**: Users no longer need to click "Go" button - search happens automatically
- **Interface Cleanup**: Removed "Go" button since it's no longer needed
- **Files Updated**:
  - `EditReceipts.ascx` - Added AutoPostBack="True" to dropdown, removed Go button
  - `EditReceipts.ascx.vb` - Added drpUsers_SelectedIndexChanged event handler, removed btnGo_Click
  - `Ventrian.SubscriptionTools.dll` - Compiled with auto-search functionality
- **Deployment Location**: 
  - DLL: `C:\inetpub\packdev\bin\`
  - ASCX: `C:\inetpub\packdev\DesktopModules\SubscriptionSignup\`
- **Status**: ✅ Successfully deployed and tested

### **Additional Updates - August 20, 2025**
- **Interface Improvement**: Changed label from "UserName:" to "User:" for better user experience
- **Files Updated**:
  - `EditReceipt.ascx` - Updated resourcekey from "UserName" to "User"
  - `EditReceipt.ascx.resx` - Added new "User.Text" resource key with value "User"
- **Result**: Cleaner, more intuitive interface with proper localization support

### **Version 1.5.0**
- Latest stable release
- Comprehensive subscription management features
- Payment processing integration
- Reporting and analytics capabilities

### **Previous Versions**
- See individual version SQL scripts for detailed change history
- Check module manifest for version information

---

*Last Updated: [Current Date]*
*Module Version: 1.5.0*
*DNN Compatibility: 6.x and higher*
*Status: Production Ready*
