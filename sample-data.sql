-- .NET Expense Sharing Application Database Setup
-- Server: LIN-5CG1153CJ0

-- Sample data to get started (run after application creates tables)
-- Insert sample users
INSERT INTO Users (Username, Email, Name, CreatedAt) VALUES 
('john_doe', 'john@example.com', 'John Doe', GETDATE()),
('jane_smith', 'jane@example.com', 'Jane Smith', GETDATE()),
('bob_wilson', 'bob@example.com', 'Bob Wilson', GETDATE()),
('alice_brown', 'alice@example.com', 'Alice Brown', GETDATE());

-- Insert sample groups
INSERT INTO Groups (Name, Description, CreatedById, CreatedAt) VALUES 
('Roommates', 'Shared apartment expenses', 1, GETDATE()),
('Trip to Paris', 'Vacation expenses for Paris trip', 2, GETDATE()),
('Office Lunch', 'Weekly office lunch group', 3, GETDATE());

-- Insert group members
INSERT INTO GroupMembers (GroupsId, MembersId) VALUES 
(1, 1), (1, 2), (1, 3),  -- Roommates: John, Jane, Bob
(2, 2), (2, 4),          -- Paris Trip: Jane, Alice
(3, 1), (3, 3), (3, 4);  -- Office Lunch: John, Bob, Alice

-- Insert sample expenses
INSERT INTO Expenses (Description, Amount, PaidById, GroupId, SplitType, CreatedAt) VALUES 
('Grocery Shopping', 120.00, 1, 1, 0, GETDATE()),  -- 0 = EQUAL
('Electricity Bill', 90.00, 2, 1, 0, GETDATE()),
('Hotel Booking', 600.00, 2, 2, 0, GETDATE()),
('Team Lunch', 80.00, 3, 3, 0, GETDATE());

-- Insert expense splits
INSERT INTO ExpenseSplits (ExpenseId, UserId, Amount, Percentage) VALUES 
-- Grocery Shopping (Equal split among 3 people: $40 each)
(1, 1, 40.00, 33.33),
(1, 2, 40.00, 33.33),
(1, 3, 40.00, 33.34),
-- Electricity Bill (Equal split among 3 people: $30 each)
(2, 1, 30.00, 33.33),
(2, 2, 30.00, 33.33),
(2, 3, 30.00, 33.34),
-- Hotel Booking (Equal split among 2 people: $300 each)
(3, 2, 300.00, 50.00),
(3, 4, 300.00, 50.00),
-- Team Lunch (Equal split among 3 people: $26.67 each)
(4, 1, 26.67, 33.33),
(4, 3, 26.67, 33.33),
(4, 4, 26.66, 33.34);

PRINT '.NET Database initialized successfully with sample data!';