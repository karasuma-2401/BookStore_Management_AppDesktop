# Add Employee Feature - Testing Guide

## Quick Start Testing

### Test Case 1: Open Add Employee Window
**Steps:**
1. Open Employee Management page
2. Click "Add New Employee" button
3. AddEmployeeWindow should open as modal dialog

**Expected Result:**
- ✅ Window opens centered on screen
- ✅ ComboBox populated with all users
- ✅ All input fields are empty
- ✅ Cancel and Add Employee buttons visible

---

## ComboBox Testing

### Test Case 2: ComboBox Loads Users
**Steps:**
1. Open AddEmployeeWindow
2. Click on ComboBox dropdown

**Expected Result:**
- ✅ All users display in dropdown
- ✅ User names (FullName) are visible
- ✅ Dropdown is searchable/editable

---

### Test Case 3: ComboBox Filter by Username
**Steps:**
1. Open AddEmployeeWindow
2. Type "john" in ComboBox search field

**Expected Result:**
- ✅ ComboBox filters to show only users with "john" in username
- ✅ Real-time filtering (updates as you type)
- ✅ Case-insensitive (works with "JOHN", "John", "john")

---

### Test Case 4: ComboBox Filter by Email
**Steps:**
1. Open AddEmployeeWindow
2. Type "example.com" in ComboBox search field

**Expected Result:**
- ✅ ComboBox filters to show only users with "example.com" in email
- ✅ Real-time filtering works

---

### Test Case 5: ComboBox Filter by Full Name
**Steps:**
1. Open AddEmployeeWindow
2. Type "Smith" in ComboBox search field

**Expected Result:**
- ✅ ComboBox filters to show only users with "Smith" in their full name
- ✅ Real-time filtering works

---

### Test Case 6: ComboBox Clear Filter
**Steps:**
1. Open AddEmployeeWindow
2. Type "john" in ComboBox (filters users)
3. Clear the search field (Ctrl+A, Delete)

**Expected Result:**
- ✅ All users display again in ComboBox
- ✅ No error messages

---

## Add User Button Testing

### Test Case 7: Add User Button Click
**Steps:**
1. Open AddEmployeeWindow
2. Click "Add User" button

**Expected Result:**
- ✅ Informational message appears
- ✅ Message says to create user in User Management section
- ✅ Dialog closes when OK clicked
- ✅ ComboBox refreshes with users

---

## Employee Information Input Testing

### Test Case 8: Input Full Name
**Steps:**
1. Open AddEmployeeWindow
2. Select a user from ComboBox
3. Click on "Full Name" field
4. Type "John Doe"

**Expected Result:**
- ✅ Text appears in field
- ✅ Field accepts input
- ✅ No restrictions on characters

---

### Test Case 9: Input Age
**Steps:**
1. Open AddEmployeeWindow
2. Click on "Age" field
3. Type "30"

**Expected Result:**
- ✅ Number appears in field
- ✅ Field is optional (can be skipped)

---

### Test Case 10: Input Phone
**Steps:**
1. Open AddEmployeeWindow
2. Click on "Phone" field
3. Type "555-1234567"

**Expected Result:**
- ✅ Phone number appears in field
- ✅ Accepts various phone formats

---

### Test Case 11: Input Address
**Steps:**
1. Open AddEmployeeWindow
2. Click on "Address" field
3. Type "123 Main Street, City, State 12345"

**Expected Result:**
- ✅ Address appears in field
- ✅ Field accepts long addresses

---

### Test Case 12: Input Salary
**Steps:**
1. Open AddEmployeeWindow
2. Click on "Salary" field
3. Type "50000"

**Expected Result:**
- ✅ Salary amount appears in field
- ✅ Field is optional (can be skipped)

---

## Validation Testing

### Test Case 13: Validate - No User Selected
**Steps:**
1. Open AddEmployeeWindow
2. Leave ComboBox empty (don't select a user)
3. Fill in all other fields
4. Click "Add Employee"

**Expected Result:**
- ✅ Error message: "Please select a User account."
- ✅ Window stays open
- ✅ Data is preserved

---

### Test Case 14: Validate - Missing Full Name
**Steps:**
1. Open AddEmployeeWindow
2. Select a user from ComboBox
3. Leave "Full Name" field empty
4. Fill in Phone and Address
5. Click "Add Employee"

**Expected Result:**
- ✅ Error message: "Please enter employee full name."
- ✅ Window stays open
- ✅ Data is preserved

---

### Test Case 15: Validate - Missing Phone
**Steps:**
1. Open AddEmployeeWindow
2. Select a user from ComboBox
3. Fill in Full Name and Address
4. Leave "Phone" field empty
5. Click "Add Employee"

**Expected Result:**
- ✅ Error message: "Please enter employee phone number."
- ✅ Window stays open
- ✅ Data is preserved

---

### Test Case 16: Validate - Missing Address
**Steps:**
1. Open AddEmployeeWindow
2. Select a user from ComboBox
3. Fill in Full Name and Phone
4. Leave "Address" field empty
5. Click "Add Employee"

**Expected Result:**
- ✅ Error message: "Please enter employee address."
- ✅ Window stays open
- ✅ Data is preserved

---

## Save/Add Employee Testing

### Test Case 17: Successful Add Employee
**Steps:**
1. Open AddEmployeeWindow
2. Select a user from ComboBox
3. Fill in all required fields:
   - Full Name: "Jane Smith"
   - Phone: "555-9876543"
   - Address: "456 Oak Avenue"
   - Age: "28" (optional)
   - Salary: "60000" (optional)
4. Click "Add Employee"

**Expected Result:**
- ✅ Success message: "Employee added successfully!"
- ✅ Window closes
- ✅ EmployeePage refreshes with new employee in list
- ✅ Window.DialogResult = true (for parent notification)

---

### Test Case 18: Duplicate Employee (User Already Has Profile)
**Steps:**
1. Open AddEmployeeWindow
2. Select a user that already has an Employee profile
3. Fill in all required fields
4. Click "Add Employee"

**Expected Result:**
- ✅ Error message: "Failed to save employee. Check if User already has an Employee profile or try again later."
- ✅ Window stays open
- ✅ User can modify data and retry

---

### Test Case 19: API Error Handling
**Steps:**
1. Open AddEmployeeWindow
2. Fill in all fields correctly
3. Simulate API error (turn off internet or mock API failure)
4. Click "Add Employee"

**Expected Result:**
- ✅ Error message appears
- ✅ Window stays open
- ✅ User can retry or cancel
- ✅ Error details logged to debug output

---

## Cancel Testing

### Test Case 20: Cancel Button
**Steps:**
1. Open AddEmployeeWindow
2. Fill in some data
3. Click "Cancel" button

**Expected Result:**
- ✅ Window closes
- ✅ No employee is created
- ✅ EmployeePage list is NOT refreshed
- ✅ Unsaved data is discarded

---

### Test Case 21: Close Window (X Button)
**Steps:**
1. Open AddEmployeeWindow
2. Fill in some data
3. Click window close button (X)

**Expected Result:**
- ✅ Window closes
- ✅ No employee is created
- ✅ EmployeePage list is NOT refreshed

---

## Window Behavior Testing

### Test Case 22: Window as Modal Dialog
**Steps:**
1. Open EmployeePage
2. Click "Add New Employee"
3. Try to interact with main window while dialog is open

**Expected Result:**
- ✅ AddEmployeeWindow is modal (modal to main window)
- ✅ Cannot interact with main window
- ✅ Must close or cancel dialog first

---

### Test Case 23: Window Drag
**Steps:**
1. Open AddEmployeeWindow
2. Click on title bar and drag

**Expected Result:**
- ✅ Window can be dragged around screen
- ✅ Smooth dragging behavior

---

## Data Binding Testing

### Test Case 24: Data Binding - ComboBox to ViewModel
**Steps:**
1. Open AddEmployeeWindow
2. Select different users from ComboBox

**Expected Result:**
- ✅ SelectedUser property updates in ViewModel
- ✅ No binding errors in output

---

### Test Case 25: Data Binding - TextBox to ViewModel
**Steps:**
1. Open AddEmployeeWindow
2. Type in various fields
3. Open Visual Studio output window

**Expected Result:**
- ✅ Data binds correctly to ViewModel properties
- ✅ No binding errors logged
- ✅ PropertyChanged notifications work

---

## Performance Testing

### Test Case 26: Large User List Performance
**Steps:**
1. Ensure database has 1000+ users
2. Open AddEmployeeWindow
3. Watch ComboBox population time
4. Type in search field

**Expected Result:**
- ✅ ComboBox loads within reasonable time (< 5 seconds)
- ✅ Search/filtering is responsive (< 1 second updates)
- ✅ No UI freezing

---

### Test Case 27: Rapid Input
**Steps:**
1. Open AddEmployeeWindow
2. Rapidly type in various fields
3. Rapidly click buttons

**Expected Result:**
- ✅ UI remains responsive
- ✅ No crashes or exceptions
- ✅ Data remains consistent

---

## Integration Testing

### Test Case 28: Parent Window Refresh
**Steps:**
1. Note current employee count on EmployeePage
2. Open AddEmployeeWindow
3. Add a new employee successfully
4. Check employee count on EmployeePage

**Expected Result:**
- ✅ Employee count increases by 1
- ✅ New employee appears in list
- ✅ EmployeePage.InitializeDataAsync() was called

---

### Test Case 29: Multiple Windows
**Steps:**
1. Open EmployeePage
2. Click "Add New Employee" (AddEmployeeWindow #1 opens)
3. Without closing first window, open EmployeePage in another tab
4. Click "Add New Employee" (AddEmployeeWindow #2 opens)

**Expected Result:**
- ✅ Both windows work independently
- ✅ No conflicts or shared state issues
- ✅ Both windows can add employees successfully

---

## Accessibility Testing

### Test Case 30: Tab Navigation
**Steps:**
1. Open AddEmployeeWindow
2. Press Tab key multiple times

**Expected Result:**
- ✅ Focus moves through all controls in logical order
- ✅ ComboBox → Full Name → Age → Phone → Address → Salary → Cancel → Add
- ✅ Visual focus indicator visible

---

### Test Case 31: Enter Key to Submit
**Steps:**
1. Open AddEmployeeWindow
2. Fill in all required fields
3. Press Enter key

**Expected Result:**
- ✅ Form submits (same as clicking Add Employee)
- ✅ Employee is created successfully

---

## Edge Cases

### Test Case 32: Special Characters in Input
**Steps:**
1. Open AddEmployeeWindow
2. Enter special characters: `!@#$%^&*()`
3. Fill other required fields
4. Click "Add Employee"

**Expected Result:**
- ✅ Special characters are accepted
- ✅ Employee is created with special characters
- ✅ No SQL injection or security issues

---

### Test Case 33: Very Long Input
**Steps:**
1. Open AddEmployeeWindow
2. Fill in very long text (> 1000 characters)
3. Click "Add Employee"

**Expected Result:**
- ✅ Input is trimmed to field limits
- ✅ OR error message if field has max length
- ✅ No crashes

---

### Test Case 34: Whitespace Only Input
**Steps:**
1. Open AddEmployeeWindow
2. Select user
3. Fill in Full Name with only spaces: "    "
4. Click "Add Employee"

**Expected Result:**
- ✅ Validation error: "Please enter employee full name."
- ✅ Whitespace-only input is treated as empty

---

## Test Execution Summary

| Category | Test Cases | Status |
|----------|-----------|--------|
| Window Behavior | 3 | ⭕ To Test |
| ComboBox Features | 5 | ⭕ To Test |
| Add User Button | 1 | ⭕ To Test |
| Input Fields | 5 | ⭕ To Test |
| Validation | 4 | ⭕ To Test |
| Save/Add | 3 | ⭕ To Test |
| Cancel | 2 | ⭕ To Test |
| Data Binding | 2 | ⭕ To Test |
| Performance | 2 | ⭕ To Test |
| Integration | 2 | ⭕ To Test |
| Accessibility | 2 | ⭕ To Test |
| Edge Cases | 3 | ⭕ To Test |
| **TOTAL** | **34** | **⭕ 0/34** |

---

**Legend:**
- ✅ = Pass
- ❌ = Fail
- ⭕ = To Test
- 🔄 = In Progress
